using System;
using System.Collections.Generic;
using System.Diagnostics;

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
	public class RootNode : Node, IComparable<RootNode>
	{// classes at the top level (sort of 0-grams) also remember the text, not just the hash code
	 // this allows us to retrieve the text for any hash code
		public readonly string Text;
		public readonly string Lower; // the above, always in lower case (this is stored to enable faster comparison when predicting)
									  // Comparing text without case is much slower than a simple comparison
		public int SourceSession; // hash of session in which it was discovered.  
		public const int MULTI_SESSION = -1; // means seen in more than one session (therefore keep)
		public int CapitalisedCount; //if the text is lowercase, this is the number of times this word has been seen with a capital letter
									 // NOT at the beginning of a sentence.  It's the text contained capital letters this will be 0
		public readonly CharType TokenType; // not stored in data - calculated as loaded/created

		#region Constructors, IO etc extended for extra fields
		public RootNode(string strText, int intSessionHash)
			: base(strText)
		{
			Text = strText;
			Lower = Text.ToLower();
			SourceSession = intSessionHash;
			CapitalisedCount = 0;
			Debug.Assert(strText != "");
			TokenType = Tree.GetCharType(Text[0]);
		}

		public RootNode(System.IO.BinaryReader input, int intFileVersion)
			: base(input)
		{ // construct from binary file. Base constructor has loaded most of the data
			Text = input.ReadString();
			if (string.IsNullOrEmpty(Text))
				TokenType = CharType.None;
			else
				TokenType = Tree.GetCharType(Text[0]);
			Lower = Text.ToLower();
			SourceSession = input.ReadInt32();
			if (intFileVersion >= 2) CapitalisedCount = input.ReadInt32();
		}

		public override Node Clone()
		{
			RootNode objNew = new RootNode(Text, SourceSession);
			objNew.Count = Count;
			objNew.Hash = Hash; // doesn't always match Text
			objNew.CapitalisedCount = CapitalisedCount;

			if (Previous != null)
			{
				objNew.Previous = new Dictionary<int, Node>();
				foreach (Node objPrevious in Previous.Values)
					objNew.Previous.Add(objPrevious.Hash, objPrevious.Clone());
			}
			return objNew;
		}

		public override void Write(System.IO.BinaryWriter output)
		{
			base.Write(output);
			output.Write(Text); // need to remember this for root nodes - not way to recover the text otherwise
								// must be on end to allow constructor chaining when reading
			output.Write(SourceSession);
			output.Write(CapitalisedCount);
		}

		public void Write(System.IO.TextWriter output, Tree objTree)
		{ // equivalent of the text Write method on Node
			string strText = PredictionText;
			output.Write(strText);
			output.Write(" : ");
			output.Write(Count.ToString());
			if (CapitalisedCount > 0)
			{
				output.WriteLine("    Capped: " + CapitalisedCount.ToString() + "; Exact: " + ExactCaseCount().ToString());
			}
			else
				output.WriteLine("");
			if (Previous != null)
			{ // write sub-items
				foreach (int intKey in Previous.Keys)
				{
					if (intKey != UNKNOWN && intKey != OTHER) // these 2 written at end
						Previous[intKey].Write(output, strText, objTree);
				}
				if (Previous.ContainsKey(OTHER))
					Previous[OTHER].Write(output, strText, objTree);
				if (Previous.ContainsKey(UNKNOWN))
					Previous[UNKNOWN].Write(output, strText, objTree);
			}
		}

		#endregion

		public int CompareTo(RootNode other)
		{ return Text.CompareTo(other.Text); }

		internal void SeenInSession(int intSessionHash)
		{
			if (SourceSession == 0)
				SourceSession = intSessionHash;
			else if (SourceSession != intSessionHash)
				SourceSession = MULTI_SESSION;
		}

		internal override void Add(Token objToken, int intMultiplyFrequency)
		{
			base.Add(objToken, intMultiplyFrequency);
			if (objToken.WasCapitalised && objToken.Previous != null && objToken.Previous.Hash != START)
				CapitalisedCount += intMultiplyFrequency;
		}

		public override void Add(Node objOther, int intMultiplyFrequency)
		{
			base.Add(objOther, intMultiplyFrequency);
			if (Count < 0)
				CapitalisedCount = 0;
			else
				CapitalisedCount += (objOther as RootNode).CapitalisedCount * intMultiplyFrequency;
		}

		public override void MultiplyFrequency(int intMultiplier)
		{
			base.MultiplyFrequency(intMultiplier);
			CapitalisedCount *= intMultiplier;
		}

		internal void SetDeleted()
		{ // sets frequency to -1 indicating that the word has been expressly deleted by the user
			Count = -1;
			if (Previous != null) Previous.Clear();
			CapitalisedCount = 0;
		}

		internal void UserAddFrequency()
		{ // called when adding frequency due to user action - no previous available
			if (Count < 0)
				Count = 1; // undeleting a word
			else
				Count += Tree.USERADDCOUNT;
			SourceSession = MULTI_SESSION; // if user adds manually, word always accepted under multi session filter
			if (Previous != null)
			{
				AddOtherFreqency(Tree.USERADDCOUNT); // adds to OTHER / UNKNOWN
			}
		}

		public override void Clear(bool bolKeepStructure)
		{
			CapitalisedCount = 0;
			SourceSession = 0;
			base.Clear(bolKeepStructure);
		}

		public int ExactCaseCount()
		{
			int intExact = Count - CapitalisedCount;
			if (Previous != null && Previous.ContainsKey(START))
				intExact -= Previous[START].Count;
			return intExact;
		}

		public string PredictionText
		{
			get
			{
				if (CapitalisedCount <= 3 * ExactCaseCount() || Count < 0)
					return Text;
				if (Text.Length == 1) return Text.ToUpper();
				return char.ToUpper(Text[0]) + Text.Substring(1);
			}
		}

		#region Frequency sorter
		public class FrequencySortRoot : IComparer<RootNode>
		{ // there is a definition of this in Node, but it can't be used as a parameter to List<RootNode>.Sort - so as far as I know it needs to be repeated.  Ugh.
			public int Compare(RootNode x, RootNode y)
			{
				return -x.Count.CompareTo(y.Count);
			}
		}
		#endregion

		// approx memory used:
		// List<Node>: 40 bytes each
		// List<RootNode>: 106 bytes each (4 chars text)
		// Dictionary<int, RootNode>: 138 bytes each (4 chars text)
	}
}