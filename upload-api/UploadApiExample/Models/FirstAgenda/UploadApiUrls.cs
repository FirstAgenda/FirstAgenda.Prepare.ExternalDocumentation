namespace UploadApiExample.Models.FirstAgenda;

public class UploadApiUrls(string fetchAccessToken, string fileUpload)
{
    public string FetchAccessToken { get; set; } = fetchAccessToken;
    public string FileUpload { get; set; } = fileUpload;
}