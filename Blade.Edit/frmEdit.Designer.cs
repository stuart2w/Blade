namespace Blade
{
    partial class frmEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEdit));
			this.btnProcessFolder = new System.Windows.Forms.Button();
			this.btnProcessFile = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtPrediction = new System.Windows.Forms.TextBox();
			this.lstPredictions = new System.Windows.Forms.ListBox();
			this.txtPredictionInfo = new System.Windows.Forms.TextBox();
			this.dlgFolder = new System.Windows.Forms.FolderBrowserDialog();
			this.lblInfo = new System.Windows.Forms.Label();
			this.btnClearFrequencies = new System.Windows.Forms.Button();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.btnOptimise = new System.Windows.Forms.Button();
			this.btnDumpText = new System.Windows.Forms.Button();
			this.btnLimitDepth = new System.Windows.Forms.Button();
			this.nudDepth = new System.Windows.Forms.NumericUpDown();
			this.btnTestPredictions = new System.Windows.Forms.Button();
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.btnSave = new System.Windows.Forms.Button();
			this.chkLearnOnTest = new System.Windows.Forms.CheckBox();
			this.btnLearnClear = new System.Windows.Forms.Button();
			this.rdoBase = new System.Windows.Forms.RadioButton();
			this.rdoUser = new System.Windows.Forms.RadioButton();
			this.rdoTest = new System.Windows.Forms.RadioButton();
			this.pnlTools = new System.Windows.Forms.Panel();
			this.btnXMLExtract = new System.Windows.Forms.Button();
			this.txtMaxWords = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnLoad = new System.Windows.Forms.Button();
			this.pnlHeader = new System.Windows.Forms.FlowLayoutPanel();
			this.pnlTest = new System.Windows.Forms.Panel();
			this.label6 = new System.Windows.Forms.Label();
			this.dlgSave = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.nudDepth)).BeginInit();
			this.pnlTools.SuspendLayout();
			this.pnlHeader.SuspendLayout();
			this.pnlTest.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnProcessFolder
			// 
			this.btnProcessFolder.Location = new System.Drawing.Point(160, 64);
			this.btnProcessFolder.Margin = new System.Windows.Forms.Padding(4);
			this.btnProcessFolder.Name = "btnProcessFolder";
			this.btnProcessFolder.Size = new System.Drawing.Size(160, 48);
			this.btnProcessFolder.TabIndex = 0;
			this.btnProcessFolder.Text = "Learn from all text files in folder and subfolder";
			this.btnProcessFolder.UseVisualStyleBackColor = true;
			this.btnProcessFolder.Click += new System.EventHandler(this.btnProcessFolder_Click);
			// 
			// btnProcessFile
			// 
			this.btnProcessFile.Location = new System.Drawing.Point(8, 64);
			this.btnProcessFile.Margin = new System.Windows.Forms.Padding(4);
			this.btnProcessFile.Name = "btnProcessFile";
			this.btnProcessFile.Size = new System.Drawing.Size(144, 49);
			this.btnProcessFile.TabIndex = 3;
			this.btnProcessFile.Text = "Learn from single text file";
			this.btnProcessFile.UseVisualStyleBackColor = true;
			this.btnProcessFile.Click += new System.EventHandler(this.btnProcessFile_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(11, 8);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 20);
			this.label2.TabIndex = 5;
			this.label2.Text = "Predict from:";
			// 
			// txtPrediction
			// 
			this.txtPrediction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPrediction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPrediction.Location = new System.Drawing.Point(128, 8);
			this.txtPrediction.Margin = new System.Windows.Forms.Padding(4);
			this.txtPrediction.Name = "txtPrediction";
			this.txtPrediction.Size = new System.Drawing.Size(600, 26);
			this.txtPrediction.TabIndex = 6;
			this.txtPrediction.TextChanged += new System.EventHandler(this.txtPrediction_TextChanged);
			// 
			// lstPredictions
			// 
			this.lstPredictions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.lstPredictions.FormattingEnabled = true;
			this.lstPredictions.ItemHeight = 16;
			this.lstPredictions.Location = new System.Drawing.Point(128, 40);
			this.lstPredictions.Margin = new System.Windows.Forms.Padding(4);
			this.lstPredictions.Name = "lstPredictions";
			this.lstPredictions.Size = new System.Drawing.Size(233, 420);
			this.lstPredictions.TabIndex = 7;
			this.lstPredictions.SelectedIndexChanged += new System.EventHandler(this.lstPredictions_SelectedIndexChanged);
			// 
			// txtPredictionInfo
			// 
			this.txtPredictionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPredictionInfo.Location = new System.Drawing.Point(368, 40);
			this.txtPredictionInfo.Margin = new System.Windows.Forms.Padding(4);
			this.txtPredictionInfo.Multiline = true;
			this.txtPredictionInfo.Name = "txtPredictionInfo";
			this.txtPredictionInfo.ReadOnly = true;
			this.txtPredictionInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtPredictionInfo.Size = new System.Drawing.Size(448, 424);
			this.txtPredictionInfo.TabIndex = 8;
			// 
			// lblInfo
			// 
			this.lblInfo.BackColor = System.Drawing.Color.AliceBlue;
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInfo.Location = new System.Drawing.Point(0, 0);
			this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(888, 48);
			this.lblInfo.TabIndex = 12;
			// 
			// btnClearFrequencies
			// 
			this.btnClearFrequencies.Location = new System.Drawing.Point(120, 264);
			this.btnClearFrequencies.Margin = new System.Windows.Forms.Padding(4);
			this.btnClearFrequencies.Name = "btnClearFrequencies";
			this.btnClearFrequencies.Size = new System.Drawing.Size(280, 30);
			this.btnClearFrequencies.TabIndex = 13;
			this.btnClearFrequencies.Text = "Clear Frequencies (but retain structure)";
			this.btnClearFrequencies.UseVisualStyleBackColor = true;
			this.btnClearFrequencies.Click += new System.EventHandler(this.btnClearFrequencies_Click);
			// 
			// btnClearAll
			// 
			this.btnClearAll.Location = new System.Drawing.Point(8, 264);
			this.btnClearAll.Margin = new System.Windows.Forms.Padding(4);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Size = new System.Drawing.Size(107, 30);
			this.btnClearAll.TabIndex = 14;
			this.btnClearAll.Text = "Clear all data";
			this.btnClearAll.UseVisualStyleBackColor = true;
			this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
			// 
			// btnOptimise
			// 
			this.btnOptimise.Location = new System.Drawing.Point(8, 136);
			this.btnOptimise.Margin = new System.Windows.Forms.Padding(4);
			this.btnOptimise.Name = "btnOptimise";
			this.btnOptimise.Size = new System.Drawing.Size(144, 30);
			this.btnOptimise.TabIndex = 15;
			this.btnOptimise.Text = "Optimise";
			this.btnOptimise.UseVisualStyleBackColor = true;
			this.btnOptimise.Click += new System.EventHandler(this.btnOptimise_Click);
			// 
			// btnDumpText
			// 
			this.btnDumpText.Location = new System.Drawing.Point(312, 208);
			this.btnDumpText.Margin = new System.Windows.Forms.Padding(4);
			this.btnDumpText.Name = "btnDumpText";
			this.btnDumpText.Size = new System.Drawing.Size(192, 30);
			this.btnDumpText.TabIndex = 16;
			this.btnDumpText.Text = "Dump text version of data";
			this.btnDumpText.UseVisualStyleBackColor = true;
			this.btnDumpText.Click += new System.EventHandler(this.btnDumpText_Click);
			// 
			// btnLimitDepth
			// 
			this.btnLimitDepth.Location = new System.Drawing.Point(8, 304);
			this.btnLimitDepth.Margin = new System.Windows.Forms.Padding(4);
			this.btnLimitDepth.Name = "btnLimitDepth";
			this.btnLimitDepth.Size = new System.Drawing.Size(149, 30);
			this.btnLimitDepth.TabIndex = 17;
			this.btnLimitDepth.Text = "Truncate at depth:";
			this.btnLimitDepth.UseVisualStyleBackColor = true;
			this.btnLimitDepth.Click += new System.EventHandler(this.btnLimitDepth_Click);
			// 
			// nudDepth
			// 
			this.nudDepth.Location = new System.Drawing.Point(168, 304);
			this.nudDepth.Margin = new System.Windows.Forms.Padding(4);
			this.nudDepth.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudDepth.Name = "nudDepth";
			this.nudDepth.Size = new System.Drawing.Size(56, 23);
			this.nudDepth.TabIndex = 18;
			this.nudDepth.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// btnTestPredictions
			// 
			this.btnTestPredictions.Location = new System.Drawing.Point(8, 352);
			this.btnTestPredictions.Margin = new System.Windows.Forms.Padding(4);
			this.btnTestPredictions.Name = "btnTestPredictions";
			this.btnTestPredictions.Size = new System.Drawing.Size(128, 30);
			this.btnTestPredictions.TabIndex = 19;
			this.btnTestPredictions.Text = "Test predictions";
			this.btnTestPredictions.UseVisualStyleBackColor = true;
			this.btnTestPredictions.Click += new System.EventHandler(this.btnTestPredictions_Click);
			// 
			// dlgOpen
			// 
			this.dlgOpen.FileName = "wp test.txt";
			this.dlgOpen.Filter = "*.txt|*.txt";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(8, 208);
			this.btnSave.Margin = new System.Windows.Forms.Padding(4);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(144, 30);
			this.btnSave.TabIndex = 20;
			this.btnSave.Text = "Save data file";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// chkLearnOnTest
			// 
			this.chkLearnOnTest.AutoSize = true;
			this.chkLearnOnTest.Location = new System.Drawing.Point(146, 354);
			this.chkLearnOnTest.Margin = new System.Windows.Forms.Padding(4);
			this.chkLearnOnTest.Name = "chkLearnOnTest";
			this.chkLearnOnTest.Size = new System.Drawing.Size(145, 21);
			this.chkLearnOnTest.TabIndex = 21;
			this.chkLearnOnTest.Text = "Learn while testing";
			this.chkLearnOnTest.UseVisualStyleBackColor = true;
			// 
			// btnLearnClear
			// 
			this.btnLearnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLearnClear.Location = new System.Drawing.Point(736, 8);
			this.btnLearnClear.Margin = new System.Windows.Forms.Padding(4);
			this.btnLearnClear.Name = "btnLearnClear";
			this.btnLearnClear.Size = new System.Drawing.Size(80, 30);
			this.btnLearnClear.TabIndex = 22;
			this.btnLearnClear.Text = "Learn";
			this.btnLearnClear.UseVisualStyleBackColor = true;
			this.btnLearnClear.Click += new System.EventHandler(this.btnLearnClear_Click);
			// 
			// rdoBase
			// 
			this.rdoBase.AutoSize = true;
			this.rdoBase.Checked = true;
			this.rdoBase.Location = new System.Drawing.Point(4, 4);
			this.rdoBase.Margin = new System.Windows.Forms.Padding(4);
			this.rdoBase.Name = "rdoBase";
			this.rdoBase.Size = new System.Drawing.Size(176, 22);
			this.rdoBase.TabIndex = 23;
			this.rdoBase.TabStop = true;
			this.rdoBase.Text = "Language base data";
			this.rdoBase.UseVisualStyleBackColor = true;
			this.rdoBase.CheckedChanged += new System.EventHandler(this.rdoHeader_CheckedChanged);
			// 
			// rdoUser
			// 
			this.rdoUser.AutoSize = true;
			this.rdoUser.Location = new System.Drawing.Point(188, 4);
			this.rdoUser.Margin = new System.Windows.Forms.Padding(4);
			this.rdoUser.Name = "rdoUser";
			this.rdoUser.Size = new System.Drawing.Size(99, 22);
			this.rdoUser.TabIndex = 24;
			this.rdoUser.Text = "User data";
			this.rdoUser.UseVisualStyleBackColor = true;
			this.rdoUser.CheckedChanged += new System.EventHandler(this.rdoHeader_CheckedChanged);
			// 
			// rdoTest
			// 
			this.rdoTest.AutoSize = true;
			this.rdoTest.Location = new System.Drawing.Point(295, 4);
			this.rdoTest.Margin = new System.Windows.Forms.Padding(4);
			this.rdoTest.Name = "rdoTest";
			this.rdoTest.Size = new System.Drawing.Size(59, 22);
			this.rdoTest.TabIndex = 25;
			this.rdoTest.Text = "Test";
			this.rdoTest.UseVisualStyleBackColor = true;
			this.rdoTest.Click += new System.EventHandler(this.rdoTest_Click);
			this.rdoTest.CheckedChanged += new System.EventHandler(this.rdoHeader_CheckedChanged);
			// 
			// pnlTools
			// 
			this.pnlTools.Controls.Add(this.btnXMLExtract);
			this.pnlTools.Controls.Add(this.txtMaxWords);
			this.pnlTools.Controls.Add(this.label7);
			this.pnlTools.Controls.Add(this.label5);
			this.pnlTools.Controls.Add(this.label4);
			this.pnlTools.Controls.Add(this.label3);
			this.pnlTools.Controls.Add(this.label1);
			this.pnlTools.Controls.Add(this.btnLoad);
			this.pnlTools.Controls.Add(this.btnProcessFolder);
			this.pnlTools.Controls.Add(this.btnProcessFile);
			this.pnlTools.Controls.Add(this.btnClearFrequencies);
			this.pnlTools.Controls.Add(this.chkLearnOnTest);
			this.pnlTools.Controls.Add(this.lblInfo);
			this.pnlTools.Controls.Add(this.btnClearAll);
			this.pnlTools.Controls.Add(this.btnSave);
			this.pnlTools.Controls.Add(this.btnOptimise);
			this.pnlTools.Controls.Add(this.btnTestPredictions);
			this.pnlTools.Controls.Add(this.btnDumpText);
			this.pnlTools.Controls.Add(this.nudDepth);
			this.pnlTools.Controls.Add(this.btnLimitDepth);
			this.pnlTools.Location = new System.Drawing.Point(8, 72);
			this.pnlTools.Margin = new System.Windows.Forms.Padding(4);
			this.pnlTools.Name = "pnlTools";
			this.pnlTools.Size = new System.Drawing.Size(888, 448);
			this.pnlTools.TabIndex = 26;
			// 
			// btnXMLExtract
			// 
			this.btnXMLExtract.Location = new System.Drawing.Point(8, 400);
			this.btnXMLExtract.Name = "btnXMLExtract";
			this.btnXMLExtract.Size = new System.Drawing.Size(248, 32);
			this.btnXMLExtract.TabIndex = 29;
			this.btnXMLExtract.Text = "Extract text from XML corpus file";
			this.btnXMLExtract.UseVisualStyleBackColor = true;
			this.btnXMLExtract.Click += new System.EventHandler(this.btnXMLExtract_Click);
			// 
			// txtMaxWords
			// 
			this.txtMaxWords.Location = new System.Drawing.Point(96, 176);
			this.txtMaxWords.Name = "txtMaxWords";
			this.txtMaxWords.Size = new System.Drawing.Size(88, 23);
			this.txtMaxWords.TabIndex = 28;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 176);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 17);
			this.label7.TabIndex = 27;
			this.label7.Text = "Max words:";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(296, 352);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(592, 64);
			this.label5.TabIndex = 26;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(232, 304);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(648, 56);
			this.label4.TabIndex = 25;
			this.label4.Text = "This limits the number of previous words stored.  ie 1 previous word = bigrams; 2" +
				" = trigrams.  Deeper data is discarded.  This is not remembered, so more complex" +
				" data may be generated again.";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(200, 136);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(680, 64);
			this.label3.TabIndex = 24;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(336, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(528, 56);
			this.label1.TabIndex = 23;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(160, 208);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(144, 32);
			this.btnLoad.TabIndex = 22;
			this.btnLoad.Text = "Load data file";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// pnlHeader
			// 
			this.pnlHeader.Controls.Add(this.rdoBase);
			this.pnlHeader.Controls.Add(this.rdoUser);
			this.pnlHeader.Controls.Add(this.rdoTest);
			this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pnlHeader.Location = new System.Drawing.Point(0, 0);
			this.pnlHeader.Margin = new System.Windows.Forms.Padding(4);
			this.pnlHeader.Name = "pnlHeader";
			this.pnlHeader.Size = new System.Drawing.Size(960, 39);
			this.pnlHeader.TabIndex = 27;
			// 
			// pnlTest
			// 
			this.pnlTest.Controls.Add(this.label6);
			this.pnlTest.Controls.Add(this.txtPrediction);
			this.pnlTest.Controls.Add(this.label2);
			this.pnlTest.Controls.Add(this.lstPredictions);
			this.pnlTest.Controls.Add(this.btnLearnClear);
			this.pnlTest.Controls.Add(this.txtPredictionInfo);
			this.pnlTest.Location = new System.Drawing.Point(136, 48);
			this.pnlTest.Name = "pnlTest";
			this.pnlTest.Size = new System.Drawing.Size(824, 472);
			this.pnlTest.TabIndex = 28;
			this.pnlTest.Visible = false;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(8, 40);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 416);
			this.label6.TabIndex = 23;
			this.label6.Text = "Note: Clicking shows probability info - and doesn\'t insert the text.  You need to" +
				" type the entire words!";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// dlgSave
			// 
			this.dlgSave.Filter = "*.txt|*.txt";
			// 
			// frmEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(960, 552);
			this.Controls.Add(this.pnlTools);
			this.Controls.Add(this.pnlTest);
			this.Controls.Add(this.pnlHeader);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(1200, 590);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(600, 590);
			this.Name = "frmEdit";
			this.ShowIcon = false;
			this.Text = "Blade word prediction base data editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.nudDepth)).EndInit();
			this.pnlTools.ResumeLayout(false);
			this.pnlTools.PerformLayout();
			this.pnlHeader.ResumeLayout(false);
			this.pnlHeader.PerformLayout();
			this.pnlTest.ResumeLayout(false);
			this.pnlTest.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnProcessFolder;
        private System.Windows.Forms.Button btnProcessFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPrediction;
        private System.Windows.Forms.ListBox lstPredictions;
        private System.Windows.Forms.TextBox txtPredictionInfo;
        private System.Windows.Forms.FolderBrowserDialog dlgFolder;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnClearFrequencies;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnOptimise;
        private System.Windows.Forms.Button btnDumpText;
        private System.Windows.Forms.Button btnLimitDepth;
        private System.Windows.Forms.NumericUpDown nudDepth;
        private System.Windows.Forms.Button btnTestPredictions;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkLearnOnTest;
        private System.Windows.Forms.Button btnLearnClear;
        private System.Windows.Forms.RadioButton rdoBase;
        private System.Windows.Forms.RadioButton rdoUser;
        private System.Windows.Forms.RadioButton rdoTest;
        private System.Windows.Forms.Panel pnlTools;
        private System.Windows.Forms.FlowLayoutPanel pnlHeader;
        private System.Windows.Forms.Panel pnlTest;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMaxWords;
        private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btnXMLExtract;
    }
}

