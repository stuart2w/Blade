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
	public class Node
	{
		public int Hash; // of the text for this word.  Look in Tree to find the actual text
		public int Count = 0; // on RootNodes ONLY, this can be set to -1 in the user wordlist indicating the word should be omitted from prediction even though it occurs in the system list
		public Dictionary<int, Node> Previous = null; // nodes for words before this one

		public const int EXPAND_FREQUENCY = 20; // only expand nodes once their frequency reaches this
		public const int UNKNOWN = -1; // dummy hash for sub-items where the text is unknown (ie where they were recorded before this was expanded)
		public const int OTHER = -2; // dummy hash for all other text, if the list has been compacted and some have been discarded
		public const int START = -3; // dummy for start of sentence
		public const int SPECIAL = -4; // used by TotalPredictions token

		#region Constructors - including from binary file
		public Node(string strText)
		{
			Hash = Tree.GetHash(strText);
		}

		public Node(int intHash)
		{
			Hash = intHash;
		}

		internal Node(System.IO.BinaryReader input)
		{ // construct from binary file
			Hash = input.ReadInt32();
			Count = input.ReadInt32();
			int intSubitems = input.ReadInt32();
			if (intSubitems > 0)
			{
				Previous = new Dictionary<int, Node>();
				for (int i = 0; i < intSubitems; i++)
				{
					Node objNew = new Node(input);
					Previous.Add(objNew.Hash, objNew);
				}
			}
		}
		#endregion

		#region Editing
		internal void Expand()
		{
			// forces this to expand into sub-items.  Usually used internally
			Debug.Assert(Previous == null, "Node already expanded");
			Debug.Assert(Hash != OTHER && Hash != UNKNOWN && Hash != START); // these should never be expanded (bit pointless if we don't know what this word is to record what went before it!)
			Previous = new Dictionary<int, Node>(5);
			if (Count > 0)
			{ // store entire existing count as "unknown" predecessors
				Previous.Add(UNKNOWN, new Node(UNKNOWN));
				Previous[UNKNOWN].Count = Count;
			}
		}

		internal virtual void Add(Token objToken, int intMultiplyFrequency)
		{
			Debug.Assert(objToken.Hash == Hash || Hash == SPECIAL);
			Count += intMultiplyFrequency;
			if (Hash == START) return;  // we never try to analyse what was before start
			Debug.Assert(objToken.Previous != null); // shouldn't be null if it wasn't START
			if (Previous == null && Count >= EXPAND_FREQUENCY)
			{
				Expand();
				// CheckSum();
				return; // mustn't let code below add to Previous again - it has been initialised with Count which is already updated
			}
			if (Previous != null)
			{// this node is subdivided -check for previous entries
				int intPrevious = objToken.Previous.Hash;
				if (!Previous.ContainsKey(intPrevious))
				{ // we don't currently have an entry for this - add it
					Previous.Add(intPrevious, new Node(intPrevious));
				}
				int intSpecific = Math.Max(Math.Min(intMultiplyFrequency, Count / 100), 1); // count to be added to specific predecessors
				Previous[intPrevious].Add(objToken.Previous, intSpecific);
				if (intSpecific < intMultiplyFrequency)
					EnsurePrevious(OTHER).Count += intMultiplyFrequency - intSpecific;
			}
			// CheckSum();
		}

		protected void AddOtherFreqency(int intFrequency)
		{// adds frequency to OTHER or UNKNOWN
			if (Previous.ContainsKey(UNKNOWN) && !Previous.ContainsKey(OTHER))
				Previous[UNKNOWN].Count += intFrequency;
			else
				EnsurePrevious(OTHER).Count += intFrequency;
		}

		private Node EnsurePrevious(int intHash)
		{ // ensures given key exists and returns it
			if (Previous.ContainsKey(intHash))
				return Previous[intHash];
			Node objNew = new Node(intHash);
			Previous.Add(intHash, objNew);
			return objNew;
		}

		public virtual void Clear(bool bolKeepStructure)
		{ // if bolKeepStructure then all counts are zeroed, but entries kept
			Count = 0;
			if (Previous == null) return;
			if (bolKeepStructure)
			{
				foreach (Node objPrevious in Previous.Values)
					objPrevious.Clear(true);
			}
			else
				Previous.Clear();
		}

		public virtual void Add(Node objOther, int intMultiplyFrequency)
		{ // all frequencies in the other data should be multiplied by intMultiplyFrequency
			if (objOther.Count < 0)
			{ // this entry marks explicit deletion by the user of this word
				Count = -1; // we need to keep a deletion entry in the combined list, so the word doesn't get learnt again while typing
				if (Previous != null)
					Previous.Clear();
			}
			else if (objOther.Previous == null)
			{
				Count += objOther.Count * intMultiplyFrequency;
				if (Previous == null) return; // if neither of us store details that's OK
				// I have a breakdown - so need to add these items somewhere - but other object doesn't specify where, so they are all unknown
				EnsurePrevious(UNKNOWN).Count += objOther.Count * intMultiplyFrequency;
			}
			else
			{ // other provides breakdown - add to my breakdown
				if (Previous == null) Expand();
				Count += objOther.Count * intMultiplyFrequency; // must be after expand command, because Count is added to UNKNOWN when expanding, and if that included the numbers from objOther then the loop below would be adding them a second time
				// if intMultiplyFrequency (which is for increasing user data) is high we shouldn't increase specific sequences greatly.
				// intMultiplyFrequency works fine at top level but distorts if tens are added to a rarer combination
				int intSpecific = Math.Max(Math.Min(intMultiplyFrequency, Count / 100), 1); // count to be added to specific predecessors

				foreach (Node objSub in objOther.Previous.Values)
				{
					EnsurePrevious(objSub.Hash).Add(objSub, intSpecific);
				}
				if (intSpecific < intMultiplyFrequency)
				{
					EnsurePrevious(OTHER);
					foreach (Node objSub in objOther.Previous.Values)
						Previous[OTHER].Count += intMultiplyFrequency - intSpecific;
				}
			}
		}

		public virtual void MultiplyFrequency(int intMultiplier)
		{// multiplies all frequencies - used when combining user and main data
			Count *= intMultiplier;
			if (Previous != null)
			{
				int intSpecific = Math.Max(Math.Min(intMultiplier, Count / 100), 1); // count to be added to specific predecessors
				if (intSpecific < intMultiplier) EnsurePrevious(OTHER);
				foreach (Node objSub in Previous.Values)
				{
					objSub.MultiplyFrequency(intMultiplier);
					if (intSpecific < intMultiplier)
						Previous[OTHER].Count += (intMultiplier - intSpecific);
				}
			}
		}

		//public void Subtract(Node objOther)
		//{// subtracts the amount in the other node from this one (ie assumes that everything in other must have been in this tree already)
		//    Debug.Assert(Count >= objOther.Count);
		//    Count = Math.Max(0, Count - objOther.Count);
		//    if (Previous == null || objOther.Previous == null) return;
		//    foreach (Node objSub in objOther.Previous.Values)
		//    {
		//        if (Previous.ContainsKey(objSub.Hash))
		//        {
		//            Previous[objSub.Hash].Subtract(objSub);
		//            if (Previous[objSub.Hash].Count == 0)
		//                Previous.Remove(objSub.Hash);
		//        }
		//        else
		//            Debug.Assert(objSub.Hash == UNKNOWN || objSub.Hash == OTHER, "Node.Subtract: does not contain entry in other");
		//        // will be OK if other is a generic one - it may be that 'this' is more specific
		//    }
		//}

		public void MapHashCodes(Dictionary<int, int> colMapping)
		{
			if (colMapping.ContainsKey(Hash))
				Hash = colMapping[Hash];
			else if (!SpecialHash(Hash))
			{
				Debug.WriteLine("Can't map hash code: " + Hash.ToString());
				Hash = UNKNOWN; // but might no longer match index in container
			}
			if (Previous != null)
			{
				Dictionary<int, Node> colNew = new Dictionary<int, Node>(); // need to make a new list to replace Previous with updated keys
				foreach (int intHash in Previous.Keys)
				{
					if (!SpecialHash(intHash))
					{
						if (colMapping.ContainsKey(intHash))
						{
							Previous[intHash].MapHashCodes(colMapping);
							int intNew = colMapping[intHash];
							if (colNew.ContainsKey(intNew))
							{ // collisions are possible where 2 different texts now have the same hash (although I'm surprised how many of these I'm getting
								colNew[intNew].Add(Previous[intHash], 1);
								//Debug.WriteLine("Collides mapping " + intHash.ToString() + " to " + intNew.ToString());
							}
							else
								colNew.Add(intNew, Previous[intHash]);
						}
					}
					else
					{
						Previous[intHash].MapHashCodes(colMapping);
						colNew.Add(intHash, Previous[intHash]);
					}
				}
				Previous = colNew;
			}
		}

		public virtual Node Clone()
		{
			Node objNew = new Node(Hash);
			objNew.Hash = Hash;
			objNew.Count = Count;
			if (Previous != null)
			{
				objNew.Previous = new Dictionary<int, Node>();
				foreach (Node objPrevious in Previous.Values)
					objNew.Previous.Add(objPrevious.Hash, objPrevious.Clone());
			}
			return objNew;
		}
		#endregion

		public void Write(System.IO.TextWriter output, string strSubsequent, Tree objTree)
		{ // strSubsequent is the text following this - ie from items higher up the tree
			// write self first, regardless of whether sub-divided
			string strText = objTree.GetText(Hash); // my text
			output.Write(strText);
			if (strSubsequent != "")
				output.Write(" << " + strSubsequent);
			output.Write(" : ");
			output.WriteLine(Count.ToString());
			if (Previous != null)
			{ // write sub-items
				if (strSubsequent != "")// prepend myself onto strSubsequent
					strSubsequent = " << " + strSubsequent;
				strSubsequent = strText + strSubsequent;
				foreach (int intKey in Previous.Keys)
				{
					if (intKey != UNKNOWN && intKey != OTHER) // these 2 written at end
						Previous[intKey].Write(output, strSubsequent, objTree);
				}
				if (Previous.ContainsKey(OTHER))
					Previous[OTHER].Write(output, strSubsequent, objTree);
				if (Previous.ContainsKey(UNKNOWN))
					Previous[UNKNOWN].Write(output, strSubsequent, objTree);
			}
		}

		public virtual void Write(System.IO.BinaryWriter output)
		{
			// write self first, regardless of whether sub-divided
			// this writes only hash values - more compact
			output.Write(Hash);
			output.Write(Count);
			if (Previous != null)
			{ // write sub-items
				output.Write(Previous.Count);
				foreach (int intKey in Previous.Keys)
				{ // unlike text dump, we don't care which order these are stored in
					Previous[intKey].Write(output);
				}
			}
			else
				output.Write(0); // no sub-items
		}

		#region Information
		public int CountNodes()
		{ // total of objects in self + sub-items.  Not total frequency which is just Count (which always equals total of all sub items)
			if (Previous == null) return 1; // just self
			int intCount = 1; // ie self
			foreach (Node objPrevious in Previous.Values)
				intCount += objPrevious.CountNodes();
			return intCount;
		}

		public int GreatestDepth()
		{ // returns greatest depth of nested nodes from here. 1 if just self;  2 if bi-gram etc...
			if (Previous == null)
				return 1;
			int intDeepest = 0;
			foreach (Node objNode in Previous.Values)
				intDeepest = Math.Max(intDeepest, objNode.GreatestDepth());
			return intDeepest + 1;
		}

		public string GreatestDepthText(Tree objTree)
		{ // returns greatest depth of nested nodes from here. 1 if just self;  2 if bi-gram etc...
			if (Previous == null)
				return objTree.GetText(Hash);
			int intDeepest = 0;
			string strDeepestText = "";
			foreach (Node objNode in Previous.Values)
			{
				int intDepth = objNode.GreatestDepth();
				if (intDepth > intDeepest)
				{
					intDeepest = intDepth;
					strDeepestText = objNode.GreatestDepthText(objTree);
				}
			}
			return strDeepestText + " " + objTree.GetText(Hash);
		}

		public static bool SpecialHash(int intHash)
		{
			return (intHash >= -4 && intHash <= -1);
		}
		#endregion

		#region Frequencies and list tidying
		internal void CalculateFrequency(int intDepth, Token objPrevious, Frequency objMatching, Frequency objTotal, int intMaxPreviousToUse)
		{// both objMatching and objTotal are updated - the same objTotal is passed to each root object, giving a total
			// objMatching is specific to this text
			// intMaxPreviousToUse is max number of previous words allowed. 0 = use just this item.
			Debug.Assert(objMatching != objTotal && objTotal != null);
			if (Hash == UNKNOWN)
			{
				objMatching.Unknown[intDepth] += Count;
				objTotal.Unknown[intDepth] += Count;
			}
			else if (Hash == OTHER)
			{
				objMatching.Other[intDepth] += Count;
				objTotal.Other[intDepth] += Count;
			}
			else
			{
				objMatching.Exact[intDepth] += Count;
				objTotal.Exact[intDepth] += Count;
			}
			if (Previous == null || intMaxPreviousToUse <= 0) return; // that's it - can't recurse down
			if (Previous.ContainsKey(UNKNOWN)) // if we have unknowns they are always counted as a match (in their own list)
				Previous[UNKNOWN].CalculateFrequency(intDepth + 1, null, objMatching, objTotal, intMaxPreviousToUse - 1);// doesn't need previous text
			if (objPrevious != null && Previous.ContainsKey(objPrevious.Hash))
			{ // we have an exact match for previous word
				Previous[objPrevious.Hash].CalculateFrequency(intDepth + 1, objPrevious.Previous, objMatching, objTotal, intMaxPreviousToUse - 1);
			}
			else if (Previous.ContainsKey(OTHER))
			{ // no match - this included in OTHER (if there is any OTHER)
				Previous[OTHER].CalculateFrequency(intDepth + 1, objPrevious.Previous, objMatching, objTotal, intMaxPreviousToUse - 1);
			}
		}

		internal void RemoveLowWords(Dictionary<int, int> colRemoved)
		{// parameter is hash codes for words which have been removed at top level
			int intRemovedCount = 0;
			if (Previous == null) return;
			List<int> colRemoveLocal = new List<int>();
			foreach (Node objPrevious in Previous.Values)
			{
				if (colRemoved.ContainsKey(objPrevious.Hash))
				{// this word removed.  Reduce my frequency
					intRemovedCount += objPrevious.Count;
					colRemoveLocal.Add(objPrevious.Hash);
				}
				else
					objPrevious.RemoveLowWords(colRemoved); // allow sub-item to check it's contents as well
			}
			if (colRemoveLocal.Count > 0)
			{
				foreach (int intHash in colRemoveLocal)
					Previous.Remove(intHash);
				// add the frequency of these to OTHER or UNKNOWN - whichever we already have
				Debug.Assert(intRemovedCount > 0);
				if (Previous.ContainsKey(OTHER))
					Previous[OTHER].Count += intRemovedCount;
				else if (Previous.ContainsKey(UNKNOWN))
					Previous[UNKNOWN].Count += intRemovedCount;
				else
				{
					Node objNew = new Node(OTHER);
					objNew.Count = intRemovedCount;
					Previous.Add(OTHER, objNew);
				}
			}
		}

		internal void RemoveLowEntries(int intMinimum)
		{// collapses individual previous entries with VERY low frequencies into OTHER or UNKNOWN
			if (Previous == null) return;
			List<int> colRemove = new List<int>(); // can't update Previous while iterating it
			int intRemovedFrequency = 0; // total frequency of removed words will be added to OTHER or UNKNOWN
			// need to add once at end outside iterator as AddOtherFreqency might need to create OTHER or UNKNOWN
			foreach (Node objPrevious in Previous.Values)
			{
				if (objPrevious.Count < intMinimum && objPrevious.Hash != OTHER && objPrevious.Hash != UNKNOWN)
				{// OTHER and UNK can't be cleaned like this! we are cleaning INTO them
					Debug.Assert(objPrevious.Previous == null || objPrevious.Previous.Count == 1); // the node itself shouldn't be subdivided?
					colRemove.Add(objPrevious.Hash);
					intRemovedFrequency += objPrevious.Count;
				}
				else
					objPrevious.RemoveLowEntries(intMinimum);
			}
			AddOtherFreqency(intRemovedFrequency);
			foreach (int intHash in colRemove)
				Previous.Remove(intHash);
		}

		internal void RemovePoorPredictors(Node objTotal)
		{// see comments in Tree. Param is node matching this in objTotal
			if (Previous == null) return;
			if (Count > objTotal.Count * Tree.FRACTION_OF_TOTAL_ASSUMED_SUFFIFIENT)
			{// this node already gives a high probability (enough for 0-letter pred?) No need to check further
				Previous = null;
				// should we remember this so it doesn't get added back again later?
			}
			else
			{
				if (objTotal.Previous == null)
				{
					Debug.Fail("RemovePoorPredictors: total is not as detailed as individual node!?");
					return;
				}
				foreach (Node objPrevious in Previous.Values)
				{
					if (objTotal.Previous.ContainsKey(objPrevious.Hash))
						objPrevious.RemovePoorPredictors(objTotal.Previous[objPrevious.Hash]);
				}
			}
		}

		internal void LimitDepth(int intRemaining)
		{ // intRemaining is the number of previous levels which can be stored below this
			if (Previous == null)
				return;
			if (intRemaining == 0)
				Previous = null;
			else
			{
				foreach (Node objPrevious in Previous.Values)
					objPrevious.LimitDepth(intRemaining - 1);
			}
		}

		// function was used to check consistency, add calls at end of suspect functions
		//        private void CheckSum()
		//        { // checks sum of previous against count for debugging
		//#if DEBUG
		//            if (Previous == null) return;
		//            int intTotal = 0;
		//            foreach (Node objPrevious in Previous.Values)
		//                intTotal += objPrevious.Count;
		//            Debug.Assert(intTotal == Count);
		//#endif
		//        }
		#endregion

		#region Frequency sorter
		public class FrequencySort : IComparer<Node>
		{  // sort into order of descending frequency
			public int Compare(Node x, Node y)
			{
				return -x.Count.CompareTo(y.Count);
			}
		}
		#endregion

	}
}