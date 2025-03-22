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

        private string GetId() => Id;
        private string GetContent() => Content;
        private string GetCreatedBy() => CreatedBy;
        private string GetPostId() => PostId;
        private string GetCreatedAt() => CreatedAt;

        private void SetId(string id) => Id = id;
        private void SetContent(string content) => Content = content;
        private void SetCreatedBy(string createdBy) => CreatedBy = createdBy;
        private void SetPostId(string postId) => PostId = postId;
        private void SetCreatedAt(string createdAt) => CreatedAt = createdAt;

        public Dictionary<string, object> getDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "Content", GetContent() },
                { "CreatedBy", GetCreatedBy() },
                { "PostId", GetPostId() },
                { "CreatedAt", GetCreatedAt() }
            };
        }
    }
}
