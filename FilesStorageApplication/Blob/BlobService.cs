using FilesStorageApplication.StorageServiceFactory;
using FilesStorageApplicationContract.Blob;
using FilesStorageDomain.Blob;
using FilesStorageDomain.Exceptions;
using FilesStorageDomain.Interfaces;

namespace FilesStorageApplication.Blob
{
    public class BlobService: IBlobService
    {
        private readonly IBlobRepository _blobRepository;
        private readonly IStorageService _storageService;

        public BlobService(IBlobRepository blobRepository, IStorageServiceFactory storageServiceFactory)
        {
            _blobRepository = blobRepository;
            _storageService = storageServiceFactory.CreateStorageService();
        }

        public async Task StoreBlobAsync(BlobStoreDto input)
        {

            await ValidateData(input);

            var blob = BlobObject.Create(input.Id);

            await _blobRepository.SaveBlobAsync(blob);

            await _storageService.SaveBlobAsync(blob.Id, input.Data);

        }

        private async Task ValidateData(BlobStoreDto input)
        {
            if (string.IsNullOrEmpty(input.Data))
            {
                throw new ArgumentException("Data cannot be null or empty", nameof(input.Data));
            }

            //check decoding
            try
            {
                byte[] fileBytes = Convert.FromBase64String(input.Data);
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid data", nameof(input.Data));
            }


            //check if id exised before
            var dbblob = await _blobRepository.GetBlobAsync(input.Id);
            if (dbblob != null)
            {
                throw new ArgumentException($"Blob with ID '{input.Id}' already existed", nameof(input.Id));
            }
        }

        public async Task<BlobDto> RetrieveBlobAsync(string blobId)
        {
            var blob = await _blobRepository.GetBlobAsync(blobId);
            if (blob == null)
            {
                throw new NotFoundException(blobId);
            }
            var res=  await _storageService.GetBlobAsync(blob.Id);

            return  new BlobDto
            {
                Id = blobId,
                Data = res.data,
                Size = res.size,
                created_at = blob.CreatedAt
            };
        }
    }
}
