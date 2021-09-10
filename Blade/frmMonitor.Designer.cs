namespace Blade
{
	partial class frmMonitor
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
			this.lblPartial = new System.Windows.Forms.Label();
			this.lblSentence = new System.Windows.Forms.Label();
			this.lblPredict = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblPartial
			// 
			this.lblPartial.AutoSize = true;
			this.lblPartial.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblPartial.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPartial.Location = new System.Drawing.Point(628, 0);
			this.lblPartial.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.lblPartial.Name = "lblPartial";
			this.lblPartial.Size = new System.Drawing.Size(52, 17);
			this.lblPartial.TabIndex = 0;
			this.lblPartial.Text = "label1";
			// 
			// lblSentence
			// 
			this.lblSentence.AutoSize = true;
			this.lblSentence.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblSentence.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblSentence.Location = new System.Drawing.Point(582, 0);
			this.lblSentence.Name = "lblSentence";
			this.lblSentence.Size = new System.Drawing.Size(46, 17);
			this.lblSentence.TabIndex = 1;
			this.lblSentence.Text = "label1";
			// 
			// lblPredict
			// 
			this.lblPredict.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPredict.Location = new System.Drawing.Point(0, 24);
			this.lblPredict.Name = "lblPredict";
			this.lblPredict.Size = new System.Drawing.Size(680, 24);
			this.lblPredict.TabIndex = 2;
			this.lblPredict.Text = "label1";
			// 
			// frmMonitor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(680, 50);
			this.Controls.Add(this.lblPredict);
			this.Controls.Add(this.lblSentence);
			this.Controls.Add(this.lblPartial);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmMonitor";
			this.ShowIcon = false;
			this.Text = "Word prediction monitor (shows what last predictions based upon)";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblPartial;
		private System.Windows.Forms.Label lblSentence;
		private System.Windows.Forms.Label lblPredict;
	}
}