using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Interfaces.M3T1;



namespace ClearCare.Models.Control.M3T1
{
    public class ResourceManager
    {
        private readonly ResourceGateway _gateway;

        public ResourceManager()
        {
            _gateway = new ResourceGateway();
        }
        public async Task<string> ProcessResourceWithStrategy(
           string title,
            string description,
            int uploadedBy,
            string dateCreated,
            byte[] image,
            string coverImageName,
            object? fileOrUrl,
            string resourceType)
        {
            IResourceStrategy strategy = resourceType.ToLower() switch
            {
                "article" => new ArticleUrlStrategy(),
                "video" => new VideoUploadStrategy(),
                _ => throw new ArgumentException("Unsupported resource type")
            };

            return await strategy.UploadAsync(
                title,
                description,
                uploadedBy,
                dateCreated,
                image,
                coverImageName,
                fileOrUrl
            );
        }

        public async Task<string> addResource(string title, string description, int uploadedBy, string dateCreated, byte[] image, string coverImageName, string? url)
        {
            return await _gateway.insertResource(title, description, uploadedBy, dateCreated, image, coverImageName, url);
        }


        public async Task<List<Resource>> viewResource()
        {
            return await _gateway.fetchResource();
        }

        public async Task<Resource> getResource(string id)
        {
            return await _gateway.fetchResourceById(id);
        }

        public async Task<bool> updateResource(string id, string title, string description, int uploadedBy, byte[] image, string coverImageName, string? url)
        {
            return await _gateway.updateResource(id, title, description, uploadedBy, image, coverImageName, url);
        }

        public async Task<bool> deleteResource(string id)
        {
            return await _gateway.deleteResource(id);
        }

    }
}
