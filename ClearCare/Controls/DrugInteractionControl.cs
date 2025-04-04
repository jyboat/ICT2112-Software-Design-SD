using ClearCare.Interfaces;
using ClearCare.Models;
using ClearCare.Gateways;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Observer;
using System.Text.Json;
using System.Web;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http;

namespace ClearCare.Controls
{
    public class DrugInteractionControl
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="DrugInteractionControl"/> class.
        /// </summary>
        /// <param name="mapper">The PatientDrugMapper instance.</param>
        /// <param name="httpClient">The HttpClient instance for making API
        ///   requests.</param>
        public DrugInteractionControl(PatientDrugMapper mapper, HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        ///   Retrieves drug interaction information from an external API.
        /// </summary>
        /// <param name="drugName1">The name of the first drug.</param>
        /// <param name="drugName2">The name of the second drug.</param>
        /// <returns>
        ///   A DrugInteractionResponse containing the interaction
        ///   information. Returns a default "No known interaction" message if
        ///   the API request fails or returns null.
        /// </returns>
        public async Task<DrugInteractionResponse> GetDrugInteractionAsync(
            string drugName1,
            string drugName2
        )
        {
            string ddinterApi =
                $"https://portfolio-website-lyart-five-75.vercel.app/api/receive?drug1={drugName1}&drug2={drugName2}";
            try
            {
                var resp = await _httpClient.GetStringAsync(ddinterApi);
                var result = JsonSerializer.Deserialize<DrugInteractionResponse>(resp);

                if (result is null)
                {
                    // Fallback if deserialization returned null
                    return new DrugInteractionResponse
                    {
                        Results = new List<DrugInteraction>
                        {
                            new DrugInteraction
                            {
                                Drug1Name = "Error",
                                Drug2Name = "Error",
                                Interaction = "No known interaction"
                            }
                        }
                    };
                }

                return result;
            }
            catch (HttpRequestException)
            {
                return new DrugInteractionResponse
                {
                    Results = new List<DrugInteraction>
                    {
                        new DrugInteraction
                        {
                            Drug1Name = "Error",
                            Drug2Name = "Error",
                            Interaction = "No known interaction"
                        }
                    }
                };
            }
        }

        /// <summary>
        ///   Uploads a new drug interaction to an external API.
        /// </summary>
        /// <param name="drugName1">The name of the first drug.</param>
        /// <param name="drugName2">The name of the second drug.</param>
        /// <param name="interaction">The interaction description.</param>
        /// <returns>
        ///   True if the upload was successful; otherwise, false if the API
        ///   request fails.
        /// </returns>
        public async Task<bool> UploadInteractionAsync(
            string drugName1,
            string drugName2,
            string interaction
        )
        {
            var newInteraction = new
            {
                Drug1Name = drugName1,
                Drug2Name = drugName2,
                Interaction = interaction
            };

            string json = JsonSerializer.Serialize(newInteraction);
            HttpContent content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );
            string ddinterApi =
                "https://portfolio-website-lyart-five-75.vercel.app/api/receive";

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(
                    ddinterApi,
                    content
                );
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}
