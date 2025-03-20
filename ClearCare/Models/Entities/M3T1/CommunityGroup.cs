using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities.M3T1
{
    [FirestoreData]
    public class CommunityGroup
    {
        [FirestoreDocumentId] 
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Description { get; set; }

        [FirestoreProperty]
        public DateTime? CreationDate { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string CreatedBy { get; set; }

        [FirestoreProperty]
        public List<string> MemberIds { get; set; }

        public CommunityGroup() { }

        public CommunityGroup(string name, string description, DateTime creationDate, string userId)
        {
            Name = name;
            Description = description;
            CreationDate = creationDate;
            CreatedBy = userId;
            MemberIds = new List<string> { userId };
        }

        // Implementing the FromFirestore method for deserialization
        //public CommunityGroup FromFirestore(object value)
        //{
        //    var dict = (Dictionary<string, object>)value;
        //    return new CommunityGroup
        //    {
        //        Id = dict["Id"].ToString(),
        //        Name = dict["Name"].ToString(),
        //        Description = dict["Description"].ToString(),
        //        CreationDate = Convert.ToDateTime(dict["CreationDate"]),
        //        CreatedBy = dict["CreatedBy"].ToString(),
        //        MemberIds = ((List<object>)dict["MemberIds"]).Cast<string>().ToList()
        //    };
        //}

        // Implementing the ToFirestore method for serialization
        //public object ToFirestore(CommunityGroup value)
        //{
        //    return new Dictionary<string, object>
        //{
        //    { "Id", value.Id },
        //    { "Name", value.Name },
        //    { "Description", value.Description },
        //    { "CreationDate", value.CreationDate },
        //    { "CreatedBy", value.CreatedBy },
        //    { "MemberIds", value.MemberIds }
        //};
        //}

        //public string Id { get; set; }
        //[FirestoreProperty]
        //public string Name { get; set; }
        //[FirestoreProperty]
        //public string Description { get; set; }
        //[FirestoreProperty]
        //public DateTime CreationDate { get; set; }
        //[FirestoreProperty]
        //public int CreatedBy { get; set; }

        //public List<int> MemberIds { get; set; }

        //public CommunityGroup() { }

        //public CommunityGroup(string name, string description, DateTime creationDate, int userId, List<int> memberIds)
        //{
        //    Name = name;
        //    Description = description;
        //    CreationDate = creationDate;
        //    CreatedBy = userId;
        //    MemberIds = new List<int>(memberIds);
        //}

        // Getters for the private fields
        public string GetId() => Id;
        public string GetName() => Name;
        public string GetDescription() => Description;
        public DateTime? GetCreationDate() => CreationDate;
        public string GetCreatedBy() => CreatedBy;
        public List<string> GetMemberIds() => MemberIds;

        // Setters for the private fields
        public void SetId(string id) => Id = id;
        public void SetName(string name) => Name = name;
        public void SetDescription(string description) => Description = description;
        public void SetCreationDate(DateTime creationDate) => CreationDate = creationDate;
        public void SetCreatedBy(string userId) => CreatedBy = userId;
        public void SetMemberIds(List<string> memberIds) => MemberIds = memberIds;

        // Getter and setter methods for each of the fields
        //public string GetCommunityGroupId() => GetId();
        //public void SetCommunityGroupId(string id) => SetId(id);

        //public string GetGroupName() => GetName();
        //public void SetGroupName(string name) => SetName(name);

        //public string GetGroupDescription() => GetDescription();
        //public void SetGroupDescription(string description) => SetDescription(description);

        //public DateTime GetGroupCreationDate() => GetCreationDate();
        //public void SetGroupCreationDate(DateTime creationDate) => SetCreationDate(creationDate);
        //public int GetCommunityGroupOwner() => GetCreatedBy();
        //public void SetCommunityGroupOwner(int id) => SetCreatedBy(id);

        //public List<int> GetGroupMembers() => GetMemberIds();
        //public void SetGroupMembers(List<int> memberIds) => SetMemberIds(memberIds);

        //// getDetails() method to return a string description of the group
        //public string GetDetails()
        //{
        //    return $"Group Name: {Name}, Description: {Description}, Members: {string.Join(", ", MemberIds)}";
        //}

        //// setDetails() method to set the group's name, description, and member IDs
        //public void SetDetails(string name, string description, List<int> memberIds)
        //{
        //    SetName(name);
        //    SetDescription(description);
        //    SetMemberIds(memberIds);
        //}

    }
}
