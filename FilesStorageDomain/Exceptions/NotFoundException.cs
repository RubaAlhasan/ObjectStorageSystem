namespace FilesStorageDomain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string id)
            : base($"Blob with ID '{id}' was not found.")
        {
        }
    }

}
