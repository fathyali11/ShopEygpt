namespace Web.Entites.ModelsValidation;
public static class ImageSignatureValidator
{
    private static readonly Dictionary<string, byte[]> _imageSignatures = new()
    {
        // JPEG / JPG
        { "jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },

        // PNG
        { "png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } }
    };
    public static bool IsValidImage(IFormFile image)
    {
        if (image == null || image.Length < 4)
            return false;

        try
        {
            using var stream = image.OpenReadStream();
            Span<byte> header = stackalloc byte[8];
            int bytesRead = stream.Read(header);

            foreach (var signature in _imageSignatures.Values)
            {
                if (bytesRead >= signature.Length && header[..signature.Length].SequenceEqual(signature))
                    return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }
}
