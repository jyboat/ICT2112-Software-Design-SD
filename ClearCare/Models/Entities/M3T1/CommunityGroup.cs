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
        private string getId() => Id;
        private string getName() => Name;
        private string getDescription() => Description;
        private string getCreationDate() => CreationDate;
        private string getCreatedBy() => CreatedBy;
        private List<string> getMemberIds() => MemberIds;

        // Setters for the private fields
        private void setId(string id) => Id = id;
        private void setName(string name) => Name = name;
        private void setDescription(string description) => Description = description;
        private void setCreationDate(string creationDate) => CreationDate = creationDate;
        private void setCreatedBy(string userId) => CreatedBy = userId;
        private void setMemberIds(List<string> memberIds) => MemberIds = memberIds;

        public Dictionary<string, object> getDetails()
        {
            return new Dictionary<string, object>
            {
                { "Id", getId() },
                { "Name", getName() },
                { "Description", getDescription() },
                { "CreationDate", getCreationDate() },
                { "CreatedBy", getCreatedBy() },
                { "MemberIds", getMemberIds() }
            };
        }

    }
}
