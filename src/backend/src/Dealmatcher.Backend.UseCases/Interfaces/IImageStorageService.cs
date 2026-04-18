namespace Dealmatcher.Backend.UseCases.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteImageAsync(string fileUrl, CancellationToken cancellationToken = default);
}
