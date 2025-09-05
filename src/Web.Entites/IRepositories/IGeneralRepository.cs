namespace Web.Entites.IRepositories;
public interface IGeneralRepository
{
    Task<List<ValidationError>?> ValidateRequest<TSource, TRequest>(TSource source, TRequest request)
        where TSource : IValidator<TRequest>
        where TRequest : class;

}
