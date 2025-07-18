using Microsoft.AspNetCore.Authentication;
using OneOf;
using Web.Entites.ViewModels;
using Web.Entites.ViewModels.UsersVMs;

namespace Web.Entites.IRepositories;

public interface IAuthRepository
{
    Task<OneOf<List<ValidationError>, bool>> RegisterAsync(RegisterVM request, CancellationToken cancellationToken = default);
    Task<OneOf<List<ValidationError>, bool>> LoginAsync(LoginVM request, CancellationToken cancellationToken = default);
}