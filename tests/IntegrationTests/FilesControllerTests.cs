using System.Net;
using Aiursoft.Tracer.Services.FileStorage;

namespace Aiursoft.Tracer.Tests.IntegrationTests;

[TestClass]
public class FilesControllerTests : TestBase
{
    [TestMethod]
    public async Task TestUploadAndDownload()
    {
        // 1. Upload
        var content = new StringContent("Hello World");
        var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(content, "file", "test.txt");

        var uploadResponse = await Http.PostAsync("/upload/test", multipartContent);
        uploadResponse.EnsureSuccessStatusCode();
        var uploadResult = await uploadResponse.Content.ReadFromJsonAsync<UploadResult>();
        Assert.IsNotNull(uploadResult);
        Assert.IsNotNull(uploadResult.Path);

        // 2. Download
        var downloadResponse = await Http.GetAsync("/download/" + uploadResult.Path);
        downloadResponse.EnsureSuccessStatusCode();
        var downloadContent = await downloadResponse.Content.ReadAsStringAsync();
        Assert.AreEqual("Hello World", downloadContent);
    }

    [TestMethod]
    public async Task TestPrivateUploadAndDownload()
    {
        var storage = GetService<StorageService>();
        var subfolder = "private-test";
        var uploadUrl = storage.GetUploadUrl(subfolder, isVault: true);

        // 1. Upload
        var content = new StringContent("Private Hello World");
        var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(content, "file", "private-test.txt");

        var uploadResponse = await Http.PostAsync(uploadUrl, multipartContent);
        uploadResponse.EnsureSuccessStatusCode();
        var uploadResult = await uploadResponse.Content.ReadFromJsonAsync<UploadResult>();
        Assert.IsNotNull(uploadResult);
        Assert.IsNotNull(uploadResult.Path);
        Assert.IsNotNull(uploadResult.InternetPath);
        Assert.Contains("token=", uploadResult.InternetPath);

        // 2. Download using InternetPath (which contains token)
        var downloadResponse = await Http.GetAsync(uploadResult.InternetPath);
        downloadResponse.EnsureSuccessStatusCode();
        var downloadContent = await downloadResponse.Content.ReadAsStringAsync();
        Assert.AreEqual("Private Hello World", downloadContent);

        // 3. Try download without token
        var rawPath = uploadResult.Path;
        var unauthorizedResponse = await Http.GetAsync("/download-private/" + rawPath);
        Assert.AreEqual(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);

        // 4. Try download with invalid token
        var invalidTokenResponse = await Http.GetAsync("/download-private/" + rawPath + "?token=invalid");
        Assert.AreEqual(HttpStatusCode.Unauthorized, invalidTokenResponse.StatusCode);
    }

    [TestMethod]
    public async Task TestPrivateUploadWithInvalidToken()
    {
        var subfolder = "private-test";
        var uploadResponse = await Http.PostAsync($"/upload-private/{subfolder}?token=invalid", new MultipartFormDataContent());
        Assert.AreEqual(HttpStatusCode.Unauthorized, uploadResponse.StatusCode);
    }

    [TestMethod]
    public async Task TestUploadInvalidFileName()
    {
        var content = new StringContent("Hello World");
        var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(content, "file", "../test.txt");

        var uploadResponse = await Http.PostAsync("/upload/test", multipartContent);
        Assert.AreEqual(HttpStatusCode.BadRequest, uploadResponse.StatusCode);
    }

    [TestMethod]
    public async Task TestDownloadNotFound()
    {
        var downloadResponse = await Http.GetAsync("/download/non-existing.txt");
        Assert.AreEqual(HttpStatusCode.NotFound, downloadResponse.StatusCode);
    }

    private class UploadResult
    {
        public string Path { get; init; } = string.Empty;
        public string InternetPath { get; init; } = string.Empty;
    }
}
