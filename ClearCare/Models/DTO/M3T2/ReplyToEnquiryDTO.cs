using System.Collections.Generic;
using ClearCare.Models.Entities.M3T2;

namespace ClearCare.Models.DTO.M3T2 // Replace "YourNamespace" with your actual project namespace
{
    public class ReplyToEnquiryDTO
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
