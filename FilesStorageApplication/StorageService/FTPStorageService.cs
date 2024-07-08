using FilesStorageDomain.Exceptions;
using FilesStorageDomain.Interfaces;
using System.Net;

namespace FilesStorageApplication.StorageService
{
    public class FTPStorageService : IStorageService
    {
        private readonly string _ftpHost;
        private readonly string _username;
        private readonly string _password;

        public FTPStorageService(IConfiguration configuration)
        {
            _ftpHost = configuration["StorageSettings:FTP:Host"];
            _username = configuration["StorageSettings:FTP:Username"];
            _password = configuration["StorageSettings:FTP:Password"];
        }

        public async Task SaveBlobAsync(Guid blobId, string data)
        {
            var id = blobId.ToString();
            var byteData = Convert.FromBase64String(data);
            var request = (FtpWebRequest)WebRequest.Create($"{_ftpHost}/{id}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_username, _password);

            using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(byteData, 0, data.Length);
            }

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != FtpStatusCode.CommandOK && response.StatusCode != FtpStatusCode.ClosingData)
                {
                    throw new Exception($"Error uploading file to FTP: {response.StatusDescription}");
                }
            }
        }

        public async Task<(string data,int size)> GetBlobAsync(Guid blobId)
        {
            var id = blobId.ToString();
            var request = (FtpWebRequest)WebRequest.Create($"{_ftpHost}/{id}");

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(_username, _password);

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != FtpStatusCode.OpeningData && response.StatusCode != FtpStatusCode.DataAlreadyOpen)
                {
                    throw new NotFoundException(id);
                }

                using (var responseStream = response.GetResponseStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await responseStream.CopyToAsync(memoryStream);
                        var data = memoryStream.ToArray();
                        var createdAt = response.LastModified.ToUniversalTime();
                        return (Convert.ToBase64String(data),data.Length);
                    }
                }
            }
        }
    }
}
