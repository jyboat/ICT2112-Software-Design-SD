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

        public async Task createGroup(string userId, string name, DateTime creationDate, string description)
        {
            var group = new CommunityGroup(name, description, creationDate, userId);

            await _dataMapper.insertGroup(group);
        }

        public async Task updateGroup(string groupId, string name, string description)
        {
            try
            {
                // Retrieve the existing group by ID
                var existingGroup = await _dataMapper.fetchGroupById(groupId);
                if (existingGroup == null)
                {
                    throw new Exception("Group not found.");
                }

                // Update the properties
                existingGroup.Name = name;
                existingGroup.Description = description;

                // Save changes
                await _dataMapper.updateCommunityGroup(existingGroup);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating group: {ex.Message}");
                throw;
            }
        }

        public async Task deleteGroup(string groupId)
        {
            try
            {
                await _dataMapper.deleteGroup(groupId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting group: {ex.Message}");
                throw;
            }
        }

        public async Task<List<CommunityGroup>> viewGroups()
        {
            try
            {
                return await _dataMapper.fetchCommunityGroups();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching groups: {ex.Message}");
                throw;
            }
        }

        public async Task<CommunityGroup> viewGroupById(string groupId)
        {
            try
            {
                return await _dataMapper.fetchGroupById(groupId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching groups: {ex.Message}");
                throw;
            }
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
