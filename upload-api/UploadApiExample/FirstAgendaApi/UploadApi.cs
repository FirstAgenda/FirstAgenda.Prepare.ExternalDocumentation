using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Web;
using UploadApiExample.Models.FirstAgenda;

namespace UploadApiExample.FirstAgendaApi;

public class UploadApi
{
    public async Task<UploadApiUrls?> GetUrls()
    {
        using var httpClient = new HttpClient();

        var response = await httpClient.GetFromJsonAsync<UploadApiUrls>("https://uploadapi.firstagenda.com/api/v2/upload/settings/urls");

        Console.WriteLine($"Found urls. FetchAccessToken: '{response?.FetchAccessToken}' FileUpload: '{response?.FileUpload}'");

        return response;
    }

    public async Task<UploadFileResult> UploadFile(string uploadDirectory, string fileToUpload, string? uri, string sessionId, string? token)
    {
        var content = new MultipartFormDataContent();

        var relativePath = HttpUtility.HtmlEncode(MakeRelativePath(uploadDirectory, fileToUpload));
        content.Add(new StringContent(relativePath), "RelativePath");

        AddMd5ToContent(fileToUpload, content);
        AddFileContent(fileToUpload, content);

        var message = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = content,
            RequestUri = new Uri(uri)
        };

        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        message.Headers.Add("X-SessionUid", sessionId);

        using var httpClient = new HttpClient();

        var response = await httpClient.SendAsync(message);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return UploadFileResult.TokenExpired;
        }

        response.EnsureSuccessStatusCode();

        return UploadFileResult.Success;
    }

    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
    /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from the start directory to the end path.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
    private string MakeRelativePath(string fromPath, string toPath)
    {
        if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
        if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

        if (!fromPath.EndsWith('\\')) fromPath += '\\';

        var fromUri = new Uri(fromPath);
        var toUri = new Uri(toPath);

        if (fromUri.Scheme != toUri.Scheme)
        {
            return toPath;
        } // path can't be made relative.

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if (toUri.Scheme.ToUpperInvariant() == "FILE")
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        return relativePath;
    }

    private void AddMd5ToContent(string pathToFile, MultipartFormDataContent content)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(pathToFile);
        var checksum = md5.ComputeHash(stream);
        var md5Checksum = BitConverter.ToString(checksum).Replace("-", string.Empty);

        content.Add(new StringContent(md5Checksum), "MD5");
    }

    private void AddFileContent(string pathToFile, MultipartFormDataContent content)
    {
        var fileArray = File.ReadAllBytes(pathToFile);
        var fileName = Path.GetFileName(pathToFile);

        content.Add(new ByteArrayContent(fileArray), "File", fileName);
    }
}