namespace FilesStorageDomain.Blob
{
    public class BlobContent
    {
        public Guid Id { get; set; }
        public Guid BlobId { get; set; }
        public virtual BlobObject Blob  { get; set; }
        public string Data { get; set; }

        internal BlobContent() { }

        public static BlobContent Create(Guid blobId, string data)
        {
            return new BlobContent
            {
                Id = Guid.NewGuid(),
                BlobId = blobId,
                Data = data
            };

        }


    }
}
