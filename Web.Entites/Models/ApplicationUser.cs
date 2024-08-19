

using Microsoft.AspNetCore.Identity;

namespace Web.Entites.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name {  get; set; }
        public string Phone {  get; set; }
        public string City {  get; set; }
        public string Role { get; set; }
    }
}
