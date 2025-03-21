using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Entities.M3T1;

namespace ClearCare.DataSource.M3T1
{
    public class ResourceGateway
    {
        private readonly FirestoreDb _db;

        public ResourceGateway()
        {
            _db = FirebaseService.Initialize();
        }

        public async Task<string> insertResource(string title, string description, int uploadedBy, string dateCreated, byte[] image, string coverImageName, string? targetUrl)
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
        { "TargetUrl", targetUrl }
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
                    string title = doc.ContainsField("Title") ? doc.GetValue<string>("Title") : "";
                    string description = doc.ContainsField("Description") ? doc.GetValue<string>("Description") : "";
                    int uploadedBy = doc.ContainsField("UploadedBy") ? doc.GetValue<int>("UploadedBy") : 0;
                    string dateCreated = doc.ContainsField("DateCreated") ? doc.GetValue<string>("DateCreated") : "";
                    byte[] coverImage = doc.ContainsField("CoverImage") ? doc.GetValue<byte[]>("CoverImage") : Array.Empty<byte>();
                    string coverImageName = doc.ContainsField("CoverImageName") ? doc.GetValue<string>("CoverImageName") : "";
                    string targetUrl = doc.ContainsField("TargetUrl") ? doc.GetValue<string>("TargetUrl") : "";

                    resources.Add(new Resource(
                        doc.Id,
                        title,
                        description,
                        uploadedBy,
                        dateCreated,
                        coverImage,
                        coverImageName,
                        targetUrl
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
                snapshot.GetValue<string>("DateCreated"),
                snapshot.GetValue<byte[]>("CoverImage"),
                snapshot.GetValue<string>("CoverImageName"),
                snapshot.GetValue<string>("TargetUrl")
            );
        }



        public async Task<bool> updateResource(string id, string title, string description, int uploadedBy, byte[] image, string coverImageName, string targetUrl)
        {
            DocumentReference docRef = _db.Collection("Resource").Document(id);

            var updatedData = new Dictionary<string, object>
            {
                { "Title", title },
                { "Description", description },
                { "UploadedBy", uploadedBy },
                { "CoverImage", image },
                { "CoverImageName", coverImageName },
                { "TargetUrl", targetUrl }
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
