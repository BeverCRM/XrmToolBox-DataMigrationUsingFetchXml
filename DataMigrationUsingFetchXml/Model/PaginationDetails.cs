using System.Xml;

namespace DataMigrationUsingFetchXml.Model
{
    public static class PaginationDetails
    {
        public static bool ContainsTopAttribute { get; set; }
        public static int PageNumber { get; set; }
        public static int PageCount { get; set; }
        public static string PagingCookie { get; set; }

        public static void SetPaginationAttributes(string fetchXml)
        {
            PageNumber = 1;
            PageCount = 5000;
            PagingCookie = null;
            ContainsTopAttribute = false;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fetchXml);
            XmlNodeList fetchNodes = xmlDoc.DocumentElement.SelectNodes("/fetch");

            if (fetchNodes[0].Attributes["top"] != null)
            {
                ContainsTopAttribute = true;
            }
            if (fetchNodes[0].Attributes["count"] != null)
            {
                PageCount = System.Convert.ToInt32(fetchNodes[0].Attributes["count"].Value);
            }
            if (fetchNodes[0].Attributes["page"] != null)
            {
                PageNumber = System.Convert.ToInt32(fetchNodes[0].Attributes["page"].Value);
            }
        }
    }
}