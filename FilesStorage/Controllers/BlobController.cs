using FilesStorageApplicationContract.Blob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilesStorage.Controllers
{
    [ApiController]
    [Route("v1/blobs")]
    [Authorize]
    public class BlobsController : ControllerBase
    {
        private readonly IBlobService _blobService;

        public BlobsController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<IActionResult> StoreBlob([FromBody] BlobStoreDto input)
        {
            try
            {
                await _blobService.StoreBlobAsync(input);
                return Ok();
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> RetrieveBlob(string id)
        {
            try
            {
                var blob = await _blobService.RetrieveBlobAsync(id);
                return Ok(blob);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
