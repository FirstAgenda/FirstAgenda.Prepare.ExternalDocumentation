using Newtonsoft.Json;

namespace UploadApi.Models.OAuth;

public class OAuth2GrantRequestModel(string clientId, string clientSecret, string grantType)
{
    [JsonProperty(PropertyName = "client_id")]
    public string ClientId { get; set; } = clientId;

    [JsonProperty(PropertyName = "client_secret")]
    public string ClientSecret { get; set; } = clientSecret;

    [JsonProperty(PropertyName = "grant_type")]
    public string GrantType { get; set; } = grantType;
}