using FilesStorageDomain.Blob;
using FilesStorageDomain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilesStorageEntityFramework.Repositories
{
    public class BlobRepository : IBlobRepository
    {
        private readonly FilesStorageEntityFrameworkDBContext _context;

        public BlobRepository(FilesStorageEntityFrameworkDBContext context)
        {
            _context = context;
        }

        public async Task<BlobObject> GetBlobAsync(string blobId)
        {
            return await _context.Blobs.FirstOrDefaultAsync(x=> x.BlobId == blobId);
        }

        public async Task SaveBlobAsync(BlobObject blob)
        {
            _context.Blobs.Add(blob);
            await _context.SaveChangesAsync();
        }
    }
}
