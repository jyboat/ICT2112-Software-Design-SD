using ClearCare.DataSource.M3T1;

public class YouTubeStrategy : IResourceStrategy
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
        if (string.IsNullOrWhiteSpace(url) || !url.Contains("youtube.com") && !url.Contains("youtu.be"))
            throw new ArgumentException("A valid YouTube URL is required.");

        return await _gateway.insertResource(title, description, uploadedBy, dateCreated, image, coverImageName, url);
    }
}
