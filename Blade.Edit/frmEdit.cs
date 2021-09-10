using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

namespace Blade
{

	public partial class frmEdit : Form
	{
		private Engine m_objEngine;
		private int m_intSessionHash;
		private bool m_bolBaseChanged = false;
		private bool m_bolUserChanged = false;
		private Tree m_objBase;
		private Tree m_objUser;
		private Tree m_objMain = null; // this is the copy used for testing.  If it is defined it is a clone of m_objBase
		// with m_objUser added

		public frmEdit()
		{
			InitializeComponent();
			pnlHeader.SendToBack(); // otherwise pnlTest or pnlTools can disappear behind it
			pnlTest.Dock = DockStyle.Fill; // these are Undocked in the editor as it makes it easier to switch between them
			pnlTools.Dock = DockStyle.Fill;

			m_objEngine = new Blade.Engine();
			m_intSessionHash = DateTime.Now.GetHashCode(); // an essentially random number
			m_objUser = new Tree();
			string strFile = Blade.Edit.Properties.Settings.Default.DataFile;
			try
			{
				if (strFile != "" && IO.File.Exists(strFile))
					m_objBase = Tree.Read(strFile);
				else
					m_objBase = new Tree();
			}
			catch
			{ m_objBase = new Tree(); }
			m_objEngine.Initialise("", null);
			m_objEngine.NumberPredictions = 20;
			m_objEngine.OmitPreviousSuggestions = false;
			ShowInfoOnDataChange();

			Debug.WriteLine(Guid.NewGuid().ToString());
		}

		#region Selection of dataset
		private bool User
		{ get { return rdoUser.Checked; } }

		private Tree Data
		{
			get { if (User) return m_objUser; else return m_objBase; }
			set { if (User) m_objUser = value; else m_objBase = value; }
		}

		private bool Changed
		{
			get
			{ if (User) return m_bolUserChanged; else return m_bolBaseChanged; }
			set
			{
				if (User) m_bolUserChanged = value; else m_bolBaseChanged = value;
				if (value) m_objMain = null;
			}
		}

		private void rdoHeader_CheckedChanged(object sender, EventArgs e)
		{
			pnlTools.Visible = (rdoUser.Checked || rdoBase.Checked);
			pnlTest.Visible = (rdoTest.Checked);
			txtPredictionInfo.Clear();
			lstPredictions.Items.Clear();
			if (!rdoTest.Checked)
				ShowInfoOnDataChange();
		}

		private void rdoTest_Click(object sender, EventArgs e)
		{// initialise combined user and main data
			if (m_objMain == null)
			{
				m_objMain = m_objBase.Clone();
				m_objMain.AddUserData(m_objUser);
				m_objEngine.Initialise(m_objMain, m_objUser, false);
			}
			else
				m_objEngine.ClearRecency();
			if (m_objEngine.OptimisationNeeded >= 3)
				MessageBox.Show("WARNING: Data was not optimised - processing may be very slow and return spurious words.");
		}
		#endregion

		#region Analysing input text
		private void btnProcessFolder_Click(object sender, EventArgs e)
		{
			dlgFolder.SelectedPath = Blade.Edit.Properties.Settings.Default.ProcessFolder;
			if (dlgFolder.ShowDialog() != DialogResult.OK)
				return;
			Blade.Edit.Properties.Settings.Default.ProcessFolder = dlgFolder.SelectedPath;
			Data.CorpusMode = true; // will reset automatically when needed
			AnalyseFolder(dlgFolder.SelectedPath);
			Changed = true;
			ShowInfoOnDataChange();
			MessageBox.Show("Processing complete");
		}

		private void btnProcessFile_Click(object sender, EventArgs e)
		{
			dlgOpen.Filter = "*.txt|*.txt";
			dlgOpen.FileName = Blade.Edit.Properties.Settings.Default.ProcessFile;
			if (dlgOpen.ShowDialog() != DialogResult.OK)
				return;
			Blade.Edit.Properties.Settings.Default.ProcessFile = dlgOpen.FileName;
			Data.CorpusMode = true; // will reset automatically when needed
			AnalyseFile(dlgOpen.FileName, true);
			Changed = true;
			ShowInfoOnDataChange();
		}

		private void AnalyseFolder(string strFolder)
		{
			foreach (string strFile in IO.Directory.GetFiles(strFolder, "*.txt"))
			{
				AnalyseFile(strFile, false);
				Application.DoEvents();
			}
			foreach (string strSub in IO.Directory.GetDirectories(strFolder))
				AnalyseFolder(strSub);
		}

