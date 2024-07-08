
using FilesStorageApplication.StorageService;
using FilesStorageDomain.Interfaces;

namespace FilesStorageApplication.StorageServiceFactory
{
    public class StorageServiceFactory : IStorageServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public StorageServiceFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public IStorageService CreateStorageService()
        {
            var type = _configuration["StorageSettings:Type"];

            return type switch
            {
                "S3" => _serviceProvider.GetRequiredService<S3StorageService>(),
                "Database" => _serviceProvider.GetRequiredService<DatabaseStorageService>(),
                "LocalFileSystem" => _serviceProvider.GetRequiredService<LocalFileSystemStorageService>(),
                "FTP" => _serviceProvider.GetRequiredService<FTPStorageService>(),
                _ => throw new InvalidOperationException($"Unsupported storage: {type}")
            };
        }
    }
}
