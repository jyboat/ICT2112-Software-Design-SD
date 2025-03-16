using System.Collections.Generic;
using ClearCare.Models.Entities.M3T2;

namespace ClearCare.Models // Replace "YourNamespace" with your actual project namespace
{
    public class ReplyToEnquiryViewModel
    {
        public Enquiry Enquiry { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
