using Google.Cloud.Firestore;

namespace ClearCare.Models.Entities.M3T1
{
    [FirestoreData]
    public class CommunityGroup
    {
        [FirestoreDocumentId]
        private string Id { get; set; }

        [FirestoreProperty]
        private string Name { get; set; }

        [FirestoreProperty]
        private string Description { get; set; }

        [FirestoreProperty]
        private string CreationDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [FirestoreProperty]
        private string CreatedBy { get; set; }

        [FirestoreProperty]
        private List<string> MemberIds { get; set; } = new List<string>();

        public CommunityGroup() { }

        public CommunityGroup(string id, string name, string description, string creationDate, string createdBy, List<string> memberIds)
        {
            Id = id;
            Name = name;
            Description = description;
            CreationDate = creationDate;
            CreatedBy = createdBy;
            MemberIds = memberIds;
        }

        // Getters for the private fields
        private string GetId() => Id;
        private string GetName() => Name;
        private string GetDescription() => Description;
        private string GetCreationDate() => CreationDate;
        private string GetCreatedBy() => CreatedBy;
        private List<string> GetMemberIds() => MemberIds;

        // Setters for the private fields
        private void SetId(string id) => Id = id;
        private void SetName(string name) => Name = name;
        private void SetDescription(string description) => Description = description;
        private void SetCreationDate(string creationDate) => CreationDate = creationDate;
        private void SetCreatedBy(string userId) => CreatedBy = userId;
        private void SetMemberIds(List<string> memberIds) => MemberIds = memberIds;

        public Dictionary<string, object> getDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", GetId() },
                { "Name", GetName() },
                { "Description", GetDescription() },
                { "CreationDate", GetCreationDate() },
                { "CreatedBy", GetCreatedBy() },
                { "MemberIds", GetMemberIds() }
            };
        }

    }
}
