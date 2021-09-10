using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IO = System.IO;

//This file is part of the Blade word prediction engine
//Copyright (C) 2012  ACE Centre Oxford

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Blade.Sample
{
    public partial class frmSample : Form, Blade.INotification
    {
        private int m_intSessionHash;
        private Engine m_objEngine;

        public frmSample()
        {
            InitializeComponent();
            m_intSessionHash = DateTime.Now.GetHashCode(); // a lazy way to get a random number
            // find where the base data is stored
            string strBase = IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\..\\..\\..\\BaseData-%lang%.bin";
            // the engine automatically loads user data from the user's application data folder, but this base data file 
            m_objEngine = new Engine();
            m_objEngine.Initialise(strBase, this);
            if (m_objEngine.GetDataObject(false).File == null)
            {
                MessageBox.Show("No prediction data was found either in the current user's application data, or at the expected base data:\r\n" +
                    strBase);
                Application.Exit();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            m_objEngine.LearnLine(txtMain.Text, m_intSessionHash);
            txtMain.Clear();
        }

        private void txtMain_TextChanged(object sender, EventArgs e)
        {
            // get predictions... (the easy bit)
            List<string> colPredictions = m_objEngine.Predict(txtMain.Text);
            // display them... (a bit more complex!)
            pnlResults.SuspendLayout();
            pnlResults.Controls.Clear();
            foreach (string strPrediction in colPredictions)
            {
                Label lblPredict = new Label();
                lblPredict.Text = strPrediction;
                lblPredict.Margin = new Padding(3);
                lblPredict.Cursor = Cursors.Hand;
                lblPredict.BackColor = Color.White;
                lblPredict.Size = new Size(120, 20);
                pnlResults.Controls.Add(lblPredict);
                lblPredict.Click += new EventHandler(Prediction_Click);
            }
            pnlResults.ResumeLayout();
        }

        private void Prediction_Click(object sender, System.EventArgs e)
        {
            string strWord = m_objEngine.LastPredictPartialWord(); // the characters which prediction treated as being the current word
            // (not just a case of splitting at space - it will merge hyphenated and abbreviated to some extent)
            txtMain.Text = txtMain.Text.Substring(0, txtMain.Text.Length - strWord.Length) + (sender as Label).Text + " ";
            // note automatically adds space on end.  Will trigger new predictions
            txtMain.Focus();
            txtMain.SelectionStart = txtMain.Text.Length; // force cursor back to end (tends to reset to start otherwise)
        }

        #region INotification implementations
        public void Notify(Notification eState)
        { // we shouldn't really receive any of these, they are mostly errors at the moment
            MessageBox.Show("Notification from prediction engine: " + eState.ToString());
        }

        public bool Learning(string strText)
        { // called at the end of a nominal complete sentence, when the prediction engine adds that text to its frequency list
            return true;
        }
        #endregion

        private void frmSample_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_objEngine.SaveData(false);
            // the false parameter indicates that the engine cannot clean up the data (potentially quite slow)
            // this sample has made no provision for this cleaning.  An actual application should either use Close (true)
            // or directly call Engine.
            m_objEngine = null;
        }
    }
}