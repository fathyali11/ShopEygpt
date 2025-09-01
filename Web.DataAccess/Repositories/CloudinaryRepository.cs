using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Account = CloudinaryDotNet.Account;
namespace Web.DataAccess.Repositories;
public class CloudinaryRepository: ICloudinaryRepository
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryRepository(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }
    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return uploadResult.SecureUrl.ToString(); 
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return false;

        
        var publicId = GetPublicIdFromUrl(imageUrl);
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        return result.Result == "ok";
    }

    public async Task<string?> UpdateImageAsync(string oldImageUrl, IFormFile newFile)
    {
        var isDeleted = false;
        if (!string.IsNullOrEmpty(oldImageUrl))
            isDeleted=await DeleteImageAsync(oldImageUrl);
        if(isDeleted)
        {
            var imageUrl = await UploadImageAsync(newFile);
            return imageUrl is not null ? imageUrl : null;
        }
        return null;
    }

    public string? GetPublicIdFromUrl(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var filename = Path.GetFileNameWithoutExtension(uri.Segments.Last());
            return filename;
        }
        catch
        {
            return null;
        }
    }

}
