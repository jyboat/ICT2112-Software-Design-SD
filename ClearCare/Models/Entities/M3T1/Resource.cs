using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace ClearCare.Models.Entities.M3T1
{
    public class Resource
    {
        [FirestoreProperty]
        private string Id { get; set; }
        [FirestoreProperty]
        private string Title { get; set; }
        [FirestoreProperty]
        private string Description { get; set; }
        [FirestoreProperty]
        private int UploadedBy { get; set; }
        [FirestoreProperty]
        private string DateCreated { get; set; }
        [FirestoreProperty]
        private byte[] CoverImage { get; set; }
        [FirestoreProperty]
        private string CoverImageName { get; set; }
        [FirestoreProperty]
        private string TargetUrl { get; set; }

        private string GetId() => Id;
        private string GetTitle() => Title;
        private string GetDescription() => Description;
        private int GetUploadedBy() => UploadedBy;
        private string GetDateCreated() => DateCreated;
        private byte[] GetCoverImage() => CoverImage;
        private string GetCoverImageName() => CoverImageName;
        private string GetTargetUrl() => TargetUrl;

        private void SetId(string id) => Id = id;
        private void SetTitle(string title) => Title = title;
        private void SetDescription(string description) => Description = description;
        private void SetUploadedBy(int uploadedBy) => UploadedBy = uploadedBy;
        private void SetDateCreated(string dateCreated) => DateCreated = dateCreated;
        private void SetCoverImage(byte[] image) => CoverImage = image;
        private void SetCoverImageName(string coverImageName) => CoverImageName = coverImageName;
        private void SetTargetUrl(string targetUrl) => TargetUrl = targetUrl;

        public Resource() { }

        public Resource(string id, string title, string description, int uploadedBy, string dateCreated, byte[] image, string coverImageName, string targetUrl)
        {
            Id = id;
            Title = title;
            Description = description;
            UploadedBy = uploadedBy;
            DateCreated = dateCreated;
            CoverImage = image;
            CoverImageName = coverImageName;
            TargetUrl = targetUrl;
        }

        public Dictionary<string, object> GetDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "Title", GetTitle() },
                { "Description", GetDescription() },
                { "UploadedBy", GetUploadedBy() },
                { "DateCreated", GetDateCreated() },
                { "CoverImage", GetCoverImage() },
                { "CoverImageUrl", GetCoverImageName() },
                { "TargetUrl", GetTargetUrl() }
            };
        }
    }
}
