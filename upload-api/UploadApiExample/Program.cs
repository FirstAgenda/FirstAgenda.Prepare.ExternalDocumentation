using UploadApiExample.FirstAgendaApi;
using UploadApiExample.Models.FirstAgenda;

const string clientId = "";
const string clientSecret = "";
const string grantType = "client_credentials";

var uploadApi = new UploadApi();
var tokenApi = new TokenApi();

var urls = await uploadApi.GetUrls();

var token = await tokenApi.FetchToken(urls?.FetchAccessToken, clientId, clientSecret, grantType);

var sessionId = Guid.NewGuid().ToString();

var uploadDirectory = Path.Combine(AppContext.BaseDirectory, "FilesForUpload");
var fileToUploads = Directory.GetFiles(uploadDirectory, "*.*", SearchOption.AllDirectories);

Console.WriteLine($"Found {fileToUploads.Length} files to upload");

foreach (var fileToUpload in fileToUploads)
{
    var uploadFileResult = await uploadApi.UploadFile(uploadDirectory, fileToUpload, urls?.FileUpload, sessionId, token?.AccessToken);

    Console.WriteLine($"Upload file result: {uploadFileResult}");

    // instead of checking for TokenExpired you could also check token.ExpiresIn/On to see if a new token is needed
    if (uploadFileResult == UploadFileResult.TokenExpired)
    {
        Console.WriteLine("Token is expired, fetching new");

        token = await tokenApi.FetchToken(urls?.FetchAccessToken, clientId, clientSecret, grantType);

        var uploadFileResultWithNewToken = await uploadApi.UploadFile(uploadDirectory, fileToUpload, urls?.FileUpload, sessionId, token?.AccessToken);

        Console.WriteLine($"Upload file result with new token: {uploadFileResultWithNewToken}");
    }
}

Console.WriteLine("Successfully uploaded all files");