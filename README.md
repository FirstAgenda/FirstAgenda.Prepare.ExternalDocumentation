# FirstAgenda.Prepare.ExternalDocumentation



# FirstAgenda Upload API Client

### Link to example GitHub Repository:
[FirstAgenda Upload API Example](https://github.com/FirstAgenda/FirstAgenda.Prepare.ExternalDocumentation/tree/4cab3f5a2c9c2bd429ed42c2248937066df6cb77/upload-api/UploadApiExample)

Within the repository, you will find the following key classes:
- `UploadApi.cs`: Contains the upload method and logic.
- `TokenApi.cs`: Contains the token generation logic.
- `Program.cs`: Calls the classes and verifies the token on each iteration.

---

## Generating a Bearer Token for the Upload API

The URL to generate a token is retrieved via FirstAgenda’s Upload API. In the response from the API, a URL named `FetchAccessToken` is found. In the repository, there is a method to fetch this URL, which can be found in `UploadApi.GetUrls()`.

`FetchAccessToken` URL: [https://uploadapi.firstagenda.com/api/v2/upload/settings/urls](https://uploadapi.firstagenda.com/api/v2/upload/settings/urls)

The `FetchAccessToken` endpoint is used to generate a bearer token in the class `TokenApi.FetchToken()`. This method generates an active token by calling the `FetchAccessToken` endpoint with the request body consisting of the following parameters.

```plaintext
{
"client_id": "Test",
"client_secret": "xxxxxxxxxxx",
"grant_type": "client_credentials"
}
```


The returned token is represented in the class `OAuth2GrantResponseModel.cs`, which includes:
- **Token value**
- **expiresIn**
- **expiresOn**

It is recommended to check whether the used token has expired for each file uploaded. In `Program.cs`, there is an example of this check, where, in the case of an expired token, a new one is generated before further iterations. The example can be seen below:


```plaintext
if (uploadFileResult == UploadFileResult.TokenExpired)
{
    Console.WriteLine("Token is expired, fetching new");

    token = await tokenApi.FetchToken(urls?.FetchAccessToken, clientId, clientSecret, grantType);

    var uploadFileResultWithNewToken = await uploadApi.UploadFile(uploadDirectory, fileToUpload, urls?.FileUpload, sessionId, token?.AccessToken);

    Console.WriteLine($"Upload file result with new token: {uploadFileResultWithNewToken}");
}
```

---

## Uploading a File via the Upload API with an Active Bearer Token

The class `UploadApi.cs` is designed to asynchronously upload individual files through the API via a POST request.

The content of the POST request can be either a JSON body or form-data. In the example, it is generated using `MultipartFormDataContent`—thus, form-data is used.

The method `UploadApi.UploadFile()` takes the following parameters:
- `uploadDirectory`: The folder from which the file is uploaded. It is used to generate a relative path in the method `MakeRelativePath()`.
- `fileToUpload`: The full path to the file to be uploaded.
- `uri`: The FileUpload endpoint.
- `sessionId`: A GUID sent in the request header.
- `token`: Bearer token sent in the Authorization header.

In the example, form-data is used where the following are added:
- **Relative Path**:
    - The method `MakeRelativePath()` combines the folder path with the individual file to be uploaded.
- **MD5 Checksum**:
    - Generated in the method `AddMd5ToContent()`. The method generates a hash value based on the checksum for the individual file's byte stream. This hash value is stored in a database to be used in the future to scan if a meeting has changes or if an identical meeting is being attempted to be sent through the API.
    - If this method is changed in a new system, it will still function as long as the method remains consistent.
- **File Content**:
    - The method `AddFileContent()` writes the file to be uploaded into a byte array and adds it to the request form along with the file name.

#### Expected structure:

```plaintext
{
  // Relative path generated via MakeRelativePath()
  "path": "/relative/path/to/file",
  
  // MD5 checksum added via AddMd5ToContent()
  "md5": "checksum-value",
  
  // File content added via AddFileContent()
  "file": [file-byte-data]
}
```


### POST-Request Headers

The POST request must include the following headers:
- **Bearer Token**: Sent in the `Authorization` header.
- **Session ID**: Sent in the request header.

The final `HttpRequestMessage` will be composed of the following.


```plaintext

var message = new HttpRequestMessage
{
Method = HttpMethod.Post,
Content = content,  -- above mentioned form-data or JSON body
RequestUri = new Uri(uri)
};

message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
message.Headers.Add("X-SessionUid", sessionId);

using var httpClient = new HttpClient();

var response = await httpClient.SendAsync(message);
```



`UploadApi.UploadFile()` returns an enum containing:
- **Status of the POST request**
- **Status of the bearer token** after attempting the POST request.

```plaintext
public enum UploadFileResult
{
Success,
TokenExpired
}
```


It is recommended to handle all error scenarios when an individual meeting upload fails in `Program.cs`. This ensures that the upload process for the remaining meetings can continue without interruptions.