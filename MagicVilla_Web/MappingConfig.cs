using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web;

public sealed class MappingConfig
{
    public VillaCreateDTO ToVillaCreateDTO(VillaDTO source)
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

    public VillaDTO ToVillaDTO(VillaCreateDTO source)
    {
        if (source is null) return null!;
        return new VillaDTO
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

    public VillaUpdateDTO ToVillaUpdateDTO(VillaDTO source)
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

    public VillaDTO ToVillaDTO(VillaUpdateDTO source)
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

    public VillaNumberCreateDTO ToVillaNumberCreateDTO(VillaNumberDTO source)
    {
        if (source is null) return null!;
        return new VillaNumberCreateDTO
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public VillaNumberDTO ToVillaNumberDTO(VillaNumberCreateDTO source)
    {
        if (source is null) return null!;
        return new VillaNumberDTO
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public VillaNumberUpdateDTO ToVillaNumberUpdateDTO(VillaNumberDTO source)
    {
        if (source is null) return null!;
        return new VillaNumberUpdateDTO
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public VillaNumberDTO ToVillaNumberDTO(VillaNumberUpdateDTO source)
    {
        if (source is null) return null!;
        return new VillaNumberDTO
        {
            VillaNo = source.VillaNo,
            VillaID = source.VillaID,
            SpecialDetails = source.SpecialDetails,
        };
    }

    public List<VillaCreateDTO> ToVillaCreateDTOList(IEnumerable<VillaDTO> source) =>
        source?.Select(ToVillaCreateDTO).ToList() ?? new List<VillaCreateDTO>();

    public List<VillaDTO> ToVillaDTOList(IEnumerable<VillaCreateDTO> source) =>
        source?.Select(ToVillaDTO).ToList() ?? new List<VillaDTO>();

    public List<VillaUpdateDTO> ToVillaUpdateDTOList(IEnumerable<VillaDTO> source) =>
        source?.Select(ToVillaUpdateDTO).ToList() ?? new List<VillaUpdateDTO>();

    public List<VillaDTO> ToVillaDTOList(IEnumerable<VillaUpdateDTO> source) =>
        source?.Select(ToVillaDTO).ToList() ?? new List<VillaDTO>();

    public List<VillaNumberCreateDTO> ToVillaNumberCreateDTOList(IEnumerable<VillaNumberDTO> source) =>
        source?.Select(ToVillaNumberCreateDTO).ToList() ?? new List<VillaNumberCreateDTO>();

    public List<VillaNumberDTO> ToVillaNumberDTOList(IEnumerable<VillaNumberCreateDTO> source) =>
        source?.Select(ToVillaNumberDTO).ToList() ?? new List<VillaNumberDTO>();

    public List<VillaNumberUpdateDTO> ToVillaNumberUpdateDTOList(IEnumerable<VillaNumberDTO> source) =>
        source?.Select(ToVillaNumberUpdateDTO).ToList() ?? new List<VillaNumberUpdateDTO>();

    public List<VillaNumberDTO> ToVillaNumberDTOList(IEnumerable<VillaNumberUpdateDTO> source) =>
        source?.Select(ToVillaNumberDTO).ToList() ?? new List<VillaNumberDTO>();
}
