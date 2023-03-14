using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Services.Interfaces;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    internal partial class FetchXmlPopup : Form
    {
        public bool IsEdit { get; set; }
        public int EditIndex { get; set; }
        public List<string> FetchXmls { get; private set; }

        private IDataverseService _dataverseService;

        private MatchingCriteria _matchingCriteria;

        public FetchXmlPopup()
        {
            InitializeComponent();
            FetchXmls = new List<string>();
            MinimumSize = new Size(350, 250);
        }

        public void SetDataverseService(IDataverseService dataverseService)
        {
            _dataverseService = dataverseService;
        }

        private void BtnBrowseFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select FetchXML File ";
                openFileDialog.Filter = "XML File (*.xml)|*.xml";

                try
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(openFileDialog.FileName);

                        if (openFileDialog.FileName != "")
                        {
                            textBoxFetch.Text = FormatFetchXmlString(xmlDoc.OuterXml);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Invalid FetchXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string FormatFetchXmlString(string fetchXml)
        {
            string formattedFetchXml = "";

            try
            {
                using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
                {
                    using (XmlTextWriter textWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented })
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(fetchXml);
                        document.WriteContentTo(textWriter);

                        formattedFetchXml = stringWriter.ToString();
                    }
                }
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message, "Invalid FetchXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return formattedFetchXml;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                _dataverseService.ThrowExceptionIfFetchXmlIsInvalid(textBoxFetch.Text);

                if (IsEdit && FetchXmls[EditIndex] == textBoxFetch.Text)
                {
                    Close();
                    return;
                }

                if (IsFetchDuplicate() && (FetchXmls[EditIndex] != textBoxFetch.Text || !IsEdit))
                {
                    return;
                }

                if (IsEdit)
                {
                    Services.ConfigReader.CurrentFetchXml = FetchXmls[EditIndex];

                    if (!_matchingCriteria.AcceptToClearMatchingCriteriaInCaseOfMissingFieldsInFetchXml(textBoxFetch.Text, EditIndex))
                    {
                        return;
                    }
                }
                else
                {
                    FetchXmls.Add(textBoxFetch.Text);
                    Services.ConfigReader.CurrentFetchXml = textBoxFetch.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Invalid FetchXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private bool IsFetchDuplicate()
        {
            if (FetchXmls.Contains(textBoxFetch.Text))
            {
                MessageBox.Show(Localization_MessageBoxText.DuplicateFetchXml, "Duplicate FetchXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

            return false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (textBoxFetch.Text != null)
            {
                textBoxFetch.Text = string.Empty;
            }

            Close();
        }

        public void SetTextBoxText(string text) => textBoxFetch.Text = text;

        public string GetTextBoxText() => textBoxFetch.Text;

        internal void SetMatchingCriteria(MatchingCriteria matchingCriteria)
        {
            _matchingCriteria = matchingCriteria;
        }
    }
}