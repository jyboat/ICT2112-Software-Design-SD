using ClearCare.Models.Entities.M3T1;
using Google.Cloud.Firestore;
using System.Text.RegularExpressions;

namespace ClearCare.DataSource.M3T1
{
    public class CommunityDataMapper
    {
        private FirestoreDb _db;

        public CommunityDataMapper()
        {
            _db = FirebaseService.Initialize();
        }

        public async Task<List<CommunityPost>> fetchCommunityPosts()
        {
            try
            {
                var snapshot = await _db.Collection("CommunityPosts").GetSnapshotAsync();
                return snapshot.Documents.Select(doc => doc.ConvertTo<CommunityPost>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching community posts: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching community posts.", ex);
            }
        }
        public async Task<List<CommunityPost>> fetchGroupPosts(string groupId)
        {
            try
            {
                // Fetch posts filtered by GroupId
                var snapshot = await _db.Collection("CommunityPosts")
                                         .WhereEqualTo("GroupId", groupId)  
                                         .GetSnapshotAsync();

                // Convert documents to CommunityPost objects
                return snapshot.Documents.Select(doc => doc.ConvertTo<CommunityPost>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching community posts: {ex.Message}");
                return null;
            }
        }
        public async Task<CommunityPost> fetchPostById(string id)
        {
            try
            {
                var docRef = _db.Collection("CommunityPosts").Document(id);
                var doc = await docRef.GetSnapshotAsync();

                if (doc == null || !doc.Exists)
                {
                    Console.WriteLine($"Post with ID {id} not found.");
                    return null;
                }

                return doc.ConvertTo<CommunityPost>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching post by ID: {ex.Message}");
                return null;
            }
        }

        public async Task insertPost(CommunityPost post)
        {
            try
            {
                await _db.Collection("CommunityPosts").AddAsync(post);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error inserting community post: {ex.Message}");
                throw new ApplicationException("An error occurred while inserting the community post.", ex);
            }
        }

        //public async Task updateCommunityPost(CommunityPost post)
        //{
        //    try
        //    {
        //        var docRef = _db.Collection("CommunityPosts").Document(post.Id);
        //        await docRef.SetAsync(post, SetOptions.MergeAll());
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating community post: {ex.Message}");
        //        throw new ApplicationException("An error occurred while updating the community post.", ex);
        //    }
        //}

        public async Task deletePost(string id)
        {
            try
            {
                await _db.Collection("CommunityPosts").Document(id).DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting community post: {ex.Message}");
                throw new ApplicationException("An error occurred while deleting the community post.", ex);
            }
        }

        public async Task<List<CommunityComment>> fetchCommunityComments()
        {
            try
            {
                var snapshot = await _db.Collection("CommunityComments").GetSnapshotAsync();
                return snapshot.Documents.Select(doc => doc.ConvertTo<CommunityComment>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching community comments: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching community comments.", ex);
            }
        }

        public async Task insertComment(CommunityComment comment)
        {
            try
            {
                await _db.Collection("CommunityComments").AddAsync(comment);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error inserting community comment: {ex.Message}");
                throw new ApplicationException("An error occurred while inserting the community comment.", ex);
            }
        }

        //public async Task updateCommunityComment(CommunityComment comment)
        //{
        //    try
        //    {
        //        var docRef = _db.Collection("CommunityComments").Document(comment.Id);
        //        await docRef.SetAsync(comment, SetOptions.MergeAll());
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating community comment: {ex.Message}");
        //        throw new ApplicationException("An error occurred while updating the community comment.", ex);
        //    }
        //}

        public async Task<List<CommunityGroup>> fetchCommunityGroups()
        {
            try
            {
                var snapshot = await _db.Collection("CommunityGroups").GetSnapshotAsync();
                return snapshot.Documents.Select(doc => doc.ConvertTo<CommunityGroup>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching community groups: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching community groups.", ex);
            }
        }

        public async Task<CommunityGroup> fetchGroupById(string id)
        {
            try
            {
                var docRef = _db.Collection("CommunityGroups").Document(id);
                var doc = await docRef.GetSnapshotAsync();

                if (doc == null || !doc.Exists)
                {
                    Console.WriteLine($"Group with ID {id} not found.");
                    return null;
                }

                return doc.ConvertTo<CommunityGroup>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group by ID: {ex.Message}");
                return null;
            }
        }

        public async Task insertGroup(CommunityGroup group)
        {
            try
            {
                var collection = _db.Collection("CommunityGroups");
                await collection.AddAsync(group);
                Console.WriteLine($"Community group added: {group.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting community group: {ex.Message}");
            }
        }

        //public async Task updateCommunityGroup(CommunityGroup group)
        //{
        //    try
        //    {
        //        var docRef = _db.Collection("CommunityGroups").Document(group.Id);
        //        await docRef.SetAsync(group, SetOptions.MergeAll());
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error updating community group: {ex.Message}");
        //        throw new ApplicationException("An error occurred while updating the community group.", ex);
        //    }
        //}

        public async Task deleteGroup(string id)
        {
            try
            {
                await _db.Collection("CommunityGroups").Document(id).DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting community group: {ex.Message}");
                throw new ApplicationException("An error occurred while deleting the community group.", ex);
            }
        }
    }
}
