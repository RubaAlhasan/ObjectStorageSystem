using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FilesStorage.Test
{
    public class BlobControllerIntegrationTests 
    {
        private readonly HttpClient _client;
        private readonly string Host= "https://localhost:44337";
        public BlobControllerIntegrationTests()
        {
            //  var webApplicationFactory = new WebApplicationFactory<Program>();
            _client = new HttpClient();
        }



        [Fact]
        public async Task StoreBlob_Should_Return_Ok()
        {
            var token = await GetJwtTokenAsync();

            // Arrange
            var request = new
            {
                Url = $"{Host}/v1/blobs",
                Body = new
                {
                    Id = "test-id",
                    Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello, World!"))
                }
            };

            _client.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Bearer", token);
            
            
            // Act
            var response = await _client.PostAsJsonAsync(request.Url, request.Body);

            // Assert
            Assert.NotNull( response.EnsureSuccessStatusCode());
        }

        [Fact]
        public async Task RetrieveBlob_Should_Return_Blob_data()
        {

            var token = await GetJwtTokenAsync();

            // Arrange
            var id = "test-id";
            var storeRequest = new
            {
                Url = $"{Host}/v1/blobs",
                Body = new
                {
                    Id = id,
                    Data = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello, World!"))
                }
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await _client.PostAsJsonAsync(storeRequest.Url, storeRequest.Body);



            // Act
            var retrieveResponse = await _client.GetAsync($"{Host}/v1/blobs/{id}");

            // Assert
            retrieveResponse.EnsureSuccessStatusCode();
            var content = await retrieveResponse.Content.ReadAsStringAsync();
            
            Assert.Contains(id, content);

        }

        private async Task<string> GetJwtTokenAsync()
        {
            var loginRequest = new
            {
                Username = "admin@gmail.com",
                Password = "P@ssw0rd123"
            };

            var response = await _client.PostAsync($"{Host}/api/Auth/login",
                new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseString);
            return jsonResponse.RootElement.GetProperty("token").GetString();
        }
    }
}