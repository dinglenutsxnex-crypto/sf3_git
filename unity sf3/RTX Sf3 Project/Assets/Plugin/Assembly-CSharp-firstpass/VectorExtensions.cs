using UnityEngine;

public static class VectorExtensions
{
	public enum Vector3ValueType
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	private const int Negator = -1;

	public static Vector3 Negate(this Vector3 vector)
	{
		return -1f * vector;
	}

	public static Vector3 Negate(this Vector3 vector, params Vector3ValueType[] valuesToNegate)
	{
		for (int i = 0; i < valuesToNegate.Length; i++)
		{
			switch (valuesToNegate[i])
			{
			case Vector3ValueType.X:
				vector.x *= -1f;
				break;
			case Vector3ValueType.Y:
				vector.y *= -1f;
				break;
			case Vector3ValueType.Z:
				vector.z *= -1f;
				break;
			}
		}
		return vector;
	}
}
