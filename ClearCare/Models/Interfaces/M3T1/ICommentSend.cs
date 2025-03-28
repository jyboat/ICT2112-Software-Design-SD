using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface ICommentSend
    {
        Task<string> insertComment(string content, string postId, string createdBy);
        Task<List<CommunityComment>> fetchPostComments(string postId);
        Task<bool> deleteComment(string commentId);
        Task<bool> updateCommunityComment(string commentId, string content);
    }
}
