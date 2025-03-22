using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.ViewModels.M3T1
{
    public class CommunityViewModel
    {
        public CommunityGroup CommunityGroup { get; set; }

        public CommunityPost CommunityPost { get; set; }
        public IEnumerable<CommunityPost> Posts { get; set; }
        public IEnumerable<CommunityComment> Comments { get; set; }

        // Default constructor (for model binding)
        public CommunityViewModel() { }

        // Constructor for community group with posts
        public CommunityViewModel(CommunityGroup communityGroup, IEnumerable<CommunityPost> posts)
        {
            CommunityGroup = communityGroup;
            Posts = posts;
        }

        // Constructor for a single post with comments
        public CommunityViewModel(CommunityPost communityPost, IEnumerable<CommunityComment> comments)
        {
            CommunityPost = communityPost;
            Comments = comments;
        }
    }
}
