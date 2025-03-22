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
    }
}
