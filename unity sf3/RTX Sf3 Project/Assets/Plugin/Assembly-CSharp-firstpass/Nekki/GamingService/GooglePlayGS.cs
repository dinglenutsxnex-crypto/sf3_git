using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Nekki.GamingService
{
	public class GooglePlayGS : GamingService
	{
		public override void authorize(Action<bool> callback)
		{
			try
			{
				PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().Build();
				PlayGamesPlatform.InitializeInstance(configuration);
				PlayGamesPlatform.DebugLogEnabled = DEBUG_LOG;
				PlayGamesPlatform.Activate();
				UnityEngine.Social.localUser.Authenticate((Action<bool>)delegate
				{
					bool flag = PlayGamesPlatform.Instance.IsAuthenticated();
					if (flag)
					{
						Debug.Log("PlayGamesPlatform Token will be taken. Not used for now.");
					}
					else
					{
						Debug.Log("PlayGamesPlatform Token was not taken");
					}
					callback(flag);
				});
			}
			catch (Exception)
			{
				callback(false);
			}
		}

		public override string getAccount()
		{
			return PlayGamesPlatform.Instance.GetUserId();
		}

		public override bool isAutorized()
		{
			return PlayGamesPlatform.Instance.IsAuthenticated();
		}

		public override void logout()
		{
			PlayGamesPlatform.Instance.SignOut();
		}

		public override string getVerificationToken()
		{
			return (!isAutorized()) ? string.Empty : PlayGamesPlatform.Instance.GetIdToken();
		}
	}
}
