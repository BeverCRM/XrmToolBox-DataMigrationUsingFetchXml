using DataMigrationUsingFetchXml.Model;

namespace DataMigrationUsingFetchXml.Forms.Popup
{
    partial class MatchingCriteria
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
            this.BtnAdd = new System.Windows.Forms.Button();
            this.BtnRemoveLast = new System.Windows.Forms.Button();
            this.BtnClearSelection = new System.Windows.Forms.Button();
            this.BtnApply = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnAdd
            // 
            this.BtnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAdd.Location = new System.Drawing.Point(375, 5);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(97, 23);
            this.BtnAdd.TabIndex = 2;
            this.BtnAdd.Text = "Add New Row";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // BtnRemoveLast
            // 
            this.BtnRemoveLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveLast.Location = new System.Drawing.Point(375, 34);
            this.BtnRemoveLast.Name = "BtnRemoveLast";
            this.BtnRemoveLast.Size = new System.Drawing.Size(97, 23);
            this.BtnRemoveLast.TabIndex = 3;
            this.BtnRemoveLast.Text = "Remove Last";
            this.BtnRemoveLast.UseVisualStyleBackColor = true;
            this.BtnRemoveLast.Click += new System.EventHandler(this.BtnRemoveLast_Click);
            // 
            // BtnClearSelection
            // 
            this.BtnClearSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClearSelection.Location = new System.Drawing.Point(375, 63);
            this.BtnClearSelection.Name = "BtnClearSelection";
            this.BtnClearSelection.Size = new System.Drawing.Size(97, 23);
            this.BtnClearSelection.TabIndex = 4;
            this.BtnClearSelection.Text = "Clear Selection";
            this.BtnClearSelection.UseVisualStyleBackColor = true;
            this.BtnClearSelection.Click += new System.EventHandler(this.BtnClearSelection_Click);
            // 
            // BtnApply
            // 
            this.BtnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnApply.Location = new System.Drawing.Point(316, 326);
            this.BtnApply.Name = "BtnApply";
            this.BtnApply.Size = new System.Drawing.Size(75, 23);
            this.BtnApply.TabIndex = 4;
            this.BtnApply.Text = "Apply";
            this.BtnApply.UseVisualStyleBackColor = true;
            this.BtnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Location = new System.Drawing.Point(397, 326);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // MatchingCriteria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnApply);
            this.Controls.Add(this.BtnClearSelection);
            this.Controls.Add(this.BtnRemoveLast);
            this.Controls.Add(this.BtnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "MatchingCriteria";
            this.Text = "Matching Criteria";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MatchingCriteria_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnAdd;
        private System.Windows.Forms.Button BtnRemoveLast;
        private System.Windows.Forms.Button BtnClearSelection;
        private System.Windows.Forms.Button BtnApply;
        private System.Windows.Forms.Button BtnCancel;
    }
}