		private System.Text.RegularExpressions.Regex m_regexBracket =
			new System.Text.RegularExpressions.Regex("(?<bracket>\\[.+?\\])");
		private void AnalyseFile(string strFile, bool bolTreatAsMultiSession)
		{
			// this does assume 'line' breaks are paragraph breaks.  If the document is word-wrapped in some way then sentences will be chopped apart rather inappropriately
			lblInfo.Text = IO.Path.GetFileName(strFile);
			lblInfo.Refresh();
			int intFileHash = IO.Path.GetFileName(strFile).GetHashCode(); // used to check words appear in multi files
			if (bolTreatAsMultiSession)
				intFileHash = RootNode.MULTI_SESSION;
			using (IO.StreamReader strm = new IO.StreamReader(strFile, true))
			{
				string strText = strm.ReadLine();
				while (strText != null)
				{
					strText = strText.Replace("&amp;", "&").Replace("&lt;", "<");// the SVE bloggmix corpus was XML but invalid - so I just stripped <> - which leaves some encoded chars
					// because much of the sample text comes from Wikipedia it is somewhat polluted by [citation needed]
					// which is especially nasty as there is often no space before this, meaning the . and [ run together as a single token
					Match objMatch = m_regexBracket.Match(strText);
					if (objMatch.Success)
					{
						foreach (Capture objCapture in objMatch.Groups["bracket"].Captures)
						{
							strText = strText.Replace(objCapture.Value, "");
						}
					}
					Data.AnalyseLine(strText.Trim(), intFileHash);
					strText = strm.ReadLine();
				}
			}
		}
		#endregion

		private void btnLoad_Click(object sender, EventArgs e)
		{
			dlgOpen.Filter = "*.bin|*.bin";
			if (rdoBase.Checked)
				dlgOpen.FileName = Blade.Edit.Properties.Settings.Default.DataFile;
			if (dlgOpen.ShowDialog() != DialogResult.OK)
				return;
			if (rdoBase.Checked)
				Blade.Edit.Properties.Settings.Default.DataFile = dlgOpen.FileName;
			Data = Tree.Read(dlgOpen.FileName);
			ShowInfoOnDataChange();
			Changed = false;
			m_objMain = null;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			dlgSave.Filter = "*.bin|*.bin";
			if (rdoBase.Checked)
			{
				dlgSave.InitialDirectory = IO.Path.GetDirectoryName(Blade.Edit.Properties.Settings.Default.DataFile);
				dlgSave.FileName = IO.Path.GetFileName(Blade.Edit.Properties.Settings.Default.DataFile);
			}
			if (dlgSave.ShowDialog() != DialogResult.OK)
				return;
			if (rdoBase.Checked)
				Blade.Edit.Properties.Settings.Default.DataFile = dlgSave.FileName;
			Cursor = Cursors.WaitCursor;
			Data.Write(dlgSave.FileName, true);
			Changed = false;
			Cursor = Cursors.Default;
		}

		private void btnDumpText_Click(object sender, EventArgs e)
		{
			dlgSave.Filter = "*.txt|*.txt";
			dlgSave.InitialDirectory = "";
			if (dlgSave.ShowDialog() != DialogResult.OK)
				return;
			Data.Write(dlgSave.FileName, false);
			lblInfo.Text = "Saved.";
		}

		#region Other edits
		private void btnClearFrequencies_Click(object sender, EventArgs e)
		{
			Data.Clear(true);
			ShowInfoOnDataChange();
			Changed = true;
		}

		private void btnClearAll_Click(object sender, EventArgs e)
		{
			Data.Clear(false);
			ShowInfoOnDataChange();
		}

		private void btnLimitDepth_Click(object sender, EventArgs e)
		{
			Data.LimitDepth((int)nudDepth.Value);
			ShowInfoOnDataChange();
		}

		private void btnOptimise_Click(object sender, EventArgs e)
		{
			int intMaxWords = 0;
			if (txtMaxWords.Text != "")
				if (!int.TryParse(txtMaxWords.Text, out intMaxWords) || intMaxWords < 1000 || intMaxWords > 1000000)
				{
					MessageBox.Show("Maximum words must be a number between 1000 and 1000000; or blank for no limit");
					return;
				}
			Cursor = Cursors.WaitCursor;
			Data.Optimise(intMaxWords);
			Cursor = Cursors.Default;
			ShowInfoOnDataChange();
		}
		#endregion

		private void ShowInfoOnDataChange()
		{
			if (Data.File == null && Data.TotalFrequency == 0)
				lblInfo.Text = "No base data found - initialised to empty dataset";
			else
			{
				lblInfo.Text = Data.StatText() + "; Depth = " + Data.GreatestDepth().ToString();
				Debug.WriteLine("Depth: " + Data.GreatestDepth().ToString());
				Debug.WriteLine("Deepest text: " + Data.GreatestDepthText());
			}
		}

