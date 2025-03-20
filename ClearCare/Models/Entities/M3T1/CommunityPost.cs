using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;

namespace ClearCare.Models.Entities.M3T1
{
    [FirestoreData]
    public class CommunityPost
    {
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Content { get; set; }
        //public string FilePath { get; set; }

        [FirestoreProperty]
        public string PostedBy { get; set; }

        [FirestoreProperty]
        public DateTime PostedAt { get; set; }

        [FirestoreProperty]
        public string GroupId { get; set; }

        public CommunityPost() { }

        public CommunityPost(string title, string content, string postedBy, DateTime postedAt, string groupId)
        {
            Title = title;
            Content = content;
            PostedBy = postedBy;
            PostedAt = postedAt;
            GroupId =  groupId;
        }

        public string GetId() => Id;
        public string GetTitle() => Title;
        public string GetContent() => Content;
        public string GetPostedBy() => PostedBy;
        public DateTime? GetPostedAt() => PostedAt;
        public string GetGroupId() => GroupId;

        public void SetId(string id) => Id = id;
        public void SetTitle(string title) => Title = title;
        public void SetContent(string content) => Content = content;
        public void SetPostedBy(string postedBy) => PostedBy = postedBy;
        public void SetCreationDate(DateTime postedAt) => PostedAt = postedAt;
        public void SetGroupId(string groupId) => GroupId = groupId;
    }
}
