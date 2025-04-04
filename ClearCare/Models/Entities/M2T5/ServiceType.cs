using System.ComponentModel.DataAnnotations;

namespace ClearCare.Models.Entities
{
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }

        [Required(ErrorMessage = "Service Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Requirements field is required.")]
        public string Requirements { get; set; }

        public string Status { get; set; } = "active"; // can be "active" or "discontinued"
        public string Modality { get; set; } // default can be "Virtual" or "Physical (Level 1 Room A)"

        // Constructor
        public ServiceType() { }

        public ServiceType(int id, string name, int duration, string requirements, string modality)
        {
            ServiceTypeId = id;
            Name = name;
            Duration = duration;
            Requirements = requirements;
            Modality = modality;
        }

        // Getter and Setter Methods
        public int getServiceTypeId() => ServiceTypeId;
        public void setServiceTypeId(int id) => ServiceTypeId = id;

        public string getName() => Name;
        public void setName(string name) => Name = name;

        public int getDuration() => Duration;
        public void setDuration(int duration) => Duration = duration;

        public string getRequirements() => Requirements;
        public void setRequirements(string requirements) => Requirements = requirements;

        public string getStatus() => Status;
        public void setStatus(string status) => Status = status;

        public string getModality() => Modality;
        public void setModality(string modality) => Modality = modality;

    }
}
