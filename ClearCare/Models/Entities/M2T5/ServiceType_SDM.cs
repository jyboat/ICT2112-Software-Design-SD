using System.ComponentModel.DataAnnotations;

namespace ClearCare.Models.Entities
{
    public class ServiceType_SDM
    {
        public int ServiceTypeId { get; set; }

        [Required(ErrorMessage = "Service Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Requirements field is required.")]
        public string Requirements { get; set; }

        public ServiceType_SDM() { }

        public ServiceType_SDM(int id, string name, int duration, string requirements)
        {
            ServiceTypeId = id;
            Name = name;
            Duration = duration;
            Requirements = requirements;
        }
    }
}
