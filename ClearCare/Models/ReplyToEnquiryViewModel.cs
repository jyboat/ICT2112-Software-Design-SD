using System.Collections.Generic;

namespace ClearCare.Models // Replace "YourNamespace" with your actual project namespace
{
    public class ReplyToEnquiryViewModel
    {
        public Enquiry Enquiry { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
