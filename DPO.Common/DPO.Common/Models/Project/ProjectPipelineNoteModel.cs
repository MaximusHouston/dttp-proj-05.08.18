 
namespace DPO.Common
{
    public class ProjectPipelineNoteModel : PageModel
    {
        public long? ProjectPipelineNoteId { get; set; }

        public ProjectPipelineNoteTypeModel ProjectPipelineNoteType { get; set; }

        public string ProjectPipelineNoteTypeName { get; set; }

        public long ProjectId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public long OwnerId { get; set; }

        public string OwnerName { get; set; }
    }
}
