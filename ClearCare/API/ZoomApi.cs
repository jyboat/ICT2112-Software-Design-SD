using System.Text;
using ClearCare.Models.Interfaces.M3T1;

namespace ClearCare.API;

public class ZoomApi : IZoomApi
{
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

    public async Task<IZoomApi.TokenResponse?> generateAccessToken(
        string authorizationCode
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
        return await response.Content.ReadFromJsonAsync<IZoomApi.TokenResponse>();
    }

    public async Task<IZoomApi.TokenResponse?> generateServerAccessToken(
        string accountId
    )
    {
        // Console.WriteLine($"Auth code: {authorizationCode}, auth token: {getAuthToken()}");
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
                { "grant_type", "account_credentials" },
                { "account_id", accountId /* DO NOT USE IN PROD */ }
            })
        };

        using var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Could not send request: {await response.Content.ReadAsStringAsync()}");
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IZoomApi.TokenResponse>();
    }

    public async Task<IZoomApi.TokenResponse?> refreshAccessToken(
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
        
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Could not send request: {await response.Content.ReadAsStringAsync()}");
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IZoomApi.TokenResponse>();
    }

    public async Task<IZoomApi.MeetingResponse?> createMeeting(
        string accessToken,
        IZoomApi.MeetingData data
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
            Content = JsonContent.Create(data)
        };

        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Could not send request: {await response.Content.ReadAsStringAsync()}");
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IZoomApi.MeetingResponse>();
    }
}