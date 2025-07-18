using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.Entites.Mappings;
public class UserMapping
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<RegisterVM, ApplicationUser>.NewConfig()
            .Map(dest => dest.UserName, src => src.Email);
    }
}
