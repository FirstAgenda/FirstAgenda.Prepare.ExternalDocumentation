using System.Text.Json.Serialization;

namespace UploadApiExample.Models.OAuth;

public class OAuth2GrantRequestModel(string clientId, string clientSecret, string grantType)
{
    [JsonPropertyName("client_id")] public string ClientId { get; set; } = clientId;

    [JsonPropertyName("client_secret")] public string ClientSecret { get; set; } = clientSecret;

    [JsonPropertyName("grant_type")] public string GrantType { get; set; } = grantType;
}