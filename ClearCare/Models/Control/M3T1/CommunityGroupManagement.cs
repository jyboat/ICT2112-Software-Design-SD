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

        public async Task<bool> updateGroup(string groupId, string name, string description)
        {
            return await _dataMapper.updateGroup(groupId, name, description);
        }

        public async Task<bool> addMember(string groupId, string userId)
        {
            CommunityGroup group = await _dataMapper.fetchGroupById(groupId);
            var groupDetails = group.getDetails();

            List<string> currentMembers = new List<string>();

            if (group != null && groupDetails.ContainsKey("MemberIds") && groupDetails["MemberIds"] is List<string> memberList)
            {
                currentMembers = memberList;
                if (!currentMembers.Contains(userId))
                {
                    currentMembers.Add(userId);
                }
                else
                {
                    return false;
                }

                return await _dataMapper.updateGroupMembers(groupId, userId, currentMembers);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> removeMember(string groupId, string userId)
        {
            CommunityGroup group = await _dataMapper.fetchGroupById(groupId);
            var groupDetails = group.getDetails();

            List<string> currentMembers = new List<string>();

            if (group != null && groupDetails.ContainsKey("MemberIds") && groupDetails["MemberIds"] is List<string> memberList)
            {
                currentMembers = memberList;
                if (currentMembers.Contains(userId))
                {
                    currentMembers.Remove(userId);
                }
                else
                {
                    return false;
                }

                return await _dataMapper.updateGroupMembers(groupId, userId, currentMembers);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> deleteGroup(string groupId)
        {
            return await _dataMapper.deleteGroup(groupId);
        }

        public async Task<List<CommunityGroup>> getUserGroups(string userId)
        {
            var allGroups = await _dataMapper.fetchCommunityGroups();
            List<CommunityGroup> userGroups = new List<CommunityGroup>();

            foreach (var group in allGroups)
            {
                var groupDetails = group.getDetails();
                if (groupDetails.ContainsKey("MemberIds") && groupDetails["MemberIds"] is List<string> memberList)
                {
                    // Check if the MemberList contains the target ID
                    if (memberList.Contains(userId))
                    {
                        userGroups.Add(group); // Add the group if the ID is found
                    }
                }
            }
            return userGroups;
        }

        public async Task<List<CommunityGroup>> getNonUserGroups(string userId)
        {
            var allGroups = await _dataMapper.fetchCommunityGroups();
            List<CommunityGroup> nonUserGroups = new List<CommunityGroup>();

            foreach (var group in allGroups)
            {
                var groupDetails = group.getDetails();
                if (groupDetails.ContainsKey("MemberIds") && groupDetails["MemberIds"] is List<string> memberList)
                {
                    // Check if the MemberList does contain the target ID
                    if (!memberList.Contains(userId))
                    {
                        nonUserGroups.Add(group); // Add the group if the ID is found
                    }
                }
            }
            return nonUserGroups;
        }

        public async Task<CommunityGroup> viewGroupById(string groupId)
        {
            return await _dataMapper.fetchGroupById(groupId);
        }

    }
}