		private void btnTestPredictions_Click(object sender, EventArgs e)
		{
			//TestSendKeys(); return;
			dlgOpen.Filter = "*.txt|*.txt";
			if (dlgOpen.ShowDialog() != DialogResult.OK)
				return;
			//m_objEngine.ClearRecency();
			m_objEngine.NumberPredictions = 8;
			DateTime dtStart = DateTime.Now;
			using (IO.StreamWriter output = new System.IO.StreamWriter("f:\\temp\\test output.txt"))
			{
				using (IO.StreamReader strm = new IO.StreamReader(dlgOpen.FileName))
				{
					int intWordsTested = 0;
					int intRequiredLetters = 0; // the number of letters which would need to be typed
					int intNotInDictionaryLetters = 0; // letters typed for words not in dictionary at all
					int intInDictionary = 0;
					int intWordLength = 0; // total length of words considered
					string strText = strm.ReadLine();
					while (strText != null)
					{
						// because much of the sample text comes from Wikipedia it is somewhat polluted by [citation needed]
						// which is especially nasty as there is often no space before this, meaning the . and [ run together as a single token
						Match objMatch = m_regexBracket.Match(strText);
						if (objMatch.Success)
						{
							foreach (Capture objCapture in objMatch.Groups["bracket"].Captures)
							{
								strText = strText.Replace(objCapture.Value, "");
							}
						}
						lblInfo.Text = strText;
						lblInfo.Refresh();
						m_objEngine.Test(strText, chkLearnOnTest.Checked, ref intWordsTested, ref intRequiredLetters, ref intWordLength, ref intInDictionary, ref intNotInDictionaryLetters, output);
						strText = strm.ReadLine();
					} // while lines remaining in file
					lblInfo.Text = "Tested: " + intWordsTested.ToString() + " (" + intInDictionary.ToString() +
						" in dictionary) ; Required: " + intRequiredLetters.ToString() + " = " + ((float)intRequiredLetters / intWordsTested).ToString("0.00") +
						"/word; total chars: " + intWordLength.ToString() + " = " + ((float)intWordLength / intWordsTested).ToString("0.00") + "/word" +
						"; Chars for words not in dictionary: " + intNotInDictionaryLetters.ToString() + " = " + ((float)intNotInDictionaryLetters / intWordsTested).ToString("0.00") + "/word";
				} // using
				//Engine.UserRecency.DebugDump();
				Debug.WriteLine("Test: ms elapsed = " + DateTime.Now.Subtract(dtStart).TotalMilliseconds.ToString());
			}
			m_objEngine.NumberPredictions = 20;
		}

		private void TestSendKeys()
		{  // no GUI for this currently - add call to some other button
			// test code which sends a file as key presses;  assumes SAW (or equivalent) is listening in
			dlgOpen.Filter = "*.txt|*.txt";
			if (dlgOpen.ShowDialog() != DialogResult.OK)
				return;
			m_objEngine.ClearRecency();
			DateTime dtStart = DateTime.Now;
			rdoBase.Focus();
			using (IO.StreamReader strm = new IO.StreamReader(dlgOpen.FileName))
			{
				string strText = strm.ReadLine();
				foreach (char ch in strText)
				{
					//m_objEngine.Type(new string(ch,1), true);
					m_objEngine.TrackCharacterTyped(ch, 0);
					m_objEngine.Predict(m_objEngine.TrackedMessage);
					Debug.WriteLine(m_objEngine.TrackedMessage + " >> " + m_objEngine.GetLastPredictions("|"));
					//if (ch != '(' && ch != ')')
					//    System.Windows.Forms.SendKeys.Send(new string(ch, 1));

					Application.DoEvents();
					System.Threading.Thread.Sleep(100);
					Application.DoEvents();
				}
			} // using
		}

		#region Prediction Test area
		private void txtPrediction_TextChanged(object sender, EventArgs e)
		{
			if (txtPrediction.Text == "") return;
			lstPredictions.Items.Clear();
			foreach (string strResult in m_objEngine.Predict(txtPrediction.Text))
			{
				lstPredictions.Items.Add(strResult);
			}
		}

		private void lstPredictions_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtPredictionInfo.Clear();
			if (lstPredictions.SelectedIndex >= 0)
			{
				txtPredictionInfo.Text = m_objEngine.GetInfoForPrediction(lstPredictions.Text);
				txtPredictionInfo.AppendText("\r\n\r\nHash = " + Tree.GetHash(lstPredictions.Text).ToString());
			}
		}

