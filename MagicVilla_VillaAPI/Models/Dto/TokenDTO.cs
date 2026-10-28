using System.ComponentModel.DataAnnotations;
namespace MagicVilla_VillaAPI.Models.Dto
{
    public class TokenDTO
    {
        [Required, MaxLength(8192)]
        public string AccessToken { get; set; }
        [Required, MaxLength(256)]
        public string RefreshToken { get; set; }
    }
}
