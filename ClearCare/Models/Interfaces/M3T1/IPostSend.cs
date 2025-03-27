using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IPostSend
    {
        Task<List<CommunityPost>> fetchCommunityPosts();
        Task<List<CommunityPost>> fetchGroupPosts(string groupId);
        Task<List<CommunityPost>> fetchUserPosts(string userId);
        Task<CommunityPost> fetchPostById(string postId);
        Task<string> insertPost(string title, string content, string postedBy, string? groupId);
        Task<bool> updateCommunityPost(string postId, string title, string content);
        Task<bool> deletePost(string postId);
    }
}
