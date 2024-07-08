namespace FilesStorageApplicationContract.Blob
{
    public class BlobDto
    {
        public string Id { get; set; }
        public string Data { get; set; }
        public float Size { get; set; }
        public DateTime created_at { get; set; }
    }
}
