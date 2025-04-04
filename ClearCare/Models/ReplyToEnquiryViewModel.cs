using System.Collections.Generic;

namespace ClearCare.Models 
{
    public class ReplyToEnquiryViewModel
    {
        /// <summary>
        ///   Gets or sets the enquiry to which the replies are associated.
        /// </summary>
        public Enquiry Enquiry { get; set; } = new Enquiry();

        /// <summary>
        ///   Gets or sets the list of replies for the enquiry.
        /// </summary>
        public List<Reply> Replies { get; set; } = new List<Reply>();
    }
}
