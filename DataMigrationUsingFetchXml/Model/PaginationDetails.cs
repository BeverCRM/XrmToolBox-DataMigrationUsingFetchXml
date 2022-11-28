namespace DataMigrationUsingFetchXml.Model
{
    public static class PaginationDetails
    {
        public static bool ContainsTopAttribute { get; set; }

        public static int PageNumber { get; set; }

        public static int PageCount { get; set; }

        public static string PagingCookie { get; set; }
    }
}