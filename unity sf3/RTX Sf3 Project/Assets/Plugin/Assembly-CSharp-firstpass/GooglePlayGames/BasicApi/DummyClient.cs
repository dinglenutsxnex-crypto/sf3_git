using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi.Quests;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi
{
	public class DummyClient : IPlayGamesClient
	{
		public void Authenticate(Action<bool> callback, bool silent)
		{
			LogUsage();
			if (callback != null)
			{
				callback(false);
			}
		}

		public bool IsAuthenticated()
		{
			LogUsage();
			return false;
		}

		public void SignOut()
		{
			LogUsage();
		}

		public string GetAccessToken()
		{
			LogUsage();
			return "DummyAccessToken";
		}

		public string GetIdToken()
		{
			LogUsage();
			return "DummyIdToken";
		}

		public string GetUserId()
		{
			LogUsage();
			return "DummyID";
		}

		public string GetUserDisplayName()
		{
			LogUsage();
			return "Player";
		}

		public string GetUserImageUrl()
		{
			LogUsage();
			return null;
		}

		public List<Achievement> GetAchievements()
		{
			LogUsage();
			return new List<Achievement>();
		}

		public Achievement GetAchievement(string achId)
		{
			LogUsage();
			return null;
		}

		public void UnlockAchievement(string achId, Action<bool> callback)
		{
			LogUsage();
			if (callback != null)
			{
				callback(false);
			}
		}

		public void RevealAchievement(string achId, Action<bool> callback)
		{
			LogUsage();
			if (callback != null)
			{
				callback(false);
			}
		}

		public void IncrementAchievement(string achId, int steps, Action<bool> callback)
		{
			LogUsage();
			if (callback != null)
			{
				callback(false);
			}
		}

		public void ShowAchievementsUI(Action<UIStatus> callback)
		{
			LogUsage();
			if (callback != null)
			{
				callback(UIStatus.VersionUpdateRequired);
			}
		}

		public void ShowLeaderboardUI(string lbId, Action<UIStatus> callback)
		{
			LogUsage();
			if (callback != null)
			{
				callback(UIStatus.VersionUpdateRequired);
			}
		}

		public void SubmitScore(string lbId, long score, Action<bool> callback)
		{
			LogUsage();
			if (callback != null)
			{
				callback(false);
			}
		}

		public void LoadState(int slot, OnStateLoadedListener listener)
		{
			LogUsage();
			if (listener != null)
			{
				listener.OnStateLoaded(false, slot, null);
			}
		}

		public void UpdateState(int slot, byte[] data, OnStateLoadedListener listener)
		{
			LogUsage();
		}

		public IRealTimeMultiplayerClient GetRtmpClient()
		{
			LogUsage();
			return null;
		}

		public ITurnBasedMultiplayerClient GetTbmpClient()
		{
			LogUsage();
			return null;
		}

		public ISavedGameClient GetSavedGameClient()
		{
			LogUsage();
			return null;
		}

		public IEventsClient GetEventsClient()
		{
			LogUsage();
			return null;
		}

		public IQuestsClient GetQuestsClient()
		{
			LogUsage();
			return null;
		}

		public void RegisterInvitationDelegate(InvitationReceivedDelegate deleg)
		{
			LogUsage();
		}

		public Invitation GetInvitationFromNotification()
		{
			LogUsage();
			return null;
		}

		public bool HasInvitationFromNotification()
		{
			LogUsage();
			return false;
		}

		private static void LogUsage()
		{
			Logger.d("Received method call on DummyClient - using stub implementation.");
		}
	}
}
