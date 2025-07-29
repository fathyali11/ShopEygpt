using Mapster;
using Web.Entites.Models;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.Entites.Mappings;
public class UserMapping:IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterVM, ApplicationUser>();
    }
}
