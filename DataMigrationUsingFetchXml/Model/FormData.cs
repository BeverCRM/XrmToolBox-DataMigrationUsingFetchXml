using System.Windows.Forms;

namespace DataMigrationUsingFetchXml.Model
{
    public sealed class FormData
    {
        public Label LblInfo { get; set; }
        public Label LblTitle { get; set; }
        public Label LblError { get; set; }

        public FormData(Label lblInfo, Label lblTitle, Label lblError)
        {
            LblInfo = lblInfo;
            LblTitle = lblTitle;
            LblError = lblError;
        }
    }
}
