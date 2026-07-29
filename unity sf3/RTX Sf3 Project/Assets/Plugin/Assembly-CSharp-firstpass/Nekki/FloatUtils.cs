using UnityEngine;

namespace Nekki
{
	public static class FloatUtils
	{
		public static bool IsEqualByEpsilon(this float a, float b, float epsilon = float.Epsilon)
		{
			return Mathf.Abs(a - b) < epsilon;
		}
	}
}
