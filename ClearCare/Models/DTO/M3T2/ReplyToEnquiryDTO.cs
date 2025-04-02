using System.Collections.Generic;
using ClearCare.Models.Entities.M3T2;

namespace ClearCare.Models.DTO.M3T2 // Replace "YourNamespace" with your actual project namespace
{
    public class ReplyToEnquiryDTO
    {
        public Enquiry Enquiry { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
