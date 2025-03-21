namespace ClearCare.Interfaces.M3T1
{
 public interface IResourceStrategy
{
    Task<string> UploadAsync(
        string title,
        string description,
        int uploadedBy,
        string dateCreated,
        Stream? fileStream,
        string? fileName,
        string? contentType,
        string? articleUrl,
        string coverImageUrl
    );
}

}
