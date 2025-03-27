using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;
using System.Text.RegularExpressions;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityPostManagement : IPostReceive
    {
        //private CommunityDataMapper _dataMapper = new CommunityDataMapper();

        private readonly IPostSend _dataMapper;

        public CommunityPostManagement(IPostSend mapper)
        {
            _dataMapper = mapper;
        }

        public async Task<string> createPost(string title, string content, string postedBy, string? groupId)
        {
            return await _dataMapper.insertPost(title, content, postedBy, groupId);
        }

        public async Task<List<CommunityPost>> viewPosts()
        {
            return await _dataMapper.fetchCommunityPosts();
        }

        public async Task<List<CommunityPost>> viewGroupPosts(string groupId)
        {
            return await _dataMapper.fetchGroupPosts(groupId);
        }
        public async Task<List<CommunityPost>> viewUserPosts(string userId)
        {
            return await _dataMapper.fetchUserPosts(userId);
        }

        public async Task<List<CommunityPost>> viewUserGroupPosts(string userId, string groupId)
        {
            List<CommunityPost> userPosts = await _dataMapper.fetchUserPosts(userId);

            List<CommunityPost> userGroupPosts = new List<CommunityPost>();

            foreach (var post in userPosts)
            {
                var postDetails = post.getDetails();
                if (postDetails.ContainsKey("GroupId"))
                {
                    // Check if the MemberList contains the target ID
                    if (!string.IsNullOrEmpty(postDetails["GroupId"].ToString()) && postDetails["GroupId"].ToString() == groupId)
                    {
                        userGroupPosts.Add(post); // Add the group if the ID is found
                    }
                }
            }
            return userGroupPosts;
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

        // Implement IPostReceive methods for receiving data from the mapper
        public Task receiveAllCommunityPosts(List<CommunityPost> communityPosts)
        {
            if (communityPosts.Count > 0)
                Console.WriteLine($"Received {communityPosts.Count} community posts");
            else
                Console.WriteLine("No community posts received");

            return Task.CompletedTask;
        }

        public Task receiveGroupPosts(List<CommunityPost> groupPosts)
        {
            if (groupPosts.Count > 0)
                Console.WriteLine($"Received {groupPosts.Count} group posts");
            else
                Console.WriteLine("No group posts received");

            return Task.CompletedTask;
        }

        public Task receiveUserPosts(List<CommunityPost> userPosts)
        {
            if (userPosts.Count > 0)
                Console.WriteLine($"Received {userPosts.Count} user posts");
            else
                Console.WriteLine("No user posts received");

            return Task.CompletedTask;
        }

        public Task receiveUserGroupPosts(List<CommunityPost> userGroupPosts)
        {
            if (userGroupPosts.Count > 0)
                Console.WriteLine($"Received {userGroupPosts.Count} user-group posts");
            else
                Console.WriteLine("No user-group posts received");

            return Task.CompletedTask;
        }

        public Task receivePost(CommunityPost communityPost)
        {
            if (communityPost != null)
                Console.WriteLine("Received community post");
            else
                Console.WriteLine("No community post received");

            return Task.CompletedTask;
        }

        public Task receiveInsertStatus(string postId)
        {
            if (!string.IsNullOrEmpty(postId))
                Console.WriteLine($"Inserted post successfully with ID: {postId}");
            else
                Console.WriteLine("Failed to insert post");

            return Task.CompletedTask;
        }

        public Task receiveUpdateStatus(bool status)
        {
            Console.WriteLine(status ? "Updated post successfully" : "Failed to update post");
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(bool status)
        {
            Console.WriteLine(status ? "Deleted post successfully" : "Failed to delete post");
            return Task.CompletedTask;
        }
    }
}
