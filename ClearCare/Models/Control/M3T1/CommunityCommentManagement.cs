using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Entities;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using System.Text.RegularExpressions;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityCommentManagement
    {
        private CommunityDataMapper _dataMapper = new CommunityDataMapper();

        public async Task createComment(string content, string createdBy, string postId, DateTime createdAt)
        {
            var comment = new CommunityComment(content, createdBy, postId, createdAt);

            await _dataMapper.insertComment(comment);
        }

        public async Task<List<CommunityComment>> viewPostComments(string postId)
        {
            return await _dataMapper.fetchPostComments(postId);
        }

        public async Task deleteComment(string commentId)
        {
            try
            {
                await _dataMapper.deleteComment(commentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
                throw;
            }
        }

        public async Task updateComment(string userId, string commentId, string content)
        {
            try
            {
                // Retrieve the existing comment by ID
                var existingComment = await _dataMapper.fetchCommentById(commentId);
                if (existingComment == null)
                {
                    throw new Exception("Comment not found.");
                }

                // Check if the user is authorized to edit this comment (assuming the comment has a CreatedBy property)
                if (existingComment.CreatedBy != userId)
                {
                    throw new Exception("User is not authorized to edit this comment.");
                }

                // Update the content of the comment
                existingComment.Content = content;

                // Save the updated comment
                await _dataMapper.updateCommunityComment(existingComment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating comment: {ex.Message}");
                throw;
            }
        }
    }
}
