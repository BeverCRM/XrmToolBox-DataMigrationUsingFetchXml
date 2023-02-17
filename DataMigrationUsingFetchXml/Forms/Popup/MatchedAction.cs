using System;
using System.Windows.Forms;
using System.Collections.Generic;
using DataMigrationUsingFetchXml.Model;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    internal partial class MatchedAction : Form
    {
        public static List<byte> CheckedRadioButtonNumbers { get; set; }
        public static List<string> SelectedActionShortDescription { get; private set; }
        public static Dictionary<int, string> SelectedActionDescription { get; private set; }
        public int RowIndex { get; set; }

        public MatchedAction()
        {
            InitializeComponent();
            CheckedRadioButtonNumbers = new List<byte>();
            SelectedActionDescription = new Dictionary<int, string>();
            SelectedActionShortDescription = new List<string> { "Create", "Delete & Create", "Upsert", "Don't Create" };

            foreach (RadioButton item in MatchedActionPanel.Controls)
            {
                SelectedActionDescription.Add(item.TabIndex, item.Text);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (radioButtonCreate.Checked)
            {
                CheckedRadioButtonNumbers[RowIndex] = (byte)MatchedActionForRecord.Create;
            }
            else if (radioButtonDeleteAndCreate.Checked)
            {
                CheckedRadioButtonNumbers[RowIndex] = (byte)MatchedActionForRecord.DeleteAndCreate;
            }
            else if (radioButtonUpsert.Checked)
            {
                CheckedRadioButtonNumbers[RowIndex] = (byte)MatchedActionForRecord.Upsert;
            }
            else
            {
                CheckedRadioButtonNumbers[RowIndex] = (byte)MatchedActionForRecord.DoNotCreate;
            }
            DialogResult = DialogResult.OK;
        }

        public void CheckRadioButton(int index)
        {
            foreach (RadioButton item in MatchedActionPanel.Controls)
            {
                if (item.TabIndex == CheckedRadioButtonNumbers[index])
                {
                    item.Checked = true;
                    break;
                }
            }
        }
    }
}