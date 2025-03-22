using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using System.Text.RegularExpressions;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityPostManagement
    {
        private CommunityDataMapper _dataMapper = new CommunityDataMapper();

        public async Task<string> createPost(string title, string content, string postedBy)
        {
            return await _dataMapper.insertPost(title, content, postedBy);
        }

        public async Task<List<CommunityPost>> viewPosts()
        {
            return await _dataMapper.fetchCommunityPosts();
        }

        public async Task<List<CommunityPost>> viewGroupPosts(string groupId)
        {
            return await _dataMapper.fetchGroupPosts(groupId);
        }

        public async Task<CommunityPost> viewPost(string id)
        {
            return await _dataMapper.fetchPostById(id);
        }

        public async Task<bool> deletePost(string postId)
        {
            return await _dataMapper.deletePost(postId);
        }

        public async Task<bool> updatePost(string postId, string title, string content)
        {
            return await _dataMapper.updateCommunityPost(postId, title, content);
        }
    }
}
