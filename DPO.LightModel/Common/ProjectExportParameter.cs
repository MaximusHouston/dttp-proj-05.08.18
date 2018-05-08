using DPO.Common;

namespace DPO.Model.Light
{
    //public class ProjectExportParameter : QueryInfo
    public class ProjectExportParameter 
    {
        public long ProjectId {get; set;}
        public ProjectExportTypeEnum? ProjectExportType { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }
        public bool ShowDeletedProjects { get; set; }
        public bool ReturnTotals { get; set; }
        public int TotalRecords { get; set; }
    }
}
