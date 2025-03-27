using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IGroupReceive
    {
        Task receiveAllCommunityGroups(List<CommunityGroup> communityGroups);
        Task receiveUserGroups(List<CommunityGroup> communityGroups);
        Task receiveNonUserGroups(List<CommunityGroup> communityGroups);
        Task receiveGroup(CommunityGroup communityGroup);
        Task receiveInsertStatus(string groupId);
        Task receiveUpdateStatus(bool status);
        Task receiveAddMemberStatus(bool status);
        Task receiveRemoveMemberStatus(bool status);
        Task receiveDeleteStatus(bool status);
    }
}
