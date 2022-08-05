using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace NewXrmToolBoxTool1.Model
{
    public class ConfigReader
    {
        public static string GetQuery(string entityName, out List<string> searchAttrs, string fetchPath)
        {
            // Load the xml file into XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fetchPath + "\\" + entityName + ".xml");

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);

            xmlDoc.WriteTo(xw);
            string fetchXml = sw.ToString();
            searchAttrs = new List<string>();

            string[] fetchSplit = fetchXml.Split('$');

            if (fetchSplit.Length > 1)
            {
                foreach (string attr in fetchSplit)
                {
                    if (!attr.StartsWith("\"") && !attr.EndsWith("\""))
                        searchAttrs.Add(attr);
                }
            }
            fetchXml = fetchXml.Replace("$", "");

            return fetchXml;
        }

        public static string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }
    }
}