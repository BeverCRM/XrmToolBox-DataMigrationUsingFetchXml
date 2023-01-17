using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Services;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    public partial class MatchingCriteria : Form
    {
        public static List<List<string>> FinalAttributeNamesResult { get; }
        public static List<List<string>> FinalLogicalOperatorsResult { get; }

        private readonly List<List<string>> fetchXmlAttributesNames = new List<List<string>>();
        private readonly List<TableLayoutPanel> logicalOperatorsPanels = new List<TableLayoutPanel>();
        private readonly List<TableLayoutPanel> attributeNamesPanels = new List<TableLayoutPanel>();
        private readonly List<string> primaryKeyNames = new List<string>();
        private int rowIndex;

        public MatchingCriteria()
        {
            InitializeComponent();
        }

        static MatchingCriteria()
        {
            FinalAttributeNamesResult = new List<List<string>>();
            FinalLogicalOperatorsResult = new List<List<string>>();
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
                attributeNamesPanels.Insert(index, attributeNamesPanel);
            }
            else
            {
                attributeNamesPanels.Add(attributeNamesPanel);
            }
        }

        private void CreateLogicalOperatorsPanel(int index)
        {
            foreach (var item in logicalOperatorsPanels)
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
                logicalOperatorsPanels.Insert(index, logicalOperatorPanel);
            }
            else
            {
                logicalOperatorsPanels.Add(logicalOperatorPanel);
            }
        }

        public void CreateLayoutPanels(string fetchXml, int rowIndex)
        {
            int index = rowIndex;

            if (rowIndex != -1)
            {
                FinalAttributeNamesResult[rowIndex].Clear();
                FinalLogicalOperatorsResult[rowIndex].Clear();
                RemoveLayoutPanelData(rowIndex, true);
                primaryKeyNames.Insert(rowIndex, ConfigReader.GetFetchXmlPrimaryKey(fetchXml));
                fetchXmlAttributesNames.Insert(rowIndex, ConfigReader.GetAttributesNames(fetchXml));
            }
            else
            {
                primaryKeyNames.Add(ConfigReader.GetFetchXmlPrimaryKey(fetchXml));
                FinalAttributeNamesResult.Add(new List<string>());
                FinalLogicalOperatorsResult.Add(new List<string>());
                fetchXmlAttributesNames.Add(ConfigReader.GetAttributesNames(fetchXml));
                index = fetchXmlAttributesNames.Count - 1;
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

            if (primaryKeyNames[index] != null)
            {
                attributeNamesBox.Enabled = false;
                attributeNamesBox.Items.Add(primaryKeyNames[index]);
                attributeNamesBox.SelectedItem = primaryKeyNames[index];
                fetchXmlAttributesNames[index].Remove(primaryKeyNames[index]);
                logicalOperatorBox.Enabled = false;
                logicalOperatorBox.SelectedItem = "OR";
            }
            else
            {
                foreach (var item in fetchXmlAttributesNames[index])
                {
                    attributeNamesBox.Items.Add(item);
                }
            }
            logicalOperatorsPanels[index].Controls.Add(logicalOperatorBox, 0, 0);
            attributeNamesPanels[index].Controls.Add(attributeNamesBox, 0, 1);
            attributeNamesPanels[index].Controls.Add(new Label() { Text = "Exact Match" }, 1, 1);
        }

        public void RemoveLayoutPanelData(int rowIndex, bool isEdit = false)
        {
            this.rowIndex = rowIndex;
            if (Controls.Count > 5)
            {
                int deleteIndex = Controls.Count - 2;
                Controls.RemoveAt(deleteIndex);
                Controls.RemoveAt(deleteIndex);
            }
            attributeNamesPanels.RemoveAt(rowIndex);
            logicalOperatorsPanels.RemoveAt(rowIndex);
            fetchXmlAttributesNames.RemoveAt(rowIndex);
            primaryKeyNames.RemoveAt(rowIndex);

            if (isEdit)
            {
                FinalAttributeNamesResult[rowIndex].Clear();
                FinalLogicalOperatorsResult[rowIndex].Clear();
            }
            else
            {
                FinalAttributeNamesResult.RemoveAt(rowIndex);
                FinalLogicalOperatorsResult.RemoveAt(rowIndex);
            }
        }

        public void SetLayoutPanelData(int rowIndex)
        {
            this.rowIndex = rowIndex;

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
                ComboBox attributeNamesBox = new ComboBox();
                attributeNamesBox.DropDownStyle = ComboBoxStyle.DropDownList;

                foreach (var item in fetchXmlAttributesNames[rowIndex])
                {
                    attributeNamesBox.Items.Add(item);
                }
                attributeNamesPanels[rowIndex].RowCount++;
                attributeNamesPanels[rowIndex].Height += 25;
                attributeNamesPanels[rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                attributeNamesPanels[rowIndex].Controls.Add(attributeNamesBox, 0, attributeNamesPanels[rowIndex].RowCount - 1);
                attributeNamesPanels[rowIndex].Controls.Add(new Label() { Text = "Exact Match" }, 1, attributeNamesPanels[rowIndex].RowCount - 1);
                BtnRemoveLast.Enabled = true;

                if (selectionName != null)
                {
                    attributeNamesBox.SelectedItem = selectionName;
                }
            }
            else
            {
                ComboBox attributeNamesBox = attributeNamesPanels[rowIndex].Controls[2] as ComboBox;
                attributeNamesBox.SelectedItem = selectionName;
            }
        }

        private void AddRowToLogicalOperatorPanel(string selectionName = null)
        {
            if (selectionName == null || (logicalOperatorsPanels[rowIndex].Controls[0] as ComboBox).SelectedItem != null)
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
                logicalOperatorsPanels[rowIndex].RowCount++;
                logicalOperatorsPanels[rowIndex].Height += 25;
                logicalOperatorsPanels[rowIndex].RowStyles.Add(new RowStyle(SizeType.Percent, 25));
                logicalOperatorsPanels[rowIndex].Controls.Add(logicalOperatorBox, 0, logicalOperatorsPanels[rowIndex].RowCount - 1);
            }
            else
            {
                ComboBox logicalOperatorBox = logicalOperatorsPanels[rowIndex].Controls[0] as ComboBox;
                logicalOperatorBox.SelectedItem = selectionName;
            }
        }

        private void BtnAdd_Click(object sender, System.EventArgs e)
        {
            ComboBox lastAttributeNamesBox = (ComboBox)attributeNamesPanels[rowIndex].Controls[attributeNamesPanels[rowIndex].Controls.Count - 2];
            ComboBox lastLogicalOperatorsBox = (ComboBox)logicalOperatorsPanels[rowIndex].Controls[logicalOperatorsPanels[rowIndex].Controls.Count - 1];

            if (lastAttributeNamesBox.SelectedItem != null && lastLogicalOperatorsBox.SelectedItem != null)
            {
                AddRowToAttributeNamesPanel();

                if (attributeNamesPanels[rowIndex].RowCount > 3)
                {
                    AddRowToLogicalOperatorPanel();
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
                        foreach (var item in attributeNamesPanels[rowIndex].Controls)
                        {
                            if (item is ComboBox attributeNamesBox)
                            {
                                if (!attributeNamesBox.Items.Contains(deletedItem.SelectedItem))
                                {
                                    attributeNamesBox.Items.Add(deletedItem.SelectedItem.ToString());
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

        private void BtnClearSelection_Click(object sender, System.EventArgs e)
        {
            int count = attributeNamesPanels[rowIndex].RowCount - 2;

            for (int i = 0; i < count; i++)
            {
                BtnRemoveLast_Click(sender, e);
            }
            ComboBox attributeNamesBox = (ComboBox)attributeNamesPanels[rowIndex].Controls[2];
            ComboBox logicalOperatorBox = (ComboBox)logicalOperatorsPanels[rowIndex].Controls[0];

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
            foreach (var item in attributeNamesPanels[rowIndex].Controls)
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
                foreach (var item in logicalOperatorsPanels[rowIndex].Controls)
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
            if (checkBeforeApplying)
            {
                MessageBox.Show("Please select before applying.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                FinalAttributeNamesResult[rowIndex] = attrNames;
                FinalLogicalOperatorsResult[rowIndex] = logicalOperators;
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

            if (!logicalOperatorsPanels[rowIndex].Controls[0].Enabled)
            {
                isPrimaryKey = true;
            }
            foreach (var item in FinalAttributeNamesResult[rowIndex])
            {
                if (item != primaryKeyNames[rowIndex])
                {
                    AddRowToAttributeNamesPanel(item);
                }
            }
            foreach (var item in FinalLogicalOperatorsResult[rowIndex])
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