using System;
using System.Collections.Generic;
using System.Text;
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
{ // This class is not usually required externally, it is public for the benefit of Blade.Edit

	public enum CharType
	{
		None = -1,
		Letter = 0,
		Punct,
		WhiteSpace,
		Numeral
	}

	internal class Token
	{ // stores a 'word', and its hash code when processing the incoming text
		public string Text;
		public int Hash;
		internal bool Whitespace; // although it's not used at the moment remembering whitespace would allow
		// for some more pre-processing of tokens such as recognising hyphenated groups
		public Token Previous;
		internal CharType CharacterType;
		internal bool WasCapitalised; // true if the Text has been converted to lower case, but originally had a capital letter
		// remains false is Text still contains capital letters (anything with more than one capital is assumed not to be due to the beginning of a sentence
		// and is intrinsic (e.g. BBC))
		private bool AllCaps = false; // true if word is all capitals (Text will still be all capitals initially)
		// used by CheckAllCaps
		public int StartedAt; // index into the original string at which this word started

		internal Token(string strText, bool bolWhitespace, Token objPrevious, CharType type, int startedAt)
		{
			// numbers are grouped - we don't want to clutter data with masses of numbers
			StartedAt = startedAt;
			if (type == CharType.Numeral)
			{
				int value = 0;
				if (int.TryParse(strText, out value))
				{ // changes number to one of 1,2,10,100,1000,10000
					if (value >= 10000 || value < 0)
						value = 10000; // this value is also used for all decimal groups - see MergeCompoundWords
					// it is useful to keep 10000 as well as 1000, because 1000 will include all years
					else if (value >= 1000)
						value = 1000;
					else if (value >= 100)
						value = 100;
					else if (value >= 10)
						value = 10;
					else if (value != 1)
						value = 2; // will use plural nouns
					else
						value = 1;
				}
				strText = value.ToString();
			}
			string strLower = strText.ToLower();
			if (strLower != strText)
			{ // need to see there are any capital letters other than the first letter
				// I think this won't correctly cope with some odd situations where first letter lower - eg all remaining upper will be treated as all upper
				// but this is (a) rare, and (b) probably not such a bad way of handling it anyway
				bool capital = false;
				bool lower = false;
				for (int index = 1; index < strText.Length; index++)
					if (char.IsUpper(strText[index]))
						capital = true;
					else if (char.IsLower(strText[index]))
						lower = true;
				if (!capital)
				{ // only the first letter is a capital letter
					WasCapitalised = true;
					strText = strLower;
				} // otherwise we keep the text as it is - multiple capitals are assumed to be intrinsic (BBC or III)
				else if (!lower)
				{ // if entirely in upper case, check previous words
					AllCaps = true;
				}
			}
			Text = strText;
			Hash = Tree.GetHash(strText);
			Whitespace = bolWhitespace;
			Previous = objPrevious;
			CharacterType = type;
		}

		public static Token Start;

		// returns true if text will clash with anything used in saving to a text file and must be omitted
		public bool TextIsDisallowed => Text == "<<";

		public string LineText()
		{// returns text for this and all previous
			if (this == Token.Start) return "";
			if (Previous == null) return Text;
			return Previous.LineText() + (Whitespace ? " " : "") + OriginalText;
		}

		public string OriginalText
		{ // returns the text, with a capital letter, or all caps if that's how it was originally detected.
			get
			{
				if (AllCaps) return Text.ToUpper();
				if (!WasCapitalised) return Text;
				if (Text.Length == 1) return Text.ToUpper();
				return char.ToUpper(Text[0]) + Text.Substring(1);
			}
		}

		public Token SplitSentence()
		{// updates self to stop at first . and returns remaining tokens if any
			// ensures this sequence starts with START
			Debug.Assert(!TextIsDisallowed);
			Token token = this;
			Token next = null; // the token we have come from
			while (token != null)
			{
				if (token.TextIsDisallowed)
				{
					Debug.Assert(next != null);
					next.Previous = token.Previous;
				}
				else if (token.Text == "." && next != null)
				{ // sentence break - send all this group for analysis (unless empty)
					next.Previous = Token.Start;
					if (CheckAllCaps() >= CONVERT_ALL_CAPS_THRESHOLD) ConvertAllCaps();
					return token; // previous sentence if everything . and before, which has now been removed from current
				}
				next = token;
				token = token.Previous;
			}
			next.Previous = Token.Start;
			// no previous sentence
			if (CheckAllCaps() >= CONVERT_ALL_CAPS_THRESHOLD) ConvertAllCaps();
			return null;
		}

		public void MergeCompoundWords()
		{
			// checks for compounds such as "didn't" which is originally parsed as didn + ' + t
			Token current = this; // are initially made this recursive, but some of the base text is too large and would stack overflow
			while (current != null)
			{
				while (current.IsCompound())
				{
					current.Merge(2); // this word preceeded by punct and then more letters and no spaces.  Note that this can be done more than once
					if (current.CharacterType == CharType.Numeral)
					{
						current.Text = "10000"; // Somewhat arbitrarily this is used for all compounds.
						// We don't keep all possible text for numbers - see the constructor
						current.Hash = Tree.GetHash(current.Text);
					}
				}
				if (current.Text == "." && current.DirectlyPrecededBy(CharType.Letter) && current.Previous.IsCompound())
					Merge(3); // this is the .  at the end of an abbreviation, such as "e.g."
				current = current.Previous;
			}
		}

		private void Merge(int previous)
		{ // merges the given number of previous tokens into this one
			while (previous > 0)
			{
				Text = Previous.Text + Text;
				Whitespace = Previous.Whitespace; // whitespace-before now based on start of the sequence
				WasCapitalised = Previous.WasCapitalised;
				Previous = Previous.Previous; // removes the previous word(s) consumed into this one
				previous--;
			}
			Hash = Tree.GetHash(Text);
		}

		private bool IsCompound()
		{ // returns true if this is text, preceded by punctuation and then more text
			// but excludes certain punctuation
			return ((CharacterType == CharType.Letter || CharacterType == CharType.Numeral) &&
				DirectlyPrecededBy(CharType.Punct) && Previous.DirectlyPrecededBy(CharacterType)
				&& Previous.PunctTextPermitsMerging() &&
				(!Previous.Text.Contains(",") || CharacterType == CharType.Numeral));
			// "," excluded, unless this is a number (base text contains a number of things such as lists, or commas after brackets which should not be merged)
		}

		private bool DirectlyPrecededBy(CharType eType)
		{ // returns true if this is directly preceded by the given type, with no whitespace between
			if (Previous == null || Whitespace)
				return false;
			return (Previous.CharacterType == eType);
		}

		private bool PunctTextPermitsMerging()
		{ // only relevant for punctuation.  Returns false if it contains certain text which is assumed to preclude merging tokens
			if (Text.Contains("/") || Text.Contains("}") || Text.Contains(")") || Text.Contains("]"))
				return false;
			return (Text.Length < 3); // I can't imagine many long sequences of punctuation should be treated as a single word ?!
		}

		private int CONVERT_ALL_CAPS_THRESHOLD = 3; // this many or more consecutive all caps words are treated as lower case.
		private int CheckAllCaps()
		{// returns number of words including this which are/were all capitals
			// called by SplitSentence
			int caps = 0;
			if (Previous != null)
				caps = Previous.CheckAllCaps();
			if (AllCaps)
				caps++;
			else if (caps >= CONVERT_ALL_CAPS_THRESHOLD) // if this is all capitals, but was preceded by a long sequence of them, then convert them
				Previous.ConvertAllCaps();
			return caps;
		}

		private void ConvertAllCaps()
		{ // called externally if this returned >=3 from CheckAllCaps
			// not called internally, so we can start the update from the end of a longer sequence, rather than repeating for every word
			if (!AllCaps) return;
			Text = Text.ToLower();
			Hash = Tree.GetHash(Text);
			Previous?.ConvertAllCaps();
		}
	}

}
