using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IResponseReceive
    {
        Task receiveResponses(List<FeedbackResponse> responses);

        Task receiveResponseByFeedbackId(FeedbackResponse response);

        Task receiveAddResponse(bool success);

        Task receiveUpdateResponse(bool success);

        Task receiveDeleteResponse(bool success);
    }
}