		private void btnLearnClear_Click(object sender, EventArgs e)
		{ // clears the test message, telling the engine to learn that text first
			m_objEngine.LearnLine(txtPrediction.Text, m_intSessionHash);
			txtPrediction.Clear();
		}
		#endregion

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_bolBaseChanged)
			{
				if (MessageBox.Show("Base data is changed, but not saved.  Close anyway?", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					e.Cancel = true;
					return;
				}
			}
			Blade.Edit.Properties.Settings.Default.Save();
		}

		//private void btnXMLExtract_Click(object sender, EventArgs e)
		//{ // normal version, parses XML
		//    dlgOpen.Filter = "*.xml|*.xml|*.*|*.*";
		//    if (dlgOpen.ShowDialog() != DialogResult.OK) return;
		//    dlgSave.FileName = IO.Path.ChangeExtension(dlgOpen.FileName, ".txt");
		//    dlgSave.Filter = "*.txt|*.txt";
		//    if (dlgSave.ShowDialog() != DialogResult.OK) return;
		//    try
		//    {
		//        using (IO.StreamWriter output = new System.IO.StreamWriter(dlgSave.FileName, false))
		//        {
		//            IO.FileStream fs = new IO.FileStream(dlgOpen.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
		//            byte[] b = new byte[10000];
		//            fs.Seek(0, System.IO.SeekOrigin.Begin); //-10000, System.IO.SeekOrigin.End);
		//            fs.Read(b, 0, 10000);
		//            output.BaseStream.Write(b, 0, 10000);
		//            return;
		//            for (int i = 0; i < 1000; i++)
		//            {
		//                IO.StreamReader r = new System.IO.StreamReader(fs, System.Text.Encoding.UTF7);
		//                output.WriteLine(r.ReadLine());
		//            }
		//            return;
		//            System.Xml.XmlReaderSettings s = new System.Xml.XmlReaderSettings();
		//            s.ConformanceLevel = System.Xml.ConformanceLevel.Fragment;
		//            System.Xml.XmlReader xr = System.Xml.XmlReader.Create(fs, s);
		//            //xr.Settings.ConformanceLevel = System.Xml.ConformanceLevel.Fragment;
		//            while (!xr.EOF)
		//            {
		//                switch (xr.MoveToContent())
		//                {
		//                    case System.Xml.XmlNodeType.Element:
		//                        if (xr.Name == "Sentence")
		//                            output.WriteLine(); // inserting new line at start of next sentence splits them
		//                        xr.Read();
		//                        break; // will continue within this next time around
		//                    case System.Xml.XmlNodeType.Text:
		//                        output.Write(" " + xr.Value);
		//                        xr.Read();
		//                        break;
		//                    default: xr.Read(); // end markers etc, continue onwards
		//                        break;
		//                }
		//            }
		//            xr.Close();
		//            fs.Close();
		//        }
		//        MessageBox.Show("Done");
		//    }
		//    catch (Exception ex)
		//    { MessageBox.Show(ex.Message); }
		//}

		private static Regex BlogContent = new Regex("<w[^>]*>(?<content>.+)</w>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private void btnXMLExtract_Click(object sender, EventArgs e)
		{ // above doesn't work for bloggmix as it is not valid XML (some tags are not closed)
			// however seems to be one tag per line, therefore we can load the lines and strip out everything inside <>
			dlgOpen.Filter = "*.xml|*.xml|*.*|*.*";
			if (dlgOpen.ShowDialog() != DialogResult.OK) return;
			dlgSave.FileName = IO.Path.ChangeExtension(dlgOpen.FileName, ".txt");
			dlgSave.Filter = "*.txt|*.txt";
			if (dlgSave.ShowDialog() != DialogResult.OK) return;
			try
			{
				IO.StreamWriter output = new System.IO.StreamWriter(dlgSave.FileName, false);
				IO.StreamReader r = new System.IO.StreamReader(new IO.FileStream(dlgOpen.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read), System.Text.Encoding.UTF8);
				string strLine = r.ReadLine();
				int intFile = 1;
				while (strLine != null)
				{ // lines with content in form <w ...>Content</w>
					if (strLine.StartsWith("<sentence"))
						output.WriteLine(); // end of a line of text
					else
					{
						Match objMatch = BlogContent.Match(strLine);
						if (objMatch.Success)
						{
							string strText = objMatch.Groups["content"].Value;
							if (strText != "")
								output.Write(objMatch.Groups["content"].Value + ' ');
							if (output.BaseStream.Position > 100000000 && strText == ".")
							{// split very large files into 100M maximum
								output.Dispose();
								intFile++;
								string strFile = dlgSave.FileName;
								strFile = IO.Path.GetDirectoryName(strFile) + IO.Path.DirectorySeparatorChar + IO.Path.GetFileNameWithoutExtension(strFile) + "-" + intFile.ToString() + IO.Path.GetExtension(strFile);
								output = new System.IO.StreamWriter(strFile, false);
								Debug.WriteLine("Switching to file: " + IO.Path.GetFileName(strFile));
							}
						}
					}
					strLine = r.ReadLine();
					//intLines++;
					//if (intLines > 100) break;
				}
				r.Close();
				output.Dispose();
				MessageBox.Show("Done");
			}
			catch (Exception ex)
			{ MessageBox.Show(ex.Message); }
		}

	} // form class
}