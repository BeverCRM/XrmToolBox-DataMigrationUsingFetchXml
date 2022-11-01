﻿
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataMigrationUtilityControl));
            this.TxtLogsPath = new System.Windows.Forms.TextBox();
            this.BtnBrowseLogs = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.BtnSelectTargetInstance = new System.Windows.Forms.Button();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.BtnTransferData = new System.Windows.Forms.Button();
            this.LblTarget = new System.Windows.Forms.Label();
            this.LblTargetText = new System.Windows.Forms.Label();
            this.LblSourceText = new System.Windows.Forms.Label();
            this.LblSource = new System.Windows.Forms.Label();
            this.BtnClearLogs = new System.Windows.Forms.Button();
            this.LogsLabel = new System.Windows.Forms.Label();
            this.LblAddFetchXml = new System.Windows.Forms.Label();
            this.FetchDataGridView = new System.Windows.Forms.DataGridView();
            this.CheckBox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.displayNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.schemaNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewImageColumn();
            this.Remove = new System.Windows.Forms.DataGridViewImageColumn();
            this.fetchXmlDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.richTextBoxLogs = new System.Windows.Forms.RichTextBox();
            this.LblInfo = new System.Windows.Forms.Label();
            this.LblTitle = new System.Windows.Forms.Label();
            this.LblLoading = new System.Windows.Forms.Label();
            this.pictureBoxAdd = new System.Windows.Forms.PictureBox();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.BtnStopMigration = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.FetchDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fetchXmlDataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtLogsPath
            // 
            this.TxtLogsPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLogsPath.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.TxtLogsPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtLogsPath.Location = new System.Drawing.Point(887, 129);
            this.TxtLogsPath.MaxLength = 500;
            this.TxtLogsPath.Multiline = true;
            this.TxtLogsPath.Name = "TxtLogsPath";
            this.TxtLogsPath.Size = new System.Drawing.Size(474, 25);
            this.TxtLogsPath.TabIndex = 5;
            this.TxtLogsPath.Leave += new System.EventHandler(this.TxtLogsPathLeave);
            // 
            // BtnBrowseLogs
            // 
            this.BtnBrowseLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBrowseLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnBrowseLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBrowseLogs.Location = new System.Drawing.Point(786, 129);
            this.BtnBrowseLogs.Name = "BtnBrowseLogs";
            this.BtnBrowseLogs.Size = new System.Drawing.Size(95, 25);
            this.BtnBrowseLogs.TabIndex = 6;
            this.BtnBrowseLogs.Text = "Browse";
            this.BtnBrowseLogs.UseVisualStyleBackColor = true;
            this.BtnBrowseLogs.Click += new System.EventHandler(this.BtnBrowseLogs_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(884, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Logs Path";
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
            this.BtnSelectTargetInstance.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelectTargetInstance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSelectTargetInstance.Location = new System.Drawing.Point(959, 9);
            this.BtnSelectTargetInstance.Name = "BtnSelectTargetInstance";
            this.BtnSelectTargetInstance.Size = new System.Drawing.Size(150, 30);
            this.BtnSelectTargetInstance.TabIndex = 10;
            this.BtnSelectTargetInstance.Text = "Select Target Instance\r\n";
            this.BtnSelectTargetInstance.UseVisualStyleBackColor = true;
            this.BtnSelectTargetInstance.Click += new System.EventHandler(this.BtnSelectTargetInstance_Click);
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
            this.BtnTransferData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnTransferData.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnTransferData.Location = new System.Drawing.Point(622, 20);
            this.BtnTransferData.Name = "BtnTransferData";
            this.BtnTransferData.Size = new System.Drawing.Size(120, 48);
            this.BtnTransferData.TabIndex = 18;
            this.BtnTransferData.Text = "Transfer Data";
            this.BtnTransferData.UseVisualStyleBackColor = false;
            this.BtnTransferData.Click += new System.EventHandler(this.BtnTransferData_Click);
            // 
            // LblTarget
            // 
            this.LblTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LblTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblTarget.Location = new System.Drawing.Point(1115, 9);
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
            this.LblTargetText.Location = new System.Drawing.Point(1204, 9);
            this.LblTargetText.Name = "LblTargetText";
            this.LblTargetText.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.LblTargetText.Size = new System.Drawing.Size(137, 94);
            this.LblTargetText.TabIndex = 21;
            this.LblTargetText.Text = "Target Text";
            this.LblTargetText.Visible = false;
            // 
            // LblSourceText
            // 
            this.LblSourceText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblSourceText.Location = new System.Drawing.Point(122, 9);
            this.LblSourceText.Name = "LblSourceText";
            this.LblSourceText.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.LblSourceText.Size = new System.Drawing.Size(375, 94);
            this.LblSourceText.TabIndex = 23;
            this.LblSourceText.Text = "Source Text";
            this.LblSourceText.Visible = false;
            // 
            // LblSource
            // 
            this.LblSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblSource.Location = new System.Drawing.Point(23, 9);
            this.LblSource.Name = "LblSource";
            this.LblSource.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.LblSource.Size = new System.Drawing.Size(93, 27);
            this.LblSource.TabIndex = 22;
            this.LblSource.Text = "Source   - ";
            this.LblSource.Visible = false;
            // 
            // BtnClearLogs
            // 
            this.BtnClearLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClearLogs.Location = new System.Drawing.Point(70, 401);
            this.BtnClearLogs.Name = "BtnClearLogs";
            this.BtnClearLogs.Size = new System.Drawing.Size(70, 25);
            this.BtnClearLogs.TabIndex = 25;
            this.BtnClearLogs.Text = "Clear Logs";
            this.BtnClearLogs.UseVisualStyleBackColor = true;
            this.BtnClearLogs.Click += new System.EventHandler(this.BtnClearLogs_Click);
            // 
            // LogsLabel
            // 
            this.LogsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogsLabel.Location = new System.Drawing.Point(24, 405);
            this.LogsLabel.Name = "LogsLabel";
            this.LogsLabel.Size = new System.Drawing.Size(43, 21);
            this.LogsLabel.TabIndex = 26;
            this.LogsLabel.Text = "Logs";
            // 
            // LblAddFetchXml
            // 
            this.LblAddFetchXml.AutoSize = true;
            this.LblAddFetchXml.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAddFetchXml.Location = new System.Drawing.Point(60, 103);
            this.LblAddFetchXml.Name = "LblAddFetchXml";
            this.LblAddFetchXml.Size = new System.Drawing.Size(95, 16);
            this.LblAddFetchXml.TabIndex = 29;
            this.LblAddFetchXml.Text = "Add FetchXML";
            // 
            // FetchDataGridView
            // 
            this.FetchDataGridView.AllowUserToAddRows = false;
            this.FetchDataGridView.AutoGenerateColumns = false;
            this.FetchDataGridView.BackgroundColor = System.Drawing.SystemColors.InactiveBorder;
            this.FetchDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FetchDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckBox,
            this.displayNameDataGridViewTextBoxColumn,
            this.schemaNameDataGridViewTextBoxColumn,
            this.Edit,
            this.Remove});
            this.FetchDataGridView.DataSource = this.fetchXmlDataBindingSource;
            this.FetchDataGridView.Location = new System.Drawing.Point(23, 129);
            this.FetchDataGridView.Name = "FetchDataGridView";
            this.FetchDataGridView.Size = new System.Drawing.Size(563, 178);
            this.FetchDataGridView.TabIndex = 30;
            this.FetchDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.FetchDataGridView_CellContentClick);
            // 
            // CheckBox
            // 
            this.CheckBox.Frozen = true;
            this.CheckBox.HeaderText = "";
            this.CheckBox.Name = "CheckBox";
            this.CheckBox.Width = 30;
            // 
            // displayNameDataGridViewTextBoxColumn
            // 
            this.displayNameDataGridViewTextBoxColumn.DataPropertyName = "DisplayName";
            this.displayNameDataGridViewTextBoxColumn.HeaderText = "DisplayName";
            this.displayNameDataGridViewTextBoxColumn.Name = "displayNameDataGridViewTextBoxColumn";
            this.displayNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.displayNameDataGridViewTextBoxColumn.Width = 220;
            // 
            // schemaNameDataGridViewTextBoxColumn
            // 
            this.schemaNameDataGridViewTextBoxColumn.DataPropertyName = "SchemaName";
            this.schemaNameDataGridViewTextBoxColumn.HeaderText = "SchemaName";
            this.schemaNameDataGridViewTextBoxColumn.Name = "schemaNameDataGridViewTextBoxColumn";
            this.schemaNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.schemaNameDataGridViewTextBoxColumn.Width = 220;
            // 
            // Edit
            // 
            this.Edit.HeaderText = "";
            this.Edit.Image = global::XrmMigrationUtility.Properties.Resources.Edit;
            this.Edit.Name = "Edit";
            this.Edit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Edit.Width = 25;
            // 
            // Remove
            // 
            this.Remove.HeaderText = "";
            this.Remove.Image = global::XrmMigrationUtility.Properties.Resources.Remove;
            this.Remove.Name = "Remove";
            this.Remove.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Remove.Width = 25;
            // 
            // fetchXmlDataBindingSource
            // 
            this.fetchXmlDataBindingSource.DataSource = typeof(XrmMigrationUtility.Model.FetchXmlData);
            // 
            // richTextBoxLogs
            // 
            this.richTextBoxLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxLogs.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.richTextBoxLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxLogs.Location = new System.Drawing.Point(23, 432);
            this.richTextBoxLogs.Name = "richTextBoxLogs";
            this.richTextBoxLogs.ReadOnly = true;
            this.richTextBoxLogs.Size = new System.Drawing.Size(1318, 276);
            this.richTextBoxLogs.TabIndex = 31;
            this.richTextBoxLogs.Text = "";
            // 
            // LblInfo
            // 
            this.LblInfo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.LblInfo.BackColor = System.Drawing.SystemColors.Window;
            this.LblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblInfo.Location = new System.Drawing.Point(622, 376);
            this.LblInfo.Name = "LblInfo";
            this.LblInfo.Size = new System.Drawing.Size(170, 50);
            this.LblInfo.TabIndex = 34;
            this.LblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblInfo.Visible = false;
            // 
            // LblTitle
            // 
            this.LblTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.LblTitle.BackColor = System.Drawing.SystemColors.Window;
            this.LblTitle.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblTitle.Location = new System.Drawing.Point(575, 343);
            this.LblTitle.Name = "LblTitle";
            this.LblTitle.Size = new System.Drawing.Size(217, 33);
            this.LblTitle.TabIndex = 35;
            this.LblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblTitle.Visible = false;
            // 
            // LblLoading
            // 
            this.LblLoading.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.LblLoading.BackColor = System.Drawing.SystemColors.Window;
            this.LblLoading.Image = global::XrmMigrationUtility.Properties.Resources.loading;
            this.LblLoading.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblLoading.Location = new System.Drawing.Point(572, 376);
            this.LblLoading.Name = "LblLoading";
            this.LblLoading.Size = new System.Drawing.Size(50, 50);
            this.LblLoading.TabIndex = 33;
            this.LblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblLoading.Visible = false;
            // 
            // pictureBoxAdd
            // 
            this.pictureBoxAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxAdd.Image = global::XrmMigrationUtility.Properties.Resources.iconPlus30;
            this.pictureBoxAdd.Location = new System.Drawing.Point(23, 97);
            this.pictureBoxAdd.Name = "pictureBoxAdd";
            this.pictureBoxAdd.Size = new System.Drawing.Size(35, 35);
            this.pictureBoxAdd.TabIndex = 28;
            this.pictureBoxAdd.TabStop = false;
            this.pictureBoxAdd.Click += new System.EventHandler(this.PictureBoxAdd_Click);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.Image = ((System.Drawing.Image)(resources.GetObject("dataGridViewImageColumn1.Image")));
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn1.Width = 25;
            // 
            // dataGridViewImageColumn2
            // 
            this.dataGridViewImageColumn2.HeaderText = "";
            this.dataGridViewImageColumn2.Image = ((System.Drawing.Image)(resources.GetObject("dataGridViewImageColumn2.Image")));
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            this.dataGridViewImageColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn2.Width = 25;
            // 
            // BtnStopMigration
            // 
            this.BtnStopMigration.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BtnStopMigration.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnStopMigration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnStopMigration.Location = new System.Drawing.Point(798, 376);
            this.BtnStopMigration.Name = "BtnStopMigration";
            this.BtnStopMigration.Size = new System.Drawing.Size(103, 50);
            this.BtnStopMigration.TabIndex = 36;
            this.BtnStopMigration.Text = "Stop Migration";
            this.BtnStopMigration.UseVisualStyleBackColor = true;
            this.BtnStopMigration.Visible = false;
            this.BtnStopMigration.Click += new System.EventHandler(this.BtnStopMigration_Click);
            // 
            // DataMigrationUtilityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.BtnStopMigration);
            this.Controls.Add(this.LblTitle);
            this.Controls.Add(this.LblInfo);
            this.Controls.Add(this.LblLoading);
            this.Controls.Add(this.richTextBoxLogs);
            this.Controls.Add(this.FetchDataGridView);
            this.Controls.Add(this.LblAddFetchXml);
            this.Controls.Add(this.pictureBoxAdd);
            this.Controls.Add(this.LogsLabel);
            this.Controls.Add(this.BtnClearLogs);
            this.Controls.Add(this.LblSourceText);
            this.Controls.Add(this.LblSource);
            this.Controls.Add(this.LblTargetText);
            this.Controls.Add(this.LblTarget);
            this.Controls.Add(this.BtnTransferData);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.BtnSelectTargetInstance);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnBrowseLogs);
            this.Controls.Add(this.TxtLogsPath);
            this.Name = "DataMigrationUtilityControl";
            this.Size = new System.Drawing.Size(1364, 722);
            this.Load += new System.EventHandler(this.DataMigrationUtilityControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FetchDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fetchXmlDataBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox TxtLogsPath;
        private System.Windows.Forms.Button BtnBrowseLogs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Button BtnSelectTargetInstance;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Button BtnTransferData;
        private System.Windows.Forms.Label LblTarget;
        private System.Windows.Forms.Label LblTargetText;
        private System.Windows.Forms.Label LblSourceText;
        private System.Windows.Forms.Label LblSource;
        private System.Windows.Forms.Button BtnClearLogs;
        private System.Windows.Forms.Label LogsLabel;
        private System.Windows.Forms.PictureBox pictureBoxAdd;
        private System.Windows.Forms.Label LblAddFetchXml;
        private System.Windows.Forms.DataGridView FetchDataGridView;
        private System.Windows.Forms.BindingSource fetchXmlDataBindingSource;
        private System.Windows.Forms.RichTextBox richTextBoxLogs;
        private System.Windows.Forms.Label LblLoading;
        private System.Windows.Forms.Label LblInfo;
        private System.Windows.Forms.Label LblTitle;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn displayNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn schemaNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn Edit;
        private System.Windows.Forms.DataGridViewImageColumn Remove;
        private System.Windows.Forms.Button BtnStopMigration;
    }
}