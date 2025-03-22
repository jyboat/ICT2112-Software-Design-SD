using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityGroupManagement
    {
        private CommunityDataMapper _dataMapper = new CommunityDataMapper();

        public async Task<string> createGroup(string userId, string name, string description, List<string> memberIds)
        {
            return await _dataMapper.insertGroup(name, description, userId, memberIds);
        }

        public async Task<bool> updateGroup(string groupId, string name, string description, List<string> memberIds)
        {
            return await _dataMapper.updateCommunityGroup(groupId, name, description, memberIds);
        }

        public async Task<bool> deleteGroup(string groupId)
        {
            return await _dataMapper.deleteGroup(groupId);
        }

        public async Task<List<CommunityGroup>> getAllgroups()
        {
            return await _dataMapper.fetchCommunityGroups();
        }

        public async Task<CommunityGroup> viewGroupById(string groupId)
        {
            return await _dataMapper.fetchGroupById(groupId);
        }

        //public async Task addMember(int userId, CommunityGroup group)
        //{
        //    if (!group.Members.Contains(userId))
        //    {
        //        group.Members.Add(userId);
        //        await _dataMapper.updateGroup(group.Id, group);
        //    }
        //}

        //public async Task removeMember(int userId, CommunityGroup group)
        //{
        //    if (group.Members.Contains(userId))
        //    {
        //        group.Members.Remove(userId);
        //        await _dataMapper.updateGroup(group.Id, group);
        //    }
        //}

    }
}
