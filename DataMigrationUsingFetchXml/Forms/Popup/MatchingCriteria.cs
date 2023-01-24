using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Services;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    public partial class MatchingCriteria : Form
    {
        public static List<List<string>> SelectedAttributeNames { get; }
        public static List<List<string>> SelectedLogicalOperators { get; }

        private readonly List<List<string>> _fetchXmlAttributesNames = new List<List<string>>();
        private readonly List<TableLayoutPanel> _logicalOperatorsPanels = new List<TableLayoutPanel>();
        private readonly List<TableLayoutPanel> _attributeNamesPanels = new List<TableLayoutPanel>();
        private readonly List<string> _primaryKeyNames = new List<string>();
        private int _rowIndex;

        public MatchingCriteria()
        {
            InitializeComponent();
        }

        static MatchingCriteria()
        {
            SelectedAttributeNames = new List<List<string>>();
            SelectedLogicalOperators = new List<List<string>>();
        }

        private void CreateAttributeNamesPanel(int index)
        {
            foreach (var item in _attributeNamesPanels)
            {
                if (Controls.Contains(item))
                {
                    Controls.Remove(item);
                }
            }

            TableLayoutPanel attributeNamesPanel = new TableLayoutPanel
            {
                AutoSize = true,
                Location = new System.Drawing.Point(58, 2),
                Name = "attributeNamesPanel",
                Size = new System.Drawing.Size(215, 50),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                RowCount = 2,
                ColumnCount = 2
            };
            attributeNamesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            attributeNamesPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            attributeNamesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            attributeNamesPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            attributeNamesPanel.Controls.Add(new Label() { Text = "Field" }, 0, 0);
            attributeNamesPanel.Controls.Add(new Label() { Text = "Criteria" }, 1, 0);
            Controls.Add(attributeNamesPanel);

            if (index != -1)
            {
                _attributeNamesPanels.Insert(index, attributeNamesPanel);
            }
            else
            {
                _attributeNamesPanels.Add(attributeNamesPanel);
            }
        }

        private void CreateLogicalOperatorsPanel(int index)
        {
            foreach (var item in _logicalOperatorsPanels)
            {
                if (Controls.Contains(item))
                {
                    Controls.Remove(item);
                }
            }

            TableLayoutPanel logicalOperatorPanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                AutoSize = true,
                Location = new System.Drawing.Point(1, 43),
                Name = "logicalOperatorPanel",
                Size = new System.Drawing.Size(25, 25),
                RowCount = 1,
                ColumnCount = 1
            };
            logicalOperatorPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            logicalOperatorPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            Controls.Add(logicalOperatorPanel);

            if (index != -1)
            {
                _logicalOperatorsPanels.Insert(index, logicalOperatorPanel);
            }
            else
            {
                _logicalOperatorsPanels.Add(logicalOperatorPanel);
            }
        }

        private void ChangeAttributeNamesBoxItems(List<string> attributeNames)
        {
            int index = 0;
            foreach (var control in _attributeNamesPanels[_rowIndex].Controls)
            {
                if (control is ComboBox attributeNamesBox)
                {
                    attributeNamesBox.Items.Clear();
                    if (!attributeNamesBox.Enabled)
                    {
                        attributeNamesBox.Items.Add(_primaryKeyNames[_rowIndex]);
                    }
                    foreach (var attributeName in attributeNames)
                    {
                        attributeNamesBox.Items.Add(attributeName);
                    }
                    attributeNamesBox.SelectedItem = SelectedAttributeNames[_rowIndex][index++];
                }
            }
        }

        public void CreateLayoutPanels(string fetchXml, int rowIndex)
        {
            int index = rowIndex;
            _rowIndex = rowIndex;
            ConfigReader.CurrentFetchXml = fetchXml;
            string primaryKeyName = ConfigReader.GetFetchXmlPrimaryKey();
            List<string> attributeNames = ConfigReader.GetAttributesNames();

            if (rowIndex != -1)
            {
                if (SelectedAttributeNames[rowIndex].Count > 0 && (SelectedAttributeNames[rowIndex].Contains(_primaryKeyNames[rowIndex]) || (!SelectedAttributeNames[rowIndex].Contains(_primaryKeyNames[rowIndex]) && primaryKeyName == null)))
                {
                    bool clearSelectedAttributes = false;
                    foreach (var item in SelectedAttributeNames[rowIndex])
                    {
                        if (!attributeNames.Contains(item))
                        {
                            clearSelectedAttributes = true;
                            break;
                        }
                    }
                    if (!clearSelectedAttributes)
                    {
                        attributeNames.Remove(primaryKeyName);
                        _primaryKeyNames[rowIndex] = primaryKeyName;
                        _fetchXmlAttributesNames[rowIndex] = attributeNames;
                        ChangeAttributeNamesBoxItems(attributeNames);

                        return;
                    }
                }
                SelectedAttributeNames[rowIndex].Clear();
                SelectedLogicalOperators[rowIndex].Clear();
                RemoveLayoutPanelData(rowIndex, true);
                _primaryKeyNames.Insert(rowIndex, primaryKeyName);
                _fetchXmlAttributesNames.Insert(rowIndex, attributeNames);
            }
            else
            {
                _primaryKeyNames.Add(primaryKeyName);
                SelectedAttributeNames.Add(new List<string>());
                SelectedLogicalOperators.Add(new List<string>());
                _fetchXmlAttributesNames.Add(attributeNames);
                index = _fetchXmlAttributesNames.Count - 1;
            }
            CreateAttributeNamesPanel(rowIndex);
            CreateLogicalOperatorsPanel(rowIndex);

            ComboBox attributeNamesBox = new ComboBox();
            attributeNamesBox.DropDownStyle = ComboBoxStyle.DropDownList;

            ComboBox logicalOperatorBox = new ComboBox();
            logicalOperatorBox.Size = new System.Drawing.Size(50, 50);
            logicalOperatorBox.DropDownStyle = ComboBoxStyle.DropDownList;
            logicalOperatorBox.Items.Add("And");
            logicalOperatorBox.Items.Add("OR");

            if (_primaryKeyNames[index] != null)
            {
                attributeNamesBox.Enabled = false;
                attributeNamesBox.Items.Add(_primaryKeyNames[index]);
                attributeNamesBox.SelectedItem = _primaryKeyNames[index];
                _fetchXmlAttributesNames[index].Remove(_primaryKeyNames[index]);
                logicalOperatorBox.Enabled = false;
                logicalOperatorBox.SelectedItem = "OR";
            }
            else
            {
                foreach (var item in _fetchXmlAttributesNames[index])
                {
                    attributeNamesBox.Items.Add(item);
                }
            }
            _logicalOperatorsPanels[index].Controls.Add(logicalOperatorBox, 0, 0);
            _attributeNamesPanels[index].Controls.Add(attributeNamesBox, 0, 1);
            _attributeNamesPanels[index].Controls.Add(new Label() { Text = "Exact Match" }, 1, 1);
        }

        public void RemoveLayoutPanelData(int rowIndex, bool isEdit = false)
        {
            _rowIndex = rowIndex;
            if (Controls.Count > 5)
            {
                int deleteIndex = Controls.Count - 2;
                Controls.RemoveAt(deleteIndex);
                Controls.RemoveAt(deleteIndex);
            }
            _attributeNamesPanels.RemoveAt(rowIndex);
            _logicalOperatorsPanels.RemoveAt(rowIndex);
            _fetchXmlAttributesNames.RemoveAt(rowIndex);
            _primaryKeyNames.RemoveAt(rowIndex);

            if (isEdit)
            {
                SelectedAttributeNames[rowIndex].Clear();
                SelectedLogicalOperators[rowIndex].Clear();
            }
            else
            {
                SelectedAttributeNames.RemoveAt(rowIndex);
                SelectedLogicalOperators.RemoveAt(rowIndex);
            }
        }

        public void SetLayoutPanelData(int rowIndex)
        {
            _rowIndex = rowIndex;

            foreach (var item in _attributeNamesPanels)
            {
                if (Controls.Contains(item) && item != _attributeNamesPanels[rowIndex])
                {
                    Controls.Remove(item);
                    Controls.Add(_attributeNamesPanels[rowIndex]);
                }
            }
            foreach (var item in _logicalOperatorsPanels)
            {
                if (Controls.Contains(item) && item != _logicalOperatorsPanels[rowIndex])
                {
                    Controls.Remove(item);
                    Controls.Add(_logicalOperatorsPanels[rowIndex]);
                }
            }
        }

        private void AddRowToAttributeNamesPanel(string selectionName = null)
        {
            if (selectionName == null || (_attributeNamesPanels[_rowIndex].Controls[2] as ComboBox).SelectedItem != null)
            {
                ComboBox attributeNamesBox = new ComboBox();
                attributeNamesBox.DropDownStyle = ComboBoxStyle.DropDownList;

                foreach (var item in _fetchXmlAttributesNames[_rowIndex])
                {
                    attributeNamesBox.Items.Add(item);
                }
                _attributeNamesPanels[_rowIndex].RowCount++;
                _attributeNamesPanels[_rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                _attributeNamesPanels[_rowIndex].Controls.Add(attributeNamesBox, 0, _attributeNamesPanels[_rowIndex].RowCount - 1);
                _attributeNamesPanels[_rowIndex].Controls.Add(new Label() { Text = "Exact Match" }, 1, _attributeNamesPanels[_rowIndex].RowCount - 1);
                BtnRemoveLast.Enabled = true;

                if (selectionName != null)
                {
                    attributeNamesBox.SelectedItem = selectionName;
                }
            }
            else
            {
                ComboBox attributeNamesBox = _attributeNamesPanels[_rowIndex].Controls[2] as ComboBox;
                attributeNamesBox.SelectedItem = selectionName;
            }
        }

        private void AddRowToLogicalOperatorPanel(string selectionName = null)
        {
            if (selectionName == null || (_logicalOperatorsPanels[_rowIndex].Controls[0] as ComboBox).SelectedItem != null)
            {
                ComboBox logicalOperatorBox = new ComboBox
                {
                    Size = new System.Drawing.Size(50, 50),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                logicalOperatorBox.Items.Add("And");
                logicalOperatorBox.Items.Add("OR");

                if (selectionName != null)
                {
                    logicalOperatorBox.SelectedItem = selectionName;
                }
                _logicalOperatorsPanels[_rowIndex].RowCount++;
                _logicalOperatorsPanels[_rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                _logicalOperatorsPanels[_rowIndex].Controls.Add(logicalOperatorBox, 0, _logicalOperatorsPanels[_rowIndex].RowCount - 1);
            }
            else
            {
                ComboBox logicalOperatorBox = _logicalOperatorsPanels[_rowIndex].Controls[0] as ComboBox;
                logicalOperatorBox.SelectedItem = selectionName;
            }
        }

        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            ComboBox lastAttributeNamesBox = (ComboBox)_attributeNamesPanels[_rowIndex].Controls[_attributeNamesPanels[_rowIndex].Controls.Count - 2];
            ComboBox lastLogicalOperatorsBox = (ComboBox)_logicalOperatorsPanels[_rowIndex].Controls[_logicalOperatorsPanels[_rowIndex].Controls.Count - 1];

            if (lastAttributeNamesBox.SelectedItem != null && lastLogicalOperatorsBox.SelectedItem != null)
            {
                AddRowToAttributeNamesPanel();

                if (_attributeNamesPanels[_rowIndex].RowCount > 3)
                {
                    AddRowToLogicalOperatorPanel();
                }
            }
            else
            {
                MessageBox.Show("Please fill all fields before adding.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveLayoutPanelRow(TableLayoutPanel layoutPanel)
        {
            layoutPanel.Controls.RemoveAt(layoutPanel.Controls.Count - 1);
            layoutPanel.RowStyles.RemoveAt(layoutPanel.RowStyles.Count - 1);
            layoutPanel.RowCount--;
        }

        private void BtnRemoveLast_Click(object sender, System.EventArgs e)
        {
            if (_logicalOperatorsPanels[_rowIndex].Controls.Count > 1)
            {
                RemoveLayoutPanelRow(_logicalOperatorsPanels[_rowIndex]);
            }

            if (_attributeNamesPanels[_rowIndex].RowCount > 2)
            {
                _attributeNamesPanels[_rowIndex].Controls.RemoveAt(_attributeNamesPanels[_rowIndex].Controls.Count - 1);
                RemoveLayoutPanelRow(_attributeNamesPanels[_rowIndex]);
            }

            if (_attributeNamesPanels[_rowIndex].RowCount == 2)
            {
                BtnRemoveLast.Enabled = false;
            }
        }

        private void BtnClearSelection_Click(object sender, System.EventArgs e)
        {
            //count is needed because each time the RowCount changes.
            int count = _attributeNamesPanels[_rowIndex].RowCount - 2;

            for (int i = 0; i < count; i++)
            {
                BtnRemoveLast_Click(sender, e);
            }
            ComboBox attributeNamesBox = (ComboBox)_attributeNamesPanels[_rowIndex].Controls[2];
            ComboBox logicalOperatorBox = (ComboBox)_logicalOperatorsPanels[_rowIndex].Controls[0];

            if (attributeNamesBox.Enabled)
            {
                attributeNamesBox.Items.Add(" ");
                attributeNamesBox.SelectedItem = " ";
                attributeNamesBox.Items.Remove(" ");
                logicalOperatorBox.Items.Add(" ");
                logicalOperatorBox.SelectedItem = " ";
                logicalOperatorBox.Items.Remove(" ");
            }
        }

        private void BtnApply_Click(object sender, System.EventArgs e)
        {
            bool checkBeforeApplying = false;
            List<string> attrNames = new List<string>();
            List<string> logicalOperators = new List<string>();

            //Checking when you didn't clear selection before applying.
            if (!(SelectedAttributeNames[_rowIndex].Count > 0 && _attributeNamesPanels[_rowIndex].Controls.Count == 4 &&
                string.IsNullOrEmpty((_attributeNamesPanels[_rowIndex].Controls[2] as ComboBox).Text) && string.IsNullOrEmpty((_logicalOperatorsPanels[_rowIndex].Controls[0] as ComboBox).Text)))
            {
                foreach (var item in _attributeNamesPanels[_rowIndex].Controls)
                {
                    if (item is ComboBox attributeNamesBox)
                    {
                        if (string.IsNullOrEmpty(attributeNamesBox.Text))
                        {
                            checkBeforeApplying = true;
                            break;
                        }
                        attrNames.Add(attributeNamesBox.Text);
                    }
                }
                if (!checkBeforeApplying && attrNames.Count != 1)
                {
                    foreach (var item in _logicalOperatorsPanels[_rowIndex].Controls)
                    {
                        if (item is ComboBox logicalOperatorBox)
                        {
                            if (string.IsNullOrEmpty(logicalOperatorBox.Text))
                            {
                                checkBeforeApplying = true;
                                break;
                            }
                            logicalOperators.Add(logicalOperatorBox.Text);
                        }
                    }
                }
            }
            if (checkBeforeApplying)
            {
                MessageBox.Show("Please select before applying.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SelectedAttributeNames[_rowIndex] = attrNames;
                SelectedLogicalOperators[_rowIndex] = logicalOperators;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void MatchingCriteria_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            BtnClearSelection_Click(sender, e);
            bool isPrimaryKey = false;

            if (!_logicalOperatorsPanels[_rowIndex].Controls[0].Enabled)
            {
                isPrimaryKey = true;
            }
            foreach (var item in SelectedAttributeNames[_rowIndex])
            {
                if (item != _primaryKeyNames[_rowIndex])
                {
                    AddRowToAttributeNamesPanel(item);
                }
            }
            foreach (var item in SelectedLogicalOperators[_rowIndex])
            {
                if (!isPrimaryKey)
                {
                    AddRowToLogicalOperatorPanel(item);
                }
                isPrimaryKey = false;
            }
        }
    }
}