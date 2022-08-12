using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services
{
    internal sealed class ConfigReader
    {
        public static string GetQuery(string entityName, out List<string> searchAttrs, string fetchPath)
        {
            // Load the xml file into XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fetchPath + "\\" + entityName + ".xml");

            string fetchXml = xmlDoc.OuterXml;
            searchAttrs = new List<string>();

            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/fetch/entity/attribute");

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["id"] != null)
                {
                    searchAttrs.Add(node.Attributes["name"].Value);
                }
            }

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

            return doc.OuterXml;
        }
    }
}