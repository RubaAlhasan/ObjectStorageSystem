using FilesStorageDomain.Interfaces;

namespace FilesStorageApplication.StorageServiceFactory
{
    public interface IStorageServiceFactory
    {
       IStorageService CreateStorageService();
    }
}
