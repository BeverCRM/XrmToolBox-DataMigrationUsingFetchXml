using System.Xml;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;

namespace DataMigrationUsingFetchXml.Services
{
    internal sealed class ConfigReader
    {
        public static List<string> GetPrimaryFields(string fetchXml, out bool idExists)
        {
            idExists = false;
            List<string> searchAttrs = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fetchXml);
            XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("/fetch/entity/attribute");
            string entityName = xmlDoc.DocumentElement.SelectNodes("/fetch/entity")[0].Attributes["name"].Value;

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

        public static string CreateXml(string xml, string cookie)
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
            pageAttr.Value = System.Convert.ToString(PaginationDetails.PageNumber);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(PaginationDetails.PageCount);
            attrs.Append(countAttr);

            return doc.OuterXml;
        }
    }
}