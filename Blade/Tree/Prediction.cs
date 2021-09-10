using System;
using System.Collections.Generic;

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
	internal struct Prediction : IComparable<Prediction>
	{
		public float Probability;
		public string Text;
		public Frequency FrequencyData;

		internal Prediction(float sngProbability, string strText, Frequency objFrequency)
		{
			Probability = sngProbability;
			Text = strText;
			FrequencyData = objFrequency;
		}

		// sorts into descending probabilty
		public int CompareTo(Prediction other) => -Probability.CompareTo(other);

		public class AlphabeticalSort : IComparer<Prediction>
		{

			public int Compare(Prediction x, Prediction y) => x.Text.CompareTo(y.Text);

		}
	}
}