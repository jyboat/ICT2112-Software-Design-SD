using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.Control.M3T1
{
    public class ResourceManager : IResourceReceive
    {
        private readonly IResourceSend _gateway;

        public ResourceManager(IResourceSend gateway)
        {
            _gateway = gateway;
        }

        // Callback Methods
        public Task receiveResources(List<Resource> resources)
        {
            Console.WriteLine($"Received {resources.Count} resources");
            return Task.CompletedTask;
        }

        public Task receiveResource(Resource resource)
        {
            Console.WriteLine("Received a resource");
            return Task.CompletedTask;
        }

        public Task receiveInsertStatus(bool success)
        {
            Console.WriteLine(success ? "Inserted resource successfully" : "Failed to insert resource");
            return Task.CompletedTask;
        }

        public Task receiveUpdateStatus(bool success)
        {
            Console.WriteLine(success ? "Updated resource successfully" : "Failed to update resource");
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(bool success)
        {
            Console.WriteLine(success ? "Deleted resource successfully" : "Failed to delete resource");
            return Task.CompletedTask;
        }

        // Gateway Forwarding Methods
        public Task<string> addResource(string title, string description, string uploadedBy, string dateCreated, byte[] image, string coverImageName, string? url)
        {
            return _gateway.insertResource(title, description, uploadedBy, dateCreated, image, coverImageName, url);
        }

        public Task<List<Resource>> viewResource()
        {
            return _gateway.fetchResource();
        }

        public Task<Resource> getResource(string id)
        {
            return _gateway.fetchResourceById(id);
        }

        public Task<bool> updateResource(string id, string title, string description, string uploadedBy, byte[] image, string coverImageName, string? url)
        {
            return _gateway.updateResource(id, title, description, uploadedBy, image, coverImageName, url!);
        }

        public Task<bool> deleteResource(string id)
        {
            return _gateway.deleteResource(id);
        }

        public async Task<string> processResourceWithStrategy(
            string title,
            string description,
            string uploadedBy,
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

            return await strategy.uploadAsync(
                title,
                description,
                uploadedBy,
                dateCreated,
                image,
                coverImageName,
                fileOrUrl
            );
        }
    }
}
