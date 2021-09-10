using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blade
{
	public partial class frmMonitor : Form
	{
		public frmMonitor()
		{
			InitializeComponent();
			lblPredict.Text = "";
			lblPartial.Text = "";
			lblSentence.Text = "";
		}

		public void SetContextDisplay(string strPartial, string strSentence)
		{
			lblPartial.Text = "\"" + strPartial + "\"";
			lblSentence.Text = strSentence;
			if (strPartial == "" && strSentence == "")
			{
				lblSentence.Text = "(No text or start of sentence)";
				lblPartial.Text = "";
			}
			lblPartial.Refresh();
			this.Show(); // in case hidden since
			this.BringToFront();
		}

		public void SetPredictions(string strPredictions, bool bolError)
		{
			lblPredict.Text = strPredictions;
			lblPredict.ForeColor = bolError ? Color.Red : Color.Black;
		}
	}
}