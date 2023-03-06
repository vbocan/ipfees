namespace IPFFees.FileManager
{
    public class FileManager : IFileManager
    {
        public FileManager()
        {
        }

        public bool AddFile(string fileName, string content)
        {
            throw new NotImplementedException();
        }

        public string GetFileContents(string fileName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFiles()
        {
            throw new NotImplementedException();
        }

        public bool RemoveFile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}