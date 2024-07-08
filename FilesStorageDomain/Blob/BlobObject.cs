namespace FilesStorageDomain.Blob
{
    public class BlobObject
    {
        public Guid Id { get; set; }
        public string BlobId { get; set; }
        public DateTime CreatedAt { get; set; }

        internal BlobObject() { }

        public static BlobObject Create(string BlobId)
        {
            return new BlobObject
            {
            Id = Guid.NewGuid(),
            BlobId = BlobId,
            CreatedAt = DateTime.UtcNow
            };
            
        }


    }
}
