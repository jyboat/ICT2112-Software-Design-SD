using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

namespace ClearCare.Models.Entities.M3T1
{
    [FirestoreData]
    public class CommunityComment
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Content { get; set; }
        [FirestoreProperty]
        public string CreatedBy { get; set; }
        [FirestoreProperty]
        public string PostId { get; set; }
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        public CommunityComment() { }

        public CommunityComment(string content, string createdBy, string postId, DateTime createdAt)
        {
            Content = content;
            CreatedBy = createdBy;
            PostId = postId;
            CreatedAt = createdAt;
        }

        public string GetId() => Id;
        public string GetContent() => Content;
        public string GetCreatedBy() => CreatedBy;
        public string GetPostId() => PostId;
        public DateTime? GetCreatedAt() => CreatedAt;

        public void SetId(string id) => Id = id;
        public void SetContent(string content) => Content = content;
        public void SetCreatedBy(string createdBy) => CreatedBy = createdBy;
        public void SetPostId(string postId) => PostId = postId;
        public void SetCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
    }
}
