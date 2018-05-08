using System;
using System.Collections.Generic;

namespace DPO.Common.DaikinUniversity
{
    public class SearchGlobalCatalog
    {
        public SearchGlobalCatalog()
        {
            TrainingType = new List<DaikinUniversity.TrainingType>();
            Skills = new List<string>();
            Competency = new List<string>();
            OuId = new List<string>();
            OuType = new List<string>();
        }

        public List<string> Competency { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// xml or json
        /// </summary>
        public string Format { get; set; }

        public DateTime? FromDate { get; set; }

        public bool IncludeAllVersions { get; set; }

        public bool IncludeInactive { get; set; }

        public string Instructor { get; set; }

        public string Language { get; set; }

        public string Location { get; set; }

        /// <summary>
        /// Required if OuType populated
        /// </summary>
        public List<string> OuId { get; set; }

        /// <summary>
        /// Required if OuId populated
        /// </summary>
        public List<string> OuType { get; set; }

        /// <summary>
        /// Page size is 25 records
        /// </summary>
        public int? PageNumber { get; set; }

        public string Provider { get; set; }

        public List<string> Skills { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }

        public DateTime? ToDate { get; set; }

        public List<TrainingType> TrainingType { get; set; }
    }
}