using System.Text.RegularExpressions;

public class CertificateValidator
{
	private const string HTTPS_PREFIX = "https://";

	private const string BASE_URL = "nekkimobile\\.ru";

	private const string PATTERN = "^https://([^\\/]+\\.|)nekkimobile\\.ru(:[0-9]*)*(\\/([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&amp;%\\$#_]*)?|)$";

	public static bool IsHttpsUrlValid(string url)
	{
		return Valid(url);
	}

	private static bool Valid(string url)
	{
		return new Regex("^https://([^\\/]+\\.|)nekkimobile\\.ru(:[0-9]*)*(\\/([a-zA-Z0-9\\-\\.\\?\\,\\'\\/\\\\\\+&amp;%\\$#_]*)?|)$").Matches(url).Count == 1;
	}
}
