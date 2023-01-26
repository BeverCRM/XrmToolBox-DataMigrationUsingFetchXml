using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    internal partial class FetchXmlPopup : Form
    {
        public bool IsEdit { get; set; }
        public int EditIndex { get; set; }
        public List<string> FetchXmls { get; private set; }

        private string _currentFetchXml;

        public FetchXmlPopup()
        {
            InitializeComponent();
            FetchXmls = new List<string>();
            MinimumSize = new Size(350, 250);
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
                    }

                    if (openFileDialog.FileName != "")
                    {
                        textBoxFetch.Text = FormatFetchXmlString(_currentFetchXml);
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
            bool isValidXml = CheckXml();
            if (isValidXml)
            {
                if (!IsEdit)
                {
                    if (IsFetchDuplicate())
                        return;

                    FetchXmls.Add(_currentFetchXml);
                }
                else
                {
                    if (FetchXmls[EditIndex] != _currentFetchXml)
                    {
                        if (IsFetchDuplicate())
                            return;
                    }
                }
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("FetchXML is not valid", "Invalid FetchXML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private bool CheckXml()
        {
            if (string.IsNullOrEmpty(textBoxFetch.Text))
            {
                return false;
            }

            string word = "</fetch>";
            int index = textBoxFetch.Text.IndexOf(word);
            int validFetchLength = index + word.Length;
            string detail = textBoxFetch.Text.Trim();
            string fetch = textBoxFetch.Text.Substring(0, validFetchLength);
            string spaceInFetch = textBoxFetch.Text.Substring(validFetchLength, textBoxFetch.Text.Length - validFetchLength);

            if (string.IsNullOrWhiteSpace(spaceInFetch))
            {
                textBoxFetch.Text = fetch;
            }
            if (!detail.StartsWith("<") && !detail.EndsWith(">"))
            {
                return false;
            }

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(string.Format("<Root>{0}</Root>", detail));
            }
            catch (XmlException)
            {
                return false;
            }

            if (textBoxFetch.Text.Length > validFetchLength)
            {
                return false;
            }
            _currentFetchXml = textBoxFetch.Text;

            return true;
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