using FilesStorageDomain.Blob;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilesStorageEntityFramework
{
    public class FilesStorageEntityFrameworkDBContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
            public DbSet<BlobObject> Blobs { get; set; }
            public DbSet<BlobContent> BlobContents { get; set; }

            public FilesStorageEntityFrameworkDBContext(DbContextOptions<FilesStorageEntityFrameworkDBContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)

            {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<BlobObject>().HasKey(b => b.Id);

                modelBuilder.Entity<BlobContent>().HasKey(b => b.Id);
            }
        
    }
}
