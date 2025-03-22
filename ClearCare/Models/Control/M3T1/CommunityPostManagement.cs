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

        public async Task<CommunityPost> viewPost(string postId)
        {
            return await _dataMapper.fetchPostById(postId);
        }

        public async Task deletePost(string postId)
        {
            try
            {
                await _dataMapper.deletePost(postId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post: {ex.Message}");
                throw;
            }
        }

        public async Task updatePost(string postId, string title, string content)
        {
            try
            {
                // Retrieve the existing post by ID
                var existingPost = await _dataMapper.fetchPostById(postId);
                if (existingPost == null)
                {
                    throw new Exception("Post not found.");
                }

                // Update the properties
                existingPost.Title = title;
                existingPost.Content = content;

                // Save changes
                await _dataMapper.updateCommunityPost(existingPost);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating post: {ex.Message}");
                throw;
            }
        }
    }
}
