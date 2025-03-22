using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.ViewModels.M3T1
{
    public class CommunityViewModel
    {
        public CommunityGroup? CommunityGroup { get; set; }

        public CommunityPost? CommunityPost { get; set; }

        public List<Dictionary<string, object>>? AllGroups { get; set; }
        public List<Dictionary<string, object>>? UserGroups { get; set; }
        public List<Dictionary<string, object>>? Posts { get; set; }
        public List<Dictionary<string, object>>? UserPosts { get; set; }

    }
}
