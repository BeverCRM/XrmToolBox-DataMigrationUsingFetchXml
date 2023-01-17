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

        public static void SetPaginationAttributes(string fetchXml)
        {
            PaginationDetails.PageNumber = 1;
            PaginationDetails.PageCount = 5000;
            PaginationDetails.PagingCookie = null;
            PaginationDetails.ContainsTopAttribute = false;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fetchXml);
            XmlNodeList fetchNodes = xmlDoc.DocumentElement.SelectNodes("/fetch");

            if (fetchNodes[0].Attributes["top"] != null)
            {
                PaginationDetails.ContainsTopAttribute = true;
            }
            if (fetchNodes[0].Attributes["count"] != null)
            {
                PaginationDetails.PageCount = System.Convert.ToInt32(fetchNodes[0].Attributes["count"].Value);
            }
            if (fetchNodes[0].Attributes["page"] != null)
            {
                PaginationDetails.PageNumber = System.Convert.ToInt32(fetchNodes[0].Attributes["page"].Value);
            }
        }

        public static string CreatePaginationAttributes(string xml, string cookie)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlAttributeCollection attributeCollection = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attributeCollection.Append(pagingAttr);
            }
            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(PaginationDetails.PageNumber);
            attributeCollection.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(PaginationDetails.PageCount);
            attributeCollection.Append(countAttr);

            return doc.OuterXml;
        }

        public static List<string> GetAttributesNames(string fetchXml)
        {
            List<string> attributeNames = new List<string>();
            XmlNodeList nodes = GetFetchNodes(fetchXml);

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["name"] != null)
                {
                    attributeNames.Add(node.Attributes["name"].Value);
                }
            }

            return attributeNames;
        }

        private static XmlNodeList GetFetchNodes(string fetch)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fetch);

            return xmlDoc.DocumentElement.SelectNodes("/fetch/entity/attribute");
        }

        private static string GetEntityName(string fetch)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fetch);
            XmlNode entityTag = xmlDoc.DocumentElement.SelectSingleNode("/fetch/entity");

            return entityTag.Attributes["name"].Value;
        }

        public static string GetFetchXmlPrimaryKey(string fetchXml)
        {
            XmlNodeList nodes = GetFetchNodes(fetchXml);
            string entityName = GetEntityName(fetchXml);

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["name"] != null && node.Attributes["name"].Value == entityName + "id")
                {
                    return node.Attributes["name"].Value;
                }
            }

            return null;
        }
    }
}