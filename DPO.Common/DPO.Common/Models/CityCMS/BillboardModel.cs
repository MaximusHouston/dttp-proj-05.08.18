 
using System.Collections.Generic;

namespace DPO.Common
{
    public class BillboardModel : PageModel
    {
        public List<PosterModel> poster { get; set; }
        public PosterModel SinglePoster { get; set; }
    }
}
