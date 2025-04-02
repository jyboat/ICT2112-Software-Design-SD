using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClearCare.Models
{
    public class DrugInteraction
    {
        /// <summary>
        ///   Gets or sets the name of the first drug involved in the
        ///   interaction.
        /// </summary>
        public string Drug1Name { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the name of the second drug involved in the
        ///   interaction.
        /// </summary>
        public string Drug2Name { get; set; } = string.Empty;

        /// <summary>
        ///   Gets or sets the description of the interaction between the two
        ///   drugs.
        /// </summary>
        public string Interaction { get; set; } = string.Empty;
    }

    public class DrugInteractionResponse
    {
        /// <summary>
        ///   Gets or sets the list of drug interactions.
        /// </summary>
        [JsonPropertyName("results")]
        public List<DrugInteraction> Results { get; set; } = new List<DrugInteraction>();
    }
}
