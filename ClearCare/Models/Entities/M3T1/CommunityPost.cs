using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;

namespace ClearCare.Models.Entities.M3T1
{
    [FirestoreData]
    public class CommunityPost
    {
        [FirestoreDocumentId]
        private string Id { get; set; }

        [FirestoreProperty]
        private string Title { get; set; }

        [FirestoreProperty]
        private string Content { get; set; }
        //public string FilePath { get; set; }

        [FirestoreProperty]
        private string PostedBy { get; set; }

        [FirestoreProperty]
        private string PostedAt { get; set; }

        [FirestoreProperty]
        private string GroupId { get; set; }

        public CommunityPost() { }

        public CommunityPost(string id, string title, string content, string postedBy, string postedAt, string groupId)
        {
            Id = id;
            Title = title;
            Content = content;
            PostedBy = postedBy;
            PostedAt = postedAt;
            GroupId =  groupId;
        }

        private string getId() => Id;
        private string getTitle() => Title;
        private string getContent() => Content;
        private string getPostedBy() => PostedBy;
        private string getPostedAt() => PostedAt;
        private string getGroupId() => GroupId;

        private void setId(string id) => Id = id;
        private void setTitle(string title) => Title = title;
        private void setContent(string content) => Content = content;
        private void setPostedBy(string postedBy) => PostedBy = postedBy;
        private void setCreationDate(string postedAt) => PostedAt = postedAt;
        private void setGroupId(string groupId) => GroupId = groupId;

        public Dictionary<string, object> getDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", getId() },
                { "Title", getTitle() },
                { "Content", getContent() },
                { "PostedBy", getPostedBy() },
                { "PostedAt", getPostedAt() },
                { "GroupId", getGroupId() }
            };
        }
    }
}
