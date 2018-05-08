using System.Web.Mvc;

namespace DPO.Web.Areas.Apps
{
    public class AppsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TradeShow";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Apps_default",
                "TradeShow/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}