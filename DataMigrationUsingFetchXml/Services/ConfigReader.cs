using System.Xml;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;

namespace DataMigrationUsingFetchXml.Services
{
    internal sealed class ConfigReader
    {
        public static string CurrentFetchXml { get; set; }

        public static List<string> GetPrimaryFields(out bool idExists)
        {
            idExists = false;
            List<string> searchAttrs = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(CurrentFetchXml);
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

        public static void SetPaginationAttributes()
        {
            PaginationDetails.PageNumber = 1;
            PaginationDetails.PageCount = 5000;
            PaginationDetails.PagingCookie = null;
            PaginationDetails.ContainsTopAttribute = false;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(CurrentFetchXml);
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

        public static string CreateElementInFetchXml(string fetchXml, string elementValue)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(fetchXml);
            XmlNode RootNode = xmlDocument.SelectSingleNode("//entity");

            XmlElement xmlElement = xmlDocument.CreateElement("attribute");
            xmlElement.SetAttribute("name", elementValue);
            RootNode.AppendChild(xmlElement);

            return xmlDocument.InnerXml;
        }

        public static string CreatePaginationAttributes(string cookie)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(CurrentFetchXml);
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

        public static List<string> GetAttributesNames()
        {
            List<string> attributeNames = new List<string>();
            XmlNodeList attributeNodes = GetMainEntityAttributeNodesFromFetchXml();

            foreach (XmlNode node in attributeNodes)
            {
                if (node.Attributes["name"] != null)
                {
                    attributeNames.Add(node.Attributes["name"].Value);
                }
            }

            return attributeNames;
        }

        private static XmlNodeList GetMainEntityAttributeNodesFromFetchXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(CurrentFetchXml);

            return xmlDoc.DocumentElement.SelectNodes("/fetch/entity/attribute");
        }

        public static string GetEntityName()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(CurrentFetchXml);
            XmlNode entityTag = xmlDoc.DocumentElement.SelectSingleNode("/fetch/entity");

            return entityTag.Attributes["name"].Value;
        }

        public static string GetFetchXmlPrimaryKey()
        {
            XmlNodeList attributeNodes = GetMainEntityAttributeNodesFromFetchXml();
            string entityName = GetEntityName();

            foreach (XmlNode node in attributeNodes)
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