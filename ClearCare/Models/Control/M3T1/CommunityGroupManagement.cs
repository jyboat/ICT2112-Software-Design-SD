using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.DataSource.M3T1;
using ClearCare.Models.Entities.M3T1;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.Models.Control.M3T1
{
    public class CommunityGroupManagement : IGroupReceive
    {
        //private CommunityDataMapper _dataMapper = new CommunityDataMapper();

        private readonly IGroupSend _dataMapper;

        public CommunityGroupManagement(IGroupSend mapper)
        {
            _dataMapper = mapper;
        }

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

        // Implement IGroupReceive methods for receiving data from the mapper
        public Task receiveAllCommunityGroups(List<CommunityGroup> communityGroups)
        {
            Console.WriteLine($"Received {communityGroups.Count} community groups");
            return Task.CompletedTask;
        }

        public Task receiveUserGroups(List<CommunityGroup> communityGroups)
        {
            Console.WriteLine($"Received {communityGroups.Count} user groups");
            return Task.CompletedTask;
        }

        public Task receiveNonUserGroups(List<CommunityGroup> communityGroups)
        {
            Console.WriteLine($"Received {communityGroups.Count} non-user groups");
            return Task.CompletedTask;
        }

        public Task receiveGroup(CommunityGroup communityGroup)
        {
            if (communityGroup != null)
            {
                Console.WriteLine("Received community group details");
            }
            else
            {
                Console.WriteLine("No group found");
            }
            return Task.CompletedTask;
        }

        public Task receiveInsertStatus(string groupId)
        {
            if (!string.IsNullOrEmpty(groupId))
            {
                Console.WriteLine($"Inserted group successfully with ID: {groupId}");
            }
            else
            {
                Console.WriteLine("Failed to insert group");
            }
            return Task.CompletedTask;
        }

        public Task receiveUpdateStatus(bool status)
        {
            Console.WriteLine(status ? "Updated group successfully" : "Failed to update group");
            return Task.CompletedTask;
        }

        public Task receiveAddMemberStatus(bool status)
        {
            Console.WriteLine(status ? "Added member successfully" : "Failed to add member");
            return Task.CompletedTask;
        }

        public Task receiveRemoveMemberStatus(bool status)
        {
            Console.WriteLine(status ? "Removed member successfully" : "Failed to remove member");
            return Task.CompletedTask;
        }

        public Task receiveDeleteStatus(bool status)
        {
            Console.WriteLine(status ? "Deleted group successfully" : "Failed to delete group");
            return Task.CompletedTask;
        }

    }
}
