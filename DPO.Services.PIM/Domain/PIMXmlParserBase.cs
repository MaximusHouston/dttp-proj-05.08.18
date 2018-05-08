using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DPO.Services.PIM.Domain
{
    public abstract class PIMXmlParserBase
    {
        //private static readonly Regex m_NotDigits = new Regex(@"[\D]");
        public XDocument XmlDocument { get; set; }

        protected ILog Log = LogManager.GetLogger(typeof(PIMXmlParserBase));

        public PIMXmlParserBase(string xmlDocLocation)
        {
            XmlDocument = XDocument.Load(xmlDocLocation);
        }

        public PIMXmlParserBase(XDocument doc)
        {
            XmlDocument = doc;
        }

        /// <summary>
        /// Takes an ID Pattern from the PIM and pulls only the ID.
        /// </summary>
        /// <param name="idPattern"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string CleanLOVIDWithPattern(string idPattern, string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return id;
            }

            // Replace the ID Pattern
            if (!String.IsNullOrWhiteSpace(idPattern))
            {
                idPattern = idPattern.Replace("[id]", @"(\d+)");
                Regex reg = new Regex(String.Format(@"^{0}$", idPattern));

                Match m = reg.Match(id);

                if (m.Success && m.Groups.Count > 1)
                {
                    string newId = m.Groups[1].Value;
                    id = newId ?? id;
                }
            }

            //var split = id.Split('-');
            //if (split.Length > 1)
            //{
            //    // Return last numbers of ID
            //    return split[1];
            //}

            return id;
        }

        protected string GetAttributeValue(XElement el, string attributeName)
        {
            if (el == null)
            {
                return null;
            }

            var attr = el.Attribute(attributeName);

            if (attr == null)
            {
                return null;
            }

            return String.IsNullOrWhiteSpace(attr.Value) ? null : attr.Value;
        }

        protected string GetNodeText(XElement el)
        {
            if (el == null)
            {
                return null;
            }

            return String.IsNullOrWhiteSpace(el.Value) ? null : el.Value;
        }
    }
}
