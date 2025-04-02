using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCare.Observer;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using ClearCare.Models.Interfaces.M3T2;

namespace ClearCare.Controls
{
    public class DrugLogSideEffectsService : IFetchSideEffects
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DrugLogSideEffectsService> _logger;
        private const string BaseUrl = "https://portfolio-website-lyart-five-75.vercel.app/api/side-effects?drugName=";
        
        public DrugLogSideEffectsService(HttpClient httpClient, ILogger<DrugLogSideEffectsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> fetchDrugSideEffect(string drugName) {
            if (string.IsNullOrWhiteSpace(drugName)) {
                throw new ArgumentException("Please insert a drug name.", nameof(drugName));
            }

            try {
                string reqUrl = $"{BaseUrl}{Uri.EscapeDataString(drugName)}";
                _logger.LogInformation($"Fetching side effects for {drugName} from {reqUrl}");

                HttpResponseMessage response = await _httpClient.GetAsync(reqUrl);

                if(!response.IsSuccessStatusCode) {
                    _logger.LogError($"Error fetching data: {response.StatusCode}");
                    return $"Error: {response.StatusCode}";
                }

                return await response.Content.ReadAsStringAsync();
            }catch (Exception e) {
                _logger.LogError($"Exception Occurred: {e.Message}");
                return "An error occurred."; 
            }
        }
    }
}
