using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Blade
{
	public partial class frmListEdit : Form
	{
		private Tree m_objData; // user word list - this is what is edited
		private Tree m_objCombined; // combined word list used for prediction
		private Tree m_objSystem; // system (installed) data
		private List<RootNode> m_colUser; // all words in the user vocab
		private List<RootNode> m_colDisplayed; // words in the order that they are displayed.
		private Modes m_eMode = Modes.All;
		private bool m_bolReinstated = false; // true if any words which were explicitly deleted in user list have been reinstated
		// (forces optimisation on exit)
		private Abbreviations m_objAbbreviations;
		private List<String> m_colAbbreviationsDisplayed;
		private bool m_bolFilling = false;
		private Engine m_objEngine; // only needed for Log command
		private Recency m_objRecency; // only used in diagnostics dump

		private enum Modes
		{
			All = 0, // values must match combo indices
			NoSystem,
			Deleted, // words which appear in the system list, but have been set to frequency -1 in the user list to force them to be omitted
			// essential that deleted words only appear in Deleted mode (due to btnAdd)
			Abbreviations
		}

		public frmListEdit(Engine objEngine, Tree objData, Tree objCombined, Tree objSystem, Abbreviations objAbbreviations, Recency objRecency)
		{ // recency is only used for diagnostics
			InitializeComponent();
			pnlAbbreviations.Location = txtWord.Location; // it can be kept out of position in the editor to make editing easier
			m_objRecency = objRecency;
			m_objEngine = objEngine;
			m_objAbbreviations = objAbbreviations;
			m_objData = objData;
			m_objSystem = objSystem;
			if (m_objSystem.Entries.Count == 0)
				MessageBox.Show("Warning!  System vocab was not loaded or is empty.");
			Debug.WriteLine("Combined data user ratio = " + m_objData.m_intUserRatio.ToString());
			m_objCombined = objCombined;
			m_colUser = m_objData.GetAllWords();
			m_eMode = Modes.NoSystem;
			cmbMode.SelectedIndex = 1; // event handler won't fill list as mode matches, so do explicitly...
			FillList();
			if (m_objCombined == null) // hide the in system column
				lstWords.Columns[1].Width = 0;
		}

		private void FillList()
		{
			m_colDisplayed = new List<RootNode>();
			pnlAbbreviations.Visible = (m_eMode == Modes.Abbreviations);
			lstWords.Visible = (m_eMode != Modes.Abbreviations);
			txtWord.Visible = (m_eMode != Modes.Abbreviations);
			if (m_eMode == Modes.Abbreviations)
			{
				FillAbbreviations();
				return;
			}
			lstWords.SuspendLayout();
			lstWords.Items.Clear();

			foreach (RootNode objEntry in m_colUser)
			{
				bool bolInclude = true;
				switch (m_eMode)
				{
					// essential that deleted words only appear in Deleted mode (due to btnAdd)
					case Modes.All: bolInclude = (objEntry.Count >= 0); // excludes words with -1 frequency indicating explicit deletion (even though the word is in the system list)
						break;
					case Modes.NoSystem:
						bolInclude = (!m_objSystem.Entries.ContainsKey(objEntry.Hash) && objEntry.Count >= 0);
						break;
					case Modes.Deleted:
						bolInclude = (objEntry.Count < 0);
						break;
					default: System.Diagnostics.Debug.WriteLine("Unexpected mode in frmListEdit::FillList");
						break;
				}
				if (bolInclude)
				{
					m_colDisplayed.Add(objEntry);
					lstWords.Items.Add(CreateLine(objEntry));
				}
			}
			lstWords.ResumeLayout();
			btnClear.Enabled = (lstWords.Items.Count > 0) && (m_eMode != Modes.Deleted);
		}

		private ListViewItem CreateLine(RootNode objEntry)
		{ // creates ListViewItem - shared between FillList and btnAdd
			ListViewItem objNew = new ListViewItem(objEntry.PredictionText);
			if (m_eMode == Modes.All && !m_objSystem.Entries.ContainsKey(objEntry.Hash))
				objNew.ForeColor = Color.Blue;
			if (objEntry.Count < 0) // word explicitly deleted from system list.  User frequency is nonsense
				objNew.SubItems.Add("");
			else
				objNew.SubItems.Add(objEntry.Count.ToString());
			return objNew;
		}

		private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			lblNotSystem.Visible = (cmbMode.SelectedIndex == (int)Modes.All);
			if ((int)m_eMode == cmbMode.SelectedIndex) return;
			m_eMode = (Modes)cmbMode.SelectedIndex;
			lblAbbreviation.Visible = (m_eMode == Modes.Abbreviations);
			FillList();
			EnableButtons();
		}

		private void EnableButtons()
		{
			// when showing the deletion list we require a Word to be selected in order to add it back
			if (m_eMode == Modes.Deleted)
				btnAdd.Enabled = (lstWords.SelectedIndices.Count > 0);
			else
				btnAdd.Enabled = (txtWord.Text.Length > 0);
			btnDelete.Enabled = lstWords.SelectedIndices.Count > 0 && (m_eMode != Modes.Deleted);
			txtWord.Enabled = (m_eMode != Modes.Deleted);
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{// in abbreviation note this can become "Change" instead
			if (m_eMode == Modes.Abbreviations)
			{ AddAbbreviation(); return; }

			// need to check capitalisation of what the user has typed (or selected)
			string strText = txtWord.Text; // may be converted to lower case
			if (strText.Contains(" "))
			{
				MessageBox.Show("Words cannot contain spaces");
				return;
			}
			m_objEngine.Log("User Add: " + strText);
			bool bolCapitalised = false; // true if single capital, and converted
			if (strText != strText.ToLower())
			{
				if (strText.Substring(1) == strText.Substring(1).ToLower())
				{// only cap at beginning
					strText = strText.ToLower();
					bolCapitalised = true;
				}
			}
			// the calls to UserAdd work OK even if it is an explicitly deleted word
			RootNode objEntry = m_objData.UserAdd(strText, bolCapitalised);
			m_objCombined.UserAdd(strText, bolCapitalised); // word will be added back, but with only frequency 1.  Optimisation on close required to get the system frequency back
			int intIndex = m_colDisplayed.IndexOf(objEntry);
			if (m_eMode == Modes.Deleted)
			{
				lstWords.Items.RemoveAt(intIndex);
				m_colDisplayed.RemoveAt(intIndex);
				m_bolReinstated = true;
			}
			else if (intIndex >= 0) // might be hidden if user has alternate view selected, or may be new word
			{
				lstWords.Items[intIndex].SubItems[1].Text = objEntry.Count.ToString();
				lstWords.Items[intIndex].Text = objEntry.PredictionText;
			}
			else
			{
				m_colDisplayed.Add(objEntry);
				m_colDisplayed.Sort();
				intIndex = m_colDisplayed.IndexOf(objEntry);
				lstWords.Items.Insert(intIndex, CreateLine(objEntry));
			}
			lstWords.SelectedItems.Clear();
			if (intIndex < lstWords.Items.Count) // condition required in Deleted mode
				lstWords.SelectedIndices.Add(intIndex);
			if (bolCapitalised && txtWord.Text.Length > 1)
			{ // selection will have replaced with lowercase version
				txtWord.Text = txtWord.Text.Substring(0, 1).ToUpper() + txtWord.Text.Substring(1);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (m_eMode == Modes.Abbreviations)
			{ DeleteAbbreviation(); return; }
			if (lstWords.SelectedIndices.Count == 0) return;
			int intSelected = lstWords.SelectedIndices[0];
			RootNode objEntry = m_colDisplayed[intSelected];
			m_objEngine.Log("User delete: " + lstWords.Items[intSelected]);
			if (m_objSystem.Entries.ContainsKey(objEntry.Hash))
			{ // word is in the system list; set its frequency to -1 in the user list indicating it should not be included
				objEntry.SetDeleted();
				m_objCombined.Entries[objEntry.Hash].SetDeleted();
				m_objCombined.Changed = true;
				m_objData.Changed = true;
			}
			else
			{
				m_objData.UserDelete(objEntry.Hash);
				m_objCombined.UserDelete(objEntry.Hash);
				m_colUser.Remove(objEntry); // although the word is in the user list it won't be displayed on screen
			}
			m_colDisplayed.Remove(objEntry);
			lstWords.Items.RemoveAt(intSelected); // will deselect.  Then select following line
			if (intSelected <= lstWords.Items.Count - 1)
				lstWords.SelectedIndices.Add(intSelected);
			lstWords.Focus();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			if (m_bolReinstated)
				m_objCombined.Unoptimised += 100000; // any large number - to trigger optimisation
			DialogResult = DialogResult.OK;
			Dispose();
		}

		#region Abbreviations

		private void FillAbbreviations()
		{
			txtAbbreviation.Text = "";
			txtExpansion.Text = "";
			m_colAbbreviationsDisplayed = m_objAbbreviations.GetSortedList();
			lstAbbreviations.SuspendLayout();
			lstAbbreviations.Items.Clear();
			foreach (string strAbbreviation in m_colAbbreviationsDisplayed)
			{
				ListViewItem objNew = lstAbbreviations.Items.Add(strAbbreviation.Replace(' ', '◊'));
				objNew.SubItems.Add(m_objAbbreviations[strAbbreviation].Replace(' ', '◊'));
			}
			lstAbbreviations.ResumeLayout();
		}

		private void txtAbbreviation_TextChanged(object sender, EventArgs e)
		{ // 
			if (txtAbbreviation.Text.Length == 0 || txtExpansion.Text.Length == 0 || txtExpansion.Text == txtAbbreviation.Text)
				btnAdd.Enabled = false;
			else
			{
				btnAdd.Enabled = true;
				if (m_objAbbreviations.ContainsKey(txtAbbreviation.Text))
					btnAdd.Text = "Change";
				else
					btnAdd.Text = "Add";
			}
			btnDelete.Enabled = (txtAbbreviation.Text.Length > 0 && m_objAbbreviations.ContainsKey(txtAbbreviation.Text));
		}

		private void AddAbbreviation()
		{
			if (m_objAbbreviations.ContainsKey(txtAbbreviation.Text))
			{ // changing an existing item
				int intIndex = m_colAbbreviationsDisplayed.IndexOf(txtAbbreviation.Text);
				if (intIndex < 0)
				{
					System.Diagnostics.Debug.Fail("List position not found when replacing abbreviation");
					return;
				}
				m_objAbbreviations[txtAbbreviation.Text] = txtExpansion.Text;
				lstAbbreviations.Items[intIndex].SubItems[1].Text = txtExpansion.Text.Replace(' ', '◊');
			}
			else
			{
				m_objAbbreviations.Add(txtAbbreviation.Text, txtExpansion.Text);
				m_colAbbreviationsDisplayed.Add(txtAbbreviation.Text);
				m_colAbbreviationsDisplayed.Sort();
				int intIndex = m_colAbbreviationsDisplayed.IndexOf(txtAbbreviation.Text);
				ListViewItem objNew = lstAbbreviations.Items.Insert(intIndex, txtAbbreviation.Text.Replace(' ', '◊'));
				objNew.SubItems.Add(txtExpansion.Text.Replace(' ', '◊'));
			}
		}

		private void DeleteAbbreviation()
		{ // strictly speaking this removes the text in the box at the top, not the one selected in the list
			if (!m_objAbbreviations.ContainsKey(txtAbbreviation.Text))
				return;
			int intIndex = m_colAbbreviationsDisplayed.IndexOf(txtAbbreviation.Text);
			m_objAbbreviations.Remove(txtAbbreviation.Text);
			if (intIndex >= 0)
			{
				m_colAbbreviationsDisplayed.RemoveAt(intIndex);
				lstAbbreviations.Items.RemoveAt(intIndex);
				if (intIndex < lstAbbreviations.Items.Count)
					lstAbbreviations.SelectedIndices.Add(intIndex);
			}
		}

		private void lstAbbreviations_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstAbbreviations.SelectedIndices.Count > 0)
			{
				txtAbbreviation.Text = lstAbbreviations.SelectedItems[0].Text.Replace('◊', ' ');
				txtExpansion.Text = lstAbbreviations.SelectedItems[0].SubItems[1].Text.Replace('◊', ' ');
			}
		}

		#endregion

		#region Minor events
		private void txtWord_TextChanged(object sender, EventArgs e)
		{
			if (m_bolFilling) return; // event was triggered by update within this code.  The code below is not necessary
			EnableButtons();
			if (m_colDisplayed == null || m_colDisplayed.Count == 0 || txtWord.Text.Length == 0) return;
			int intIndex = 0;
			while (intIndex < m_colDisplayed.Count - 1 && string.Compare(txtWord.Text, m_colDisplayed[intIndex].Text, true) > 0)
				intIndex++;
			m_bolFilling = true;
			lstWords.SelectedIndices.Clear();
			lstWords.SelectedIndices.Add(intIndex);
			lstWords.EnsureVisible(intIndex);
			m_bolFilling = false;
		}

		private void lstWords_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_bolFilling) return; // event was triggered by update within this code.  The code below is not necessary
			EnableButtons();
			m_bolFilling = true;
			if (lstWords.SelectedIndices.Count > 0)
				txtWord.Text = lstWords.SelectedItems[0].Text;
			m_bolFilling = false;
		}

		private void lstWords_Resize(object sender, EventArgs e)
		{
			lstWords.Columns[0].Width = lstWords.Width - 110;
		}

		private void lstWords_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				btnDelete_Click(sender, null);
		}

		#endregion

		private void btnClear_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this entire list?  This cannot be reversed.", "Blade", MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			switch (m_eMode)
			{
				case Modes.Abbreviations:
					m_objAbbreviations.Clear();
					break;
				case Modes.All:// need to delete user words; but they might be in combined either as user only, or also have come from system
					// close and force re-optimise
					m_objData.Clear(false);
					m_objCombined.Unoptimised += 1000000; // any large number - to trigger optimisation
					MessageBox.Show("All user data removed.  This screen will close, in order to rebuild from the system vocab.  If you want to add words back manually, please re-open this editing screen.");
					Dispose();
					break;
				case Modes.NoSystem:
					// delete only words not in system
					List<int> colDelete = new List<int>();
					foreach (RootNode objEntry in m_objData.Entries.Values)
					{
						if (!m_objSystem.Entries.ContainsKey(objEntry.Hash))
						{ // occurs in only from user list
							colDelete.Add(objEntry.Hash);
						}
					}
					foreach (int intHash in colDelete)
					{
						if (m_objCombined.Entries.ContainsKey(intHash))
							m_objCombined.UserDelete(intHash);
						m_objData.UserDelete(intHash); // no need to check Contains as only this implicit from when colDelete generated above 
					}
					m_objCombined.Unoptimised += 1000000; // any large number - to trigger optimisation
					break;
				default: System.Diagnostics.Debug.WriteLine("Unexpected mode in btnClear_Click");
					break;
			}
			FillList();
			txtAbbreviation_TextChanged(this, null);
		}

		private void btnDiagnostics_Click(object sender, EventArgs e)
		{
			if (dlgSave.ShowDialog() != DialogResult.OK)
				return;
			using (System.IO.StreamWriter output = new System.IO.StreamWriter(dlgSave.FileName))
			{
				output.WriteLine("Word,User,System,Combined,Recency");
				List<RootNode> colWords = m_objCombined.GetAllWords();
				foreach (RootNode entry in colWords)
				{
					output.Write(entry.Text);
					output.Write(",");
					RootNode objUser = m_objData.Find(entry.Hash);
					if (objUser == null)
						output.Write("--");
					else if (objUser.Count < 0)
						output.Write("DELETE");
					else
						output.Write(objUser.Count);
					output.Write(",");
					RootNode objSystem = m_objSystem.Find(entry.Hash);
					if (objSystem == null)
						output.Write("--");
					else
						output.Write(objSystem.Count);
					output.Write(",");
					if (entry.Count < 0)
						output.Write("--");
					else
						output.Write(entry.Count);
					output.Write(",");
					float sngFrequency = m_objRecency.RawFrequencyOf(entry.Hash);
					if (sngFrequency <= 0)
						output.Write("--");
					else
						output.Write(sngFrequency.ToString("0.##"));
					output.WriteLine();
				}
			}
		}

	}
}