using System.Net;

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
