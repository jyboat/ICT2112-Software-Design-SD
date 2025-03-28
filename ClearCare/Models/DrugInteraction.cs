using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClearCare.Models
{
    public class DrugInteraction
    {
        public string Drug1Name { get; set; }
        public string Drug2Name { get; set; }
        public string Interaction { get; set; }
    }

    public class DrugInteractionResponse
    {
        [JsonPropertyName("results")]
        public List<DrugInteraction> Results { get; set; }
    }
}