 

namespace DPO.Common
{
    public class CommunicationsCentreModel : PageModel
    {
        public CommunicationsCentreModel()
        {
            videos = new CommunicationsCentreVideosModel();
        }

        public string ContactUsLink { get; set; }
        public string YouTubeLink { get; set; }
        public CommunicationsCentreVideosModel videos { get; set; }
    }
}
