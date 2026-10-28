using System.Linq.Expressions;
using AutoMapper;
using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using V1 = MagicVilla_VillaAPI.Controllers.V1.VillaAPIController;
using V2 = MagicVilla_VillaAPI.Controllers.V2.VillaAPIController;

namespace Authorization.Tests;

public class VillaMutationTests : IDisposable
{
    private readonly Mock<IVillaRepository> repository = new(MockBehavior.Strict);
    private readonly IMapper mapper = new MapperConfiguration(options => options.AddProfile<MappingConfig>()).CreateMapper();
    private readonly ServiceProvider services;
    private readonly Villa stored = new() { Id = 1, Name = "Stored", Rate = 200, Occupancy = 4, Sqft = 500,
        Details = "Description", Amenity = "Pool", ImageLocalPath = "wwwroot/ProductImage/stored.png", ImageUrl = "https://example.invalid/image.png" };

    public VillaMutationTests()
    {
        var collection = new ServiceCollection(); collection.AddLogging(); collection.AddMvcCore().AddDataAnnotations();
        services = collection.BuildServiceProvider();
        repository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(stored);
    }

    [Theory]
    [InlineData(1, "/id")]
    [InlineData(2, "/id")]
    [InlineData(1, "/imageLocalPath")]
    [InlineData(2, "/imageLocalPath")]
    [InlineData(1, "/imageUrl")]
    [InlineData(2, "/image")]
    public async Task PatchCannotChangeIdentityOrStorageFields(int version, string path)
    {
        var patch = new JsonPatchDocument<VillaUpdateDTO>();
        patch.Operations.Add(new Operation<VillaUpdateDTO>("replace", path, null, "untrusted"));
        Assert.IsType<BadRequestResult>(await patchVilla(create(version), version, patch));
        repository.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task PatchValidatesBusinessValuesBeforeWriting(int version)
    {
        var patch = new JsonPatchDocument<VillaUpdateDTO>(); patch.Replace(v => v.Rate, -1);
        Assert.IsType<BadRequestObjectResult>(await patchVilla(create(version), version, patch));
        repository.Verify(r => r.UpdateAsync(It.IsAny<Villa>()), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task ValidPatchChangesOnlyBusinessFields(int version)
    {
        Villa updated = null;
        repository.Setup(r => r.UpdateAsync(It.IsAny<Villa>())).Callback<Villa>(v => updated = v).ReturnsAsync((Villa v) => v);
        var patch = new JsonPatchDocument<VillaUpdateDTO>(); patch.Replace(v => v.Name, "Changed");
        Assert.IsType<NoContentResult>(await patchVilla(create(version), version, patch));
        Assert.Equal(1, updated.Id); Assert.Equal("Changed", updated.Name);
        Assert.Equal(stored.ImageLocalPath, updated.ImageLocalPath);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task PutCannotReplaceStoredImagePathFromHiddenFields(int version)
    {
        Villa updated = null;
        repository.Setup(r => r.UpdateAsync(It.IsAny<Villa>())).Callback<Villa>(v => updated = v).ReturnsAsync((Villa v) => v);
        var dto = mapper.Map<VillaUpdateDTO>(stored); dto.ImageLocalPath = "../../outside.png"; dto.ImageUrl = "https://other.example.invalid/";
        var controller = create(version);
        var result = version == 1 ? await ((V1)controller).UpdateVilla(1, dto) : await ((V2)controller).UpdateVilla(1, dto);
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(stored.ImageLocalPath, updated.ImageLocalPath); Assert.Equal(stored.ImageUrl, updated.ImageUrl);
    }

    private ControllerBase create(int version)
    {
        ControllerBase controller = version == 1 ? new V1(repository.Object, mapper) : new V2(repository.Object, mapper);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { RequestServices = services } };
        controller.ObjectValidator = services.GetRequiredService<IObjectModelValidator>();
        return controller;
    }

    private static Task<IActionResult> patchVilla(ControllerBase controller, int version, JsonPatchDocument<VillaUpdateDTO> patch) =>
        version == 1 ? ((V1)controller).UpdatePartialVilla(1, patch) : ((V2)controller).UpdatePartialVilla(1, patch);

    public void Dispose() => services.Dispose();
}
