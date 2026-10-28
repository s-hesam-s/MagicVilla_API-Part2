using Microsoft.AspNetCore.Http;
using Xunit;
using Storage = MagicVilla_VillaAPI.Security.VillaImageStorage;

namespace Authorization.Tests;

public class ImagePolicyTests
{
    [Theory]
    [InlineData("file.svg")]
    [InlineData("file.html")]
    [InlineData("file.exe")]
    [InlineData("file.png")]
    public void UnsafeOrMismatchedContentCannotBeUploaded(string name)
    {
        var bytes = "<script>sample</script>"u8.ToArray();
        Assert.Throws<InvalidDataException>(() => Storage.Validate(new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Image", name)));
    }

    [Theory]
    [InlineData("../../outside.png")]
    [InlineData("wwwroot/ProductImage/../../outside.png")]
    [InlineData("C:/outside.png")]
    [InlineData("https://example.invalid/outside.png")]
    [InlineData("/ProductImage/file.png:alternate")]
    public void UnmanagedPathsAreNeverDeletionTargets(string value) =>
        Assert.Null(Storage.ManagedPath(value, Path.Combine(Path.GetTempPath(), "image-policy-root")));

    [Fact]
    public void LegacyManagedRelativePathsAreRecognized()
    {
        var root = Path.Combine(Path.GetTempPath(), "image-policy-root");
        Assert.Equal(Path.Combine(root, "ProductImage", "stored.png"), Storage.ManagedPath("wwwroot/ProductImage/stored.png", root));
    }
}
