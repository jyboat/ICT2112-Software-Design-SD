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

        private string GetId() => Id;
        private string GetTitle() => Title;
        private string GetContent() => Content;
        private string GetPostedBy() => PostedBy;
        private string GetPostedAt() => PostedAt;
        private string GetGroupId() => GroupId;

        private void SetId(string id) => Id = id;
        private void SetTitle(string title) => Title = title;
        private void SetContent(string content) => Content = content;
        private void SetPostedBy(string postedBy) => PostedBy = postedBy;
        private void SetCreationDate(string postedAt) => PostedAt = postedAt;
        private void SetGroupId(string groupId) => GroupId = groupId;

        public Dictionary<string, object> getDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "Title", GetTitle() },
                { "Content", GetContent() },
                { "PostedBy", GetPostedBy() },
                { "PostedAt", GetPostedAt() },
                { "GroupId", GetGroupId() }
            };
        }
    }
}
