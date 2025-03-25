using ClearCare.DataSource.M3T1;

public class ArticleUrlStrategy : IResourceStrategy
{
    private readonly ResourceGateway _gateway = new ResourceGateway();

    public async Task<string> UploadAsync(
        string title,
        string description,
        int uploadedBy,
        string dateCreated,
        byte[] image,
        string coverImageName,
        string? url
                )
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Article URL is required.");

        return await _gateway.insertResource(title, description, uploadedBy, dateCreated, image, coverImageName, url);
    }
}
