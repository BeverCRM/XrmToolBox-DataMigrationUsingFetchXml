using System.Xml;
using System.Collections.Generic;

namespace XrmMigrationUtility.Services
{
    internal sealed class ConfigReader
    {
        public static string GetQuery(string entityName, out List<string> searchAttrs, string fetchPath, out bool idExists)
        {
            idExists = false;
            // Load the xml file into XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fetchPath + "\\" + entityName + ".xml");

            string fetchXml = xmlDoc.OuterXml;
            searchAttrs = new List<string>();

            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/fetch/entity/attribute");

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

            return fetchXml;
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
    }
}