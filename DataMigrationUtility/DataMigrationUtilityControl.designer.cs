
namespace XrmMigrationUtility
{
    partial class DataMigrationUtilityControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.TxtLogsPath = new System.Windows.Forms.TextBox();
            this.TxtFetchPath = new System.Windows.Forms.TextBox();
            this.BtnBrowseLogs = new System.Windows.Forms.Button();
            this.BtnBrowseFetch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtLogs = new System.Windows.Forms.TextBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.BtnSelectTargetInstance = new System.Windows.Forms.Button();
            this.ListBoxOrganizations = new System.Windows.Forms.ListBox();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.BtnTransferData = new System.Windows.Forms.Button();
            this.TextBoxFetchFiles = new System.Windows.Forms.TextBox();
            this.LblTarget = new System.Windows.Forms.Label();
            this.LblTargetText = new System.Windows.Forms.Label();
            this.LblSourceText = new System.Windows.Forms.Label();
            this.LblSource = new System.Windows.Forms.Label();
            this.LblIsTargetFilled = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TxtLogsPath
            // 
            this.TxtLogsPath.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TxtLogsPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtLogsPath.Location = new System.Drawing.Point(23, 129);
            this.TxtLogsPath.Multiline = true;
            this.TxtLogsPath.Name = "TxtLogsPath";
            this.TxtLogsPath.Size = new System.Drawing.Size(471, 25);
            this.TxtLogsPath.TabIndex = 5;
            this.TxtLogsPath.Leave += new System.EventHandler(this.TxtLogsPathLeave);
            // 
            // TxtFetchPath
            // 
            this.TxtFetchPath.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TxtFetchPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtFetchPath.Location = new System.Drawing.Point(23, 204);
            this.TxtFetchPath.Multiline = true;
            this.TxtFetchPath.Name = "TxtFetchPath";
            this.TxtFetchPath.Size = new System.Drawing.Size(474, 25);
            this.TxtFetchPath.TabIndex = 5;
            this.TxtFetchPath.Leave += new System.EventHandler(this.TxtFetchPathLeave);
            // 
            // BtnBrowseLogs
            // 
            this.BtnBrowseLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBrowseLogs.Location = new System.Drawing.Point(521, 129);
            this.BtnBrowseLogs.Name = "BtnBrowseLogs";
            this.BtnBrowseLogs.Size = new System.Drawing.Size(100, 25);
            this.BtnBrowseLogs.TabIndex = 6;
            this.BtnBrowseLogs.Text = "Browse";
            this.BtnBrowseLogs.UseVisualStyleBackColor = true;
            this.BtnBrowseLogs.Click += new System.EventHandler(this.BtnBrowseLogs_Click);
            // 
            // BtnBrowseFetch
            // 
            this.BtnBrowseFetch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBrowseFetch.Location = new System.Drawing.Point(521, 204);
            this.BtnBrowseFetch.Name = "BtnBrowseFetch";
            this.BtnBrowseFetch.Size = new System.Drawing.Size(100, 25);
            this.BtnBrowseFetch.TabIndex = 6;
            this.BtnBrowseFetch.Text = "Browse";
            this.BtnBrowseFetch.UseVisualStyleBackColor = true;
            this.BtnBrowseFetch.Click += new System.EventHandler(this.BtnBrowseFetch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Logs Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fetch Path";
            // 
            // TxtLogs
            // 
            this.TxtLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLogs.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TxtLogs.Location = new System.Drawing.Point(23, 298);
            this.TxtLogs.Multiline = true;
            this.TxtLogs.Name = "TxtLogs";
            this.TxtLogs.ReadOnly = true;
            this.TxtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtLogs.Size = new System.Drawing.Size(1318, 394);
            this.TxtLogs.TabIndex = 8;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar1.Location = new System.Drawing.Point(1364, 1);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 720);
            this.vScrollBar1.TabIndex = 9;
            // 
            // BtnSelectTargetInstance
            // 
            this.BtnSelectTargetInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelectTargetInstance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSelectTargetInstance.Location = new System.Drawing.Point(1101, 93);
            this.BtnSelectTargetInstance.Name = "BtnSelectTargetInstance";
            this.BtnSelectTargetInstance.Size = new System.Drawing.Size(150, 30);
            this.BtnSelectTargetInstance.TabIndex = 10;
            this.BtnSelectTargetInstance.Text = "Select Target Instance\r\n";
            this.BtnSelectTargetInstance.UseVisualStyleBackColor = true;
            this.BtnSelectTargetInstance.Click += new System.EventHandler(this.BtnSelectTargetInstance_Click);
            // 
            // ListBoxOrganizations
            // 
            this.ListBoxOrganizations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxOrganizations.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ListBoxOrganizations.FormattingEnabled = true;
            this.ListBoxOrganizations.Location = new System.Drawing.Point(733, 129);
            this.ListBoxOrganizations.Name = "ListBoxOrganizations";
            this.ListBoxOrganizations.Size = new System.Drawing.Size(608, 17);
            this.ListBoxOrganizations.TabIndex = 15;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hScrollBar1.Location = new System.Drawing.Point(0, 723);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(1361, 10);
            this.hScrollBar1.TabIndex = 16;
            // 
            // BtnTransferData
            // 
            this.BtnTransferData.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BtnTransferData.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BtnTransferData.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnTransferData.Location = new System.Drawing.Point(622, 20);
            this.BtnTransferData.Name = "BtnTransferData";
            this.BtnTransferData.Size = new System.Drawing.Size(120, 48);
            this.BtnTransferData.TabIndex = 18;
            this.BtnTransferData.Text = "Transfer Data";
            this.BtnTransferData.UseVisualStyleBackColor = false;
            this.BtnTransferData.Click += new System.EventHandler(this.BtnTransferData_Click);
            // 
            // TextBoxFetchFiles
            // 
            this.TextBoxFetchFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxFetchFiles.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TextBoxFetchFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxFetchFiles.Location = new System.Drawing.Point(733, 204);
            this.TextBoxFetchFiles.Multiline = true;
            this.TextBoxFetchFiles.Name = "TextBoxFetchFiles";
            this.TextBoxFetchFiles.ReadOnly = true;
            this.TextBoxFetchFiles.Size = new System.Drawing.Size(608, 25);
            this.TextBoxFetchFiles.TabIndex = 19;
            // 
            // LblTarget
            // 
            this.LblTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblTarget.Location = new System.Drawing.Point(1118, 9);
            this.LblTarget.Name = "LblTarget";
            this.LblTarget.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.LblTarget.Size = new System.Drawing.Size(83, 27);
            this.LblTarget.TabIndex = 20;
            this.LblTarget.Text = "Target   - ";
            this.LblTarget.Visible = false;
            // 
            // LblTargetText
            // 
            this.LblTargetText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblTargetText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblTargetText.Location = new System.Drawing.Point(1207, 9);
            this.LblTargetText.Name = "LblTargetText";
            this.LblTargetText.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.LblTargetText.Size = new System.Drawing.Size(134, 27);
            this.LblTargetText.TabIndex = 21;
            this.LblTargetText.Text = "Target Text";
            this.LblTargetText.Visible = false;
            // 
            // LblSourceText
            // 
            this.LblSourceText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblSourceText.Location = new System.Drawing.Point(116, 9);
            this.LblSourceText.Name = "LblSourceText";
            this.LblSourceText.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.LblSourceText.Size = new System.Drawing.Size(134, 27);
            this.LblSourceText.TabIndex = 23;
            this.LblSourceText.Text = "Source Text";
            this.LblSourceText.Visible = false;
            // 
            // LblSource
            // 
            this.LblSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblSource.Location = new System.Drawing.Point(19, 9);
            this.LblSource.Name = "LblSource";
            this.LblSource.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.LblSource.Size = new System.Drawing.Size(91, 27);
            this.LblSource.TabIndex = 22;
            this.LblSource.Text = "Source   - ";
            this.LblSource.Visible = false;
            // 
            // LblIsTargetFilled
            // 
            this.LblIsTargetFilled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblIsTargetFilled.AutoSize = true;
            this.LblIsTargetFilled.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblIsTargetFilled.ForeColor = System.Drawing.Color.Red;
            this.LblIsTargetFilled.Location = new System.Drawing.Point(1257, 100);
            this.LblIsTargetFilled.Name = "LblIsTargetFilled";
            this.LblIsTargetFilled.Size = new System.Drawing.Size(93, 15);
            this.LblIsTargetFilled.TabIndex = 24;
            this.LblIsTargetFilled.Text = "Not selected yet";
            // 
            // DataMigrationUtilityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.LblIsTargetFilled);
            this.Controls.Add(this.LblSourceText);
            this.Controls.Add(this.LblSource);
            this.Controls.Add(this.LblTargetText);
            this.Controls.Add(this.LblTarget);
            this.Controls.Add(this.TextBoxFetchFiles);
            this.Controls.Add(this.BtnTransferData);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.ListBoxOrganizations);
            this.Controls.Add(this.BtnSelectTargetInstance);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.TxtLogs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnBrowseFetch);
            this.Controls.Add(this.BtnBrowseLogs);
            this.Controls.Add(this.TxtFetchPath);
            this.Controls.Add(this.TxtLogsPath);
            this.Name = "DataMigrationUtilityControl";
            this.Size = new System.Drawing.Size(1364, 722);
            this.Load += new System.EventHandler(this.DataMigrationUtilityControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox TxtLogsPath;
        private System.Windows.Forms.TextBox TxtFetchPath;
        private System.Windows.Forms.Button BtnBrowseLogs;
        private System.Windows.Forms.Button BtnBrowseFetch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtLogs;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Button BtnSelectTargetInstance;
        private System.Windows.Forms.ListBox ListBoxOrganizations;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Button BtnTransferData;
        private System.Windows.Forms.TextBox TextBoxFetchFiles;
        private System.Windows.Forms.Label LblTarget;
        private System.Windows.Forms.Label LblTargetText;
        private System.Windows.Forms.Label LblSourceText;
        private System.Windows.Forms.Label LblSource;
        private System.Windows.Forms.Label LblIsTargetFilled;
    }
}
