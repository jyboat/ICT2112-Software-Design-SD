using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface ICommentReceive
    {
        Task receiveAllPostComments(List<CommunityComment> comments);
        Task receiveComment(CommunityComment comment);
        Task receiveInsertStatus(string commentId);
        Task receiveUpdateStatus(bool status);
        Task receiveDeleteStatus(bool status);
    }
}
