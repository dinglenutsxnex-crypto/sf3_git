using Nekki;
using Newtonsoft.Json;
using UnityEngine;

public static class PrintableJSONExtension
{
	public static void PrintJSON(this object extensible)
	{
		Debug.Log(extensible.ToDebugJSON());
	}

	public static string ToDebugJSON(this object printable)
	{
		if (printable != null && NekkiUtils.IsDebug)
		{
			return string.Concat(printable.GetType(), JsonConvert.SerializeObject(printable, Formatting.Indented));
		}
		return string.Empty;
	}
}
