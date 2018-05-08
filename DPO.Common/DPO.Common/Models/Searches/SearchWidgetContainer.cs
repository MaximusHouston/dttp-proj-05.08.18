using System;

namespace DPO.Common
{
    /// <summary>
    /// Javascript cannot handle longs therefore I am adding string representations of each
    /// </summary>
    [Serializable]
    public class SearchWidgetContainer : Search
    {
        public SearchWidgetContainer()
            : base()
        {
        }

        public string BusinessId { get; set; }
        public int? DateTypeId { get; set; }
        public int? ExpirationDays { get; set; }
        public bool OnlyAlertedProjects { get; set; }
        public string ProjectId { get; set; }
        public int? ProjectOpenStatusTypeId { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectStartEnd { get; set; }
        public int? ProjectStatusTypeId { get; set; }
        public bool ShowDeletedProjects { get; set; }
        public string UserId { get; set; }
        public int? Year { get; set; }
    }
}