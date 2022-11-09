using System;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace XrmMigrationUtility.Forms.Popup
{
    internal partial class Popup : Form
    {
        public bool IsEdit { get; set; }
        public int EditIndex { get; set; }
        public List<string> FetchXmls { get; private set; }
        public TextBox TextBoxFetch { get; private set; }

        private string _currentFetchXml;

        public Popup()
        {
            InitializeComponent();
            FetchXmls = new List<string>();
            TextBoxFetch = textBoxFetch;
            MinimumSize = new Size(350, 250);
        }

        private void BtnBrowseFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select FetchXML File ";
                openFileDialog.Filter = "XML File (*.xml)|*.xml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(openFileDialog.FileName);
                    _currentFetchXml = xmlDoc.OuterXml;
                    textBoxFetch.Text = string.Empty;
                }
                try
                {
                    if (openFileDialog.FileName != "")
                    {
                        int index = 0;
                        int startIndex = 0;
                        while (index >= 0)
                        {
                            index = _currentFetchXml.IndexOf(">", index);
                            if (index == -1)
                            {
                                break;
                            }
                            textBoxFetch.Text += _currentFetchXml.Substring(startIndex, ++index - startIndex);
                            textBoxFetch.Text += Environment.NewLine;
                            startIndex = index;
                        }
                        textBoxFetch.Text = textBoxFetch.Text.Remove(textBoxFetch.Text.Length - 2, 2);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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
            if (string.IsNullOrEmpty(textBoxFetch.Text)) return false;

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
            if (!detail.StartsWith("<") && !detail.EndsWith(">")) return false;

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(string.Format("<Root>{0}</Root>", detail));
            }
            catch (XmlException)
            {
                return false;
            }

            if (textBoxFetch.Text.Length > validFetchLength) return false;

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
    }
}