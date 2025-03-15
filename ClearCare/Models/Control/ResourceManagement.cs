using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.DataSource;
using ClearCare.Models.Entities;

namespace ClearCare.Models.Controls
{
    public class ResourceManager
    {
        private readonly ResourceGateway _gateway;

        public ResourceManager()
        {
            _gateway = new ResourceGateway();
        }

        public async Task<string> addResource(string title, string description, int uploadedBy, string dateCreated)
        {
            return await _gateway.insertResource(title, description, uploadedBy, dateCreated);
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
    }
}
