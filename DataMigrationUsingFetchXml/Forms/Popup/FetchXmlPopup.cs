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

        private string _currentFetchXml;

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
                        _currentFetchXml = xmlDoc.OuterXml;
                        textBoxFetch.Text = string.Empty;

                        if (openFileDialog.FileName != "")
                        {
                            textBoxFetch.Text = FormatFetchXmlString(_currentFetchXml);
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
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                document.LoadXml(fetchXml);
                writer.Formatting = Formatting.Indented;
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();
                mStream.Position = 0;

                StreamReader sReader = new StreamReader(mStream);
                formattedFetchXml = sReader.ReadToEnd();
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message, "Invalid FetchXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mStream.Close();
            writer.Close();

            return formattedFetchXml;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                _dataverseService.DoesValidFetchXmlExpression(textBoxFetch.Text);
                _currentFetchXml = textBoxFetch.Text;

                if (IsEdit && FetchXmls[EditIndex] == _currentFetchXml)
                {
                    DialogResult = DialogResult.OK;
                    return;
                }

                if (IsFetchDuplicate() && (FetchXmls[EditIndex] != _currentFetchXml || !IsEdit))
                {
                    return;
                }

                if (!IsEdit)
                {
                    FetchXmls.Add(_currentFetchXml);
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
                MessageBox.Show("This FetchXML already exists. A duplicate FetchXML cannot be added.", "Duplicate FetchXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}