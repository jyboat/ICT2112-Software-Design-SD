//using Google.Cloud.Storage.V1;
//using System.IO;
//using System.Threading.Tasks;
//using ClearCare.Interfaces.M3T1;
//using ClearCare.DataSource.M3T1;

//public class VideoUploadStrategy : IResourceStrategy
//{
//    private readonly ResourceGateway _gateway = new ResourceGateway();
//    private readonly string _bucketName = "your-bucket.appspot.com";

//    public async Task<string> UploadAsync(
//        string title,
//        string description,
//        int uploadedBy,
//        string dateCreated,
//        Stream? fileStream,
//        string? fileName,
//        string? contentType,
//        string? articleUrl,
//        string coverImageUrl)
//    {
//        if (fileStream == null || string.IsNullOrWhiteSpace(fileName))
//            throw new ArgumentException("Video file must be provided.");

//        var storage = StorageClient.Create();
//        string objectName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

//        await storage.UploadObjectAsync(
//            _bucketName,
//            objectName,
//            contentType,
//            fileStream,
//            new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
//        );

//        string videoUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";

//        // Save everything to Firestore using the gateway
//        return await _gateway.insertResource(title, description, uploadedBy, dateCreated, coverImageUrl, videoUrl);
//    }
//}

