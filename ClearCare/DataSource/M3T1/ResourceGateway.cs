using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.DataSource.M3T1
{
    public class ResourceGateway : IResourceSend
    {
        private readonly FirestoreDb _db;
        private IResourceReceive _receiver;


        public ResourceGateway()
        {
            _db = FirebaseService.Initialize();
        }

        public IResourceReceive receiver
        {
            get => _receiver;
            set => _receiver = value;
        }

        public async Task<string> insertResource(string title, string description, string uploadedBy, string dateCreated, byte[] image, string coverImageName, string? url)
        {
            DocumentReference docRef = _db.Collection("Resource").Document();

            var resource = new Dictionary<string, object>
            {
                { "Title", title },
                { "Description", description },
                { "UploadedBy", uploadedBy },
                { "DateCreated", dateCreated },
                { "CoverImage", image },
                { "CoverImageName", coverImageName },
                { "Url", url }
            };

            await docRef.SetAsync(resource);
            if (_receiver != null)
{
    await _receiver.receiveInsertStatus(true);
}
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
                    string title = doc.ContainsField("Title") ? doc.GetValue<string>("Title") : "";
                    string description = doc.ContainsField("Description") ? doc.GetValue<string>("Description") : "";
string uploadedBy = doc.ContainsField("UploadedBy") ? doc.GetValue<object>("UploadedBy")?.ToString() ?? "" : "";
                    string dateCreated = doc.ContainsField("DateCreated") ? doc.GetValue<string>("DateCreated") : "";
                    byte[] coverImage = doc.ContainsField("CoverImage") ? doc.GetValue<byte[]>("CoverImage") : Array.Empty<byte>();
                    string coverImageName = doc.ContainsField("CoverImageName") ? doc.GetValue<string>("CoverImageName") : "";
                    string url = doc.ContainsField("Url") ? doc.GetValue<string>("Url") : "";

                    resources.Add(new Resource(
                        doc.Id,
                        title,
                        description,
                        uploadedBy,
                        dateCreated,
                        coverImage,
                        coverImageName,
                        url
                    ));
                }
            }
            await _receiver.receiveResources(resources);
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
                return null;
            }

            Resource resource = new Resource(
    id,
    snapshot.GetValue<string>("Title"),
    snapshot.GetValue<string>("Description"),
    snapshot.GetValue<string>("UploadedBy"),
    snapshot.GetValue<string>("DateCreated"),
    snapshot.GetValue<byte[]>("CoverImage"),
    snapshot.GetValue<string>("CoverImageName"),
    snapshot.GetValue<string>("Url")
);

            await _receiver.receiveResource(resource);
            return new Resource(
                id,
                snapshot.GetValue<string>("Title"),
                snapshot.GetValue<string>("Description"),
                snapshot.GetValue<string>("UploadedBy"),
                snapshot.GetValue<string>("DateCreated"),
                snapshot.GetValue<byte[]>("CoverImage"),
                snapshot.GetValue<string>("CoverImageName"),
                snapshot.GetValue<string>("Url")
            );
        }

        public async Task<bool> updateResource(string id, string title, string description, string uploadedBy, byte[] image, string coverImageName, string url)
        {
            DocumentReference docRef = _db.Collection("Resource").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "Title", title },
                { "Description", description },
                { "UploadedBy", uploadedBy },
                { "CoverImage", image },
                { "CoverImageName", coverImageName },
                { "Url", url }
            };

            await docRef.UpdateAsync(updatedData);
            await _receiver.receiveUpdateStatus(true);
            return true;
        }

        public async Task<bool> deleteResource(string id)
        {
            await _db.Collection("Resource").Document(id).DeleteAsync();
            await _receiver.receiveDeleteStatus(true);
            return true;
        }
    }
}
