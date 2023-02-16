
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
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.BtnApply = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.MatchedActionPanel = new System.Windows.Forms.Panel();
            this.MatchedActionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButton2
            // 
            this.radioButton2.Location = new System.Drawing.Point(11, 39);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(430, 40);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.Text = "Delete matched target record and create source record\r\n(Will create in case of no" +
    "t matching)";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.Location = new System.Drawing.Point(11, 116);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(430, 40);
            this.radioButton4.TabIndex = 4;
            this.radioButton4.Text = "Don\'t create a source record";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.Location = new System.Drawing.Point(11, 77);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(430, 40);
            this.radioButton3.TabIndex = 3;
            this.radioButton3.Text = "Update matched target record with source record data\r\n(Will create in case of not" +
    " matching)";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(11, 3);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(430, 38);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Don\'t delete matched target record and create a source record\r\n(Will error in cas" +
    "e of primary key matching)";
            this.radioButton1.UseVisualStyleBackColor = true;
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
            this.MatchedActionPanel.Controls.Add(this.radioButton2);
            this.MatchedActionPanel.Controls.Add(this.radioButton3);
            this.MatchedActionPanel.Controls.Add(this.radioButton4);
            this.MatchedActionPanel.Controls.Add(this.radioButton1);
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

        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button BtnApply;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Panel MatchedActionPanel;
    }
}