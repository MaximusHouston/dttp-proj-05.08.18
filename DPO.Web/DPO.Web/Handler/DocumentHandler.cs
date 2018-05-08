using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;


namespace DPO.Web
{

    public class DocumentRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string type = requestContext.RouteData.Values["type"] as string;
            string id = requestContext.RouteData.Values["id"] as string;
            return new DocumentHandler(type, id);

        }
    }

    public class DocumentHandler : IHttpHandler
    {
        private string id;
        private string type;

        public DocumentHandler(string type, string id)
        {
            this.id = id;
            this.type = type;
        }
        public bool IsReusable
        {
           get { return true; }
        }
        public void ProcessRequest(HttpContext context)
        {
            var urlAuth = Utilities.DocumentServerURL();

            if (string.Compare(this.type, "QuotePrint", true) == 0 || string.Compare(this.type, "QuotePrintWithCostPrice", true) == 0 )
            {
                var projectId = this.id;

                var quoteId = context.Request["quoteId"] as string;

                var controller = string.Format("{0}/{1}",urlAuth,"ProjectDashboard");

                var urlQuoteHeader = string.Format("{0}/{1}/{2}/{3}", controller, "QuotePrintHeader", projectId, quoteId);
                var urlQuoteBody = string.Format("{0}/{1}/{2}/{3}", controller, this.type, projectId, quoteId);
                var urlQuoteFooter = string.Format("{0}/{1}/{2}/{3}", controller, "QuotePrintFooter", projectId, quoteId);

                var pdf = new PdfConvertor();

                var web = new WebClientLocal(HttpContext.Current);

                pdf.Options.NoLink = false;

                pdf.Options.HeaderHtmlFormat = web.DownloadString(urlQuoteHeader);
                pdf.Options.FooterHtmlFormat = web.DownloadString(urlQuoteFooter);
                pdf.Options.FooterHtmlPosition = pdf.Options.OutputArea.Bottom - 1.25f ;

                pdf.Options.OutputArea = new System.Drawing.RectangleF(0f,1.25f,pdf.Options.OutputArea.Width, pdf.Options.OutputArea.Height - 2.5f);

                pdf.AppendHtml(web.DownloadString(urlQuoteBody));

                pdf.WriteToSession("Quote Print");

                return;
            }

            if (string.Compare(this.type, "SubmittalTemplate", true) == 0)
            {
                
                var productNumber = this.id;

                var submittalFile = new ProductServices().GetSubmittalDataFile(productNumber);

                if (submittalFile != null)
                {
                    context.Response.ContentType = MimeMapping.GetMimeMapping(submittalFile);

                    context.Response.TransmitFile(submittalFile);
                }

                return;
            }

            if (string.Compare(this.type, "DiscountRequestPrint", true) == 0 || string.Compare(this.type, "DiscountRequestPrintWithCostPricing", true) == 0)
            {
                long discountRequestId = long.Parse(this.id);

                var quoteId = context.Request["QuoteId"] as string;
                var projectId = context.Request["ProjectId"] as string;

                if (projectId == null)
                    projectId = context.Request.Params["amp;ProjectId"] as string;

                var controller = string.Format("{0}/{1}", urlAuth, "ProjectDashboard");

                var urlDiscountRequestFormBody = string.Format("{0}/{1}?discountRequestId={2}&projectId={3}&quoteId={4}", controller, this.type, discountRequestId, projectId, quoteId);
                var urlDiscountRequestFormHeader = string.Format("{0}/{1}", controller, "DiscountRequestPrintHeader", projectId, quoteId);
                var urlDiscountRequestFormFooter = string.Format("{0}/{1}", controller, "DiscountRequestPrintFooter");

                var pdf = new PdfConvertor();

                var web = new WebClientLocal(HttpContext.Current);

                pdf.Options.NoLink = false;
                pdf.Options.HeaderHtmlFormat = web.DownloadString(urlDiscountRequestFormHeader);
                pdf.Options.FooterHtmlFormat = web.DownloadString(urlDiscountRequestFormFooter);
                pdf.Options.FooterHtmlPosition = pdf.Options.OutputArea.Bottom - 1.25f;

                pdf.Options.OutputArea = new System.Drawing.RectangleF(0f, 1.25f, pdf.Options.OutputArea.Width, pdf.Options.OutputArea.Height - 2.5f);

                pdf.AppendHtml(web.DownloadString(urlDiscountRequestFormBody));

                var creaateDARpdf = false;

                if(context.Request.QueryString.ToString().Contains("createDARpdf=True"))
                {
                    creaateDARpdf = true;
                }

                string root = System.Web.HttpContext.Current.Server.MapPath("~");
                string parent = System.IO.Path.GetDirectoryName(root);
                string grandParent = System.IO.Path.GetDirectoryName(parent);


                string _last5DigitsOfProjectId = projectId.ToString().Substring(projectId.ToString().Length - 5);

                string nameFile = "Daikin City Discount " +
                                    DateTime.Now.ToString("MM-dd-yyyy") +
                                    "-" +
                                   _last5DigitsOfProjectId + ".pdf";
                
                string filePath = grandParent + "/CustomerDataFiles/DiscountRequestFiles/" + quoteId + "/" + nameFile;
          
                if (creaateDARpdf== true)
                {
                    pdf.Document.Save(filePath);
                }
                else
                {
                    pdf.WriteToSession("Discount Request Form");
                }

                return;
            }

            if (string.Compare(this.type, "CommissionRequestPrint", true) == 0 || string.Compare(this.type, "CommissionRequestPrintWithCostPricing", true) == 0)
            {
                long commissionRequestId = long.Parse(this.id);

                var quoteId = context.Request["QuoteId"] as string;

                var query = context.Request.Url.Query as string;
                int index = query.IndexOf("ProjectId=");
                index += 10;

                string projectId = query.Substring(index, query.Length - index);

                var controller = string.Format("{0}/{1}", urlAuth, "ProjectDashboard");

                var urlCommissionRequestFormBody = string.Format("{0}/{1}?commissionRequestId={2}&projectId={3}&quoteId={4}", controller, this.type, commissionRequestId, projectId, quoteId);
                var urlCommissionRequestFormHeader = string.Format("{0}/{1}", controller, "CommissionRequestPrintHeader", projectId, quoteId);
                var urlCommissionRequestFormFooter = string.Format("{0}/{1}", controller, "CommissionRequestPrintFooter");

                var pdf = new PdfConvertor();

                var web = new WebClientLocal(HttpContext.Current);

                pdf.Options.NoLink = false;
                pdf.Options.HeaderHtmlFormat = web.DownloadString(urlCommissionRequestFormHeader);
                pdf.Options.FooterHtmlFormat = web.DownloadString(urlCommissionRequestFormFooter);
                pdf.Options.FooterHtmlPosition = pdf.Options.OutputArea.Bottom - 1.25f;

                pdf.Options.OutputArea = new System.Drawing.RectangleF(0f, 1.25f, pdf.Options.OutputArea.Width, pdf.Options.OutputArea.Height - 2.5f);

                pdf.AppendHtml(web.DownloadString(urlCommissionRequestFormBody));

                pdf.WriteToSession("Commission Request Form");

                return;
            }

            if(string.Compare(this.type, "OrderPrint", true) == 0 || string.Compare(this.type, "OrderPrintWithCostPricing", true) == 0)
            {
                var query = context.Request.Url.Query as string;

                var projectId = context.Request["projectId"] as string;
                
                query = query.Replace("&amp", "");
                int position = query.IndexOf("QuoteId=");
                position += 8;

                var quoteId = query.Substring(position, query.Length - position);

                long orderId = long.Parse(this.id);
               
                var controller = string.Format("{0}/{1}", urlAuth, "ProjectDashboard");

                var urlOrderFormBody = string.Format("{0}/{1}?orderId={2}&projectId={3}&quoteId={4}", controller, this.type, orderId, projectId, quoteId);
                var urlOrderFormHeader = string.Format("{0}/{1}", controller, "OrderPrintHeader", projectId, quoteId);
                var urlOrderFormFooter = string.Format("{0}/{1}", controller, "OrderPrintFooter");

                var pdf = new PdfConvertor();

                var web = new WebClientLocal(HttpContext.Current);

                pdf.Options.NoLink = false;
                pdf.Options.HeaderHtmlFormat = web.DownloadString(urlOrderFormHeader);
                pdf.Options.FooterHtmlFormat = web.DownloadString(urlOrderFormFooter);
                pdf.Options.FooterHtmlPosition = pdf.Options.OutputArea.Bottom - 1.25f;

                pdf.Options.OutputArea = new System.Drawing.RectangleF(0f, 1.25f, pdf.Options.OutputArea.Width, pdf.Options.OutputArea.Height - 2.5f);

                pdf.AppendHtml(web.DownloadString(urlOrderFormBody));


                //string root = System.Web.HttpContext.Current.Server.MapPath("~");
                //string parent = System.IO.Path.GetDirectoryName(root);
                //string grandParent = System.IO.Path.GetDirectoryName(parent);

                //string nameFile = orderId.ToString() + ".pdf";

                //string subPath = grandParent + "/CustomerDataFiles/OrderSubmitFiles/" + orderId;

                //bool exists = System.IO.Directory.Exists(subPath);

                //if (!exists)
                //    System.IO.Directory.CreateDirectory((subPath));

                //string filePath = grandParent + "/CustomerDataFiles/OrderSubmitFiles/" + orderId + "/" + nameFile;

                //pdf.Document.Save(filePath);

                pdf.WriteToSession("Order Form");

                return;
            }

            //http://www.daikincity.com/document/dar/320370835699269632/?filename=132%20W%20%2026th%20St%20%20Quote%20_Rose%20Mech_REV%20_%204%202%2015.pdf
            if (string.Compare(this.type, "DAR", true) == 0)
            {
                long quoteid = long.Parse(this.id);

                string darfile = Path.Combine(Utilities.GetDARDirectory(quoteid), context.Request["filename"] as string);

                if (new AccountServices().CanAccessQuote(HttpContext.Current.User.Identity.Name, quoteid) && File.Exists(darfile))
                {
                    context.Response.ContentType = MimeMapping.GetMimeMapping(darfile);

                    context.Response.TransmitFile(darfile);
                }
                return;
            }

            //http://www.daikincity.com/document/quoteorder/320370835699269632/?filename=132%20W%20%2026th%20St%20%20Quote%20_Rose%20Mech_REV%20_%204%202%2015.pdf
            if (string.Compare(this.type, "QuoteOrder", true) == 0)
            {
                long quoteid = long.Parse(this.id);

                string poAttachment = Path.Combine(Utilities.GetPOAttachmentDirectory(quoteid), context.Request["filename"] as string);

                if (new AccountServices().CanAccessQuote(HttpContext.Current.User.Identity.Name, quoteid) && File.Exists(poAttachment))
                {
                    context.Response.ContentType = MimeMapping.GetMimeMapping(poAttachment);

                    context.Response.TransmitFile(poAttachment);
                }
                return;
            }

            context.Response.Clear();

            var file = Utilities.GetDocumentLocation(type, id);

            //TODO Get image from webservice then save as file
            if (file == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.ContentType = MimeMapping.GetMimeMapping(file);

            context.Response.TransmitFile(file);

            return;

        }


   
    }
}
