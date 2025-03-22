using ClearCare.Models.Entities.M3T1;
using Google.Cloud.Firestore;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ClearCare.DataSource.M3T1
{
    public class CommunityDataMapper
    {
        private FirestoreDb _db;

        public CommunityDataMapper()
        {
            _db = FirebaseService.Initialize();
        }

        // Community Groups

        public async Task<List<CommunityGroup>> fetchCommunityGroups()
        {
            try
            {
                List<CommunityGroup> groups = new List<CommunityGroup>();

                var snapshot = await _db.Collection("CommunityGroups").GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        try
                        {
                            string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                            string name = doc.ContainsField("Name") ? doc.GetValue<string>("Name") : "";
                            string description = doc.ContainsField("Description") ? doc.GetValue<string>("Description") : "";
                            string creationDate = doc.ContainsField("CreationDate") ? doc.GetValue<string>("CreationDate") : "";
                            string createdBy = doc.ContainsField("CreatedBy") ? doc.GetValue<string>("CreatedBy") : "";
                            List<string> memberIds = doc.ContainsField("MemberIds")
                            ? doc.GetValue<List<string>>("MemberIds")
                            : new List<string>();

                            CommunityGroup group = new CommunityGroup(id, name, description, creationDate, createdBy, memberIds);
                            groups.Add(group);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error converting community post {doc.Id}: {ex.Message}");
                        }
                    }
                }
                return groups;
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

                else 
                {
                    try
                    {
                        string name = doc.ContainsField("Name") ? doc.GetValue<string>("Name") : "";
                        string description = doc.ContainsField("Description") ? doc.GetValue<string>("Description") : "";
                        string creationDate = doc.ContainsField("CreationDate") ? doc.GetValue<string>("CreationDate") : "";
                        string createdBy = doc.ContainsField("CreatedBy") ? doc.GetValue<string>("CreatedBy") : "";
                        List<string> memberIds = doc.ContainsField("MemberIds")
                        ? doc.GetValue<List<string>>("MemberIds")
                        : new List<string>();

                        CommunityGroup group = new CommunityGroup(id, name, description, creationDate, createdBy, memberIds);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting community group {doc.Id}: {ex.Message}");
                    }
                }


                return doc.ConvertTo<CommunityGroup>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group by ID: {ex.Message}");
                return null;
            }
        }

        public async Task<string> insertGroup(string name, string description, string userId, List<string> memberIds)
        {
            try
            {
                var docRef = _db.Collection("CommunityGroups").Document();

                var group = new Dictionary<string, object>
                {
                    {"Name", name},
                    {"Description", description},
                    {"CreationDate", DateTime.Now.ToString("yyyy-MM-dd")},
                    { "CreatedBy", userId},
                    { "MemberIds", memberIds }
                };

                await docRef.SetAsync(group);

                return docRef.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting community group: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<bool> updateCommunityGroup(string id, string name, string description, List<string> memberIds)
        {
            try
            {
                var docRef = _db.Collection("CommunityGroups").Document(id);
                var group = new Dictionary<string, object>
                {
                    {"Name", name},
                    {"Description", description},
                    {"MemberIds", memberIds }
                };

                await docRef.UpdateAsync(group);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating community group: {ex.Message}");
                throw new ApplicationException("An error occurred while updating the community group.", ex);
            }
        }

        public async Task<bool> deleteGroup(string id)
        {
            try
            {
                await _db.Collection("CommunityGroups").Document(id).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting community group: {ex.Message}");
                throw new ApplicationException("An error occurred while deleting the community group.", ex);
            }
        }

        // Community Posts

        public async Task<List<CommunityPost>> fetchCommunityPosts()
        {
            try
            {
                List<CommunityPost> posts = new List<CommunityPost>();
                var snapshot = await _db.Collection("CommunityPosts").GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        try
                        {
                            string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                            string title = doc.ContainsField("Title") ? doc.GetValue<string>("Title") : "";
                            string content = doc.ContainsField("Content") ? doc.GetValue<string>("Content") : "";
                            string postedBy = doc.ContainsField("PostedBy") ? doc.GetValue<string>("PostedBy") : "";
                            string postedAt = doc.ContainsField("PostedAt") ? doc.GetValue<string>("PostedAt") : "";
                            string groupId = doc.ContainsField("GroupId") ? doc.GetValue<string>("GroupId") : "";

                            CommunityPost post = new CommunityPost(id, title, content, postedBy, postedAt, groupId);
                            posts.Add(post);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error converting community post {doc.Id}: {ex.Message}");
                        }
                    }
                }
                return posts;
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
                List<CommunityPost> posts = new List<CommunityPost>(); 
                // Fetch posts filtered by GroupId
                var snapshot = await _db.Collection("CommunityPosts")
                                         .WhereEqualTo("GroupId", groupId)  
                                         .GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        try
                        {
                            string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                            string title = doc.ContainsField("Title") ? doc.GetValue<string>("Title") : "";
                            string content = doc.ContainsField("Content") ? doc.GetValue<string>("Content") : "";
                            string postedBy = doc.ContainsField("PostedBy") ? doc.GetValue<string>("PostedBy") : "";
                            string postedAt = doc.ContainsField("PostedAt") ? doc.GetValue<string>("PostedAt") : "";

                            CommunityPost post = new CommunityPost(id, title, content, postedBy, postedAt, groupId);
                            posts.Add(post);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error converting community post {doc.Id}: {ex.Message}");
                        }
                    }
                }

                return posts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching community posts: {ex.Message}");
                return null;
            }
        }

        public async Task<CommunityPost> fetchPostById(string postId)
        {
            var snapshot = await _db.Collection("CommunityPosts")
                                        .Document(postId)
                                        .GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return null;
            }
            string title = snapshot.ContainsField("Title") ? snapshot.GetValue<string>("Title") : "";
            string content = snapshot.ContainsField("Content") ? snapshot.GetValue<string>("Content") : "";
            string postedBy = snapshot.ContainsField("PostedBy") ? snapshot.GetValue<string>("PostedBy") : "";
            string postedAt = snapshot.ContainsField("PostedAt") ? snapshot.GetValue<string>("PostedAt") : "";
            string groupId = snapshot.ContainsField("GroupId") ? snapshot.GetValue<string>("GroupId") : "";

            CommunityPost post = new CommunityPost(postId, title, content, postedBy, postedAt, groupId);
            return post;
            
        }

        public async Task<string> insertPost(string title, string content, string postedBy)
        {
            try
            {
                var docRef = _db.Collection("CommunityPosts").Document();
                var post = new Dictionary<string, object>
                {
                    {"Title", title},
                    {"Content", content},
                    {"PostedBy", postedBy},
                    { "PostedAt", DateTime.Now.ToString("yyyy-MM-dd")},
                    { "GrouopId", "" }
                };

                await docRef.SetAsync(post);

                return docRef.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting community post: {ex.Message}");
                //throw new ApplicationException("An error occurred while inserting the community post.", ex);
                return string.Empty;
            }
        }

        public async Task<bool> updateCommunityPost(string id, string title, string content)
        {
            try
            {
                var docRef = _db.Collection("CommunityPosts").Document(id);
                var post = new Dictionary<string, object>
                {
                    {"Content", content},
                    {"Title", title}
                };

                await docRef.UpdateAsync(post);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating community post: {ex.Message}");
                throw new ApplicationException("An error occurred while updating the community post.", ex);
            }
        }

        public async Task<bool> deletePost(string id)
        {
            try
            {
                await _db.Collection("CommunityPosts").Document(id).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting community post: {ex.Message}");
                throw new ApplicationException("An error occurred while deleting the community post.", ex);
            }
        }

        // Community Comments

        public async Task<List<CommunityComment>> fetchPostComments(string postId)
        {
            List<CommunityComment> comments = new List<CommunityComment>();
            try
            {
                var snapshot = await _db.Collection("CommunityComments")
                        .WhereEqualTo("PostId", postId) // Ensure filtering by PostId
                        .GetSnapshotAsync();

                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    if (doc.Exists)
                    {
                        try
                        {
                            string id = doc.ContainsField("Id") ? doc.GetValue<string>("Id") : doc.Id;
                            string content = doc.ContainsField("Content") ? doc.GetValue<string>("Content") : "";
                            string createdBy = doc.ContainsField("CreatedBy") ? doc.GetValue<string>("CreatedBy") : "";
                            string createdAt = doc.ContainsField("CreatedAt") ? doc.GetValue<string>("CreatedAt") : "";

                            CommunityComment comment = new CommunityComment(id, content, createdBy, postId, createdAt);
                            comments.Add(comment);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error converting community comment {doc.Id}: {ex.Message}");
                        }
                    }
                }

                return comments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching community comments: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching community comments.", ex);
            }
        }

        public async Task<string> insertComment(string content, string postId, string userId)
        {
            try
            {
                DocumentReference docRef = _db.Collection("CommunityComments").Document();
                var comment = new Dictionary<string, object>
                {
                    {"Content", content},
                    {"PostId", postId},
                    { "CreatedAt", DateTime.Now.ToString("yyyy-MM-dd")},
                    { "CreatedBy", userId }
                };

                await docRef.SetAsync(comment);

                return docRef.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting community comment: {ex.Message}");
                return string.Empty;
                //throw new ApplicationException("An error occurred while inserting the community comment.", ex);
            }
        }

        public async Task<bool> deleteComment(string id)
        {
            try
            {
                await _db.Collection("CommunityComments").Document(id).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting community comment: {ex.Message}");
                return false;
                //throw new ApplicationException("An error occurred while deleting the comment.", ex);
            }
        }

        public async Task<bool> updateCommunityComment(string id, string content)
        {
            try
            {
                var docRef = _db.Collection("CommunityComments").Document(id);

                var updatedData = new Dictionary<string, object>
                {
                    { "Content", content },
                };

                await docRef.UpdateAsync(updatedData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating community comment: {ex.Message}");
                //throw new ApplicationException("An error occurred while updating the community comment.", ex);
                return false;
            }
        }

        
    }
}
