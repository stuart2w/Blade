using System;
using System.Collections.Generic;
using System.Text;
using IO = System.IO;
using System.Diagnostics;

namespace Blade
{
	public class Abbreviations : Dictionary<string, string>
	{// stores abbreviations which will be expanded when the user types.  It isn't strictly necessary for either the abbreviation or the expansion
		// appear in the main Tree data objects.  The Engine keeps one of these, which is effectively completely separate
		// the key is the abbreviation, the value is the expanded word.  The abbreviations are case sensitive

		public string Lookup(string strAbbreviation)
		{
			if (base.ContainsKey(strAbbreviation))
				return base[strAbbreviation];
			return "";
		}

		public List<string> GetSortedList()
		{
			List<string> colList = new List<string>();
			foreach (string strAbbreviation in base.Keys)
				colList.Add(strAbbreviation);
			colList.Sort();
			return colList;
		}

		public void Write(string strFile, bool bolBinary)
		{ // bolBinary can be loaded back in again; if this is false it just dumps the text in the form "abbreviation <tab> expansion"
			Debug.WriteLine("Writing Abbreviations file: " + strFile);
			if (bolBinary)
			{
				using (IO.BinaryWriter output = new IO.BinaryWriter(new IO.FileStream(strFile, System.IO.FileMode.Create, System.IO.FileAccess.Write), System.Text.Encoding.Default))
				{
					output.Write("Abbreviations");
					output.Write(Tree.CURRENTFILEVERSION);
					output.Write(base.Count);
					foreach (string strAbbreviation in base.Keys)
					{
						output.Write(strAbbreviation);
						output.Write(base[strAbbreviation]);
					}
				}
			}
			else
			{
				using (IO.StreamWriter output = new IO.StreamWriter(strFile, false, System.Text.Encoding.Default))
				{
					foreach (string strAbbreviation in base.Keys)
						output.WriteLine(strAbbreviation + '\t' + base[strAbbreviation]);
				}
			}

		}

		public static Abbreviations Read(string strFile)
		{ // loads abbreviations from the given binary file, and returns a new object
			Debug.WriteLine("Reading Abbreviations file: " + strFile);
			Abbreviations objNew = new Abbreviations();
			using (IO.BinaryReader input = new IO.BinaryReader(new IO.FileStream(strFile, System.IO.FileMode.Open, System.IO.FileAccess.Read), System.Text.Encoding.Default))
			{
				if (input.ReadString() != "Abbreviations")
					throw new Exception("This file does not contain Blade abbreviations");
				int intVersion = input.ReadInt32();
				int intCount = input.ReadInt32();
				for (; intCount > 0; intCount--)
				{
					string strAbbrev = input.ReadString();
					objNew.Add(strAbbrev, input.ReadString());
				}
			}
			return objNew;
		}
	}
}
