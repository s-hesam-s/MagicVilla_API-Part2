using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace MagicVilla_VillaAPI.Security;

public static class VillaPatchPolicy
{
    private static readonly HashSet<string> paths = new(StringComparer.OrdinalIgnoreCase)
    { "/name", "/details", "/rate", "/occupancy", "/sqft", "/amenity" };

    public static bool Allows(JsonPatchDocument<VillaUpdateDTO>? patch) => patch != null &&
        patch.Operations.Count > 0 && patch.Operations.Count <= 32 &&
        patch.Operations.All(operation => operation.op == "replace" && operation.path != null && paths.Contains(operation.path));
}
