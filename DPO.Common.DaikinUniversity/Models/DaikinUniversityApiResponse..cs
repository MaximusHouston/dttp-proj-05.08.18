using System;
using System.Collections;
using System.Collections.Generic;

namespace DPO.Common.DaikinUniversity
{
    public class DaikinUniversityApiResponse<T> : IDaikinUniversityApiResponse
    {
        public int? CreatedRecords { get; set; }

        public List<T> Data { get; set; }

        IEnumerable IDaikinUniversityApiResponse.Data
        {
            get
            {
                return this.Data;
            }
        }

        public DaikinUniversityApiError Error { get; set; }

        public string Status { get; set; }

        public DateTime Timestamp { get; set; }

        public int? TotalRecords { get; set; }

        public List<DaikinUniversityApiMessage> Validations { get; set; }
    }
}