// using System.Collections.Generic;
// using System.Text.Json.Serialization;

// namespace ClearCare.Models.DTO.M3T2
// {
//     public class DrugInteractionDTO
//     {
//         /// <summary>
//         ///   Gets or sets the name of the first drug involved in the
//         ///   interaction.
//         /// </summary>
//         public string Drug1Name { get; set; } = string.Empty;

//         /// <summary>
//         ///   Gets or sets the name of the second drug involved in the
//         ///   interaction.
//         /// </summary>
//         public string Drug2Name { get; set; } = string.Empty;

//         /// <summary>
//         ///   Gets or sets the description of the interaction between the two
//         ///   drugs.
//         /// </summary>
//         public string Interaction { get; set; } = string.Empty;
//     }

//     public class DrugInteractionResponse
//     {
//         /// <summary>
//         ///   Gets or sets the list of drug interactions.
//         /// </summary>
//         [JsonPropertyName("results")]
//         public List<DrugInteractionDTO> Results { get; set; } = new List<DrugInteractionDTO>();
//     }
// }
