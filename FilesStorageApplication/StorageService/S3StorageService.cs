using FilesStorageDomain.Interfaces;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


//I follow the Guid to putObject (uploadfile) in https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutObject.html
//and for Authorization service I follow the https://docs.aws.amazon.com/IAM/latest/UserGuide/create-signed-request.html

namespace FilesStorageApplication.StorageService
{
    public class S3StorageService : IStorageService
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly string _region;

        public S3StorageService( IConfiguration configuration)
        {
            _bucketName = configuration["StorageSettings:S3:BucketName"];
            _accessKey = configuration["StorageSettings:S3:AccessKey"];
            _secretKey = configuration["StorageSettings:S3:SecretKey"];
            _region = configuration["StorageSettings:S3:Region"];
        }

        public async Task SaveBlobAsync(Guid id, string data)
        {
            var key = id.ToString();
            byte[] fileBytes = Convert.FromBase64String(data);
            using var memoryStream = new MemoryStream(fileBytes);

            string dateString = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            string dateKey = DateTime.UtcNow.ToString("yyyyMMdd");

            string requestUri = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}";

            using var _httpClient = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = new ByteArrayContent(fileBytes)
            };
            requestMessage.Content.Headers.Add("Content-Type", "application/octet-stream");
            requestMessage.Content.Headers.Add("Content-Length", fileBytes.Length.ToString());
            requestMessage.Headers.Add("x-amz-date", dateString);
            requestMessage.Headers.Add("x-amz-acl", "public-read");

            string authorizationHeader = CreateUpoadAuthorizationHeader(requestMessage, dateKey, dateString);
            requestMessage.Headers.Add("Authorization", authorizationHeader);

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var res=  await response.Content.ReadAsStringAsync();
        }


        public async Task<(string data,int size)>GetBlobAsync(Guid blobId)
        {
            var key = blobId.ToString();  
            string requestUri = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}";

            string dateString = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            string dateKey = DateTime.UtcNow.ToString("yyyyMMdd");
            
            using var _httpClient = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("x-amz-date", dateString);

            string authorizationHeader = CreateGetAuthorizationHeader(requestMessage, dateKey, dateString, key);
            requestMessage.Headers.Add("Authorization", authorizationHeader);

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var data= await response.Content.ReadAsByteArrayAsync();

            return (Convert.ToBase64String(data),data.Length);
        }


        private string CreateUpoadAuthorizationHeader(HttpRequestMessage request, string dateKey, string dateString)
        {
            string canonicalRequest = CreateCanonicalRequest(request);
            string stringToSign = CreateStringToSign(dateString, dateKey, canonicalRequest);
            string signature = CreateSignature(dateKey, stringToSign);

            string credentialScope = $"{dateKey}/{_region}/s3/aws4_request";
            return $"AWS4-HMAC-SHA256 Credential={_accessKey}/{credentialScope}, SignedHeaders=content-length;content-type;host;x-amz-date;x-amz-acl, Signature={signature}";
        }

        private string CreateGetAuthorizationHeader(HttpRequestMessage request, string dateKey, string dateString, string key)
        {
            string canonicalRequest = CreateGetCanonicalRequest(request, key);
            string stringToSign = CreateStringToSign(dateString, dateKey, canonicalRequest);
            string signature = CreateSignature(dateKey, stringToSign);

            string credentialScope = $"{dateKey}/{_region}/s3/aws4_request";
            return $"AWS4-HMAC-SHA256 Credential={_accessKey}/{credentialScope}, SignedHeaders=host;x-amz-date, Signature={signature}";
        }

        private string CreateGetCanonicalRequest(HttpRequestMessage request, string key)
        {
            var canonicalHeaders = new StringBuilder();
            canonicalHeaders.Append($"host:{request.RequestUri.Host}\n");
            canonicalHeaders.Append($"x-amz-date:{request.Headers.GetValues("x-amz-date").First()}\n");

            string signedHeaders = "host;x-amz-date";
            string payloadHash = Hex(SHA256Hash(string.Empty));


            return $"{request.Method}\n{request.RequestUri.AbsolutePath}/{key}\n\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";
        }


        private string CreateCanonicalRequest(HttpRequestMessage request)
        {
            var canonicalHeaders = new StringBuilder();
            canonicalHeaders.Append($"content-length:{request.Content.Headers.ContentLength}\n");
            canonicalHeaders.Append($"content-type:{request.Content.Headers.ContentType}\n");
            canonicalHeaders.Append($"host:{request.RequestUri.Host}\n");
            canonicalHeaders.Append($"x-amz-date:{request.Headers.GetValues("x-amz-date").First()}\n");
            canonicalHeaders.Append($"x-amz-acl:{request.Headers.GetValues("x-amz-acl").First()}\n");

            string signedHeaders = "content-length;content-type;host;x-amz-date;x-amz-acl";
            string payloadHash = Hex(SHA256Hash(request.Content.ReadAsByteArrayAsync().Result));

            return $"{request.Method}\n{request.RequestUri.AbsolutePath}\n\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";
        }

        private string CreateStringToSign(string dateString, string dateKey, string canonicalRequest)
        {
            string credentialScope = $"{dateKey}/{_region}/s3/aws4_request";
            return $"AWS4-HMAC-SHA256\n{dateString}\n{credentialScope}\n{Hex(SHA256Hash(canonicalRequest))}";
        }

        private string CreateSignature(string dateKey, string stringToSign)
        {
            byte[] dateKeyBytes = HmacSHA256(Encoding.UTF8.GetBytes($"AWS4{_secretKey}"), dateKey);
            byte[] dateRegionKey = HmacSHA256(dateKeyBytes, _region);
            byte[] dateRegionServiceKey = HmacSHA256(dateRegionKey, "s3");
            byte[] signingKey = HmacSHA256(dateRegionServiceKey, "aws4_request");
            return Hex(HmacSHA256(signingKey, stringToSign));
        }

        private static byte[] SHA256Hash(string value) => SHA256.HashData(Encoding.UTF8.GetBytes(value));

        private static byte[] SHA256Hash(byte[] value) => SHA256.HashData(value);
        private static byte[] HmacSHA256(byte[] key, string data)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private static string Hex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}
