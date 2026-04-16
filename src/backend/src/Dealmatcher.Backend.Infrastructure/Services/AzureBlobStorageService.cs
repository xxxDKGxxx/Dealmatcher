namespace Dealmatcher.Backend.Infrastructure.Services;

public sealed class AzureBlobStorageService : IImageStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage")
            ?? throw new InvalidOperationException("Missing connection string for Azure Blob Storage.");

        _containerName = configuration["Azure:BlobContainerName"] ?? "offer-images";

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadImageAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var folderPath = DateTime.UtcNow.ToString("yyyy/MM");
        var uniqueName = $"{folderPath}/{Guid.NewGuid()}_{fileName}";
        var blobClient = containerClient.GetBlobClient(uniqueName);

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(fileStream, uploadOptions, cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteImageAsync(string fileUrl, CancellationToken ct = default)
    {
        var decodedUrl = Uri.UnescapeDataString(fileUrl);

        var containerSegment = $"/{_containerName}/";
        var index = decodedUrl.IndexOf(containerSegment, StringComparison.OrdinalIgnoreCase);

        if (index == -1)
        {
            throw new ArgumentException("Invalid file URL format. Container name not found.", nameof(fileUrl));
        }

        var blobName = decodedUrl[(index + containerSegment.Length)..];

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
    }
}
