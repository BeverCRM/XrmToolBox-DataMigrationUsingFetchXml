using System.Xml;
using System.Collections.Generic;

namespace DataMigrationUsingFetchXml.Services
{
    internal sealed class ConfigReader
    {
        public static bool ContainsPaginationAttribute { get; set; }

        public static List<string> GetPrimaryFields(string fetchXml, out bool idExists)
        {
            idExists = false;
            List<string> searchAttrs = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fetchXml);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/fetch/entity/attribute");
            XmlNodeList fetchNodes = xmlDoc.DocumentElement.SelectNodes("/fetch");
            string entityName = xmlDoc.DocumentElement.SelectNodes("/fetch/entity")[0].Attributes["name"].Value;

            if (fetchNodes[0].Attributes["top"] != null || fetchNodes[0].Attributes["count"] != null || fetchNodes[0].Attributes["page"] != null)
            {
                ContainsPaginationAttribute = true;
            }

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["SearchByPrimaryField"] != null && node.Attributes["SearchByPrimaryField"].Value == "true")
                {
                    searchAttrs.Add(node.Attributes["name"].Value);
                }
                if (node.Attributes["name"] != null && node.Attributes["name"].Value == entityName + "id")
                {
                    idExists = true;
                }
            }

            return searchAttrs;
        }

        public static string CreateXml(string xml, string cookie, int page, int count)
        {
            // Load document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

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

        public static string CreateXml(string xml)
        {
            // Load document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return doc.OuterXml;
        }
    }
}