using System.Net.Http.Json;
using UploadApiExample.Models.OAuth;

namespace UploadApiExample.FirstAgendaApi;

public class TokenApi
{
    public async Task<OAuth2GrantResponseModel?> FetchToken(string? tokenEndpointUrl, string clientId, string clientSecret, string grantType)
    {
        var grantRequest = new OAuth2GrantRequestModel(clientId, clientSecret, grantType);
        using var httpClient = new HttpClient();

        Console.WriteLine($"Fetching token from '{tokenEndpointUrl}'");

        using var request = await httpClient.PostAsJsonAsync(tokenEndpointUrl, grantRequest);
        var response = await request.Content.ReadFromJsonAsync<OAuth2GrantResponseModel>();

        if (response == null || string.IsNullOrEmpty(response.AccessToken))
        {
            Console.WriteLine("ERROR: Response is null or token is empty");
            return null;
        }

        Console.WriteLine($"Found token '{response.AccessToken}' to use");

        return response;
    }
}