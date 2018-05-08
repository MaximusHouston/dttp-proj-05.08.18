using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Elmah;
using System.Web;
using System.Linq.Expressions;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;

namespace DPO.Common
{
    public static class HttpPostedFileBaseExtensions
    {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(this HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            var type = postedFile.ContentType.ToLower();

            if (type != "image/jpg" &&
                        type != "image/jpeg" &&
                        type != "image/pjpeg" &&
                        type != "image/gif" &&
                       type != "image/x-png" &&
                       type != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            var extension = Path.GetExtension(postedFile.FileName).ToLower();
            if (extension != ".jpg"
                && extension != ".png"
                && extension != ".gif"
                && extension != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

    public static class StringExtensions
    {
        public static string ToUpperTrim(this string value)
        {
            return value.Trim().ToUpper();
        }
    }

    public partial class Utilities
    {
        public static string SavePostedFile(HttpPostedFileBase postFile, string directory, int maxfileSizeK)
        {
            if (postFile != null && postFile.ContentLength > 0)
            {
                if (postFile.ContentLength > maxfileSizeK * 1024)
                {
                    return string.Format(Resources.ResourceModelProject.MP026, maxfileSizeK / 1024);
                }

                var filename = Path.GetFileName(postFile.FileName);

                var file = Path.Combine(directory, filename);

                switch (Path.GetExtension(filename).ToLower())
                {
                    case ".exe":
                    case ".com":
                    case ".pif":
                    case ".bat":
                    case ".scr":
                        return (Resources.SystemMessages.SM008);
                }

                if (file != null)
                {
                    List<String> illegalCharacters = new List<String> { "@", "%", "*", "#", "&" };
                    List<string> removeCharacters = new List<string> { "'", "~" };

                    foreach (string chac in illegalCharacters)
                    {
                        file = file.Replace(chac, "_");
                    }

                    foreach (string chac in removeCharacters)
                    {
                        file = file.Replace(chac, "");
                    }
                }

                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                postFile.SaveAs(file);
            }
            return null;
        }

        public static string DocumentServerURL()
        {
            return @"http://" + Utilities.Config("dpo.setup.documentservice.url");

            // return HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
        }

        public static bool IsDocumentServer()
        {
            if (HttpContext.Current == null) return false;

            var hostPort = HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port.ToString();

            return (string.Compare(hostPort, Utilities.Config("dpo.setup.docmentservice.url"), true) == 0);
        }

        public static string GetQuotePackageDirectory(long quoteId)
        {
            var directory = GetDocumentDirectory() + Utilities.Config("dpo.setup.customerdata.location") + "QuotePackage\\" + quoteId.ToString() + "\\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static string GetQuotePackageFile(long quoteId)
        {
            var baseDirectory = GetQuotePackageDirectory(quoteId);
            return Directory.GetFiles(baseDirectory, QuotePackageFileName(quoteId)).FirstOrDefault();
        }

        public static string GetPOAttachmentDirectory(long quoteId)
        {
            var directory = GetDocumentDirectory() + Utilities.Config("dpo.setup.customerdata.location") + "POAttachment\\" + quoteId.ToString() + "\\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static string GetDocumentLocation(string type, string id)
        {
            var baseDirectory = Utilities.GetDocumentDirectory();

            if (!Directory.Exists(baseDirectory + type)) return null;

            var file = Directory.GetFiles(baseDirectory + type, id + ".*").FirstOrDefault();

            return file;
        }

        public static string QuotePackageFileName(long quoteId)
        {
            return "DPO_QuotePackage_" + quoteId.ToString() + ".zip";
        }

        public static string GetDARDirectory(long quoteId)
        {
            var directory = GetDocumentDirectory() + Utilities.Config("dpo.setup.customerdata.location") + "DiscountRequestFiles\\" + quoteId.ToString() + "\\"; ;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static string GetDocumentDirectory()
        {
            var directory = GetWebDirectory() + Utilities.Config("dpo.setup.document.location");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static string GetSubmittalDirectory()
        {
            var directory = Utilities.GetDocumentDirectory() + @"Submittal Data\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        //LMW added 02/18
        public static string GetAutoSubmittalDirectory()
        {
            var directory = Utilities.GetDocumentDirectory() + @"Submittal Data AutoGen\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static string GetDaikinCityDirectory()
        {
            var directory = GetWebDirectory() + Utilities.Config("dpo.setup.daikincity.location");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static string GetWebDirectory()
        {
            var directory = Utilities.Config("dpo.setup.web.location");

            if (String.IsNullOrWhiteSpace(directory))
            {
                directory = System.Web.HttpRuntime.AppDomainAppPath;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static string GetServicesDirectory()
        {
            var directory = Utilities.Config("dpo.setup.services.location");

            if (String.IsNullOrWhiteSpace(directory))
            {
                directory = System.Web.HttpRuntime.AppDomainAppPath;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public class FastGetProperty<TObject>
        {
            public string PropertyName { get; set; }
            public Func<TObject, object> GetValue { get; set; }
        }

        // returns property getter
        public static FastGetProperty<TObject> FastGetPropertySetup<TObject>(string propertyName)
        {
            var paramExpression = Expression.Parameter(typeof(TObject), "value");

            Expression propertyGetterExpression = Expression.Property(paramExpression, propertyName);

            if (propertyGetterExpression.Type.IsValueType)
                propertyGetterExpression = Expression.Convert(propertyGetterExpression, typeof(object));

            var result = Expression.Lambda<Func<TObject, object>>(propertyGetterExpression, paramExpression).Compile();

            return new FastGetProperty<TObject> { PropertyName = propertyName, GetValue = result };
        }

        // returns property setter:
        public static Action<TObject, TProperty> FastSetPropertySetup<TObject, TProperty>(string propertyName)
        {
            var paramExpression = Expression.Parameter(typeof(TObject));

            var paramExpression2 = Expression.Parameter(typeof(TProperty), propertyName);

            var propertyGetterExpression = Expression.Property(paramExpression, propertyName);

            var result = Expression.Lambda<Action<TObject, TProperty>>
            (
                Expression.Assign(propertyGetterExpression, paramExpression2), paramExpression, paramExpression2
            ).Compile();

            return result;
        }

        public static string Trim(string trim)
        {
            if (trim == null) return null;
            return trim.Trim();
        }

        public static string Upper(string upper)
        {
            if (upper == null) return null;
            return upper.ToUpper();
        }

        public static DateTime? ExecutionTime(DateTime? startTime, int maxMillisecondsAllowed)
        {
            double timeTaken = (DateTime.Now - startTime.Value).TotalMilliseconds;

            Debug.WriteLine(string.Format("Time taken {0}", timeTaken));

            if (maxMillisecondsAllowed > 0 && timeTaken <= maxMillisecondsAllowed)
            {
                return (DateTime?)null;
            }

            return DateTime.Now;
        }

        public static dynamic Merge(object item1, object item2)
        {
            if (item1 == null || item2 == null)
                return item1 ?? item2 ?? new ExpandoObject();

            dynamic expando = new ExpandoObject();
            var result = expando as IDictionary<string, object>;
            foreach (System.Reflection.PropertyInfo fi in item1.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item1, null);
            }
            foreach (System.Reflection.PropertyInfo fi in item2.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item2, null);
            }
            return result;
        }

        public static string Config(string key)
        {
            var domain = ConfigurationManager.AppSettings["dpo.sys.domain"].ToLower();

            var result = ConfigurationManager.AppSettings[key + "." + domain];

            if (result == null)
            {
                result = ConfigurationManager.AppSettings[key];
            }
            return result;
        }

        public static bool IsLive()
        {
            return ConfigurationManager.AppSettings["dpo.sys.domain"].ToLower() == "live";
        }

        public static void Copy<T>(T fromEntity, T toObject, params string[] ignoreList) where T : class, new()
        {
            var type = typeof(T);

            var props = type.GetProperties().Where(s => !ignoreList.Contains(s.Name)).ToDictionary(k => k.Name);

            foreach (var s1 in props)
            {
                var t = s1.Value.PropertyType;
                // Only compare value types.

                if (t.IsPrimitive || t.IsValueType || (t == typeof(string)))
                {
                    var prop = props[s1.Value.Name];

                    var setMethod = prop.GetSetMethod();

                    if (setMethod != null && setMethod.IsPublic)
                    {
                        var value = prop.GetValue(fromEntity, null);

                        prop.SetValue(toObject, value);
                    }
                }
            }
        }

        //public static bool IsUserLevelPermission(PermissionTypeEnum type)
        //{
        //    return type < PermissionTypeEnum.GroupStart;
        //}

        //public static bool IsGroupLevelPermission(PermissionTypeEnum type)
        //{
        //    return type > PermissionTypeEnum.GroupStart;
        //}

        /// <summary>
        /// Log error to Elmah
        /// </summary>
        public static void ErrorLog(Exception ex, string contextualMessage = null)
        {
            try
            {
                // log error to Elmah
                if (contextualMessage != null)
                {
                    // log exception with contextual information that's visible when 
                    // clicking on the error in the Elmah log
                    var annotatedException = new Exception(contextualMessage, ex);
                    ErrorSignal.FromCurrentContext().Raise(annotatedException, HttpContext.Current);
                }
                else
                {
                    ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                }

            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
        }

        public static void Log(string message)
        {
            try
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Elmah.ApplicationException(message));
            }
            catch
            {
                // uh oh! just keep going
            }
        }
    }
}
