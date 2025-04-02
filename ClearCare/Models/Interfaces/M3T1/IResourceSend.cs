using ClearCare.Models.Entities.M3T1;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IResourceSend
    {
        Task<List<Resource>> fetchResource();
        Task<Resource> fetchResourceById(string id);
        Task<string> insertResource(string title, string description, int uploadedBy, string dateCreated, byte[] image, string coverImageName, string? url);
        Task<bool> updateResource(string id, string title, string description, byte[] image, string coverImageName, string url);
        Task<bool> deleteResource(string id);
    }
}
