using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityPostManagement
    {
        private CommunityDataMapper _dataMapper = new CommunityDataMapper();

        public async Task createPost(string title, string content, string postedBy, DateTime postedAt, string groupId)
        {
            var post = new CommunityPost(title, content, postedBy, postedAt, groupId);
            
            await _dataMapper.insertPost(post);
        }

        public async Task<List<CommunityPost>> viewPosts()
        {
            return await _dataMapper.fetchCommunityPosts();
        }

        public async Task<List<CommunityPost>> viewGroupPosts(string groupId)
        {
            return await _dataMapper.fetchGroupPosts(groupId);
        }
    }
}
