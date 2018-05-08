using System;

namespace DPO.Common
{
    public class SearchProject : Search
    {
        public long? BusinessId { get; set; }
        public int? DateTypeId { get; set; }
        public int? ExpirationDays { get; set; }
        public bool OnlyAlertedProjects { get; set; }
        public long? ProjectId { get; set; }
        public int? ProjectOpenStatusTypeId { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectStartEnd { get; set; }
        public int? ProjectStatusTypeId { get; set; }
        public ProjectLeadStatusTypeEnum? ProjectLeadStatusTypeId { get; set; }
        public bool ShowDeletedProjects { get; set; }
        public long? UserId { get; set; }
        public int? Year { get; set; }

        public ProjectDarComStatusTypeEnum? ProjectDarComStatusTypeId { get; set; }
    }
}