using FilesStorageDomain.Blob;

namespace FilesStorageDomain.Interfaces
{
    public interface IBlobContentRepository
    {
        Task<BlobContent> GetBlobContentAsync(Guid blobId);
        Task SaveBlobContentAsync(BlobContent blob);
    }
}
