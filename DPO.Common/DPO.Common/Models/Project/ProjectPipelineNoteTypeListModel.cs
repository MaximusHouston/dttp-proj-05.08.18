 
using System.Collections.Generic; 

namespace DPO.Common
{
    public class ProjectPipelineNoteTypeListModel : Search
    {
        public PagedList<ProjectPipelineNoteTypeModel> Items { get; set; }

        public ProjectPipelineNoteTypeListModel()
        {
            Items = new PagedList<ProjectPipelineNoteTypeModel>(new List<ProjectPipelineNoteTypeModel>());
        }
    }
}
