

using Microsoft.AspNetCore.Identity;

namespace Web.Entites.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        [MinLength(2)]
        public string Name {  get; set; }
        [Required]
        [Phone]
        [MaxLength(11),MinLength(11)]
        public string Phone {  get; set; }
        [Required]
        public string City {  get; set; }
        public string Role { get; set; }=string.Empty;
    }
}
