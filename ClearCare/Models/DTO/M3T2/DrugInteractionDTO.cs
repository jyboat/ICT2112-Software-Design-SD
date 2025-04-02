using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClearCare.Models.DTO.M3T2
{
    public class DrugInteractionDTO
    {
        public string Drug1Name { get; set; }
        public string Drug2Name { get; set; }
        public string Interaction { get; set; }
    }

    public class DrugInteractionResponse
    {
        [JsonPropertyName("results")]
        public List<DrugInteractionDTO> Results { get; set; }
    }
}