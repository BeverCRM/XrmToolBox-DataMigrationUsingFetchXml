
namespace XrmMigrationUtility
{
    partial class MyPluginControl
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
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSample = new System.Windows.Forms.ToolStripButton();
            this.TxtLogsPath = new System.Windows.Forms.TextBox();
            this.TxtFetchPath = new System.Windows.Forms.TextBox();
            this.BtnBrowseLogs = new System.Windows.Forms.Button();
            this.BtnBrowseFetch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtLogs = new System.Windows.Forms.TextBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.BtnAddOrganization = new System.Windows.Forms.Button();
            this.BtnRemoveOrganization = new System.Windows.Forms.Button();
            this.ListBoxOrganizations = new System.Windows.Forms.ListBox();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.ListBoxTxtFetch = new System.Windows.Forms.ListBox();
            this.BtnTransferData = new System.Windows.Forms.Button();
            this.toolStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.tsbSample});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1364, 25);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 22);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbSample
            // 
            this.tsbSample.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSample.Name = "tsbSample";
            this.tsbSample.Size = new System.Drawing.Size(46, 22);
            this.tsbSample.Text = "Try me";
            this.tsbSample.Click += new System.EventHandler(this.tsbSample_Click);
            // 
            // TxtLogsPath
            // 
            this.TxtLogsPath.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TxtLogsPath.Location = new System.Drawing.Point(23, 51);
            this.TxtLogsPath.Name = "TxtLogsPath";
            this.TxtLogsPath.Size = new System.Drawing.Size(474, 20);
            this.TxtLogsPath.TabIndex = 5;
            this.TxtLogsPath.Leave += new System.EventHandler(this.TxtLogsPathLeave);
            // 
            // TxtFetchPath
            // 
            this.TxtFetchPath.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TxtFetchPath.Location = new System.Drawing.Point(23, 113);
            this.TxtFetchPath.Name = "TxtFetchPath";
            this.TxtFetchPath.Size = new System.Drawing.Size(474, 20);
            this.TxtFetchPath.TabIndex = 5;
            this.TxtFetchPath.Leave += new System.EventHandler(this.TxtFetchPathLeave);
            // 
            // BtnBrowseLogs
            // 
            this.BtnBrowseLogs.Location = new System.Drawing.Point(518, 48);
            this.BtnBrowseLogs.Name = "BtnBrowseLogs";
            this.BtnBrowseLogs.Size = new System.Drawing.Size(85, 25);
            this.BtnBrowseLogs.TabIndex = 6;
            this.BtnBrowseLogs.Text = "Browse";
            this.BtnBrowseLogs.UseVisualStyleBackColor = true;
            this.BtnBrowseLogs.Click += new System.EventHandler(this.BtnBrowseLogs_Click);
            // 
            // BtnBrowseFetch
            // 
            this.BtnBrowseFetch.Location = new System.Drawing.Point(518, 110);
            this.BtnBrowseFetch.Name = "BtnBrowseFetch";
            this.BtnBrowseFetch.Size = new System.Drawing.Size(85, 25);
            this.BtnBrowseFetch.TabIndex = 6;
            this.BtnBrowseFetch.Text = "Browse";
            this.BtnBrowseFetch.UseVisualStyleBackColor = true;
            this.BtnBrowseFetch.Click += new System.EventHandler(this.BtnBrowseFetch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Logs Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fetch Path";
            // 
            // TxtLogs
            // 
            this.TxtLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLogs.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TxtLogs.Location = new System.Drawing.Point(626, 168);
            this.TxtLogs.Multiline = true;
            this.TxtLogs.Name = "TxtLogs";
            this.TxtLogs.ReadOnly = true;
            this.TxtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtLogs.Size = new System.Drawing.Size(710, 524);
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
            // BtnAddOrganization
            // 
            this.BtnAddOrganization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnAddOrganization.Location = new System.Drawing.Point(1216, 46);
            this.BtnAddOrganization.Name = "BtnAddOrganization";
            this.BtnAddOrganization.Size = new System.Drawing.Size(120, 25);
            this.BtnAddOrganization.TabIndex = 10;
            this.BtnAddOrganization.Text = "Add Organization";
            this.BtnAddOrganization.UseVisualStyleBackColor = true;
            this.BtnAddOrganization.Click += new System.EventHandler(this.BtnAddOrganization_Click);
            // 
            // BtnRemoveOrganization
            // 
            this.BtnRemoveOrganization.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRemoveOrganization.Location = new System.Drawing.Point(1216, 77);
            this.BtnRemoveOrganization.Name = "BtnRemoveOrganization";
            this.BtnRemoveOrganization.Size = new System.Drawing.Size(120, 25);
            this.BtnRemoveOrganization.TabIndex = 12;
            this.BtnRemoveOrganization.Text = "Remove Organization";
            this.BtnRemoveOrganization.UseVisualStyleBackColor = true;
            this.BtnRemoveOrganization.Click += new System.EventHandler(this.BtnRemoveOrganization_Click);
            // 
            // ListBoxOrganizations
            // 
            this.ListBoxOrganizations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxOrganizations.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ListBoxOrganizations.FormattingEnabled = true;
            this.ListBoxOrganizations.Location = new System.Drawing.Point(626, 113);
            this.ListBoxOrganizations.Name = "ListBoxOrganizations";
            this.ListBoxOrganizations.Size = new System.Drawing.Size(710, 17);
            this.ListBoxOrganizations.TabIndex = 15;
            this.ListBoxOrganizations.SelectedIndexChanged += new System.EventHandler(this.ListBoxOrganizations_SelectedIndexChanged);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.hScrollBar1.Location = new System.Drawing.Point(0, 723);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(1361, 10);
            this.hScrollBar1.TabIndex = 16;
            // 
            // ListBoxTxtFetch
            // 
            this.ListBoxTxtFetch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ListBoxTxtFetch.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ListBoxTxtFetch.FormattingEnabled = true;
            this.ListBoxTxtFetch.Location = new System.Drawing.Point(23, 168);
            this.ListBoxTxtFetch.Name = "ListBoxTxtFetch";
            this.ListBoxTxtFetch.ScrollAlwaysVisible = true;
            this.ListBoxTxtFetch.Size = new System.Drawing.Size(474, 524);
            this.ListBoxTxtFetch.TabIndex = 17;
            // 
            // BtnTransferData
            // 
            this.BtnTransferData.Location = new System.Drawing.Point(518, 168);
            this.BtnTransferData.Name = "BtnTransferData";
            this.BtnTransferData.Size = new System.Drawing.Size(85, 52);
            this.BtnTransferData.TabIndex = 18;
            this.BtnTransferData.Text = "Transfer Data";
            this.BtnTransferData.UseVisualStyleBackColor = true;
            this.BtnTransferData.Click += new System.EventHandler(this.BtnTransferData_Click);
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.BtnTransferData);
            this.Controls.Add(this.ListBoxTxtFetch);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.ListBoxOrganizations);
            this.Controls.Add(this.BtnRemoveOrganization);
            this.Controls.Add(this.BtnAddOrganization);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.TxtLogs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnBrowseFetch);
            this.Controls.Add(this.BtnBrowseLogs);
            this.Controls.Add(this.TxtFetchPath);
            this.Controls.Add(this.TxtLogsPath);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1364, 722);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripButton tsbSample;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.TextBox TxtLogsPath;
        private System.Windows.Forms.TextBox TxtFetchPath;
        private System.Windows.Forms.Button BtnBrowseLogs;
        private System.Windows.Forms.Button BtnBrowseFetch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtLogs;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Button BtnAddOrganization;
        private System.Windows.Forms.Button BtnRemoveOrganization;
        private System.Windows.Forms.ListBox ListBoxOrganizations;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.ListBox ListBoxTxtFetch;
        private System.Windows.Forms.Button BtnTransferData;
    }
}
