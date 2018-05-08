using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Web;
using DPO.Common;
using System.Net;
using EO.Pdf;
using System.Drawing;

namespace DPO.Common
{

    public class PdfConvertor
    {
        public PdfDocument Document { get; set; }

        public HtmlToPdfOptions Options { get; set; }

        public PdfConvertor()
        {
            this.Document = new PdfDocument();

            this.Options = new StandardA4();

            EO.Pdf.Runtime.AddLicense(
            "eJ2X+Aob5HaZyeDZz53dprEh5Kvq7QAZvFusssHNn2i1kZvLn1mXwMAM66Xm" +
            "+8+4iVmXpLHn7qvb6QP07Z/mpPUM8560psjatmiptMLfoVnt6QMe6KjlwbPg" +
            "oVmmwp61n1mXpM0e6KDl5QUg8Z61190f6bHD8PPdzaHY+wsC9pPZ2fnu5na0" +
            "wMAe6KDl5QUg8Z61kZvnrqXg5/YZ8p61kZt14+30EO2s3MKetZ9Zl6TNF+ic" +
            "3PIEEMidtbjHA7ZqrrbC/tJsrrrK47CBs7P9FOKe5ff29ON3hI6xy59Zs/D6" +
            "DuSn6un26cKh6e0Ey9Ka7eX4ELto4+30EO2s3OnPuIlZl6Sx5+Cl4/MI6YxD" +
            "l6Sxy59Zl6TNDOM=");
        }
        
        public class StandardA4 : HtmlToPdfOptions
        {
            public StandardA4() : base()
            {
                this.AutoFitX = HtmlToPdfAutoFitMode.None;

                this.AutoFitY = HtmlToPdfAutoFitMode.None;

                this.NoScript = true;

                this.NoLink = true;

                this.GeneratePageImages = false;

                this.SaveImageAsJpeg = false;

                //this.PreserveHighResImages = true;

                this.AllowLocalAccess = true;

              //  this.NoCache = false;

               // EO.Pdf.HtmlToPdf.Options.MaxLoadWaitTime = 60000;

                this.PageSize = EO.Pdf.PdfPageSizes.A4;

               // this.AutoAdjustForDPI = true;

                this.OutputArea = new RectangleF(0f, 0f, 8.26f, 11.69f);

                this.BaseUrl = Utilities.DocumentServerURL();

                //this.AfterRenderPage = new EO.Pdf.PdfPageEventHandler(On_AfterRenderPage);


            }

            //This function is called after every page is created
            private void On_AfterRenderPage(object sender, EO.Pdf.PdfPageEventArgs e)
            {
                //Set the output area to the top portion of the page. Note
                //this does not change the output area of the original
                //conversion from which we are called
    //            EO.Pdf.HtmlToPdf.Options.OutputArea = new RectangleF(0, 0, 8.5f, 1f);

                //            //Render an image and a horizontal line. Note the
                //            //second argument is the PdfPage object
                //            EO.Pdf.HtmlToPdf.ConvertHtml(@"
                //                <img src='http://www.essentialobjects.com/images/logo.gif' >
                //                <br />", 
                //                e.Page);
            }
        }




        public void WriteToFile(string html, string file)
        {
            long start = DateTime.Now.Ticks;

            EO.Pdf.HtmlToPdf.ConvertHtml(html, file, new StandardA4());

            Debug.WriteLine(string.Format("Write to file time {0}ms", new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds));

        }

        public void WriteToFile(string html, string file,bool append)
        {
            PdfDocument doc = new PdfDocument();

            var options = new StandardA4();

            long start = DateTime.Now.Ticks;

            EO.Pdf.HtmlToPdf.ConvertHtml(html, doc, options);

            Debug.WriteLine(string.Format("PDF ConvertHTML Time {0}ms",new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds));

            //foreach (PdfPage page in doc.Pages)
            //{

            //    //Note the second argument is a PdfPage object, not a PdfDocument object
            //    EO.Pdf.HtmlToPdf.ConvertHtml(html, page);
            //}


            if (append)
            {
                this.Document = PdfDocument.Merge(this.Document, doc);
            }

            start = DateTime.Now.Ticks;

            doc.Save(file);

            Debug.WriteLine(string.Format("PDF Save Time {0}ms", new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds));

        }

        public void AppendHtml(string html)
        {
            EO.Pdf.HtmlToPdf.ConvertHtml(html, this.Document, this.Options);
        }

        public void AppendFile(string file)
        {
            if (System.IO.File.Exists(file))
            {
                PdfDocument doc = new PdfDocument();

                this.Document = PdfDocument.Merge(this.Document, new PdfDocument(file));
            }
        }


        public void WriteToFile(string filename)
        {
            this.Document.Save(filename);
        }


        public void WriteFileToSession(string file)
        {

            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.Headers.Remove("Content-Disposition");
            HttpContext.Current.Response.Headers.Add("Content-Disposition", "inline; filename=" + Path.GetFileName(file));
            HttpContext.Current.Response.TransmitFile(file);

        }

        public void WriteToSession(string title)
        {

            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.Headers.Remove("Content-Disposition");
            HttpContext.Current.Response.Headers.Add("Content-Disposition", "inline; filename=" + title +".pdf");

            this.Document.Save(HttpContext.Current.Response.OutputStream);
        }

        public void UrlToFile(string url,string fileName)
        {
            PdfDocument doc = new PdfDocument();

            var options = new StandardA4();

            this.Document = EO.Pdf.HtmlToPdf.ConvertUrl(url, doc, options).PdfDocument;

            WriteToFile(fileName);
        }

    }

}
