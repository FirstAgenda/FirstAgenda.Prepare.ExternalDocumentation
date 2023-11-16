using System.Text.Json.Serialization;

namespace UploadApiExample.Models.OAuth;

public class OAuth2GrantResponseModel(string? accessToken, int expiresIn, int expiresOn)
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; } = accessToken;

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; } = expiresIn;

    [JsonPropertyName("expires_on")] public int ExpiresOn { get; set; } = expiresOn;
}