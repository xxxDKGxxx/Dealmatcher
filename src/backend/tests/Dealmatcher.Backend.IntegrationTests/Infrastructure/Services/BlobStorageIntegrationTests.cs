namespace Dealmatcher.Backend.IntegrationTests.Infrastructure.Services;

public class BlobStorageIntegrationTests : IAsyncLifetime
{
  private readonly IContainer _azuriteContainer = new ContainerBuilder(
    "mcr.microsoft.com/azure-storage/azurite"
  )
    .WithPortBinding(10000, true)
    .WithCommand("azurite", "--blobHost", "0.0.0.0", "--blobPort", "10000", "--skipApiVersionCheck")
    .Build();

  private AzureBlobStorageService _service = null!;

  public async Task InitializeAsync()
  {
    await _azuriteContainer.StartAsync();

    var host = _azuriteContainer.Hostname;
    var port = _azuriteContainer.GetMappedPublicPort(10000);

    var connectionString =
      $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://{host}:{port}/devstoreaccount1;";

    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(
        new Dictionary<string, string?>
        {
          ["ConnectionStrings:AzureBlobStorage"] = connectionString,
          ["Azure:BlobContainerName"] = "test-images",
        }
      )
      .Build();

    _service = new AzureBlobStorageService(configuration);
  }

  public async Task DisposeAsync()
  {
    await _azuriteContainer.DisposeAsync();
  }

  [Fact]
  public async Task UploadImageAsync_ShouldUploadFileToTestcontainer()
  {
    var fileName = "test-file.txt";
    var content = "Test content";
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
    var contentType = "text/plain";

    var url = await _service.UploadImageAsync(
      stream,
      fileName,
      contentType,
      CancellationToken.None
    );

    url.ShouldNotBeNullOrEmpty();
    url.ShouldContain("devstoreaccount1");

    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync(url);

    response.IsSuccessStatusCode.ShouldBeTrue();
    var downloadedContent = await response.Content.ReadAsStringAsync();
    downloadedContent.ShouldBe(content);
  }

  [Fact]
  public async Task DeleteImageAsync_ExistingFile_ShouldRemoveFileFromStorage()
  {
    var fileName = "file-to-delete.txt";
    var content = "Test content";
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
    var contentType = "text/plain";

    var url = await _service.UploadImageAsync(
      stream,
      fileName,
      contentType,
      CancellationToken.None
    );

    using var httpClient = new HttpClient();
    var beforeDeleteResponse = await httpClient.GetAsync(url);
    beforeDeleteResponse.IsSuccessStatusCode.ShouldBeTrue();

    await _service.DeleteImageAsync(url, CancellationToken.None);

    var afterDeleteResponse = await httpClient.GetAsync(url);

    afterDeleteResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
