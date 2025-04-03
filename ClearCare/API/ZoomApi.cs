using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace ClearCare.API;

public class ZoomApi
{
    public class MeetingData
    {
        [StringLength(2000)]
        [JsonPropertyName("agenda")]
        public string? Agenda { get; set; }

        [JsonPropertyName("settings")] public SettingsData? Settings { get; set; }

        public class SettingsData
        {
            [JsonPropertyName("auto_recording")] public AutoRecordingOption? AutoRecording { get; set; }

            public enum AutoRecordingOption
            {
                [JsonPropertyName("local")] Local,
                [JsonPropertyName("cloud")] Cloud,
                [JsonPropertyName("none")] None
            }
        }
    }

    public class MeetingResponse
    {
        [JsonPropertyName("agenda")] public string? Agenda { get; set; }

        [JsonPropertyName("password")] public string? Password { get; set; }

        [JsonPropertyName("join_url")] public string? JoinUrl { get; set; }
    }

    private readonly HttpClient _httpClient;

    private readonly string _clientId;
    private readonly string _clientSecret;

    public string getAuthToken()
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
    }

    public ZoomApi(
        string clientId, string clientSecret
    )
    {
        _httpClient = new HttpClient();
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public string generateAuthorizeLink(
        string redirectUri
    )
    {
        return $"https://zoom.us/oauth/authorize?response_type=code&client_id={_clientId}&redirect_uri={redirectUri}";
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
    }

    public async Task<TokenResponse?> generateAccessToken(
        string authorizationCode,
        string redirectUri
    )
    {
        Console.WriteLine($"Auth code: {authorizationCode}, auth token: {getAuthToken()}");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri =
                new Uri("https://zoom.us/oauth/token"),
            Headers =
            {
                {
                    "Authorization", $"Basic {getAuthToken()}"
                }
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authorizationCode }
            })
        };

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenResponse>();
    }

    public async Task<TokenResponse?> refreshAccessToken(
        string refreshToken
    )
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri =
                new Uri("https://zoom.us/oauth/token"),
            Headers =
            {
                {
                    "Authorization", $"Basic {getAuthToken()}"
                }
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            })
        };

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenResponse>();
    }

    public async Task<MeetingResponse?> createMeeting(
        string accessToken,
        MeetingData zoomData
    )
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.zoom.us/v2/users/me/meetings"),
            Headers =
            {
                {
                    "Authorization", $"Bearer {accessToken}"
                }
            },
            Content = JsonContent.Create(zoomData)
        };

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MeetingResponse>();
    }
}