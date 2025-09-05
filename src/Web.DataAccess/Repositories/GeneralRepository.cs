namespace Web.DataAccess.Repositories;
public class GeneralRepository(ILogger<GeneralRepository> _logger): IGeneralRepository
{
    public async Task<List<ValidationError>?> ValidateRequest<TSource, TRequest>(TSource source, TRequest request)
        where TSource : IValidator<TRequest>
        where TRequest : class
    {
        _logger.LogInformation("Validating request of type: {RequestType}", typeof(TRequest).Name);

        var validationResult = await source.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new ValidationError(
                    e.PropertyName,
                    e.ErrorMessage
                )).ToList();
            _logger.LogWarning("Validation failed for request type: {RequestType}, Errors: {Errors}", typeof(TRequest).Name, errors);
            return errors;
        }

        _logger.LogInformation("Validation successful for request type: {RequestType}", typeof(TRequest).Name);
        return null;
    }
}
