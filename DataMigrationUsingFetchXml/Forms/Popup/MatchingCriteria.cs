using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Services;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    public partial class MatchingCriteria : Form
    {
        private readonly List<Dictionary<string, string>> finalResults = new List<Dictionary<string, string>>();
        private readonly List<List<string>> fetchXmlAttributesNames = new List<List<string>>();
        private readonly List<TableLayoutPanel> dropDownPanels = new List<TableLayoutPanel>();
        private readonly List<TableLayoutPanel> tableLayoutPanels = new List<TableLayoutPanel>();
        private readonly List<int> tablesRowCounts = new List<int>();
        private string primaryKeyName;
        private string currentItem;
        private int rowIndex;

        public MatchingCriteria()
        {
            InitializeComponent();
        }

        private void CreateTableLayoutPanel(int index)
        {
            foreach (var item in tableLayoutPanels)
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
                Name = "layoutPanel",
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
                tableLayoutPanels.Insert(index, layoutPanel);
            }
            else
            {
                tableLayoutPanels.Add(layoutPanel);
            }
        }

        private void CreateDropDownTable(int index)
        {
            foreach (var item in dropDownPanels)
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
                Name = "dropDown",
                Size = new System.Drawing.Size(25, 25),
                RowCount = 1,
                ColumnCount = 1
            };
            dropDownPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            dropDownPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            Controls.Add(dropDownPanel);
            if (index != -1)
            {
                dropDownPanels.Insert(index, dropDownPanel);
            }
            else
            {
                dropDownPanels.Add(dropDownPanel);
            }
        }

        public void AddMatchingCriteriaDataBindingSource(string fetchXml, int rowIndex)
        {
            int index = rowIndex;
            primaryKeyName = ConfigReader.GetFetchXmlPrimaryKey(fetchXml);

            if (rowIndex != -1)
            {
                RemoveMatchingCriteriaDataBindingSource(rowIndex);
                fetchXmlAttributesNames.Insert(rowIndex, ConfigReader.GetAttributesNames(fetchXml));
                tablesRowCounts.Insert(rowIndex, 1);
            }
            else
            {
                finalResults.Add(new Dictionary<string, string>());
                fetchXmlAttributesNames.Add(ConfigReader.GetAttributesNames(fetchXml));
                tablesRowCounts.Add(1);
                index = tablesRowCounts.Count - 1;
            }
            CreateTableLayoutPanel(rowIndex);
            CreateDropDownTable(rowIndex);

            if (primaryKeyName != null)
            {
                ComboBox primaryKeyBox = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Enabled = false
                };
                primaryKeyBox.Items.Add(primaryKeyName);
                primaryKeyBox.SelectedItem = primaryKeyName;
                tableLayoutPanels[index].Controls.Add(primaryKeyBox, 0, 1);
                tablesRowCounts[index]++;

                ComboBox dropDownBox = new ComboBox
                {
                    Size = new System.Drawing.Size(50, 50),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Enabled = false
                };
                dropDownBox.Items.Add("And");
                dropDownBox.Items.Add("OR");
                dropDownBox.SelectedItem = "OR";
                dropDownPanels[index].Controls.Add(dropDownBox, 0, 0);
            }
            else
            {
                ComboBox attributeNamesBox = new ComboBox();
                foreach (var item in fetchXmlAttributesNames[index])
                {
                    attributeNamesBox.Items.Add(item);
                }
                attributeNamesBox.SelectedValueChanged += ComboBox_SelectedValueChanged;
                attributeNamesBox.DropDown += ComboBox_DropDown;
                attributeNamesBox.DropDownStyle = ComboBoxStyle.DropDownList;
                tableLayoutPanels[index].Controls.Add(attributeNamesBox, 0, 1);
                tablesRowCounts[index]++;

                ComboBox dropDownBox = new ComboBox
                {
                    Size = new System.Drawing.Size(50, 50),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                dropDownBox.Items.Add("And");
                dropDownBox.Items.Add("OR");
                dropDownPanels[index].Controls.Add(dropDownBox, 0, 0);
            }
        }

        public void RemoveMatchingCriteriaDataBindingSource(int rowIndex)
        {
            this.rowIndex = rowIndex;
            Controls.RemoveAt(4);
            Controls.RemoveAt(4);

            tableLayoutPanels.RemoveAt(rowIndex);
            dropDownPanels.RemoveAt(rowIndex);
            fetchXmlAttributesNames.RemoveAt(rowIndex);
            tablesRowCounts.RemoveAt(rowIndex);
        }

        public void SetDataGridViewData(int rowIndex)
        {
            this.rowIndex = rowIndex;
            if (fetchXmlAttributesNames[rowIndex].Count > 1)
            {
                BtnAdd.Enabled = true;
            }
            else
            {
                BtnAdd.Enabled = false;
            }

            foreach (var item in tableLayoutPanels)
            {
                if (Controls.Contains(item) && item != tableLayoutPanels[rowIndex])
                {
                    Controls.Remove(item);
                    Controls.Add(tableLayoutPanels[rowIndex]);
                }
            }
            foreach (var item in dropDownPanels)
            {
                if (Controls.Contains(item) && item != dropDownPanels[rowIndex])
                {
                    Controls.Remove(item);
                    Controls.Add(dropDownPanels[rowIndex]);
                }
            }
        }

        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            ComboBox lastComboBox = (ComboBox)tableLayoutPanels[rowIndex].Controls[tablesRowCounts[rowIndex]];
            ComboBox lastDropDown = (ComboBox)dropDownPanels[rowIndex].Controls[dropDownPanels[rowIndex].Controls.Count - 1];
            if (lastComboBox.SelectedItem != null && lastDropDown.SelectedItem != null)
            {
                ComboBox comboBox = new ComboBox();
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                fetchXmlAttributesNames[rowIndex].Remove(lastComboBox.SelectedItem.ToString());
                foreach (var item in fetchXmlAttributesNames[rowIndex])
                {
                    comboBox.Items.Add(item);
                }
                tableLayoutPanels[rowIndex].RowCount++;
                tableLayoutPanels[rowIndex].Height += 25;
                tableLayoutPanels[rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                tableLayoutPanels[rowIndex].Controls.Add(comboBox, 0, tablesRowCounts[rowIndex]);
                BtnRemoveLast.Enabled = true;
                comboBox.SelectedValueChanged += ComboBox_SelectedValueChanged;
                comboBox.DropDown += ComboBox_DropDown;

                if (tablesRowCounts[rowIndex] != 2)
                {
                    ComboBox dropDownBox = new ComboBox
                    {
                        Size = new System.Drawing.Size(50, 50),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    dropDownBox.Items.Add("And");
                    dropDownBox.Items.Add("OR");

                    dropDownPanels[rowIndex].RowCount++;
                    dropDownPanels[rowIndex].Height += 25;
                    dropDownPanels[rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                    dropDownPanels[rowIndex].Controls.Add(dropDownBox, 0, dropDownPanels[rowIndex].Controls.Count);
                }
                if (fetchXmlAttributesNames[rowIndex].Count <= 1)
                {
                    BtnAdd.Enabled = false;
                }
                tablesRowCounts[rowIndex]++;
            }
            else
            {
                MessageBox.Show("Please select before adding", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveLast_Click(object sender, System.EventArgs e)
        {
            if (dropDownPanels[rowIndex].Controls.Count > 1)
            {
                dropDownPanels[rowIndex].Controls.RemoveAt(dropDownPanels[rowIndex].Controls.Count - 1);
                dropDownPanels[rowIndex].RowStyles.RemoveAt(dropDownPanels[rowIndex].Controls.Count - 1);
                dropDownPanels[rowIndex].RowCount--;
                dropDownPanels[rowIndex].Height -= 26;
            }
            if (tableLayoutPanels[rowIndex].Controls.Count > 3)
            {
                BtnAdd.Enabled = true;
                var deletedItem = (tableLayoutPanels[rowIndex].Controls[tableLayoutPanels[rowIndex].Controls.Count - 1] as ComboBox).SelectedItem;
                tableLayoutPanels[rowIndex].Controls.RemoveAt(tableLayoutPanels[rowIndex].Controls.Count - 1);
                tableLayoutPanels[rowIndex].RowStyles.RemoveAt(tableLayoutPanels[rowIndex].Controls.Count - 1);
                tableLayoutPanels[rowIndex].RowCount--;
                tableLayoutPanels[rowIndex].Height -= 26;
                tablesRowCounts[rowIndex]--;

                if (deletedItem != null)
                {
                    fetchXmlAttributesNames[rowIndex].Add(deletedItem.ToString());
                    int index = 0;
                    foreach (var item in tableLayoutPanels[rowIndex].Controls)
                    {
                        if (index >= 2)
                        {
                            ComboBox fieldBox = (ComboBox)item;
                            fieldBox.Items.Add(deletedItem.ToString());
                        }
                        index++;
                    }
                }
            }
            if (tableLayoutPanels[rowIndex].Controls.Count == 3)
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
            int selectedIndex = tableLayoutPanels[rowIndex].Controls.IndexOf(comboBox);
            int index = 0;
            foreach (var item in tableLayoutPanels[rowIndex].Controls)
            {
                if (index >= 2 && index != selectedIndex)
                {
                    ComboBox fieldBox = (ComboBox)item;

                    if (fieldBox.Items.Contains(selectedItem))
                    {
                        fieldBox.Items.Remove(selectedItem);
                    }
                    if (currentItem != null)
                    {
                        fieldBox.Items.Add(currentItem);
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

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            int count = tableLayoutPanels[rowIndex].Controls.Count;

            for (int i = 3; i < count; i++)
            {
                BtnRemoveLast_Click(sender, e);
            }
            ComboBox fieldComboBox = (ComboBox)tableLayoutPanels[rowIndex].Controls[2];
            ComboBox dropDownComboBox = (ComboBox)dropDownPanels[rowIndex].Controls[0];
            if (fieldComboBox.Enabled)
            {
                fieldComboBox.Items.Add(" ");
                fieldComboBox.SelectedItem = " ";
                fieldComboBox.Items.Remove(" ");
                dropDownComboBox.Items.Add(" ");
                dropDownComboBox.SelectedItem = " ";
                dropDownComboBox.Items.Remove(" ");
            }
            Close();
        }

        private void BtnApply_Click(object sender, System.EventArgs e)
        {
            int tableIndex = 0;
            int dropDownIndex = 0;
            foreach (var item in tableLayoutPanels[rowIndex].Controls)
            {
                if (tableIndex >= 2)
                {
                    ComboBox dropDownBox = null;
                    if (dropDownIndex < dropDownPanels[rowIndex].Controls.Count)
                    {
                        dropDownBox = (ComboBox)dropDownPanels[rowIndex].Controls[dropDownIndex++];
                    }
                    ComboBox fieldBox = (ComboBox)item;
                    if (dropDownBox == null)
                    {
                        finalResults[rowIndex].Add("", fieldBox.Text);
                    }
                    else
                    {
                        finalResults[rowIndex].Add(fieldBox.Text, dropDownBox.Text);
                    }
                }
                tableIndex++;
            }
            Close();
        }
    }
}