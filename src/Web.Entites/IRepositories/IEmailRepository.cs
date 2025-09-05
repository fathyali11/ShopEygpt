namespace Web.Entites.IRepositories;
public interface IEmailRepository
{
    Task SendEmailAsync(string to, string subject, string body);
}
