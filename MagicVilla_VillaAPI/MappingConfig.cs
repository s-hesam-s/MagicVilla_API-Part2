using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI;

public sealed class MappingConfig
{
    public VillaDTO ToVillaDTO(Villa source)
    {
        if (source is null) return null!;
        return new VillaDTO
        {
            Id = source.Id,
            Name = source.Name,
            Details = source.Details,
            Rate = source.Rate,
            Occupancy = source.Occupancy,
            Sqft = source.Sqft,
            ImageUrl = source.ImageUrl,
            ImageLocalPath = source.ImageLocalPath,
            Amenity = source.Amenity,
        };
    }

    public Villa ToVilla(VillaDTO source)
    {
        if (source is null) return null!;
        return new Villa
        {
            Id = source.Id,
            Name = source.Name,
            Details = source.Details,
            Rate = source.Rate,
            Sqft = source.Sqft,
            Occupancy = source.Occupancy,
            ImageUrl = source.ImageUrl,
            ImageLocalPath = source.ImageLocalPath,
            Amenity = source.Amenity,
        };
    }

    public VillaCreateDTO ToVillaCreateDTO(Villa source)
    {
        if (source is null) return null!;
        return new VillaCreateDTO
        {
            Name = source.Name,
            Details = source.Details,
            Rate = source.Rate,
            Occupancy = source.Occupancy,
            Sqft = source.Sqft,
            ImageUrl = source.ImageUrl,
            Amenity = source.Amenity,
        };
    }

    public Villa ToVilla(VillaCreateDTO source)
    {
        if (source is null) return null!;
        return new Villa
        {
            Name = source.Name,
            Details = source.Details,
            Rate = source.Rate,
            Sqft = source.Sqft,
            Occupancy = source.Occupancy,
            ImageUrl = source.ImageUrl,
            Amenity = source.Amenity,
        };
    }

    public VillaUpdateDTO ToVillaUpdateDTO(Villa source)
    {
        if (source is null) return null!;
        return new VillaUpdateDTO
        {
            Id = source.Id,
            Name = source.Name,
            Details = source.Details,
            Rate = source.Rate,
            Occupancy = source.Occupancy,
            Sqft = source.Sqft,
            ImageUrl = source.ImageUrl,
            ImageLocalPath = source.ImageLocalPath,
            Amenity = source.Amenity,
        };
    }

    public Villa ToVilla(VillaUpdateDTO source)
    {
        if (source is null) return null!;
        return new Villa
        {
            Id = source.Id,
            Name = source.Name,
            Details = source.Details,
            Rate = source.Rate,
            Sqft = source.Sqft,
            Occupancy = source.Occupancy,
            ImageUrl = source.ImageUrl,
            ImageLocalPath = source.ImageLocalPath,
            Amenity = source.Amenity,
        };
    }

    public VillaNumberDTO ToVillaNumberDTO(VillaNumber source)
    {
        if (source is null) return null!;
        return new VillaNumberDTO
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
            Villa = ToVillaDTO(source.Villa),
        };
    }

    public VillaNumber ToVillaNumber(VillaNumberDTO source)
    {
        if (source is null) return null!;
        return new VillaNumber
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            Villa = ToVilla(source.Villa),
            SpecialDetails = source.SpecialDetails,
        };
    }

    public VillaNumberCreateDTO ToVillaNumberCreateDTO(VillaNumber source)
    {
        if (source is null) return null!;
        return new VillaNumberCreateDTO
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public VillaNumber ToVillaNumber(VillaNumberCreateDTO source)
    {
        if (source is null) return null!;
        return new VillaNumber
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public VillaNumberUpdateDTO ToVillaNumberUpdateDTO(VillaNumber source)
    {
        if (source is null) return null!;
        return new VillaNumberUpdateDTO
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public VillaNumber ToVillaNumber(VillaNumberUpdateDTO source)
    {
        if (source is null) return null!;
        return new VillaNumber
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public UserDTO ToUserDTO(ApplicationUser source)
    {
        if (source is null) return null!;
        return new UserDTO
        {
            ID = source.Id,
            UserName = source.UserName,
            Name = source.Name,
        };
    }

    public ApplicationUser ToApplicationUser(UserDTO source)
    {
        if (source is null) return null!;
        return new ApplicationUser
        {
            Name = source.Name,
            Id = source.ID,
            UserName = source.UserName,
        };
    }

    public List<VillaDTO> ToVillaDTOList(IEnumerable<Villa> source) =>
        source?.Select(ToVillaDTO).ToList() ?? new List<VillaDTO>();

    public List<Villa> ToVillaList(IEnumerable<VillaDTO> source) =>
        source?.Select(ToVilla).ToList() ?? new List<Villa>();

    public List<VillaCreateDTO> ToVillaCreateDTOList(IEnumerable<Villa> source) =>
        source?.Select(ToVillaCreateDTO).ToList() ?? new List<VillaCreateDTO>();

    public List<Villa> ToVillaList(IEnumerable<VillaCreateDTO> source) =>
        source?.Select(ToVilla).ToList() ?? new List<Villa>();

    public List<VillaUpdateDTO> ToVillaUpdateDTOList(IEnumerable<Villa> source) =>
        source?.Select(ToVillaUpdateDTO).ToList() ?? new List<VillaUpdateDTO>();

    public List<Villa> ToVillaList(IEnumerable<VillaUpdateDTO> source) =>
        source?.Select(ToVilla).ToList() ?? new List<Villa>();

    public List<VillaNumberDTO> ToVillaNumberDTOList(IEnumerable<VillaNumber> source) =>
        source?.Select(ToVillaNumberDTO).ToList() ?? new List<VillaNumberDTO>();

    public List<VillaNumber> ToVillaNumberList(IEnumerable<VillaNumberDTO> source) =>
        source?.Select(ToVillaNumber).ToList() ?? new List<VillaNumber>();

    public List<VillaNumberCreateDTO> ToVillaNumberCreateDTOList(IEnumerable<VillaNumber> source) =>
        source?.Select(ToVillaNumberCreateDTO).ToList() ?? new List<VillaNumberCreateDTO>();

    public List<VillaNumber> ToVillaNumberList(IEnumerable<VillaNumberCreateDTO> source) =>
        source?.Select(ToVillaNumber).ToList() ?? new List<VillaNumber>();

    public List<VillaNumberUpdateDTO> ToVillaNumberUpdateDTOList(IEnumerable<VillaNumber> source) =>
        source?.Select(ToVillaNumberUpdateDTO).ToList() ?? new List<VillaNumberUpdateDTO>();

    public List<VillaNumber> ToVillaNumberList(IEnumerable<VillaNumberUpdateDTO> source) =>
        source?.Select(ToVillaNumber).ToList() ?? new List<VillaNumber>();

    public List<UserDTO> ToUserDTOList(IEnumerable<ApplicationUser> source) =>
        source?.Select(ToUserDTO).ToList() ?? new List<UserDTO>();

    public List<ApplicationUser> ToApplicationUserList(IEnumerable<UserDTO> source) =>
        source?.Select(ToApplicationUser).ToList() ?? new List<ApplicationUser>();
}
