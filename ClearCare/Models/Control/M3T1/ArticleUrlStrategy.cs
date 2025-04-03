using ClearCare.DataSource.M3T1;
using ClearCare.Models.Interfaces.M3T1;


public class ArticleUrlStrategy : IResourceStrategy
{
    private readonly ResourceGateway _gateway = new ResourceGateway();

    public async Task<string> uploadAsync(
        string title,
        string description,
        string uploadedBy,
        string dateCreated,
        byte[] image,
        string coverImageName,
        object? fileOrUrl
                )
    {
            string? url = fileOrUrl as string;

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Article URL is required.");

        return await _gateway.insertResource(title, description, uploadedBy, dateCreated, image, coverImageName, url);
    }
}
