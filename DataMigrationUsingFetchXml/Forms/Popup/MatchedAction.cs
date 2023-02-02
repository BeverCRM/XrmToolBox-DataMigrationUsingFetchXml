﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;

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
            if (radioButton1.Checked)
            {
                CheckedRadioButtonNumbers[RowIndex] = 1;
            }
            else if (radioButton2.Checked)
            {
                CheckedRadioButtonNumbers[RowIndex] = 2;
            }
            else if (radioButton3.Checked)
            {
                CheckedRadioButtonNumbers[RowIndex] = 3;
            }
            else
            {
                CheckedRadioButtonNumbers[RowIndex] = 4;
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