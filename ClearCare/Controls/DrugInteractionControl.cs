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

        //Mapper
        public DrugInteractionControl(PatientDrugMapper mapper, HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DrugInteractionResponse> GetDrugInteractionAsync(string drugName1, string drugName2)
        {
            string ddinterApi = $"https://portfolio-website-lyart-five-75.vercel.app/api/receive?drug1={drugName1}&drug2={drugName2}";
            try
            {
                var resp = await _httpClient.GetStringAsync(ddinterApi);
                return JsonSerializer.Deserialize<DrugInteractionResponse>(resp);
            }
            catch (HttpRequestException)
            {
                return new DrugInteractionResponse
                {
                    Results = new List<DrugInteraction>
                    {
                        new DrugInteraction { Drug1Name = "Error", Drug2Name = "Error", Interaction = "No known interaction" }
                    }
                };
            }
        }

        public async Task<bool> UploadInteractionAsync(string drugName1, string drugName2, string interaction)
        {
            var newInteraction = new
            {
                Drug1Name = drugName1,
                Drug2Name = drugName2,
                Interaction = interaction
            };

            string json = JsonSerializer.Serialize(newInteraction);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            string ddinterApi = "https://portfolio-website-lyart-five-75.vercel.app/api/receive";
            
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(ddinterApi, content);
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