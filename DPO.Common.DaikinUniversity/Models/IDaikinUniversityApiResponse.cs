using System;
using System.Collections;
using System.Collections.Generic;

namespace DPO.Common.DaikinUniversity
{
    public interface IDaikinUniversityApiResponse
    {
        IEnumerable Data { get; }

        DaikinUniversityApiError Error { get; set; }

        string Status { get; set; }

        DateTime Timestamp { get; set; }

        int? TotalRecords { get; set; }

        List<DaikinUniversityApiMessage> Validations { get; set; }
    }
}