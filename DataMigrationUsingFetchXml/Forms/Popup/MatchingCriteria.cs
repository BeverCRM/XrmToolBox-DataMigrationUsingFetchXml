using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Services;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    public partial class MatchingCriteria : Form
    {
        public static readonly List<List<string>> finalAttributeNamesResult = new List<List<string>>();
        public static readonly List<List<string>> finalLogicalOperatorsResult = new List<List<string>>();
        private readonly List<List<string>> fetchXmlAttributesNames = new List<List<string>>();
        private readonly List<TableLayoutPanel> logicalOperatorsPanels = new List<TableLayoutPanel>();
        private readonly List<TableLayoutPanel> attributeNamesPanels = new List<TableLayoutPanel>();
        private readonly List<string> primaryKeyNames = new List<string>();
        private string currentItem;
        private int rowIndex;

        public MatchingCriteria()
        {
            InitializeComponent();
        }

        private void CreateAttributeNamesPanel(int index)
        {
            foreach (var item in attributeNamesPanels)
            {
                if (Controls.Contains(item))
                {
                    Controls.Remove(item);
                }
            }

            TableLayoutPanel layoutPanel = new TableLayoutPanel
            {
                AutoSize = true,
                Location = new System.Drawing.Point(58, 2),
                Name = "attributeNamesPanel",
                Size = new System.Drawing.Size(215, 50),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                RowCount = 2,
                ColumnCount = 2
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            layoutPanel.Controls.Add(new Label() { Text = "Field" }, 0, 0);
            layoutPanel.Controls.Add(new Label() { Text = "Criteria" }, 1, 0);

            Controls.Add(layoutPanel);
            if (index != -1)
            {
                attributeNamesPanels.Insert(index, layoutPanel);
            }
            else
            {
                attributeNamesPanels.Add(layoutPanel);
            }
        }

        private void CreateDropDownTable(int index)
        {
            foreach (var item in logicalOperatorsPanels)
            {
                if (Controls.Contains(item))
                {
                    Controls.Remove(item);
                }
            }

            TableLayoutPanel dropDownPanel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                AutoSize = true,
                Location = new System.Drawing.Point(1, 43),
                Name = "logicalOperatorPanel",
                Size = new System.Drawing.Size(25, 25),
                RowCount = 1,
                ColumnCount = 1
            };
            dropDownPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            dropDownPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            Controls.Add(dropDownPanel);
            if (index != -1)
            {
                logicalOperatorsPanels.Insert(index, dropDownPanel);
            }
            else
            {
                logicalOperatorsPanels.Add(dropDownPanel);
            }
        }

        public void CreateLayoutPanels(string fetchXml, int rowIndex)
        {
            int index = rowIndex;

            if (rowIndex != -1)
            {
                primaryKeyNames.Insert(rowIndex, ConfigReader.GetFetchXmlPrimaryKey(fetchXml));
                finalAttributeNamesResult[rowIndex].Clear();
                finalLogicalOperatorsResult[rowIndex].Clear();
                RemoveLayoutPanelData(rowIndex, true);
                fetchXmlAttributesNames.Insert(rowIndex, ConfigReader.GetAttributesNames(fetchXml));
            }
            else
            {
                primaryKeyNames.Add(ConfigReader.GetFetchXmlPrimaryKey(fetchXml));
                finalAttributeNamesResult.Add(new List<string>());
                finalLogicalOperatorsResult.Add(new List<string>());
                fetchXmlAttributesNames.Add(ConfigReader.GetAttributesNames(fetchXml));
                index = fetchXmlAttributesNames.Count - 1;
            }
            CreateAttributeNamesPanel(rowIndex);
            CreateDropDownTable(rowIndex);
            ComboBox attributeNamesBox = new ComboBox();
            ComboBox dropDownBox = new ComboBox();
            attributeNamesBox.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownBox.Size = new System.Drawing.Size(50, 50);
            dropDownBox.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownBox.Items.Add("And");
            dropDownBox.Items.Add("OR");

            if (primaryKeyNames[index] != null)
            {
                attributeNamesBox.Enabled = false;
                attributeNamesBox.Items.Add(primaryKeyNames[index]);
                attributeNamesBox.SelectedItem = primaryKeyNames[index];
                fetchXmlAttributesNames[index].Remove(primaryKeyNames[index]);

                dropDownBox.Enabled = false;
                dropDownBox.SelectedItem = "OR";
            }
            else
            {
                foreach (var item in fetchXmlAttributesNames[index])
                {
                    attributeNamesBox.Items.Add(item);
                }
                attributeNamesBox.SelectedValueChanged += ComboBox_SelectedValueChanged;
                attributeNamesBox.DropDown += ComboBox_DropDown;
            }
            logicalOperatorsPanels[index].Controls.Add(dropDownBox, 0, 0);
            attributeNamesPanels[index].Controls.Add(attributeNamesBox, 0, 1);
            attributeNamesPanels[index].Controls.Add(new Label() { Text = "Exact Match" }, 1, 1);
        }

        public void RemoveLayoutPanelData(int rowIndex, bool isEdit = false)
        {
            this.rowIndex = rowIndex;
            int deleteIndex = Controls.Count - 2;
            Controls.RemoveAt(deleteIndex);
            Controls.RemoveAt(deleteIndex);

            attributeNamesPanels.RemoveAt(rowIndex);
            logicalOperatorsPanels.RemoveAt(rowIndex);
            fetchXmlAttributesNames.RemoveAt(rowIndex);
            if (isEdit)
            {
                finalAttributeNamesResult[rowIndex].Clear();
                finalLogicalOperatorsResult[rowIndex].Clear();
            }
            else
            {
                finalAttributeNamesResult.RemoveAt(rowIndex);
                finalLogicalOperatorsResult.RemoveAt(rowIndex);
            }
        }

        public void SetLayoutPanelData(int rowIndex)
        {
            this.rowIndex = rowIndex;
            if (fetchXmlAttributesNames[rowIndex].Count >= 1)
            {
                BtnAdd.Enabled = true;
            }
            else
            {
                BtnAdd.Enabled = false;
            }

            foreach (var item in attributeNamesPanels)
            {
                if (Controls.Contains(item) && item != attributeNamesPanels[rowIndex])
                {
                    Controls.Remove(item);
                    Controls.Add(attributeNamesPanels[rowIndex]);
                }
            }
            foreach (var item in logicalOperatorsPanels)
            {
                if (Controls.Contains(item) && item != logicalOperatorsPanels[rowIndex])
                {
                    Controls.Remove(item);
                    Controls.Add(logicalOperatorsPanels[rowIndex]);
                }
            }
        }

        private void AddRowToAttributeNamesPanel(string selectionName = null)
        {
            if (selectionName == null || (attributeNamesPanels[rowIndex].Controls[2] as ComboBox).SelectedItem != null)
            {
                ComboBox comboBox = new ComboBox();
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                foreach (var item in fetchXmlAttributesNames[rowIndex])
                {
                    comboBox.Items.Add(item);
                }
                attributeNamesPanels[rowIndex].RowCount++;
                attributeNamesPanels[rowIndex].Height += 25;
                attributeNamesPanels[rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                attributeNamesPanels[rowIndex].Controls.Add(comboBox, 0, attributeNamesPanels[rowIndex].RowCount - 1);
                attributeNamesPanels[rowIndex].Controls.Add(new Label() { Text = "Exact Match" }, 1, attributeNamesPanels[rowIndex].RowCount - 1);
                BtnRemoveLast.Enabled = true;
                comboBox.SelectedValueChanged += ComboBox_SelectedValueChanged;
                comboBox.DropDown += ComboBox_DropDown;

                if (selectionName != null)
                {
                    comboBox.SelectedItem = selectionName;
                }
            }
            else
            {
                ComboBox comboBox = attributeNamesPanels[rowIndex].Controls[2] as ComboBox;
                comboBox.SelectedItem = selectionName;
            }
        }

        private void AddRowToLogicalOperatorPanel(string selectionName = null)
        {
            if (selectionName == null || (logicalOperatorsPanels[rowIndex].Controls[0] as ComboBox).SelectedItem != null)
            {
                ComboBox dropDownBox = new ComboBox
                {
                    Size = new System.Drawing.Size(50, 50),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                dropDownBox.Items.Add("And");
                dropDownBox.Items.Add("OR");
                if (selectionName != null)
                {
                    dropDownBox.SelectedItem = selectionName;
                }

                logicalOperatorsPanels[rowIndex].RowCount++;
                logicalOperatorsPanels[rowIndex].Height += 25;
                logicalOperatorsPanels[rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                logicalOperatorsPanels[rowIndex].Controls.Add(dropDownBox, 0, logicalOperatorsPanels[rowIndex].RowCount - 1);
            }
            else
            {
                ComboBox comboBox = logicalOperatorsPanels[rowIndex].Controls[0] as ComboBox;
                comboBox.SelectedItem = selectionName;
            }
        }

        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            ComboBox lastAttributeNames = (ComboBox)attributeNamesPanels[rowIndex].Controls[attributeNamesPanels[rowIndex].Controls.Count - 2];
            ComboBox lastLogicalOperator = (ComboBox)logicalOperatorsPanels[rowIndex].Controls[logicalOperatorsPanels[rowIndex].Controls.Count - 1];
            if (lastAttributeNames.SelectedItem != null && lastLogicalOperator.SelectedItem != null)
            {
                AddRowToAttributeNamesPanel();

                if (attributeNamesPanels[rowIndex].RowCount > 3)
                {
                    AddRowToLogicalOperatorPanel();
                }
                if (fetchXmlAttributesNames[rowIndex].Count <= 1)
                {
                    BtnAdd.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please select before adding.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveLayoutPanelRow(TableLayoutPanel layoutPanel)
        {
            layoutPanel.Controls.RemoveAt(layoutPanel.Controls.Count - 1);
            layoutPanel.RowStyles.RemoveAt(layoutPanel.RowStyles.Count - 1);
            layoutPanel.RowCount--;
            layoutPanel.Height -= 26;
        }

        private void BtnRemoveLast_Click(object sender, System.EventArgs e)
        {
            if (logicalOperatorsPanels[rowIndex].Controls.Count > 1)
            {
                RemoveLayoutPanelRow(logicalOperatorsPanels[rowIndex]);
            }
            if (attributeNamesPanels[rowIndex].RowCount > 2)
            {
                BtnAdd.Enabled = true;
                if (attributeNamesPanels[rowIndex].Controls[attributeNamesPanels[rowIndex].Controls.Count - 2] is ComboBox deletedItem)
                {
                    if (deletedItem.SelectedItem != null)
                    {
                        fetchXmlAttributesNames[rowIndex].Add(deletedItem.SelectedItem.ToString());
                        foreach (var item in attributeNamesPanels[rowIndex].Controls)
                        {
                            if (item is ComboBox fieldBox)
                            {
                                if (!fieldBox.Items.Contains(deletedItem.SelectedItem))
                                {
                                    fieldBox.Items.Add(deletedItem.SelectedItem.ToString());
                                }
                            }
                        }
                    }
                }
                if (attributeNamesPanels[rowIndex].RowCount > 2)
                {
                    attributeNamesPanels[rowIndex].Controls.RemoveAt(attributeNamesPanels[rowIndex].Controls.Count - 1);
                    RemoveLayoutPanelRow(attributeNamesPanels[rowIndex]);
                }
            }
            if (attributeNamesPanels[rowIndex].RowCount == 2)
            {
                BtnRemoveLast.Enabled = false;
            }
        }

        private void ComboBox_DropDown(object sender, System.EventArgs e)
        {
            currentItem = null;
            if (((ComboBox)sender).SelectedItem != null)
            {
                currentItem = ((ComboBox)sender).SelectedItem.ToString();
            }
        }

        private void ComboBox_SelectedValueChanged(object sender, System.EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedItem = comboBox.SelectedItem.ToString();
            int selectedIndex = attributeNamesPanels[rowIndex].Controls.IndexOf(comboBox);
            int index = 0;

            foreach (var item in attributeNamesPanels[rowIndex].Controls)
            {
                if (item is ComboBox fieldBox)
                {
                    if (index != selectedIndex)
                    {
                        if (fieldBox.Items.Contains(selectedItem))
                        {
                            fieldBox.Items.Remove(selectedItem);
                        }
                        if (currentItem != null)
                        {
                            fieldBox.Items.Add(currentItem);
                        }
                    }
                }
                index++;
            }
            fetchXmlAttributesNames[rowIndex].Remove(selectedItem);
            if (currentItem != null)
            {
                fetchXmlAttributesNames[rowIndex].Add(currentItem);
                currentItem = null;
            }
        }

        private void BtnClearSelection_Click(object sender, System.EventArgs e)
        {
            int count = attributeNamesPanels[rowIndex].RowCount - 2;
            finalAttributeNamesResult[rowIndex].Clear();
            finalLogicalOperatorsResult[rowIndex].Clear();
            Close();
            Visible = false;

            for (int i = 0; i < count; i++)
            {
                BtnRemoveLast_Click(sender, e);
            }
            ComboBox fieldComboBox = (ComboBox)attributeNamesPanels[rowIndex].Controls[2];
            ComboBox dropDownComboBox = (ComboBox)logicalOperatorsPanels[rowIndex].Controls[0];
            if (fieldComboBox.Enabled)
            {
                foreach (var item in fieldComboBox.Items)
                {
                    if (!fetchXmlAttributesNames[rowIndex].Contains(item.ToString()))
                    {
                        fetchXmlAttributesNames[rowIndex].Add(item.ToString());
                    }
                }
                fieldComboBox.Items.Add(" ");
                fieldComboBox.SelectedItem = " ";
                fieldComboBox.Items.Remove(" ");
                dropDownComboBox.Items.Add(" ");
                dropDownComboBox.SelectedItem = " ";
                dropDownComboBox.Items.Remove(" ");
            }
        }

        private void BtnApply_Click(object sender, System.EventArgs e)
        {
            bool checkBeforeApplying = false;
            List<string> attrNames = new List<string>();
            List<string> logicalOperators = new List<string>();
            foreach (var item in attributeNamesPanels[rowIndex].Controls)
            {
                if (item is ComboBox fieldBox)
                {
                    attrNames.Add(fieldBox.Text);
                    if (string.IsNullOrEmpty(fieldBox.Text))
                    {
                        checkBeforeApplying = true;
                    }
                }
            }
            if (!checkBeforeApplying)
            {
                foreach (var item in logicalOperatorsPanels[rowIndex].Controls)
                {
                    if (item is ComboBox logicalOperatorBox)
                    {
                        logicalOperators.Add(logicalOperatorBox.Text);
                        if (string.IsNullOrEmpty(logicalOperatorBox.Text))
                        {
                            checkBeforeApplying = true;
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
                finalAttributeNamesResult[rowIndex] = attrNames;
                finalLogicalOperatorsResult[rowIndex] = logicalOperators;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
            Visible = false;
            BtnClearSelection_Click(sender, e);
            bool isPrimaryKey = false;
            if (!logicalOperatorsPanels[rowIndex].Controls[0].Enabled)
            {
                isPrimaryKey = true;
            }
            foreach (var item in finalAttributeNamesResult[rowIndex])
            {
                if (item != primaryKeyNames[rowIndex])
                {
                    AddRowToAttributeNamesPanel(item);
                    fetchXmlAttributesNames[rowIndex].Remove(item);
                }
            }
            foreach (var item in finalLogicalOperatorsResult[rowIndex])
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