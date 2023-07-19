namespace FolderASP2.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int FolderId { get; set; }
    }
}
