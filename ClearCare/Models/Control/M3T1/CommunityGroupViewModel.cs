using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityGroupViewModel
    {
        public CommunityGroup CommunityGroup { get; set; }
        public IEnumerable<CommunityPost> Posts { get; set; }

        public CommunityGroupViewModel(CommunityGroup communityGroup, IEnumerable<CommunityPost> posts)
        {
            CommunityGroup = communityGroup;
            Posts = posts;
        }
    }
}
