using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClearCare.Models
{
    public class DrugInteraction
    {
        public string Drug1Name { get; set; } = string.Empty;
        public string Drug2Name { get; set; } = string.Empty;
        public string Interaction { get; set; } = string.Empty;
    }

    public class DrugInteractionResponse
    {
        [JsonPropertyName("results")]
        public List<DrugInteraction> Results { get; set; } = new List<DrugInteraction>();
    }
}