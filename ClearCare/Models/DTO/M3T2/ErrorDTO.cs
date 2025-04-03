namespace ClearCare.Models.DTO.M3T2
{
    public class ErrorDTO
    {
        /// <summary>
        ///   Gets or sets the ID of the request that resulted in the error.
        /// </summary>
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        ///   Gets a value indicating whether to show the request ID on the error
        ///   page.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
