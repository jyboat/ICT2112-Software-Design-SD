
namespace ClearCare.Interfaces.M3T1
{
    public interface IResourceStrategy
    {
        Task<string> UploadAsync(
            string title,
            string description,
            int uploadedBy,
            string dateCreated,
            byte[] image,
            string coverImageName,
            object? fileOrUrl
        );
    }
}

