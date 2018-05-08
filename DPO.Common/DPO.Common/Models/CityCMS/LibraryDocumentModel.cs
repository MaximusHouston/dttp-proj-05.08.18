 
using System.Collections.Generic;

namespace DPO.Common
{
    public class LibraryDocumentModel : ILibraryItem
    {
        public int id { get; set; }
        public string name { get; set; }

        public string path { get; set; }
        public string thumb { get; set; }
    }

    public class LibraryDocumentEditModel : LibraryDocumentModel
    {
        public int DirectoryId { get; set; }
    }

    public class LibraryDirectoryModel : ILibraryItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool Protected { get; set; }
        public int? parentId { get; set; }
        public int? childCount { get; set; }
        public int? depthLevel { get; set; }
        public List<LibraryDirectoryModel> documents { get; set; }
        public List<LibraryDocumentModel> document { get; set; }
    }

    public interface ILibraryItem
    {
        int id { get; set; }
        string name { get; set; }
    }
}
