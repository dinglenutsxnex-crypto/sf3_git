public static class NekkiWebExtensions
{
	public static void AbortWWW(this NekkiWebRequest webRequest, bool isError = false)
	{
		if (webRequest != null)
		{
			webRequest.Abort(isError);
		}
	}
}
