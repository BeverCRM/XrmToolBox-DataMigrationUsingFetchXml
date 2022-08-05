namespace NewXrmToolBoxTool1.Model
{
    class ResultItem
    {
        public string EntityName { get; set; }
        public int NumberOfSourceRecords { get; set; }
        public int NumberOfGeneratedRecords { get; set; }

        public ResultItem(string name, int numberOfSource = 0, int numberOfTarget = 0)
        {
            EntityName = name;
            NumberOfSourceRecords = numberOfSource;
            NumberOfGeneratedRecords = numberOfTarget;
        }
    }
}
