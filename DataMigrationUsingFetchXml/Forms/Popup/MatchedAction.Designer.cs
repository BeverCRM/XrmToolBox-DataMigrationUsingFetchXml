
namespace DataMigrationUsingFetchXml.Forms.Popup
{
    partial class MatchedAction
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radioButtonDeleteAndCreate = new System.Windows.Forms.RadioButton();
            this.radioButtonDoNotCreate = new System.Windows.Forms.RadioButton();
            this.radioButtonUpsert = new System.Windows.Forms.RadioButton();
            this.radioButtonCreate = new System.Windows.Forms.RadioButton();
            this.BtnApply = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.MatchedActionPanel = new System.Windows.Forms.Panel();
            this.MatchedActionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonDeleteAndCreate
            // 
            this.radioButtonDeleteAndCreate.Location = new System.Drawing.Point(11, 39);
            this.radioButtonDeleteAndCreate.Name = "radioButtonDeleteAndCreate";
            this.radioButtonDeleteAndCreate.Size = new System.Drawing.Size(290, 40);
            this.radioButtonDeleteAndCreate.TabIndex = 2;
            this.radioButtonDeleteAndCreate.Text = "Delete matched target record and create source record. (Will create in case of no" +
    "t matching)";
            this.radioButtonDeleteAndCreate.UseVisualStyleBackColor = true;
            // 
            // radioButtonDoNotCreate
            // 
            this.radioButtonDoNotCreate.Location = new System.Drawing.Point(11, 116);
            this.radioButtonDoNotCreate.Name = "radioButtonDoNotCreate";
            this.radioButtonDoNotCreate.Size = new System.Drawing.Size(430, 40);
            this.radioButtonDoNotCreate.TabIndex = 4;
            this.radioButtonDoNotCreate.Text = "Don\'t create a source record.";
            this.radioButtonDoNotCreate.UseVisualStyleBackColor = true;
            // 
            // radioButtonUpsert
            // 
            this.radioButtonUpsert.Location = new System.Drawing.Point(11, 77);
            this.radioButtonUpsert.Name = "radioButtonUpsert";
            this.radioButtonUpsert.Size = new System.Drawing.Size(290, 40);
            this.radioButtonUpsert.TabIndex = 3;
            this.radioButtonUpsert.Text = "Update matched target record with source record data. (Will create in case of not" +
    " matching)";
            this.radioButtonUpsert.UseVisualStyleBackColor = true;
            // 
            // radioButtonCreate
            // 
            this.radioButtonCreate.Checked = true;
            this.radioButtonCreate.Location = new System.Drawing.Point(11, 3);
            this.radioButtonCreate.Name = "radioButtonCreate";
            this.radioButtonCreate.Size = new System.Drawing.Size(324, 38);
            this.radioButtonCreate.TabIndex = 1;
            this.radioButtonCreate.TabStop = true;
            this.radioButtonCreate.Text = "Don\'t delete matched target record and create a source record. (Will error in cas" +
    "e of primary key matching)";
            this.radioButtonCreate.UseVisualStyleBackColor = true;
            // 
            // BtnApply
            // 
            this.BtnApply.Location = new System.Drawing.Point(286, 166);
            this.BtnApply.Name = "BtnApply";
            this.BtnApply.Size = new System.Drawing.Size(75, 23);
            this.BtnApply.TabIndex = 1;
            this.BtnApply.Text = "Apply";
            this.BtnApply.UseVisualStyleBackColor = true;
            this.BtnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(367, 166);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 1;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // MatchedActionPanel
            // 
            this.MatchedActionPanel.Controls.Add(this.radioButtonDeleteAndCreate);
            this.MatchedActionPanel.Controls.Add(this.radioButtonUpsert);
            this.MatchedActionPanel.Controls.Add(this.radioButtonDoNotCreate);
            this.MatchedActionPanel.Controls.Add(this.radioButtonCreate);
            this.MatchedActionPanel.Location = new System.Drawing.Point(1, 1);
            this.MatchedActionPanel.Name = "MatchedActionPanel";
            this.MatchedActionPanel.Size = new System.Drawing.Size(451, 159);
            this.MatchedActionPanel.TabIndex = 5;
            // 
            // MatchedAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(454, 193);
            this.Controls.Add(this.MatchedActionPanel);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "MatchedAction";
            this.Text = "Matching Action";
            this.MatchedActionPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonDeleteAndCreate;
        private System.Windows.Forms.RadioButton radioButtonDoNotCreate;
        private System.Windows.Forms.RadioButton radioButtonUpsert;
        private System.Windows.Forms.RadioButton radioButtonCreate;
        private System.Windows.Forms.Button BtnApply;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Panel MatchedActionPanel;
    }
}