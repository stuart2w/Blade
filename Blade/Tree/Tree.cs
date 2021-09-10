using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
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

	public class Tree
	{
		internal Dictionary<int, RootNode> Entries;
		private RootNode TotalPredecessors; // total frequency for given predecessors, for all current words
											// can be nothing - see CorpusMode
		public string File = null; // not used internally; if the data is loaded or saved, it is remembered here for convenience
		private int m_intCount = 0; // total frequency (not = Entries.Count)
		public bool Changed = false; // true if anything has changed this since loading or saving
		internal int m_intRandom = 0; // a random value which is changed every time the data is saved.  Used to detect when the base data is changed
		internal int m_intBasedOn = 0; // used (only) in the prediction data, this is the random value from the base data upon which it is based
		internal int m_intUserRatio = 1; // multiplication factor for user frequencies; see AddUserData

		public Tree()
		{
			TotalPredecessors = new RootNode("TOTAL", RootNode.MULTI_SESSION);
			TotalPredecessors.Hash = Node.SPECIAL;
			Entries = new Dictionary<int, RootNode>();
			m_intRandom = (new Random()).Next();
		}

		#region Editing
		internal void AddAll(Token objToken, int intSessionHash)
		{ // if the given token is the last in a 'sentence' this adds it, but also calls Add for each previous item
		  // Debug.WriteLine("Adding: " + objToken.LineText());
			while (objToken != null && objToken.Hash != Node.START)
			{
				Add(objToken, intSessionHash);
				objToken = objToken.Previous;
			}
		}

		private void Add(Token objToken, int intSessionHash)
		{
			// note that m_intUserRatio will only != 1 if AddUserData was called.  i.e. if this is
			if (!Entries.ContainsKey(objToken.Hash))
			{ // this text wasn't previous stored
				Entries[objToken.Hash] = new RootNode(objToken.Text, intSessionHash);
				Entries[objToken.Hash].Expand(); // we always store at least one sub-level (ie bigram)
			}
			else
			{
				if (Entries[objToken.Hash].Count < 0)
				{ // this is a deletion marker, frequency is not genuine, and does not increase!
					return;
				}
				if (Entries[objToken.Hash].Text.IndexOf(objToken.Text, StringComparison.CurrentCultureIgnoreCase) != 0) // would use .CompareTo but this has no case insensitive?  Am I missing something?
					Debug.WriteLine("Hash collision between '" + Entries[objToken.Hash].Text + "' and '" + objToken.Text + "'");
				Entries[objToken.Hash].SeenInSession(intSessionHash);
			}
			m_intCount += m_intUserRatio;
			Unoptimised++; // note that this deliberately doesn't use the user frequency adjustment.  Therefore it is literally a count of the number of words seen
			Entries[objToken.Hash].Add(objToken, m_intUserRatio);
			if (TotalPredecessors != null)
				TotalPredecessors.Add(objToken, m_intUserRatio);
			Changed = true;
		}

		public const int USERADDCOUNT = 10; // if user adds, the frequency is increased by several - adding one makes a word largely unpredicted
											// (because Blade sees a lot of junk with frequency 1)
		public RootNode UserAdd(string strText, bool bolWasCapitalised)
		{ // used when user explicitly adds a word in the editor
		  // returns the entry for this word, which might be new or an existing one.
			Unoptimised++;
			m_intCount += USERADDCOUNT;
			int intHash = GetHash(strText);
			RootNode objEntry;
			if (Entries.ContainsKey(intHash))
			{ // exists - change freq
				objEntry = Entries[intHash];
			}
			else
			{
				objEntry = new RootNode(strText, RootNode.MULTI_SESSION);
				Entries.Add(intHash, objEntry);
			}
			objEntry.UserAddFrequency();
			if (bolWasCapitalised)
				objEntry.CapitalisedCount += USERADDCOUNT;
			Changed = true;
			return objEntry;
		}

		public void UserDelete(int intHash)
		{
			if (!Entries.ContainsKey(intHash))
				return;
			m_intCount -= Entries[intHash].Count;
			Unoptimised += 5; // deletions are more damaging; since they could appear as predecessors but is best to speed up the optimisation somewhat
			Entries.Remove(intHash);
			// use mechanism from RemoveLowWords in optimisation to remove this as a predecessor
			Dictionary<int, int> colRemove = new Dictionary<int, int>();
			colRemove.Add(intHash, intHash);
			foreach (RootNode objEntry in Entries.Values)
				objEntry.RemoveLowWords(colRemove);
			Changed = true;
		}

		public void Clear(bool bolKeepStructure)
		{ // if bolKeepStructure then all counts are zeroed, but entries kept
			m_intCount = 0;
			Unoptimised = 0;
			if (TotalPredecessors != null)
				TotalPredecessors.Clear(bolKeepStructure);
			if (bolKeepStructure)
			{
				foreach (RootNode objEntry in Entries.Values)
					objEntry.Clear(true);
			}
			else
				Entries.Clear();
			Changed = false;
		}

		public Tree Clone()
		{
			Tree objNew = new Tree();
			objNew.m_intCount = m_intCount;
			objNew.TotalPredecessors = TotalPredecessors;
			objNew.Entries = new Dictionary<int, RootNode>();
			objNew.Unoptimised = Unoptimised;
			foreach (RootNode objEntry in Entries.Values)
				objNew.Entries.Add(objEntry.Hash, (RootNode)objEntry.Clone());
			return objNew;
		}

		public void AddUserData(Tree objUser)
		{ // if this object is the base data, this adds the user data specified in the parameter
		  // because the user data is potentially much smaller than the base data the frequency scores from the user data can be boosted
		  // to give the user data a realistic impact
		  // the simplest possibility would be to multiply the user frequencies by the ratio of the size of the datasets.  This would give
		  // each an equal impact on the result.   e.g. if the base data is 25M words and the user data 1M words, multiplying the user data frequencies by 25 would make them comparable
		  // In fact I modify this in a couple of ways:
		  // firstly the user data is assumed to have a minimum total frequency of 10000.  Therefore if the user data contains only one word
		  // it isn't assumed to represent 100% of the words that the user types and multiplied up by some huge number
		  // min was 1000, but that gave very large ratios for small user samples (so one occurrence borderline for getting into data; we then decreased user ratio which left the user needing 20+ occurrences once sample was larger)
			int intUser = Math.Max(objUser.TotalFrequency, 10000);
			// m_intUserRatio is kept as an integer because the frequency data is integral anyway (and when learning as we go the frequency is increased in ones normally)
			// it is stored as a member so that words learnt as we go along are also modified
			if (TotalFrequency / intUser < 8)
				// the user data frequency is never reduced, if the user data is larger than the base data.  
				// and the ratio is less than 4 the alternative below would be rounded to 1 anyway
				m_intUserRatio = 1;
			else
			{
				// and finally I think a direct adjustment is still somewhat excessive, therefore I take the square root of this
				m_intUserRatio = (int)(Math.Sqrt(TotalFrequency / intUser) / 2);
				// and later - I've divided by another 2.  System data is in the region of 20M frequency for English.
				// for the min 1000 user freq this still gives *70
			}
			Debug.WriteLine("AddUserData: User ratio = " + m_intUserRatio.ToString());
			CorpusMode = false;
			TotalPredecessors.Add(objUser.TotalPredecessors, m_intUserRatio);
			m_intCount += objUser.m_intCount * m_intUserRatio;
			Unoptimised += objUser.m_intCount * m_intUserRatio;
			foreach (RootNode objEntry in objUser.Entries.Values)
			{
				if (Entries.ContainsKey(objEntry.Hash))
					Entries[objEntry.Hash].Add(objEntry, m_intUserRatio);
				else
				{
					RootNode objNew = (RootNode)objEntry.Clone();
					// reduce the user boost for completely unknown words, especially if they are fairly low freq
					// removed again now I've increased minimum user size to 10k
					// objNew.MultiplyFrequency(m_intUserRatio > 4 ? m_intUserRatio / 4 : 1);
					objNew.MultiplyFrequency(m_intUserRatio);
					Entries.Add(objEntry.Hash, objNew);
				}
			}
			Changed = true;
		}

		/// <summary>Returns the entry for the given hash, or null if not found</summary>
		public RootNode Find(int intHash)
		{
			if (!Entries.ContainsKey(intHash))
				return null;
			return Entries[intHash];
		}

		#endregion

		#region Analysis / Learning
		internal static CharType GetCharType(char c)
		{
			if (Char.IsLetter(c)) return CharType.Letter;
			if (Char.IsDigit(c)) return CharType.Numeral;
			if (Char.IsWhiteSpace(c)) return CharType.WhiteSpace;
			return CharType.Punct;
		}

		internal static Token SplitWords(string strLine)
		{// cuts up text into a series of SeenToken, returning the last one
			StringBuilder strWord = new StringBuilder(); // current 'word' - actually any sequence of characters of same type
			bool bolWhitespace = false; // whether there was whitespace before current item
			CharType eCurrent = CharType.None; // type of current content in strWord
			Token objToken = null; //SeenToken.Start; 
			int intWordStart = 0; // index at which the CURRENTFILEVERSION word started
			for (int i = 0; i < strLine.Length; i++)// 
			{
				char c = strLine[i];
				if (c == (char)8217) // 8217 is one of the modified ' chars - quite a few in base text
					c = '\'';
				CharType eType = GetCharType(c);
				if (eType != eCurrent)
				{ // change of type.  Store current 'word'
					if (strWord.Length > 0 && eCurrent != CharType.WhiteSpace)
						objToken = new Token(strWord.ToString(), bolWhitespace, objToken, eCurrent, intWordStart);
					bolWhitespace = (eCurrent == CharType.WhiteSpace);
					eCurrent = eType;
					strWord.Length = 0;
					intWordStart = i;
				}
				strWord.Append(c); // store this char whether new word or continuing old
			}
			// store final stuff (if any)
			if (strWord.Length > 0 && eCurrent != CharType.WhiteSpace)
				objToken = (new Token(strWord.ToString(), bolWhitespace, objToken, eCurrent, intWordStart));
			if (objToken != null)
				objToken.MergeCompoundWords(); // will iterate back thru list
			return objToken;
		}

		public void AnalyseLine(string strLine, int intSessionHash)
		{
			Token objToken = SplitWords(strLine);
			// split by . and add to data
			while (objToken != null)
			{ // ensure we don't start with disallowed text
				while (objToken != null && objToken.TextIsDisallowed)
					objToken = objToken.Previous;
				Token objNextSentence = objToken.SplitSentence();
				// next sentence removed from objToken, which is updated to have START at end of chain (ie beginning of text)
				AddAll(objToken, intSessionHash);
				objToken = objNextSentence;
			}
		}

		public bool CorpusMode
		{// when scanning a large corpus the TotalPredecessors can add quite a bit to the overhead especially in terms of memory
		 // so this can be switched off at least temporarily
			get { return TotalPredecessors == null; }
			set
			{
				if (value)
					TotalPredecessors = null;
				else
					RebuildTotalPredecessors();
			}
		}
		#endregion

		public string GetText(int intHash)
		{ // looks up the text based on the hash code
			if (intHash == Node.OTHER)
				return "OTHER";
			else if (intHash == Node.UNKNOWN)
				return "UNKNOWN";
			else if (intHash == Node.START)
				return "START";
			else if (Entries.ContainsKey(intHash))
				return Entries[intHash].Text;
			else
				return "MISSING:" + intHash.ToString("x");
		}

		public static int GetHash(string strText)
		{ // I started just using strText.GetHashCode(), but this has collisions in Swedish, especially when there are accents and 2 characters
		  // separated by one matching character are both changed.
			int intHash = strText.GetHashCode();
			intHash ^= strText.IndexOf('å'); // it's mostly the accents which are the problem - especially accented char matching unaccented if one other char changes
			intHash ^= strText.IndexOf('ö');
			intHash ^= strText.IndexOf('\''); // but also the single quote sometimes
			return intHash;
		}

		public List<RootNode> GetAllWords()
		{ // returns all words, in alphabetical order
			List<RootNode> colWords = new List<RootNode>();
			foreach (RootNode e in Entries.Values)
			{
				if (e.TokenType == CharType.Letter) // this fn mostly used for user editing.  We pretend we only do actual words
					colWords.Add(e);
			}
			colWords.Sort();
			return colWords;
		}

		#region Info
		public int TotalFrequency
		{ get { return m_intCount; } }

		public int CountNodes()
		{
			int intTotal = 0;
			foreach (RootNode objEntry in Entries.Values)
				intTotal += objEntry.CountNodes();
			return intTotal;
		}

		public int CountHeadWords
		{ get { return Entries.Count; } }

		public int GreatestDepth()
		{ // returns greatest depth of nested nodes from here. 1 if just self;  2 if bi-gram etc...
			int intDeepest = 0;
			foreach (RootNode objNode in Entries.Values)
				intDeepest = Math.Max(intDeepest, objNode.GreatestDepth());
			return intDeepest;
		}

		public string GreatestDepthText()
		{ // returns greatest depth of nested nodes from here. 1 if just self;  2 if bi-gram etc...
			int intDeepest = -1;
			string strDeepestText = "";
			foreach (RootNode objNode in Entries.Values)
			{
				int intDepth = objNode.GreatestDepth();
				if (intDepth > intDeepest)
				{
					intDeepest = intDepth;
					strDeepestText = objNode.GreatestDepthText(this);
				}
			}
			return strDeepestText;
		}

		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public string StatText()
		{
			return "Total F = " + TotalFrequency.ToString("#,0") + "; Head words = " + Entries.Count.ToString() + ";  Nodes = " + CountNodes().ToString("#,0");
		}

		#endregion

		#region IO
		public void Write(IO.StreamWriter output)
		{ // dumps data in text form (cannot be loaded back - for diagnostics only)
			output.WriteLine("Total frequency: " + m_intCount.ToString());
			output.WriteLine("Total lines: " + CountNodes().ToString());
			output.WriteLine("Head entries: " + Entries.Count.ToString());
			List<RootNode> colSorted = new List<RootNode>(Entries.Values);
			colSorted.Sort();
			foreach (RootNode objEntry in colSorted)
				objEntry.Write(output, this);
			Changed = false;
		}

		public const int CURRENTFILEVERSION = 3; // 3 same as 2, but triggered some cleaning
		public void Write(IO.BinaryWriter output)
		{
			output.Write(CURRENTFILEVERSION);
			m_intRandom = (new Random()).Next();
			output.Write(m_intRandom);
			output.Write(m_intBasedOn);
			output.Write(GetHash("abcd")); // a check that GetHashCode is compatible between machines (it can differ!)
			output.Write(m_intCount);
			output.Write(Unoptimised);
			output.Write(Entries.Count);
			foreach (RootNode objEntry in Entries.Values)
			{
				objEntry.Write(output);
			}
			CorpusMode = false;
			TotalPredecessors.Write(output);
			Changed = false;
		}

		public void Write(string strFile, bool bolBinary)
		{
			Debug.WriteLine("Writing Blade file: " + strFile);
			File = strFile;
			if (bolBinary)
			{
				using (IO.BinaryWriter output = new IO.BinaryWriter(new IO.FileStream(strFile, IO.FileMode.Create, IO.FileAccess.Write), Encoding.Default))
				{ Write(output); }
			}
			else
			{
				using (IO.StreamWriter output = new IO.StreamWriter(strFile, false, Encoding.Default))
				{ Write(output); }
			}
		}

		public static Tree Read(IO.BinaryReader input)
		{
			int intVersion = input.ReadInt32();
			Tree objTree = new Tree();
			objTree.m_intRandom = input.ReadInt32(); // this is maintained when loading the file; it only changes if the file is saved again
			objTree.m_intBasedOn = input.ReadInt32();
			int intTest = input.ReadInt32();
			objTree.m_intCount = input.ReadInt32();
			objTree.Unoptimised = input.ReadInt32();
			int intEntries = input.ReadInt32();
			for (int i = 0; i < intEntries; i++)
			{
				RootNode objNew = new RootNode(input, intVersion);
				if (intVersion < 3)
				{
					// strips out some garbage picked up due to bugs in earlier versions
					if (char.IsLower(objNew.Text[0]) && objNew.Text.ToLower() != objNew.Text)
						continue;
					if (objNew.Text.Contains("start--start"))
						continue;
					if (objNew.Text.Length > 20)
						continue;
				}
				objTree.Entries.Add(objNew.Hash, objNew);
			}
			objTree.TotalPredecessors = new RootNode(input, intVersion);
			if (intVersion < 3)
				objTree.Unoptimised = 100000;

			if (intTest != GetHash("abcd"))
			{
				// indicates hash code not compatible - must update all codes.  Actually SAW always runs 32-bit, so it will mostly
				// be consistent
				objTree.UpdateHashCodes();
			}
			objTree.Changed = false;
			return objTree;
		}

		private void UpdateHashCodes()
		{ // called by binary read
			Debug.WriteLine("Recalculating hash codes in prediction tree...");
			int intStart = Environment.TickCount;
			Dictionary<int, int> colMapping = new Dictionary<int, int>(); // key is old hash, value is new hash
			foreach (RootNode objEntry in Entries.Values)
				colMapping.Add(objEntry.Hash, GetHash(objEntry.Text));
			Dictionary<int, RootNode> colNew = new Dictionary<int, RootNode>(); // new list with updated keys
			foreach (RootNode objEntry in Entries.Values)
			{
				objEntry.MapHashCodes(colMapping);// objEntry.Hash has now been updated
				if (!colNew.ContainsKey(objEntry.Hash))
					colNew.Add(objEntry.Hash, objEntry);
				else
				{ // another item now has the same hash code; add the frequencies
					colNew[objEntry.Hash].Add(objEntry, 1);
					//Debug.WriteLine("Collision at root node mapping '" + objEntry.Text + "' from " + intOld.ToString() + " to " + objEntry.Hash.ToString());
				}
			}
			Entries = colNew;
			if (TotalPredecessors != null)
				TotalPredecessors.MapHashCodes(colMapping);
			Debug.WriteLine("Hash recalculate took " + ((float)(Environment.TickCount - intStart) / 1000).ToString("0.00") + "s");
			// I wanted to check this wasn't so slow that the user needed to be notified.  It took 1-2seconds for 80k headword/2M node dataset, which seems OK
		}

		public static Tree Read(string strFile)
		{
			try
			{
				Debug.WriteLine("Reading Blade file: " + strFile);
				using (IO.BinaryReader input = new IO.BinaryReader(new IO.FileStream(strFile, IO.FileMode.Open, IO.FileAccess.Read), Encoding.Default))
				{
					Tree objNew = Read(input);
					if (objNew != null)
						objNew.File = strFile;
					return objNew;
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to load blade file {strFile}, with error: {ex.Message}");
			}
		}

		public static int ReadRandom(string strFile)
		{ // reads the m_intRandom value from a saved file without loading the entire file
			using (IO.BinaryReader input = new IO.BinaryReader(new IO.FileStream(strFile, IO.FileMode.Open, IO.FileAccess.Read), Encoding.Default))
			{
				int intVersion = input.ReadInt32();
				if (intVersion > CURRENTFILEVERSION) throw new Exception("Cannot read Blade file: it was created by a newer version of the software");
				return input.ReadInt32(); // the random value from this file
			}
		}

		#endregion

		#region Optimising
		// TotalPredecessors is corrupted by individual functions.  Because it's structure doesn't exactly match the sum
		// of the individual entries anyway (it is usually more subdivided), when an individual entry is removed it's not always
		// possible to update TotalPredecessors - at some level the removed entry is probably UNKNOWN, but was detailed in TotalPredecessors
		public int Unoptimised = 0; // additions since last optimisation

		public void Optimise(int intMaxWords)
		{ // param is maximum number of top-level words which will be retained.
			if (Unoptimised == 0 && (intMaxWords * 1.3 > TotalFrequency || intMaxWords == 0)) // tolerances given on the maximum words because the optimisation does not reduce to exactly this number
				return;
			RemoveLowWords(intMaxWords);
			RemoveLowEntries();
			RebuildTotalPredecessors();
			RemovePoorPredictors(); // this doesn't change totals (so requires no changes to TotalPredecessors), but does want TotalPredecessors to be correct on entry
									// check Count is correct (not totally confident all the modifiers, especially when multiplying freq. to merge user list works perfectly)
			int intCount = 0;
			foreach (RootNode objEntry in Entries.Values)
				intCount += objEntry.Count;
			m_intCount = intCount;
			Unoptimised = 0;
			Changed = false;
		}

		// code to remove low frequency stuff to give a more efficient structure
		internal const int MINIMUM_WORD_FREQUENCY_INV = 1000000; // one per 1M - although this is not used literally (see RemoveLowWords)
		internal const float MINIMUM_ENTRY_FREQUENCY = 1.0F / 25000000; // one per 25M
		internal const float FRACTION_OF_TOTAL_ASSUMED_SUFFIFIENT = (float)0.1; // used by RemovePoorPredictors
																				// if a word already forms this much of the total it is assumed it will zero-letter predict and no further history/refinement is needed
		private void RemoveLowWords(int intMaxWords)
		{ // looks at words whose absolute frequency is low and removes them utterly - both at top level and where they occur as predecessors
		  // the decision on the minimum frequency is made by both looking at absolute frequencies, and intMaxWords - whichever gives the highest freq
		  // (ie keeps fewest words)

			Debug.WriteLine("Start RemoveLowWords. " + StatText());
			// first decide threshold frequency - initially by frequency
			int intMinimum = 2;
			float sngRatio = (float)TotalFrequency / MINIMUM_WORD_FREQUENCY_INV; // would be literal intMinimum frequency to maintain exact 1-per-1-million words or better
			if (sngRatio > 6)
				intMinimum = (int)(Math.Sqrt(sngRatio));
			// the minimum required word frequency only increases as sqrt of vocab
			if (TotalFrequency < 1000000) intMinimum = 1; // for v small word base keep everything so that data can grow
														  // and also check if we need to reduce to limit words to intMaxWords
			if (intMaxWords > 1 && CountHeadWords > intMaxWords)
			{
				List<RootNode> colTemp = new List<RootNode>();
				colTemp.AddRange(Entries.Values);
				colTemp.Sort(new RootNode.FrequencySortRoot());
				intMinimum = Math.Max(intMinimum, colTemp[intMaxWords].Count);
			}

			Debug.WriteLine("Minimum frequency = " + intMinimum.ToString());

			// second build list of codes to remove
			Dictionary<int, int> colRemove = new Dictionary<int, int>(); // key and value are the same
			Dictionary<int, RootNode> colReducedList = new Dictionary<int, RootNode>(); // the new main list after these removed (can't update Entries while enumerating it)
			foreach (RootNode objEntry in Entries.Values)
			{
				if ((objEntry.Count < intMinimum || objEntry.SourceSession != RootNode.MULTI_SESSION
					|| (Node.SpecialHash(objEntry.Hash) == false && GetCharType(objEntry.Text[0]) == CharType.Punct))
					&& objEntry.Count >= 0)
				{// second condition filters out anything except text at current position - we don't predict punctuation, but do take account of it in the previous words
				 // technician on third line stops us deleting markers in the user list showing system words which should be ignored
					colRemove.Add(objEntry.Hash, objEntry.Hash);
					m_intCount -= objEntry.Count;
				}
				else
					colReducedList.Add(objEntry.Hash, objEntry);
			}
			Entries = colReducedList;

			// now need to notify remaining headwords to remove these low words if they were stored as predecessors (necessary because text no longer obtainable for them)
			foreach (RootNode objEntry in Entries.Values)
				objEntry.RemoveLowWords(colRemove);
			Debug.WriteLine("End RemoveLowWords. " + StatText());
		}

		private void RemoveLowEntries()
		{ // looks at individual entries in tree with very low frequency.  Especially if freq = 1 there's no knowing whether it is a pattern or a freak
		  // therefore the previous word data for these is converted to OTHER or UNKNOWN
			Debug.WriteLine("Start RemoveLowEntries. " + StatText());
			int intMinimum = (int)Math.Max(2, TotalFrequency * MINIMUM_ENTRY_FREQUENCY); // minimum frequency to keep a word
			Debug.WriteLine("Minimum frequency = " + intMinimum.ToString());
			foreach (RootNode objEntry in Entries.Values)
			{
				objEntry.RemoveLowEntries(intMinimum);
			}
			Debug.WriteLine("End RemoveLowEntries. " + StatText());
		}

		private void RemovePoorPredictors()
		{ // looks at individual entries in tree and checks if they form almost all of TotalPredictions
		  // if so the prediction is known;  and there is no need to keep any further history of tokens
			Debug.WriteLine("Start RemovePoorPredictors. " + StatText());
			foreach (RootNode objEntry in Entries.Values)
			{
				objEntry.RemovePoorPredictors(TotalPredecessors);
			}
			Debug.WriteLine("End RemovePoorPredictors. " + StatText());
		}

		public void LimitDepth(int intMaximum)
		{ // intMaximum is a number of levels to keep - must be 1+
		  // 1 keeps just the top-level words
		  // 2 keeps bigrams, etc
		  // this function NOT used by Optimise, but can be called externally
			Debug.Assert(intMaximum >= 1);
			foreach (RootNode objEntry in Entries.Values)
				objEntry.LimitDepth(intMaximum - 1);
			if (TotalPredecessors != null)
				TotalPredecessors.LimitDepth(intMaximum);
		}

		private void RebuildTotalPredecessors()
		{ // rebuilds the TotalPredecessors from scratch based on the data in Entries
			if (TotalPredecessors != null)
				Debug.WriteLine("Start RebuildTotalPredecessors; TP nodes = " + TotalPredecessors.CountNodes().ToString());
			TotalPredecessors = new RootNode("TOTAL", RootNode.MULTI_SESSION);
			TotalPredecessors.Hash = Node.SPECIAL;
			foreach (RootNode objEntry in Entries.Values)
			{
				if (objEntry.Count >= 0) // ignore explicit deletes
					TotalPredecessors.Add(objEntry, 1);
			}
			Debug.WriteLine("End RebuildTotalPredecessors; TP nodes = " + TotalPredecessors.CountNodes().ToString());
		}

		#endregion

		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		internal List<Prediction> Predict(Token objPrevious, string strPartialWord, int intPredictions,
						Recency objRecency, List<int> colIgnore, int intMaxPreviousToUse, int intMinChars, out Frequency objTotalFrequency)
		{// you should usually use Engine.Predict to generate predictions
		 // objPrevious is the words leading up to this
		 // strPartialWord is the text of the current word typed so far, can be ""
		 // colIgnore is words which should be ignored - or null
		 // objRecency may be null
		 // intMaxPreviousToUse is maximum number of previous words (including start tokens) which can be considered.  Use >=10 for default.
		 // intMinChars is minimum char length to consider for words.  Shorter words are ignored
		 // 0 will use raw frequency
			Dictionary<int, Frequency> Frequencies = new Dictionary<int, Frequency>(); // frequency for each item of text
			objTotalFrequency = new Frequency(); // Frequency
												 // first pass calculates and stores frequency sets for each word, and the total frequency
			if (intMaxPreviousToUse < 0) intMaxPreviousToUse = 0;
			string strPartialLower = strPartialWord.ToLower();
			foreach (RootNode objEntry in Entries.Values)
			{
				if (objEntry.Lower.StartsWith(strPartialLower) && objEntry.Text.Length >= intMinChars) // condition works even if strPartialWord == ""
				{
					Frequency objFrequency = new Frequency();
					objEntry.CalculateFrequency(0, objPrevious, objFrequency, objTotalFrequency, intMaxPreviousToUse);
					Frequencies.Add(objEntry.Hash, objFrequency);
				}
			}

			List<Prediction> colResults = new List<Prediction>(intPredictions); // is kept sorted by frequency
			float sngLowest = float.PositiveInfinity; // lowest probability in list (forces first match onto end of list)
			foreach (RootNode objEntry in Entries.Values)
			{
				if (objEntry.TokenType == CharType.Letter && // usually redundant unless strPartial == ""
					  objEntry.Lower.StartsWith(strPartialLower) && // condition works even if strPartialWord == ""
					  objEntry.Text.Length >= intMinChars &&
					  objEntry.Count > 0 && // if < 0 then this is just a deletion marker, and should not be predicted
					  (colIgnore == null || !colIgnore.Contains(objEntry.Hash)))
				{
					Debug.Assert(Frequencies.ContainsKey(objEntry.Hash));
					Frequency f = Frequencies[objEntry.Hash];
					//Debug.WriteLine("Matches: " + GetText(objEntry.Hash));
					if (objRecency != null)
					{
						float sngRecency = objRecency.ProbabilityOf(objEntry.Hash);
						float sngSimpleProbability = (float)objEntry.Count / TotalFrequency;
						f.AdjustForRecency(sngRecency, sngSimpleProbability);
					}
					float sngProbability = f.TotalProbability(objTotalFrequency);
					if (sngProbability > sngLowest)
					{
						int intInsertAt = colResults.Count - 1;
						while (intInsertAt > 0 && sngProbability > colResults[intInsertAt - 1].Probability)
							intInsertAt -= 1;
						if (intInsertAt == colResults.Count - 1)
						// no need to insert as such - faster to replace last element
						{
							if (colResults.Count >= intPredictions) // replace last
							{
								colResults[colResults.Count - 1] = new Prediction(sngProbability, objEntry.PredictionText, f);
								sngLowest = sngProbability; // also lowest has just changed
							}
							else// or if too few currently, add
								colResults.Insert(intInsertAt, new Prediction(sngProbability, objEntry.PredictionText, f));
						}
						else
						{
							if (colResults.Count >= intPredictions)
								colResults.RemoveAt(colResults.Count - 1); // remove last
							colResults.Insert(intInsertAt, new Prediction(sngProbability, objEntry.PredictionText, f));
							sngLowest = colResults[colResults.Count - 1].Probability;
						}
					}
					else if (colResults.Count < intPredictions) // need more predictions - add to end of list
					{
						colResults.Add(new Prediction(sngProbability, objEntry.PredictionText, f));
						sngLowest = sngProbability;
					}
				}
			}
			return colResults;
		}

	}

}
