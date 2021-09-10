using System;
using System.Collections.Generic;
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
	public class Recency
	{
		private Tree m_Tree; // the main prediction data.  Used to filter words, no point storing words not in the predictions
		private int m_Dirtiness; // the number of additions since the last cleaning cycle
		private float m_TotalFrequency;
		private Dictionary<int, float> m_Words;
		// the key is the text hash of the word.  The value is the frequency.  This is stored as a float so that it can be slowly decayed

		private const int CLEAN_AFTER = 50; // number of words before we reduce the probabilities
		private const float FREQUENCY_DECAY = 0.75F; // the frequency of each word is multiplied by this on each cleaning cycle
		private const float MINIMUM_PROBABILITY = 1 / 1000f; // words are discarded once the frequency drops below this fraction of the total frequency
		private const float MINIMUM_TOTAL_FREQUENCY = 300; // when calculating the "probability" of words in the recency
		// (i.e. which is the word's frequency divided by the total frequency) this is the minimum total frequency to use
		// this is required because if we start with just, say, one word it gets 100% probability

		public Recency(Tree tree)
		{
			m_Tree = tree;
			Clear();
		}

		public void Clear()
		{
			m_Words = new Dictionary<int, float>();
			m_Dirtiness = 0;
			m_TotalFrequency = 0;
		}

		public void Add(int hash)
		{
			if (!m_Tree.Entries.ContainsKey(hash))
				return; // we only store words in the data (no point storing others as this works as a MODIFIER of the probability)
			if (m_Words.ContainsKey(hash))
				m_Words[hash] += 1;
			else
				m_Words.Add(hash, 1);
			m_TotalFrequency++;
			m_Dirtiness++;
			if (m_Dirtiness >= CLEAN_AFTER)
				Clean();
		}

		private void Clean()
		{
			// but all the words we want to keep into a new list
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			float newTotal = 0; // will be the new value for m_TotalFrequency
			// decision on whether to discard is based on the old frequency 
			// (it doesn't make that much difference either way, MINIMUM_PROBABILITY is chosen empirically anyway)
			float threshold = MINIMUM_PROBABILITY * m_TotalFrequency;
			if (threshold > 1) threshold = 0.9F; // if we had a threshold
			foreach (int hash in m_Words.Keys)
			{
				float newFrequency = m_Words[hash] * FREQUENCY_DECAY;
				if (newFrequency >= threshold)
				{
					dictionary.Add(hash, newFrequency);
					newTotal += newFrequency;
				}
			}
			m_Words = dictionary;
			m_Dirtiness = 0;
			m_TotalFrequency = newTotal;
		}

		public float ProbabilityOf(int hash)
		{ // returns the probability of the given word; i.e. it's recency.  0 if the word is not in the list
			if (!m_Words.ContainsKey(hash))
				return 0;
			return m_Words[hash] / Math.Max(m_TotalFrequency, MINIMUM_TOTAL_FREQUENCY);
		}

		/// <summary>Returns frequency in data, or 0 if not found</summary>
		public float RawFrequencyOf(int hash)
		{
			if (!m_Words.ContainsKey(hash))
				return 0;
			return m_Words[hash];
		}

		public void DebugDump()
		{ // dumps all the data to the debug console
			Debug.WriteLine("Recency information... ");
			Debug.WriteLine("Total frequency=" + m_TotalFrequency.ToString("0.0000"));
			Debug.WriteLine("Words=" + m_Words.Count.ToString());
			Debug.WriteLine("Word since last cleaning cycle=" + m_Dirtiness.ToString());
			foreach (int hash in m_Words.Keys)
			{
				float frequency = m_Words[hash];
				float probability = frequency / m_TotalFrequency;
				Debug.Write(m_Tree.GetText(hash) + ": Frequency=" + frequency.ToString("0.00") + "; Relative probability=" +
					probability.ToString("0.0000"));
				float simpleFrequency = (float)m_Tree.Entries[hash].Count / m_Tree.TotalFrequency;
				if (probability < simpleFrequency)
					Debug.WriteLine("  No boost");
				else
					Debug.WriteLine("  Boosts by " + (probability / simpleFrequency).ToString("0.00"));
			}
		}

		public void Write(string file)
		{
			Debug.WriteLine("Writing Blade Recency: " + file);
			using (IO.BinaryWriter output = new IO.BinaryWriter(new IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write), System.Text.Encoding.Default))
			{
				output.Write(Tree.CURRENTFILEVERSION);
				output.Write("ABCD".GetHashCode()); // used to check if the hash codes are compatible
				// if not, the file is simply not reloaded (it would imply a change of machine)
				// we don't store m_Dirtiness or m_TotalFrequency as Clean is called after loading
				output.Write(m_Words.Count);
				foreach (int intKey in m_Words.Keys)
				{
					output.Write(intKey);
					output.Write(m_Words[intKey]);
				}
			}
		}

		public void LoadFrom(string file)
		{
			m_Words.Clear();
			m_TotalFrequency = 0;
			m_Dirtiness = 0;
			using (IO.BinaryReader input = new IO.BinaryReader(new IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read), System.Text.Encoding.Default))
			{
				try
				{
					int version = input.ReadInt32();
					if (version > Tree.CURRENTFILEVERSION)
						return;
					int hash = input.ReadInt32();
					if (hash != "ABCD".GetHashCode())
						return; // the hash code mechanism is not compatible from when the file was saved (e.g. 32-bit/64-bit change)
					// since this recency is somewhat ephemeral anyway, it is just discarded in this case
					int count = input.ReadInt32();
					while (count > 0)
					{
						int key = input.ReadInt32();
						float sngValue = input.ReadSingle();
						if (sngValue < 10000) // due to error in earlier versions which corrupted values
							if (!m_Words.ContainsKey(key)) // condition should be redundant, but just in case
								m_Words.Add(key, sngValue);
						count--;
					}
					Debug.WriteLine("Loaded recency from: " + file);
				}
				catch (Exception ex)
				{ // as with hash differences, it is best just to continue with blank data if there is a problem here
					System.Diagnostics.Debug.Fail("Recency.LoadFrom failed: " + ex.ToString());
				}
			}
			Clean();
		}
	}
}
