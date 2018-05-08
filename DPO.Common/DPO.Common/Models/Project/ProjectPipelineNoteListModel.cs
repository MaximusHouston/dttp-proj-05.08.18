 
using System.Collections.Generic; 

namespace DPO.Common
{
    public class ProjectPipelineNoteListModel : Search
    {
        public PagedList<ProjectPipelineNoteModel> Items { get; set; }
        
        public ProjectPipelineNoteListModel()
        {
            Items = new PagedList<ProjectPipelineNoteModel>(new List<ProjectPipelineNoteModel>());
        }
    }
}
