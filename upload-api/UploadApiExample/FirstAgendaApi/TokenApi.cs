using System.Net.Http.Json;
using UploadApi.Models.OAuth;

namespace UploadApi.FirstAgendaApiMethods;

public class TokenApi
{
    public async Task<OAuth2GrantResponseModel?> FetchToken(string clientId, string clientSecret, string grantType)
    {
        const string tokenEndpointUrl = "https://auth.firstagenda.com/connect/token";

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

        return response;
    }

    public bool TokenIsInvalid(OAuth2GrantResponseModel token)
    {
        var epochTime = long.Parse(token.ExpiresOn.Substring(0, 10));
        //Irish time is because the token uses .now on an AWS server that is hosted in Ireland
        var tokenExpirationTimeIrishTime = DateTimeOffset.FromUnixTimeSeconds(epochTime);
        var irishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        var irishTime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, irishTimeZone);

        return IsTokenExpired(tokenExpirationTimeIrishTime, irishTime);
    }

    private static bool IsTokenExpired(DateTimeOffset expirationTime, DateTimeOffset currentTime)
    {
        if (expirationTime < currentTime)
        {
            return true;
        }

        {
            return false;
        }
    }
}