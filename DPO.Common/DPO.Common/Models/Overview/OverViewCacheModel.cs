

namespace DPO.Common
{
    public class OverViewCacheModel
    {
        public string CurrentUserId { get; set; }
        public string TemplateId { get; set; }
        public OverviewFilter filter { get; set; }
       
    }

    public class OverviewFilter {
        public string UserId { get; set; }
        public string BussinessId { get; set; }
        public int? ProjectStatusTypeId { get; set; }
        public int? ProjectOpenStatusTypeId { get; set; }
        public int? Year { get; set; }
    }
}
