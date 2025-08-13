namespace Web.DataAccess.Repositories;
public class GeneralRepository(ILogger<GeneralRepository> _logger)
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
    public async Task<string> SaveImageAsync(IFormFile file,string folderName)
    {
        var imageName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var imagesPath = Path.Combine("wwwroot", folderName);
        var path = Path.Combine(imagesPath, imageName);

        if (!Directory.Exists(imagesPath))
        {
            Directory.CreateDirectory(imagesPath);
        }

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return imageName;
    }
    // add remove image
    public void DeleteImage(string imageName,string folderName)
    {
        if(!string.IsNullOrEmpty(imageName))
        {
            var path = Path.Combine("wwwroot",folderName,imageName);
            if(File.Exists(path))
                File.Delete(path);
        }
    }
}
