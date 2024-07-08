namespace FilesStorageApplicationContract.Blob
{
    public interface IBlobService
    {
     Task StoreBlobAsync(BlobStoreDto input);
     Task<BlobDto> RetrieveBlobAsync(string id);

    }
}
