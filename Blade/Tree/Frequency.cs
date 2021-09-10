using System;
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
	internal class Frequency
	{ // used when predicting - records the number of times the current tokens match AT EACH LEVEL
		// [0] is the text in the root nodes - ie simple word completion
		// [1] is looking at bigrams etc.
		/* because we have a variable amount of information at some levels - ie UNKNOWN and OTHER entries
		 * and the frequencies of these aren't really comparable, they are listed separately
		 */
		public const int MAX_LEVEL = 10;
		public int[] Exact = new int[MAX_LEVEL]; // instances matching previous tokens
		public int[] Unknown = new int[MAX_LEVEL]; // instances where data contains Unknown at this level
		public int[] Other = new int[MAX_LEVEL]; // instances where matches, but by matching to OTHER in data
		public Single RecencyFrequency = 0; // only really needed for diagnostics
		public double RecencyAdjust = 0; // set at 0 for no adjustment.  all other values take effect

		public float TotalProbability(Frequency objTotal)
		{// returns a single probability score for this within the given total frequency
			// we just add up the probabilities for each analysis - ie if this prediction looks conspicuously likely anywhere
			// it will score highly
			float sngTotal = 0;
			if (RecencyAdjust != 0)// version with adjust is slower, so only used where needed
			{
				for (int intLevel = 0; intLevel < MAX_LEVEL; intLevel++)
				{
					float sngMultiplier = intLevel * 0.4F + 1; // makes deeper levels slightly more significant
					if (objTotal.Exact[intLevel] > 0)
						sngTotal += sngMultiplier * 2 * AdjustedFrequency(Exact[intLevel], objTotal.Exact[intLevel]);
					//if (objTotal.Unknown[intLevel] > 0)
					//    sngTotal += sngMultiplier * AdjustedFrequency(Unknown[intLevel], objTotal.Unknown[intLevel]);
					//if (objTotal.Other[intLevel] > 0)
					//    sngTotal += sngMultiplier * AdjustedFrequency(Other[intLevel], objTotal.Other[intLevel]);
					if (objTotal.Unknown[intLevel] > 0 || objTotal.Other[intLevel] > 0)
						sngTotal += sngMultiplier * AdjustedFrequency(Unknown[intLevel] + Other[intLevel], objTotal.Unknown[intLevel] + objTotal.Other[intLevel]);
				}
			}
			else
			{
				for (int intLevel = 0; intLevel < MAX_LEVEL; intLevel++)
				{
					float sngMultiplier = intLevel * 0.4F + 1; // makes deeper levels slightly more significant
					if (objTotal.Exact[intLevel] > 0)
						sngTotal += sngMultiplier * 2 * ((float)Exact[intLevel] / objTotal.Exact[intLevel]);
					//if (objTotal.Unknown[intLevel] > 0)
					//    sngTotal += sngMultiplier * ((float)Unknown[intLevel] / objTotal.Unknown[intLevel]);
					//if (objTotal.Other[intLevel] > 0)
					//    sngTotal += sngMultiplier * ((float)Other[intLevel] / objTotal.Other[intLevel]);
					if (objTotal.Unknown[intLevel] > 0 || objTotal.Other[intLevel] > 0)
						sngTotal += sngMultiplier * ((float)(Unknown[intLevel] + Other[intLevel]) / (objTotal.Unknown[intLevel] + objTotal.Other[intLevel]));
				}
			}
			if (Single.IsInfinity(sngTotal))
			{
				Debug.Fail("Infinite probability - scrambled recency?");
				return 0;
			}
			return sngTotal;
		}

		private Single AdjustedFrequency(int intThis, int intTotal)
		{
			Single sngFrequency = ((float)intThis / intTotal);
			return (Single)Math.Exp(Math.Log(sngFrequency) * RecencyAdjust);
		}

		public bool HasResultsAtLevel(int intLevel)
		{
			return Exact[intLevel] > 0 || Unknown[intLevel] > 0 || Other[intLevel] > 0;
		}

		public void AdjustForRecency(Single sngRecency, Single sngSimpleProbability)
		{
			RecencyFrequency = sngRecency;
			if (sngRecency > sngSimpleProbability)
			{ // ie if the recency frequency is greater than the words simple frequency
				// then increase sngProbability which is calculated using bigrams and stuff in relation to this
				RecencyAdjust = Math.Log(sngRecency) / Math.Log(sngSimpleProbability);
				//f.RecencyAdjust = sngRecency / sngSimpleProbability;
				//sngProbability *= f.RecencyAdjust;// sngRecency / sngSimpleProbability;
			}

		}
	}
}