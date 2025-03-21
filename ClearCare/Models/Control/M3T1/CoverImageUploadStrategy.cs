using ClearCare.DataSource.M3T1;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

public class CoverImageUploadStrategy
{
    private readonly ResourceGateway _gateway = new ResourceGateway();
    private readonly string _bucketName = "your-bucket.appspot.com"; // Replace with actual Firebase bucket name

    public async Task<string> UploadCoverImageAsync(IFormFile coverImage)
    {
        if (coverImage == null || coverImage.Length == 0)
            throw new ArgumentException("Cover image is required.");

        var storage = StorageClient.Create();
        string objectName = $"{Guid.NewGuid()}{Path.GetExtension(coverImage.FileName)}";

        using var stream = coverImage.OpenReadStream();
        await storage.UploadObjectAsync(
            _bucketName,
            objectName,
            coverImage.ContentType,
            stream,
            new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
        );

        // Generate public URL
        string coverImageUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";

        return coverImageUrl;
    }
}
