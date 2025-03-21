using ClearCare.Interfaces.M3T1;
using ClearCare.DataSource.M3T1;

public class ArticleUrlStrategy : IResourceStrategy
{
    private readonly ResourceGateway _gateway = new ResourceGateway();

    public async Task<string> UploadAsync(
        string title,
        string description,
        int uploadedBy,
        string dateCreated,
        Stream? fileStream,
        string? fileName,
        string? contentType,
        string? articleUrl,
        string coverImageUrl)
    {
        if (string.IsNullOrWhiteSpace(articleUrl))
            throw new ArgumentException("Article URL is required.");

        // Save everything to Firestore using the gateway
        return await _gateway.insertResource(title, description, uploadedBy, dateCreated, coverImageUrl, articleUrl);
    }
}
