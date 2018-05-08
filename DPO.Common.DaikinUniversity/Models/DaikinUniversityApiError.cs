using System;

namespace DPO.Common.DaikinUniversity
{
    public class DaikinUniversityApiError
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        public Guid ErrorId { get; set; }

        public string Message { get; set; }
    }
}