using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IPostReceive
    {
        Task receiveAllCommunityPosts(List<CommunityPost> communityPosts);
        Task receiveGroupPosts(List<CommunityPost> groupPosts);
        Task receiveUserPosts(List<CommunityPost> userPosts);
        Task receiveUserGroupPosts(List<CommunityPost> userGroupPosts);
        Task receivePost(CommunityPost communityPost);
        Task receiveInsertStatus(string postId);
        Task receiveUpdateStatus(bool status);
        Task receiveDeleteStatus(bool status);
    }
}
