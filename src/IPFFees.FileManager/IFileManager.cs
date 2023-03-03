namespace IPFFees.FileManager
{
    public interface IFileManager
    {
        public bool AddFile(string fileName, string content);
        public bool RemoveFile(string fileName);
        public IEnumerable<string> GetFiles();
        public string GetFileContents(string fileName);
    }
}