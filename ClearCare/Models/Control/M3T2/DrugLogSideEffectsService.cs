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
        private const string BaseUrl =
            "https://portfolio-website-lyart-five-75.vercel.app/api/side-effects?drugName=";

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref="DrugLogSideEffectsService"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance for making API
        ///   requests.</param>
        /// <param name="logger">The logger for this service to record
        ///   information and errors.</param>
        public DrugLogSideEffectsService(
            HttpClient httpClient,
            ILogger<DrugLogSideEffectsService> logger
        )
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        ///   Fetches the side effects for a given drug name from an external
        ///   API.
        /// </summary>
        /// <param name="drugName">The name of the drug to fetch side effects
        ///   for.</param>
        /// <returns>
        ///   A string containing the side effects, or an error message if the
        ///   API request fails.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   Thrown if the drug name is null or empty.
        /// </exception>
        public async Task<string> fetchDrugSideEffect(string drugName)
        {
            if (string.IsNullOrWhiteSpace(drugName))
            {
                throw new ArgumentException(
                    "Please insert a drug name.",
                    nameof(drugName)
                );
            }

            try
            {
                string reqUrl = $"{BaseUrl}{Uri.EscapeDataString(drugName)}";
                _logger.LogInformation(
                    $"Fetching side effects for {drugName} from {reqUrl}"
                );

                HttpResponseMessage response = await _httpClient.GetAsync(reqUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error fetching data: {response.StatusCode}");
                    return $"Error: {response.StatusCode}";
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception Occurred: {e.Message}");
                return "An error occurred.";
            }
        }
    }
}
