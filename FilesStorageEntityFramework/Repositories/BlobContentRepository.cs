using FilesStorageDomain.Blob;
using FilesStorageDomain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilesStorageEntityFramework.Repositories
{
    public class BlobContentRepository : IBlobContentRepository
    {
        private readonly FilesStorageEntityFrameworkDBContext _context;

        public BlobContentRepository(FilesStorageEntityFrameworkDBContext context)
        {
            _context = context;
        }

        public async Task<BlobContent> GetBlobContentAsync(Guid blobId)
        {
            return  await  _context.BlobContents
                            .Include(x=> x.Blob)
                            .FirstOrDefaultAsync(x=> x.BlobId == blobId);
        }

        public async Task SaveBlobContentAsync(BlobContent blobContent)
        {
            _context.BlobContents.Add(blobContent);
            await _context.SaveChangesAsync();
        }
    }
}
