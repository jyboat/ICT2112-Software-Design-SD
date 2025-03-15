using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Entities;

namespace ClearCare.DataSource
{
    public class ResourceGateway
    {
        private readonly FirestoreDb _db;

        public ResourceGateway()
        {
            _db = FirebaseService.Initialize();
        }

        public async Task<string> insertResource(string title, string description, int uploadedBy, string dateCreated)
        {
            DocumentReference docRef = _db.Collection("Resource").Document();

            var resource = new Dictionary<string, object>
            {
                { "Title", title },
                { "Description", description },
                { "UploadedBy", uploadedBy },
                { "DateCreated", dateCreated }
            };

            await docRef.SetAsync(resource);
            return docRef.Id;
        }

        public async Task<List<Resource>> fetchResource()
        {
            List<Resource> resources = new List<Resource>();
            QuerySnapshot snapshot = await _db.Collection("Resource").GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    resources.Add(new Resource(
                        doc.Id,
                        doc.GetValue<string>("Title"),
                        doc.GetValue<string>("Description"),
                        doc.GetValue<int>("UploadedBy"),
                        doc.GetValue<string>("DateCreated")
                    ));
                }
            }

            return resources;
        }

      public async Task<Resource> fetchResourceById(string id)
{
    if (string.IsNullOrEmpty(id))
    {
        return null;
    }

    DocumentReference docRef = _db.Collection("Resource").Document(id);
    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

    if (!snapshot.Exists)
    {
        return null; // Ensure null is returned instead of causing an error
    }

    return new Resource(
        id,
        snapshot.GetValue<string>("Title"),
        snapshot.GetValue<string>("Description"),
        snapshot.GetValue<int>("UploadedBy"),
        snapshot.GetValue<string>("DateCreated")
    );
}



        public async Task<bool> updateResource(string id, string title, string description, int uploadedBy)
        {
            DocumentReference docRef = _db.Collection("Resource").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "Title", title },
                { "Description", description },
                { "UploadedBy", uploadedBy }
            };

            await docRef.UpdateAsync(updatedData);
            return true;
        }

        public async Task<bool> deleteResource(string id)
        {
            await _db.Collection("Resource").Document(id).DeleteAsync();
            return true;
        }
    }
}
