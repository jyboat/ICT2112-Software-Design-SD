using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Interfaces.M3T1
{
    public interface IGroupSend
    {
        Task<List<CommunityGroup>> fetchCommunityGroups();
        Task<CommunityGroup> fetchGroupById(string groupId);
        Task<string> insertGroup(string name, string description, string ownerId, List<string> memberIds);
        Task<bool> updateGroup(string groupId, string name, string description);
        Task<bool> updateGroupMembers(string groupId, List<string> memberIds);
        Task<bool> deleteGroup(string groupId);
    }
}
