using System.Web;
using System.Web.Optimization;

namespace DPO.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725

        private static void AngularBundles(BundleCollection bundles)
        {
           
            
            //Angular JS Core
            bundles.Add(new ScriptBundle("~/bundles/angularjs")
                .Include(
                    "~/Scripts/angular.min.js",
                    "~/Scripts/angular-route.js",
                    "~/Scripts/angular-ui-router.js",
                    "~/Scripts/angular-sanitize.js",
                    "~/Scripts/angular-resource.js",
                    "~/Scripts/angular-cookies.js",
                    "~/Scripts/angular-animate.js",
                    "~/Scripts/angular-ui/ui-bootstrap.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                    "~/Scripts/ngDialog.js"
                )
            );

            bundles.Add(new ScriptBundle("~/bundles/angularjs/dpo/core")
                .Include(
                    "~/app/js/app.js",
                    "~/app/js/modules/core/core.module.js",
                    "~/app/js/modules/core/datepicker-local.directive.js",
                    "~/app/js/modules/route/routes.module.js",
                    "~/app/js/modules/route/routes.config.js"
                )
                .Include("~/app/route/config.route.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/angularjs/dpo/projects")
                .Include("~/app/js/modules/projects/projects.module.js")
                .IncludeDirectory("~/app/js/modules/projects/pipelinenotes", "*.js")
                .IncludeDirectory("~/app/js/modules/projects/services", "*.js")


                .IncludeDirectory("~/app/shared/services", "*.js")
                .IncludeDirectory("~/app/shared/components/enum", "*.js")
                .IncludeDirectory("~/app/shared/components/errorMessage", "*.js")
                .IncludeDirectory("~/app/shared/components/validationMessage", "*.js")
                .IncludeDirectory("~/app/shared/components/pageMessage", "*.js")
                .IncludeDirectory("~/app/shared/components/popup", "*.js")
                .IncludeDirectory("~/app/shared/components/verifyAddress", "*.js")
            

                .IncludeDirectory("~/app/components/project/services", "*.js")
                .IncludeDirectory("~/app/components/project/projectEditOrderForm", "*.js")
                .IncludeDirectory("~/app/components/project/projectEdit", "*.js")
                

                .IncludeDirectory("~/app/components/quote/services", "*.js")
                .IncludeDirectory("~/app/components/quote/quote", "*.js")
                .IncludeDirectory("~/app/components/quote/quoteItems", "*.js")
                .IncludeDirectory("~/app/components/quote/discountRequest", "*.js")
                .IncludeDirectory("~/app/components/quote/commissionRequest", "*.js")
                .IncludeDirectory("~/app/components/quote/orderInQuote", "*.js")
                .IncludeDirectory("~/app/components/quote/quoteButtonBar", "*.js")

                .IncludeDirectory("~/app/components/order/services", "*.js")
                .IncludeDirectory("~/app/components/order/orders", "*.js")
                .IncludeDirectory("~/app/components/order/orderForm", "*.js")
                .IncludeDirectory("~/app/components/order/orderForm/projectDetails", "*.js")
                .IncludeDirectory("~/app/components/order/orderForm/projectLocation", "*.js")
                .IncludeDirectory("~/app/components/order/orderForm/dealerContractorInfo", "*.js")
                .IncludeDirectory("~/app/components/order/orderForm/sellerInfo", "*.js")
                .IncludeDirectory("~/app/components/order/orderForm/orderDetails", "*.js")
                
            );

            
        }

        private static void BoostrapBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Script/bootbox.js"));

            bundles.Add(new StyleBundle("~/Content/css/bootstrap")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/bootstrap-theme.css")
                
            );
        }

        private static void KendoBundles(BundleCollection bundles)
        {
           
            
            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo/jszip.min.js",
                "~/Scripts/kendo/kendo.all.min.js",
                // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler

                "~/Scripts/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                "~/Content/kendo/kendo.common-bootstrap.min.css",
                "~/Content/kendo/kendo.bootstrap.min.css",
                "~/Content/daikin.kendo.custom.css"));
        }

        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js",
            //            "~/Scripts/jquery.browser.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.9.1.min.js",
                        "~/Scripts/jquery.browser.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/d3")
                .Include("~/Scripts/d3/d3.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/ckeditor").Include("~/Scripts/ckeditor/ckeditor.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/ckeditor-adapter").Include("~/Scripts/ckeditor/adapters/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/cms_library").Include("~/Scripts/cms_library.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.unobtrusive*",
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include("~/Scripts/wearesmartcookie-{version}.js", "~/Scripts/navbar.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/tablehelpers").Include("~/Scripts/tablehelpers.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/checkboxhelpers").Include("~/Scripts/checkboxhelpers.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/numericstepperhelpers").Include("~/Scripts/numericstepperhelpers.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/importitemsmodal").Include("~/Scripts/importitemsmodal.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/packagequotemodal").Include("~/Scripts/packagequotemodal.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/confirmmodal").Include("~/Scripts/confirmmodal.js"));

            var discountRequestPath = "~/Scripts/ProjectDashboard/DiscountRequest";

            bundles.Add(new ScriptBundle("~/bundles/scripts/projectdashboard/discountrequest")
                .Include(discountRequestPath + "/DiscountRequestTotals.js",
                    discountRequestPath + "/DiscountRequestFields.js",
                    discountRequestPath + "/DiscountRequest.js"));

            var commissionRequestPath = "~/Scripts/ProjectDashboard/CommissionRequest";

            bundles.Add(new ScriptBundle("~/bundles/scripts/projectdashboard/commissionrequest")
                   .Include(commissionRequestPath + "/CommissionRequestTotals.js",
                            commissionRequestPath + "/CommissionRequestFields.js",
                            commissionRequestPath + "/CommissionRequest.js"));

           
            BoostrapBundles(bundles);
            KendoBundles(bundles);
            AngularBundles(bundles);


            var overviewFolder = "~/Scripts/Overview";
            //bundles.Add(new ScriptBundle("~/bundles/scripts/projectoverview").Include("~/Scripts/projectoverview.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/overview-widgets")
                .IncludeDirectory(overviewFolder + "/Base", "*.js")
                .IncludeDirectory(overviewFolder, "*.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/usergroups").Include("~/Scripts/usergroups.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/projectedit").Include("~/Scripts/projectedit.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/productpages").Include("~/Scripts/productpages.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/nouislider").Include("~/Scripts/jquery.nouislider.js"));
            bundles.Add(new ScriptBundle("~/bundles/scripts/navbar").Include("~/Scripts/navbar.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/navbar.css", "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/css/978").Include("~/Content/978.css"));
            bundles.Add(new StyleBundle("~/Content/css/register").Include("~/Content/register.css"));
            bundles.Add(new StyleBundle("~/Content/css/login").Include("~/Content/login.css"));
            bundles.Add(new StyleBundle("~/Content/css/passwordforms").Include("~/Content/passwordforms.css"));
            bundles.Add(new StyleBundle("~/Content/css/tablestyles").Include("~/Content/tablestyles.css"));
            bundles.Add(new StyleBundle("~/Content/css/ulstyles").Include("~/Content/ulstyles.css"));
            bundles.Add(new StyleBundle("~/Content/css/lightboxes").Include("~/Content/lightboxes.css"));
            bundles.Add(new StyleBundle("~/Content/css/tabbars").Include("~/Content/tabbars.css"));
            bundles.Add(new StyleBundle("~/Content/css/adminstyles").Include("~/Content/adminstyles.css"));
            //bundles.Add(new StyleBundle("~/Content/css/fontawesome").Include("~/Content/font-awesome.min.css"));

            bundles.Add(new StyleBundle("~/Content/css/projectoverview")
                .Include("~/Content/projectoverview.css")
                .Include("~/Content/projectoverview-charts.css"));

            bundles.Add(new StyleBundle("~/Content/css/productlisting").Include("~/Content/productlisting.css"));
            bundles.Add(new StyleBundle("~/Content/css/RequestDiscountForm").Include("~/Content/RequestDiscountForm.css"));
            bundles.Add(new StyleBundle("~/Content/css/usergroups").Include("~/Content/usergroups.css"));
            bundles.Add(new StyleBundle("~/Content/css/printstyles").Include("~/Content/printstyles.css"));
            bundles.Add(new StyleBundle("~/Content/css/nouislider").Include("~/Content/jquery.nouislider.css"));
            bundles.Add(new StyleBundle("~/Content/css/navbar").Include("~/Content/navbar.css"));

            bundles.Add(new StyleBundle("~/Content/css/pdf-base").Include("~/Content/pdf-base.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/core.css",
                        "~/Content/themes/base/resizable.css",
                        "~/Content/themes/base/selectable.css",
                        "~/Content/themes/base/accordion.css",
                        "~/Content/themes/base/autocomplete.css",
                        "~/Content/themes/base/button.css",
                        "~/Content/themes/base/dialog.css",
                        "~/Content/themes/base/slider.css",
                        "~/Content/themes/base/tabs.css",
                        "~/Content/themes/base/datepicker.css",
                        "~/Content/themes/base/progressbar.css",
                        "~/Content/themes/base/theme.css"));

            //Tell the ASP.NET bundles to allow minified files in debug mode
            bundles.IgnoreList.Clear();
        }
    }
}