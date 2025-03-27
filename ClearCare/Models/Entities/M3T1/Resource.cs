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
        private string Url { get; set; }

        private string getId() => Id;
        private string getTitle() => Title;
        private string getDescription() => Description;
        private int getUploadedBy() => UploadedBy;
        private string getDateCreated() => DateCreated;
        private byte[] getCoverImage() => CoverImage;
        private string getCoverImageName() => CoverImageName;
        private string getTargetUrl() => Url;

        private void setId(string id) => Id = id;
        private void setTitle(string title) => Title = title;
        private void setDescription(string description) => Description = description;
        private void setUploadedBy(int uploadedBy) => UploadedBy = uploadedBy;
        private void setDateCreated(string dateCreated) => DateCreated = dateCreated;
        private void setCoverImage(byte[] image) => CoverImage = image;
        private void setCoverImageName(string coverImageName) => CoverImageName = coverImageName;
        private void setTargetUrl(string url) => Url = url;

        public Resource() { }

        public Resource(string id, string title, string description, int uploadedBy, string dateCreated, byte[] image, string coverImageName, string url)
        {
            Id = id;
            Title = title;
            Description = description;
            UploadedBy = uploadedBy;
            DateCreated = dateCreated;
            CoverImage = image;
            CoverImageName = coverImageName;
            Url = url;
        }

        public Dictionary<string, object> getDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", getId() },
                { "Title", getTitle() },
                { "Description", getDescription() },
                { "UploadedBy", getUploadedBy() },
                { "DateCreated", getDateCreated() },
                { "CoverImage", getCoverImage() },
                { "CoverImageUrl", getCoverImageName() },
                { "Url", getTargetUrl() }
            };
        }
    }
}
