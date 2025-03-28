using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using System.Text.RegularExpressions;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityCommentManagement : ICommentReceive 
    {
        //private CommunityDataMapper _dataMapper = new CommunityDataMapper();

        private readonly ICommentSend _dataMapper;

        public CommunityCommentManagement(ICommentSend mapper)
        {
            _dataMapper = mapper;
        }

        public async Task<string> createComment(string content, string createdBy, string postId)
        {
            return await _dataMapper.insertComment(content, postId, createdBy);
        }

        public async Task<List<CommunityComment>> viewPostComments(string postId)
        {
            return await _dataMapper.fetchPostComments(postId);
        }

        public async Task<bool> deleteComment(string commentId)
        {
            return await _dataMapper.deleteComment(commentId);
        }

        public async Task<bool> updateComment(string commentId, string content)
        {
            return await _dataMapper.updateCommunityComment(commentId, content);
        }

        // Implementing ICommentReceive methods for receiving data from the mapper
        public Task receiveAllPostComments(List<CommunityComment> comments)
        {
            Console.WriteLine($"Received {comments.Count} comments");
            return Task.CompletedTask;
        }

        public Task receiveComment(CommunityComment comment)
        {
            if (comment != null)
            {
                Console.WriteLine("Received comment details");
            }
            else
            {
                Console.WriteLine("No comment found");
            }
            return Task.CompletedTask;
        }

        public Task receiveInsertStatus(string commentId)
        {
            if (!string.IsNullOrEmpty(commentId))
            {
                Console.WriteLine($"Inserted comment successfully with ID: {commentId}");
            }
            else
            {
                Console.WriteLine("Failed to insert comment");
            }
            return Task.CompletedTask;
        }

        public Task receiveUpdateStatus(bool status)
        {
            Console.WriteLine(status ? "Updated comment successfully" : "Failed to update comment");
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(bool status)
        {
            Console.WriteLine(status ? "Deleted comment successfully" : "Failed to delete comment");
            return Task.CompletedTask;
        }
    }
}
