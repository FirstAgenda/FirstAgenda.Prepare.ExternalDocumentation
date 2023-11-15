using Newtonsoft.Json;

namespace UploadApi.Models.OAuth;

public class OAuth2GrantResponseModel(string accessToken, string expiresIn, string expiresOn)
{
    [JsonProperty(PropertyName = "access_token")]
    public string AccessToken { get; set; } = accessToken;

    [JsonProperty(PropertyName = "expires_in")]
    public string ExpiresIn { get; set; } = expiresIn;

    [JsonProperty(PropertyName = "expires_on")]
    public string ExpiresOn { get; set; } = expiresOn;
}