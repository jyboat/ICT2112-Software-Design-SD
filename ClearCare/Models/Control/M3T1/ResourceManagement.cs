using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using Google.Cloud.Firestore;


namespace ClearCare.Models.Control.M3T1
{
    public class ResourceManager
    {
        private readonly ResourceGateway _gateway;

        public ResourceManager()
        {
            _gateway = new ResourceGateway();
        }

        public async Task<string> addResource(string title, string description, int uploadedBy, string dateCreated, string coverImageUrl, string targetUrl)
        {
            return await _gateway.insertResource(title, description, uploadedBy, dateCreated, coverImageUrl, targetUrl);
        }


        public async Task<List<Resource>> viewResource()
        {
            return await _gateway.fetchResource();
        }

        public async Task<Resource> getResource(string id)
        {
            return await _gateway.fetchResourceById(id);
        }

        public async Task<bool> updateResource(string id, string title, string description, int uploadedBy)
        {
            return await _gateway.updateResource(id, title, description, uploadedBy);
        }

        public async Task<bool> deleteResource(string id)
        {
            return await _gateway.deleteResource(id);
        }

        public async Task AddResourceCard(string title, string description, int uploadedBy, string coverImageUrl, string targetUrl)
        {
            var db = FirestoreDb.Create("your-project-id");  // Ensure Firestore is initialized

            CollectionReference resources = db.Collection("Resource");
            var resourceData = new Dictionary<string, object>
    {
        { "Title", title },
        { "Description", description },
        { "UploadedBy", uploadedBy },
        { "CoverImageUrl", coverImageUrl },
        { "TargetUrl", targetUrl },
        { "DateCreated", Timestamp.GetCurrentTimestamp().ToDateTime() }
    };

            await resources.AddAsync(resourceData);
        }

    }
}
