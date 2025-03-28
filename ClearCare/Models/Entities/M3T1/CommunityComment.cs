using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

namespace ClearCare.Models.Entities.M3T1
{
    [FirestoreData]
    public class CommunityComment
    {
        [FirestoreDocumentId]
        private string Id { get; set; }
        [FirestoreProperty]
        private string Content { get; set; }
        [FirestoreProperty]
        private string CreatedBy { get; set; }
        [FirestoreProperty]
        private string PostId { get; set; }
        [FirestoreProperty]
        private string CreatedAt { get; set; }

        public CommunityComment() { }

        public CommunityComment(string id, string content, string createdBy, string postId, string createdAt)
        {
            Id = id;
            Content = content;
            CreatedBy = createdBy;
            PostId = postId;
            CreatedAt = createdAt;
        }

        private string getId() => Id;
        private string getContent() => Content;
        private string getCreatedBy() => CreatedBy;
        private string getPostId() => PostId;
        private string getCreatedAt() => CreatedAt;

        private void setId(string id) => Id = id;
        private void setContent(string content) => Content = content;
        private void setCreatedBy(string createdBy) => CreatedBy = createdBy;
        private void setPostId(string postId) => PostId = postId;
        private void setCreatedAt(string createdAt) => CreatedAt = createdAt;

        public Dictionary<string, object> getDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", getId() },
                { "Content", getContent() },
                { "CreatedBy", getCreatedBy() },
                { "PostId", getPostId() },
                { "CreatedAt", getCreatedAt() }
            };
        }
    }
}
