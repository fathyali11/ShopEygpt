namespace Web.Entites.Mappings;
public class UserMapping:IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterVM, ApplicationUser>();
    }
}
