using System;
using System.Globalization;

public class RpnValue<T>
{
	private const string PRECOMPIE_PREFIX = "?CLC_";

	private const string FUNCTION_PREFIX = "?";

	private bool isConst;

	private T value;

	private RpnParser.Formula formula;

	public T Value
	{
		get
		{
			if (isConst)
			{
				return value;
			}
			return ConvertTo(formula.calculate().ToString());
		}
	}

	public RpnValue(T value)
	{
		this.value = value;
		isConst = true;
	}

	public RpnValue(string rawFormula, bool isStringType = false)
	{
		formula = new RpnParser.Formula(rawFormula);
		if (rawFormula.Contains("?CLC_") || !rawFormula.Contains("?"))
		{
			value = ConvertTo(formula.calculate().ToString());
			isConst = true;
			formula = null;
		}
	}

	public static implicit operator T(RpnValue<T> rpnValuer)
	{
		return rpnValuer.Value;
	}

	public static implicit operator RpnValue<T>(T value)
	{
		return new RpnValue<T>(value);
	}

	public static implicit operator RpnValue<T>(string rawFormula)
	{
		return new RpnValue<T>(rawFormula);
	}

	private static T ConvertTo(string rawValue)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(int))
		{
			return (T)(object)int.Parse(rawValue);
		}
		if (typeFromHandle == typeof(float))
		{
			return (T)(object)float.Parse(rawValue, CultureInfo.InvariantCulture);
		}
		if (typeFromHandle == typeof(bool))
		{
			return (T)(object)ParseBool(rawValue);
		}
		if (typeFromHandle == typeof(string))
		{
			return (T)(object)rawValue;
		}
		return default(T);
	}

	private static bool ParseBool(string rawValue)
	{
		return rawValue.ToLower() == "true" || rawValue == "1";
	}
}
