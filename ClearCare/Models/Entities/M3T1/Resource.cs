using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace ClearCare.Models.Entities.M3T1
{
    public class Resource
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Title { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public int UploadedBy { get; set; }
        [FirestoreProperty]
        public string DateCreated { get; set; }

        public Resource() { }

        public Resource(string id, string title, string description, int uploadedBy, string dateCreated)
        {
            Id = id;
            Title = title;
            Description = description;
            UploadedBy = uploadedBy;
            DateCreated = dateCreated;
        }

        public Dictionary<string, object> GetDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", Id },
                { "Title", Title },
                { "Description", Description },
                { "UploadedBy", UploadedBy },
                { "DateCreated", DateCreated }
            };
        }
    }
}
