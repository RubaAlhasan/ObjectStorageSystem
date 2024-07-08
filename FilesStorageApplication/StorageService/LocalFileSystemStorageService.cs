using FilesStorageDomain.Exceptions;
using FilesStorageDomain.Interfaces;

namespace FilesStorageApplication.StorageService
{
    public class LocalFileSystemStorageService : IStorageService
    {
        private readonly string _storagePath;

        public LocalFileSystemStorageService(IConfiguration configuration)
        {
            _storagePath = configuration["StorageSettings:LocalFileSystem:Path"]; 

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task SaveBlobAsync(Guid id, string data)
        {
            var byteData = Convert.FromBase64String(data);
            var filePath = Path.Combine(_storagePath, id.ToString());
            await File.WriteAllBytesAsync(filePath, byteData);
        }

        public async Task<(string data, int size)> GetBlobAsync(Guid blobId)
        {
            var filePath = Path.Combine(_storagePath, blobId.ToString());

            if (!File.Exists(filePath))
            {
                throw new NotFoundException(blobId.ToString());
            }


            var data = await File.ReadAllBytesAsync(filePath);
            var fileInfo = new FileInfo(filePath);

            return (Convert.ToBase64String(data), data.Length);
        }
    }
}
