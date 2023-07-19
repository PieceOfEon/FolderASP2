namespace FolderASP2.Models
{
    public class CloudViewModel
    {
        public IEnumerable<Folder> Folders { get; set; }
        public IEnumerable<File> Files { get; set; }

        public Folder SelectedFolder { get; set; }
        public List<File> SelectedFiles { get; set; }
    }
}
