using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.DTO.M3T1
{
    public class CommunityDTO
    {
        public bool GroupView { get; set; } = false;
        public Dictionary<string, object>? Group { get; set; }  
        public List<string>? GroupMembers { get; set; }

        public List<Dictionary<string, object>>? AllGroups { get; set; }
        public List<Dictionary<string, object>>? UserGroups { get; set; }
        public List<Dictionary<string, object>>? Posts { get; set; }
        public List<Dictionary<string, object>>? UserPosts { get; set; }

    }
}
