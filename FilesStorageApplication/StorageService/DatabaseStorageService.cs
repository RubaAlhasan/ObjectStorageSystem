using FilesStorageDomain.Blob;
using FilesStorageDomain.Exceptions;
using FilesStorageDomain.Interfaces;

namespace FilesStorageApplication.StorageService
{
    public class DatabaseStorageService : IStorageService
    {
        private readonly IBlobContentRepository _blobContentRepository;


        public DatabaseStorageService(IBlobContentRepository blobContnetRepository)
        {
            _blobContentRepository = blobContnetRepository;
        }

        public async Task SaveBlobAsync(Guid blobId, string data)
        {

            var blobContnet = BlobContent.Create(blobId, data);

            await _blobContentRepository.SaveBlobContentAsync(blobContnet);
        }

        public async Task<(string data,int size)> GetBlobAsync(Guid blobId)
        {
            var blobContnet = await _blobContentRepository.GetBlobContentAsync(blobId);
            if (blobContnet == null)
            {
                throw new NotFoundException(blobId.ToString());
            }

            var size = Convert.FromBase64String(blobContnet.Data).Length;

            return (blobContnet.Data,size);
        }
    }
}
