namespace FilesStorageDomain.Interfaces
{
    public interface IStorageService
    {
        Task SaveBlobAsync(Guid blobId, string data);
        Task<(string data, int size)> GetBlobAsync(Guid id);
    }
}
