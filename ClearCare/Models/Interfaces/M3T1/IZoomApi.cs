using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClearCare.Models.Interfaces.M3T1;

public interface IZoomApi
{
    #region OAuth
    public string getAuthToken();

    public string generateAuthorizeLink(string redirectUri);
    
    public class TokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
    }

    // Server-side access token
    public Task<TokenResponse?> generateServerAccessToken(string accountId);

    public Task<TokenResponse?> refreshAccessToken(string refreshToken);
    #endregion
    
    #region Create meeting
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

    public Task<MeetingResponse?> createMeeting(string accessToken, MeetingData data);

    #endregion
}