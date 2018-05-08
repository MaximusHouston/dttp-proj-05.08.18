 
using System.ComponentModel.DataAnnotations;

namespace DPO.Common
{
    public class PosterModel
    {
        public int id { get; set; }

        [Required(ErrorMessage="Please specify a URL")]
        public string url { get; set; }

        public string image { get; set; }
        public bool enabled { get; set; }
    }
}
