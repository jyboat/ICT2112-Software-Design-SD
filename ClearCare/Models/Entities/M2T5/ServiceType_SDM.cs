using System;

namespace ClearCare.Models.Entities
{
    public class ServiceType_SDM
    {
        public int ServiceTypeId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
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
