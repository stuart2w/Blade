namespace Blade
{
	partial class frmListEdit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmListEdit));
			this.lstWords = new System.Windows.Forms.ListView();
			this.colWord = new System.Windows.Forms.ColumnHeader();
			this.colFrequency = new System.Windows.Forms.ColumnHeader();
			this.txtWord = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.cmbMode = new System.Windows.Forms.ComboBox();
			this.lblNotSystem = new System.Windows.Forms.Label();
			this.pnlAbbreviations = new System.Windows.Forms.Panel();
			this.lstAbbreviations = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.txtExpansion = new System.Windows.Forms.TextBox();
			this.txtAbbreviation = new System.Windows.Forms.TextBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.lblAbbreviation = new System.Windows.Forms.Label();
			this.btnDiagnostics = new System.Windows.Forms.Button();
			this.dlgSave = new System.Windows.Forms.SaveFileDialog();
			this.pnlAbbreviations.SuspendLayout();
			this.SuspendLayout();
			// 
			// lstWords
			// 
			this.lstWords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstWords.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colWord,
            this.colFrequency});
			this.lstWords.FullRowSelect = true;
			this.lstWords.GridLines = true;
			this.lstWords.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lstWords.HideSelection = false;
			this.lstWords.Location = new System.Drawing.Point(8, 56);
			this.lstWords.MultiSelect = false;
			this.lstWords.Name = "lstWords";
			this.lstWords.ShowGroups = false;
			this.lstWords.Size = new System.Drawing.Size(287, 259);
			this.lstWords.TabIndex = 0;
			this.lstWords.UseCompatibleStateImageBehavior = false;
			this.lstWords.View = System.Windows.Forms.View.Details;
			this.lstWords.Resize += new System.EventHandler(this.lstWords_Resize);
			this.lstWords.SelectedIndexChanged += new System.EventHandler(this.lstWords_SelectedIndexChanged);
			this.lstWords.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstWords_KeyDown);
			// 
			// colWord
			// 
			this.colWord.Text = "Word";
			this.colWord.Width = 200;
			// 
			// colFrequency
			// 
			this.colFrequency.Text = "Occurrs";
			// 
			// txtWord
			// 
			this.txtWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtWord.Location = new System.Drawing.Point(8, 32);
			this.txtWord.Name = "txtWord";
			this.txtWord.Size = new System.Drawing.Size(287, 20);
			this.txtWord.TabIndex = 1;
			this.txtWord.TextChanged += new System.EventHandler(this.txtWord_TextChanged);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.Location = new System.Drawing.Point(303, 32);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(88, 24);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelete.Enabled = false;
			this.btnDelete.Location = new System.Drawing.Point(303, 56);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(88, 24);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.Text = "Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.Location = new System.Drawing.Point(312, 296);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(80, 24);
			this.btnClose.TabIndex = 4;
			this.btnClose.Text = "Save";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// cmbMode
			// 
			this.cmbMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbMode.FormattingEnabled = true;
			this.cmbMode.Items.AddRange(new object[] {
            "All user words",
            "User words not in system list",
            "Words deleted from system list",
            "Abbreviations"});
			this.cmbMode.Location = new System.Drawing.Point(8, 8);
			this.cmbMode.Name = "cmbMode";
			this.cmbMode.Size = new System.Drawing.Size(383, 21);
			this.cmbMode.TabIndex = 5;
			this.cmbMode.SelectedIndexChanged += new System.EventHandler(this.cmbMode_SelectedIndexChanged);
			// 
			// lblNotSystem
			// 
			this.lblNotSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblNotSystem.Location = new System.Drawing.Point(303, 96);
			this.lblNotSystem.Name = "lblNotSystem";
			this.lblNotSystem.Size = new System.Drawing.Size(96, 72);
			this.lblNotSystem.TabIndex = 6;
			this.lblNotSystem.Text = "Blue colour indicates words which don\'t appear in the system dictionary";
			// 
			// pnlAbbreviations
			// 
			this.pnlAbbreviations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlAbbreviations.Controls.Add(this.lstAbbreviations);
			this.pnlAbbreviations.Controls.Add(this.txtExpansion);
			this.pnlAbbreviations.Controls.Add(this.txtAbbreviation);
			this.pnlAbbreviations.Location = new System.Drawing.Point(16, 80);
			this.pnlAbbreviations.Name = "pnlAbbreviations";
			this.pnlAbbreviations.Size = new System.Drawing.Size(287, 283);
			this.pnlAbbreviations.TabIndex = 7;
			// 
			// lstAbbreviations
			// 
			this.lstAbbreviations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstAbbreviations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.lstAbbreviations.FullRowSelect = true;
			this.lstAbbreviations.GridLines = true;
			this.lstAbbreviations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lstAbbreviations.HideSelection = false;
			this.lstAbbreviations.Location = new System.Drawing.Point(0, 24);
			this.lstAbbreviations.MultiSelect = false;
			this.lstAbbreviations.Name = "lstAbbreviations";
			this.lstAbbreviations.ShowGroups = false;
			this.lstAbbreviations.Size = new System.Drawing.Size(287, 259);
			this.lstAbbreviations.TabIndex = 2;
			this.lstAbbreviations.UseCompatibleStateImageBehavior = false;
			this.lstAbbreviations.View = System.Windows.Forms.View.Details;
			this.lstAbbreviations.SelectedIndexChanged += new System.EventHandler(this.lstAbbreviations_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Abbrev.";
			this.columnHeader1.Width = 99;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Word";
			this.columnHeader2.Width = 170;
			// 
			// txtExpansion
			// 
			this.txtExpansion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtExpansion.Location = new System.Drawing.Point(112, 0);
			this.txtExpansion.Name = "txtExpansion";
			this.txtExpansion.Size = new System.Drawing.Size(175, 20);
			this.txtExpansion.TabIndex = 1;
			this.txtExpansion.TextChanged += new System.EventHandler(this.txtAbbreviation_TextChanged);
			// 
			// txtAbbreviation
			// 
			this.txtAbbreviation.Location = new System.Drawing.Point(0, 0);
			this.txtAbbreviation.Name = "txtAbbreviation";
			this.txtAbbreviation.Size = new System.Drawing.Size(100, 20);
			this.txtAbbreviation.TabIndex = 0;
			this.txtAbbreviation.TextChanged += new System.EventHandler(this.txtAbbreviation_TextChanged);
			// 
			// btnClear
			// 
			this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClear.Location = new System.Drawing.Point(312, 216);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(80, 24);
			this.btnClear.TabIndex = 8;
			this.btnClear.Text = "Delete all";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// lblAbbreviation
			// 
			this.lblAbbreviation.Location = new System.Drawing.Point(304, 104);
			this.lblAbbreviation.Name = "lblAbbreviation";
			this.lblAbbreviation.Size = new System.Drawing.Size(96, 80);
			this.lblAbbreviation.TabIndex = 9;
			this.lblAbbreviation.Text = "In the list spaces are indicated by \'◊\'";
			this.lblAbbreviation.Visible = false;
			// 
			// btnDiagnostics
			// 
			this.btnDiagnostics.Location = new System.Drawing.Point(312, 248);
			this.btnDiagnostics.Name = "btnDiagnostics";
			this.btnDiagnostics.Size = new System.Drawing.Size(80, 40);
			this.btnDiagnostics.TabIndex = 10;
			this.btnDiagnostics.Text = "Export as CSV";
			this.btnDiagnostics.UseVisualStyleBackColor = true;
			this.btnDiagnostics.Click += new System.EventHandler(this.btnDiagnostics_Click);
			// 
			// dlgSave
			// 
			this.dlgSave.Filter = "*.csv|*.csv";
			// 
			// frmListEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(400, 333);
			this.Controls.Add(this.btnDiagnostics);
			this.Controls.Add(this.lblAbbreviation);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.pnlAbbreviations);
			this.Controls.Add(this.lblNotSystem);
			this.Controls.Add(this.cmbMode);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.txtWord);
			this.Controls.Add(this.lstWords);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(350, 300);
			this.Name = "frmListEdit";
			this.Text = "Word prediction list";
			this.pnlAbbreviations.ResumeLayout(false);
			this.pnlAbbreviations.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView lstWords;
		private System.Windows.Forms.ColumnHeader colWord;
		private System.Windows.Forms.ColumnHeader colFrequency;
		private System.Windows.Forms.TextBox txtWord;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.ComboBox cmbMode;
		private System.Windows.Forms.Label lblNotSystem;
		private System.Windows.Forms.Panel pnlAbbreviations;
		private System.Windows.Forms.ListView lstAbbreviations;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.TextBox txtExpansion;
		private System.Windows.Forms.TextBox txtAbbreviation;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Label lblAbbreviation;
		private System.Windows.Forms.Button btnDiagnostics;
		private System.Windows.Forms.SaveFileDialog dlgSave;
	}
}