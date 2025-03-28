
using ClearCare.Models.Entities.M3T1;


namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IResourceStrategy
    {
        Task<string> uploadAsync(
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

