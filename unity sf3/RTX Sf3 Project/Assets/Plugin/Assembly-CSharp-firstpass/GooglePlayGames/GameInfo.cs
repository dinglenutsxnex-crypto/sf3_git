namespace GooglePlayGames
{
	public static class GameInfo
	{
		private const string UnescapedApplicationId = "APPID";

		private const string UnescapedIosClientId = "CLIENTID";

		public const string ApplicationId = "__APPID__";

		public const string IosClientId = "__IOS_CLIENTID__";

		public const string AndroidClientId = "177908379300-sdei6dqkisj4lj8qvj8vltbqsqo81jt8.apps.googleusercontent.com";

		public const string NearbyConnectionServiceId = "__NEARBY_SERVICE_ID__";

		public static bool ApplicationIdInitialized()
		{
			return !"__APPID__".Equals(ToEscapedToken("APPID"));
		}

		public static bool IosClientIdInitialized()
		{
			return !"__IOS_CLIENTID__".Equals(ToEscapedToken("CLIENTID"));
		}

		private static string ToEscapedToken(string token)
		{
			return string.Format("__{0}__", token);
		}
	}
}
