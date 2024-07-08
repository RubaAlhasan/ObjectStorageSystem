namespace FilesStorageDomain.Exceptions
{
    public class DuplicatedIdException : Exception
    {
        public DuplicatedIdException(string id)
            : base($"Blob with ID '{id}' already existed")
        {
        }
    }

}
