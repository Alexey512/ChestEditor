using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NaughtyAttributes.Test;
using UnityEngine;

namespace Assets.Scripts.Common
{
	public class Range<T> : IEquatable<Range<T>> where T : IComparable<T>
	{
		[SerializeField]
		private T min;
		
		[SerializeField]
		private T max;

		public T Min
		{
			get => min;
			set => min = value;
		}

	    public T Max {
		    get => max;
		    set => max = value;
	    }

	    public bool Equals(Range<T> other) => other != null && Equals(other.Min, other.Max);

	    public override string ToString() => ToString("{0},{1}", CultureInfo.InvariantCulture);

	    public string ToString(string format, string formatRange, IFormatProvider provider = null) =>
	        ToString(Min.CompareTo(Max) == 0 ? format : formatRange, provider);

	    public string ToString(string format, IFormatProvider provider = null) =>
	        string.Format(provider, format, Min, Max);

	    public bool IsValid() => Min.CompareTo(Max) <= 0;

	    public bool ContainsValue(T value) => Min.CompareTo(value) <= 0 && value.CompareTo(Max) <= 0;

	    public bool IsInsideRange(Range<T> range) =>
	        IsValid() && range.IsValid() && range.ContainsValue(Min) && range.ContainsValue(Max);

	    public bool ContainsRange(Range<T> range) =>
	        IsValid() && range.IsValid() && ContainsValue(range.Min) && ContainsValue(range.Max);

	    public override bool Equals(object obj) => obj is Range<T> other && Equals(other);

	    public bool Equals(T minimum, T maximum) => Min.CompareTo(minimum) == 0 && Max.CompareTo(maximum) == 0;

	    public override int GetHashCode() => (Minimum: Min, Maximum: Max).GetHashCode();
	}

	[Serializable]
	public class RangeInt: Range<int> {}

	[Serializable]
	public class RangeFloat: Range<float> {}

}
