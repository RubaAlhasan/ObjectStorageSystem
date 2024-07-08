using FilesStorageDomain.Blob;

namespace FilesStorageDomain.Interfaces
{
    public interface IBlobRepository
    {
        Task<BlobObject> GetBlobAsync(string id);
        Task SaveBlobAsync(BlobObject blob);
    }
}
