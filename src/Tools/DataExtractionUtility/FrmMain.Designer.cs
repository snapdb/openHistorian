namespace DataExtractionUtility
{
    partial class FrmMain
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
            label1 = new System.Windows.Forms.Label();
            BtnGetMetadata = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            TxtGEPPort = new System.Windows.Forms.TextBox();
            TxtServerIP = new System.Windows.Forms.TextBox();
            TxtHistorianInstance = new System.Windows.Forms.TextBox();
            TxtHistorianPort = new System.Windows.Forms.TextBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            label6 = new System.Windows.Forms.Label();
            txtCsvMaxFileCount = new System.Windows.Forms.TextBox();
            label9 = new System.Windows.Forms.Label();
            txtCsvMaxColumnCount = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            txtMaxFileSize = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            txtCsvMaxRowCount = new System.Windows.Forms.TextBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            LblEstimatedSize = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            cmbResolution = new System.Windows.Forms.ComboBox();
            dateStopTime = new System.Windows.Forms.DateTimePicker();
            label10 = new System.Windows.Forms.Label();
            dateStartTime = new System.Windows.Forms.DateTimePicker();
            label8 = new System.Windows.Forms.Label();
            BtnExport = new System.Windows.Forms.Button();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            tslStatusUpdate = new System.Windows.Forms.ToolStripStatusLabel();
            ChkSignalType = new System.Windows.Forms.CheckedListBox();
            GrpPointSelection = new System.Windows.Forms.GroupBox();
            lblPointCount = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            btnCategoryUncheckSelected = new System.Windows.Forms.Button();
            btnCategoryCheckSelected = new System.Windows.Forms.Button();
            tabPage2 = new System.Windows.Forms.TabPage();
            label12 = new System.Windows.Forms.Label();
            tabPage3 = new System.Windows.Forms.TabPage();
            label13 = new System.Windows.Forms.Label();
            tabPage4 = new System.Windows.Forms.TabPage();
            label14 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            statusStrip1.SuspendLayout();
            GrpPointSelection.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(98, 30);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(52, 15);
            label1.TabIndex = 1;
            label1.Text = "Server IP";
            // 
            // BtnGetMetadata
            // 
            BtnGetMetadata.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnGetMetadata.Location = new System.Drawing.Point(164, 147);
            BtnGetMetadata.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtnGetMetadata.Name = "BtnGetMetadata";
            BtnGetMetadata.Size = new System.Drawing.Size(117, 27);
            BtnGetMetadata.TabIndex = 0;
            BtnGetMetadata.Text = "Get Metadata";
            BtnGetMetadata.UseVisualStyleBackColor = true;
            BtnGetMetadata.Click += BtnGetMetadata_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(98, 90);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(53, 15);
            label4.TabIndex = 7;
            label4.Text = "GEP Port";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(76, 60);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(80, 15);
            label2.TabIndex = 3;
            label2.Text = "Historian Port";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(14, 120);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(137, 15);
            label3.TabIndex = 5;
            label3.Text = "Historian Instance Name";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(TxtGEPPort);
            groupBox1.Controls.Add(BtnGetMetadata);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(TxtServerIP);
            groupBox1.Controls.Add(TxtHistorianInstance);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(TxtHistorianPort);
            groupBox1.Location = new System.Drawing.Point(14, 14);
            groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox1.Size = new System.Drawing.Size(295, 187);
            groupBox1.TabIndex = 11;
            groupBox1.TabStop = false;
            groupBox1.Text = "Server Settings";
            // 
            // TxtGEPPort
            // 
            TxtGEPPort.Location = new System.Drawing.Point(164, 87);
            TxtGEPPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TxtGEPPort.Name = "TxtGEPPort";
            TxtGEPPort.Size = new System.Drawing.Size(116, 23);
            TxtGEPPort.TabIndex = 8;
            TxtGEPPort.Text = "6175";
            // 
            // TxtServerIP
            // 
            TxtServerIP.Location = new System.Drawing.Point(164, 27);
            TxtServerIP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TxtServerIP.Name = "TxtServerIP";
            TxtServerIP.Size = new System.Drawing.Size(116, 23);
            TxtServerIP.TabIndex = 2;
            TxtServerIP.Text = "127.0.0.1";
            // 
            // TxtHistorianInstance
            // 
            TxtHistorianInstance.Location = new System.Drawing.Point(164, 117);
            TxtHistorianInstance.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TxtHistorianInstance.Name = "TxtHistorianInstance";
            TxtHistorianInstance.Size = new System.Drawing.Size(116, 23);
            TxtHistorianInstance.TabIndex = 6;
            TxtHistorianInstance.Text = "PPA";
            // 
            // TxtHistorianPort
            // 
            TxtHistorianPort.Location = new System.Drawing.Point(164, 57);
            TxtHistorianPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TxtHistorianPort.Name = "TxtHistorianPort";
            TxtHistorianPort.Size = new System.Drawing.Size(116, 23);
            TxtHistorianPort.TabIndex = 4;
            TxtHistorianPort.Text = "38402";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(txtCsvMaxFileCount);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(txtCsvMaxColumnCount);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(txtMaxFileSize);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(txtCsvMaxRowCount);
            groupBox2.Location = new System.Drawing.Point(14, 208);
            groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox2.Size = new System.Drawing.Size(295, 159);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "CSV Options";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(18, 120);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(140, 15);
            label6.TabIndex = 13;
            label6.Text = "Maximum Files To Export";
            // 
            // txtCsvMaxFileCount
            // 
            txtCsvMaxFileCount.Location = new System.Drawing.Point(164, 117);
            txtCsvMaxFileCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCsvMaxFileCount.Name = "txtCsvMaxFileCount";
            txtCsvMaxFileCount.Size = new System.Drawing.Size(116, 23);
            txtCsvMaxFileCount.TabIndex = 14;
            txtCsvMaxFileCount.Text = "-1";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(18, 90);
            label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(144, 15);
            label9.TabIndex = 11;
            label9.Text = "Maximum Column Count";
            // 
            // txtCsvMaxColumnCount
            // 
            txtCsvMaxColumnCount.Location = new System.Drawing.Point(164, 87);
            txtCsvMaxColumnCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCsvMaxColumnCount.Name = "txtCsvMaxColumnCount";
            txtCsvMaxColumnCount.Size = new System.Drawing.Size(116, 23);
            txtCsvMaxColumnCount.TabIndex = 12;
            txtCsvMaxColumnCount.Text = "1024";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(20, 30);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(135, 15);
            label5.TabIndex = 1;
            label5.Text = "Maximum File Size (MB)";
            // 
            // txtMaxFileSize
            // 
            txtMaxFileSize.Location = new System.Drawing.Point(164, 27);
            txtMaxFileSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtMaxFileSize.Name = "txtMaxFileSize";
            txtMaxFileSize.Size = new System.Drawing.Size(116, 23);
            txtMaxFileSize.TabIndex = 2;
            txtMaxFileSize.Text = "10000";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(18, 60);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(124, 15);
            label7.TabIndex = 3;
            label7.Text = "Maximum Row Count";
            // 
            // txtCsvMaxRowCount
            // 
            txtCsvMaxRowCount.Location = new System.Drawing.Point(164, 57);
            txtCsvMaxRowCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCsvMaxRowCount.Name = "txtCsvMaxRowCount";
            txtCsvMaxRowCount.Size = new System.Drawing.Size(116, 23);
            txtCsvMaxRowCount.TabIndex = 4;
            txtCsvMaxRowCount.Text = "500000000";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(LblEstimatedSize);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(cmbResolution);
            groupBox3.Controls.Add(dateStopTime);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(dateStartTime);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(BtnExport);
            groupBox3.Location = new System.Drawing.Point(14, 376);
            groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupBox3.Size = new System.Drawing.Size(295, 158);
            groupBox3.TabIndex = 13;
            groupBox3.TabStop = false;
            groupBox3.Text = "Export Settings";
            // 
            // LblEstimatedSize
            // 
            LblEstimatedSize.AutoSize = true;
            LblEstimatedSize.Location = new System.Drawing.Point(18, 121);
            LblEstimatedSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LblEstimatedSize.Name = "LblEstimatedSize";
            LblEstimatedSize.Size = new System.Drawing.Size(85, 15);
            LblEstimatedSize.TabIndex = 7;
            LblEstimatedSize.Text = "Estimated Size:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(20, 85);
            label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(63, 15);
            label11.TabIndex = 6;
            label11.Text = "Resolution";
            // 
            // cmbResolution
            // 
            cmbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbResolution.FormattingEnabled = true;
            cmbResolution.Location = new System.Drawing.Point(93, 82);
            cmbResolution.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cmbResolution.Name = "cmbResolution";
            cmbResolution.Size = new System.Drawing.Size(187, 23);
            cmbResolution.TabIndex = 5;
            cmbResolution.SelectedIndexChanged += cmbResolution_SelectedIndexChanged;
            // 
            // dateStopTime
            // 
            dateStopTime.CustomFormat = "MM/dd/yyyy h:mm:ss tt";
            dateStopTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dateStopTime.Location = new System.Drawing.Point(93, 52);
            dateStopTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dateStopTime.Name = "dateStopTime";
            dateStopTime.Size = new System.Drawing.Size(187, 23);
            dateStopTime.TabIndex = 4;
            dateStopTime.ValueChanged += dateStopTime_ValueChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(52, 59);
            label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(31, 15);
            label10.TabIndex = 3;
            label10.Text = "Stop";
            // 
            // dateStartTime
            // 
            dateStartTime.CustomFormat = "MM/dd/yyyy h:mm:ss tt";
            dateStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dateStartTime.Location = new System.Drawing.Point(93, 22);
            dateStartTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dateStartTime.Name = "dateStartTime";
            dateStartTime.Size = new System.Drawing.Size(187, 23);
            dateStartTime.TabIndex = 2;
            dateStartTime.ValueChanged += dateStartTime_ValueChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(52, 29);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(31, 15);
            label8.TabIndex = 1;
            label8.Text = "Start";
            // 
            // BtnExport
            // 
            BtnExport.Location = new System.Drawing.Point(194, 115);
            BtnExport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BtnExport.Name = "BtnExport";
            BtnExport.Size = new System.Drawing.Size(88, 27);
            BtnExport.TabIndex = 0;
            BtnExport.Text = "Export";
            BtnExport.UseVisualStyleBackColor = true;
            BtnExport.Click += BtnExport_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tslStatusUpdate });
            statusStrip1.Location = new System.Drawing.Point(0, 546);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(723, 22);
            statusStrip1.TabIndex = 14;
            statusStrip1.Text = "statusStrip1";
            // 
            // tslStatusUpdate
            // 
            tslStatusUpdate.Name = "tslStatusUpdate";
            tslStatusUpdate.Size = new System.Drawing.Size(42, 17);
            tslStatusUpdate.Text = "Status:";
            // 
            // ChkSignalType
            // 
            ChkSignalType.Dock = System.Windows.Forms.DockStyle.Top;
            ChkSignalType.FormattingEnabled = true;
            ChkSignalType.Location = new System.Drawing.Point(4, 3);
            ChkSignalType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ChkSignalType.Name = "ChkSignalType";
            ChkSignalType.Size = new System.Drawing.Size(370, 382);
            ChkSignalType.TabIndex = 15;
            ChkSignalType.ItemCheck += ChkSignalType_ItemCheck;
            // 
            // GrpPointSelection
            // 
            GrpPointSelection.Controls.Add(lblPointCount);
            GrpPointSelection.Controls.Add(tabControl1);
            GrpPointSelection.Location = new System.Drawing.Point(316, 14);
            GrpPointSelection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GrpPointSelection.Name = "GrpPointSelection";
            GrpPointSelection.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            GrpPointSelection.Size = new System.Drawing.Size(394, 520);
            GrpPointSelection.TabIndex = 17;
            GrpPointSelection.TabStop = false;
            GrpPointSelection.Text = "Point Selection";
            // 
            // lblPointCount
            // 
            lblPointCount.AutoSize = true;
            lblPointCount.Location = new System.Drawing.Point(7, 489);
            lblPointCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPointCount.Name = "lblPointCount";
            lblPointCount.Size = new System.Drawing.Size(74, 15);
            lblPointCount.TabIndex = 8;
            lblPointCount.Text = "Point Count:";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            tabControl1.Location = new System.Drawing.Point(4, 19);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(386, 463);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnCategoryUncheckSelected);
            tabPage1.Controls.Add(btnCategoryCheckSelected);
            tabPage1.Controls.Add(ChkSignalType);
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage1.Size = new System.Drawing.Size(378, 435);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Category";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnCategoryUncheckSelected
            // 
            btnCategoryUncheckSelected.Location = new System.Drawing.Point(146, 396);
            btnCategoryUncheckSelected.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCategoryUncheckSelected.Name = "btnCategoryUncheckSelected";
            btnCategoryUncheckSelected.Size = new System.Drawing.Size(132, 27);
            btnCategoryUncheckSelected.TabIndex = 17;
            btnCategoryUncheckSelected.Text = "Uncheck Selected";
            btnCategoryUncheckSelected.UseVisualStyleBackColor = true;
            btnCategoryUncheckSelected.Visible = false;
            // 
            // btnCategoryCheckSelected
            // 
            btnCategoryCheckSelected.Location = new System.Drawing.Point(7, 396);
            btnCategoryCheckSelected.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnCategoryCheckSelected.Name = "btnCategoryCheckSelected";
            btnCategoryCheckSelected.Size = new System.Drawing.Size(132, 27);
            btnCategoryCheckSelected.TabIndex = 16;
            btnCategoryCheckSelected.Text = "Check Selected";
            btnCategoryCheckSelected.UseVisualStyleBackColor = true;
            btnCategoryCheckSelected.Visible = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label12);
            tabPage2.Location = new System.Drawing.Point(4, 24);
            tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage2.Size = new System.Drawing.Size(378, 435);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Device";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(20, 21);
            label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(91, 15);
            label12.TabIndex = 0;
            label12.Text = "Comming Soon";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(label13);
            tabPage3.Location = new System.Drawing.Point(4, 24);
            tabPage3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new System.Drawing.Size(378, 435);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Points";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(26, 21);
            label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(91, 15);
            label13.TabIndex = 1;
            label13.Text = "Comming Soon";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(label14);
            tabPage4.Location = new System.Drawing.Point(4, 24);
            tabPage4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new System.Drawing.Size(378, 435);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Summary";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(29, 32);
            label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(91, 15);
            label14.TabIndex = 1;
            label14.Text = "Comming Soon";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(723, 568);
            Controls.Add(GrpPointSelection);
            Controls.Add(statusStrip1);
            Controls.Add(groupBox3);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FrmMain";
            Text = "Data Extraction Utility";
            Load += FrmMain_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            GrpPointSelection.ResumeLayout(false);
            GrpPointSelection.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtGEPPort;
        private System.Windows.Forms.Button BtnGetMetadata;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtServerIP;
        private System.Windows.Forms.TextBox TxtHistorianInstance;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtHistorianPort;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCsvMaxFileCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCsvMaxColumnCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxFileSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCsvMaxRowCount;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BtnExport;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tslStatusUpdate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbResolution;
        private System.Windows.Forms.DateTimePicker dateStopTime;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dateStartTime;
        private System.Windows.Forms.Label LblEstimatedSize;
        private System.Windows.Forms.CheckedListBox ChkSignalType;
        private System.Windows.Forms.GroupBox GrpPointSelection;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblPointCount;
        private System.Windows.Forms.Button btnCategoryUncheckSelected;
        private System.Windows.Forms.Button btnCategoryCheckSelected;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
    }
}

