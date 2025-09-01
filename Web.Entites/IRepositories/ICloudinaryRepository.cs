namespace Web.Entites.IRepositories;
public interface ICloudinaryRepository
{
    Task<string?> UploadImageAsync(IFormFile file);
    Task<bool> DeleteImageAsync(string imageUrl);
    Task<string?> UpdateImageAsync(string oldImageUrl, IFormFile newFile);
    string? GetPublicIdFromUrl(string imageUrl);
}
