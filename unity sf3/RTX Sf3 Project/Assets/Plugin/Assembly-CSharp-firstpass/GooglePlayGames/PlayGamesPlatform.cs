using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi.Nearby;
using GooglePlayGames.BasicApi.Quests;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.OurUtils;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesPlatform : ISocialPlatform
	{
		private static volatile PlayGamesPlatform sInstance;

		private readonly PlayGamesClientConfiguration mConfiguration;

		private PlayGamesLocalUser mLocalUser;

		private IPlayGamesClient mClient;

		private static volatile bool sNearbyInitializePending;

		private static volatile INearbyConnectionClient sNearbyConnectionClient;

		private string mDefaultLbUi;

		private Dictionary<string, string> mIdMap = new Dictionary<string, string>();

		public static PlayGamesPlatform Instance
		{
			get
			{
				if (sInstance == null)
				{
					GooglePlayGames.OurUtils.Logger.d("Instance was not initialized, using default configuration.");
					InitializeInstance(PlayGamesClientConfiguration.DefaultConfiguration);
				}
				return sInstance;
			}
		}

		public static INearbyConnectionClient Nearby
		{
			get
			{
				if (sNearbyConnectionClient == null && !sNearbyInitializePending)
				{
					sNearbyInitializePending = true;
					InitializeNearby(null);
				}
				return sNearbyConnectionClient;
			}
		}

		public static bool DebugLogEnabled
		{
			get
			{
				return GooglePlayGames.OurUtils.Logger.DebugLogEnabled;
			}
			set
			{
				GooglePlayGames.OurUtils.Logger.DebugLogEnabled = value;
			}
		}

		public IRealTimeMultiplayerClient RealTime
		{
			get
			{
				return mClient.GetRtmpClient();
			}
		}

		public ITurnBasedMultiplayerClient TurnBased
		{
			get
			{
				return mClient.GetTbmpClient();
			}
		}

		public ISavedGameClient SavedGame
		{
			get
			{
				return mClient.GetSavedGameClient();
			}
		}

		public IEventsClient Events
		{
			get
			{
				return mClient.GetEventsClient();
			}
		}

		public IQuestsClient Quests
		{
			get
			{
				return mClient.GetQuestsClient();
			}
		}

		public ILocalUser localUser
		{
			get
			{
				return mLocalUser;
			}
		}

		private PlayGamesPlatform(PlayGamesClientConfiguration configuration)
		{
			mLocalUser = new PlayGamesLocalUser(this);
			mConfiguration = configuration;
		}

		internal PlayGamesPlatform(IPlayGamesClient client)
		{
			mClient = Misc.CheckNotNull(client);
			mLocalUser = new PlayGamesLocalUser(this);
			mConfiguration = PlayGamesClientConfiguration.DefaultConfiguration;
		}

		public void Authenticate(ILocalUser user, Action<bool, string> callback)
		{
		}

		public static void InitializeInstance(PlayGamesClientConfiguration configuration)
		{
			if (sInstance != null)
			{
				GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform already initialized. Ignoring this call.");
			}
			else
			{
				sInstance = new PlayGamesPlatform(configuration);
			}
		}

		public static void InitializeNearby(Action<INearbyConnectionClient> callback)
		{
			Debug.Log("Calling InitializeNearby!");
			if (sNearbyConnectionClient == null)
			{
				NearbyConnectionClientFactory.Create(delegate(INearbyConnectionClient client)
				{
					Debug.Log("Nearby Client Created!!");
					sNearbyConnectionClient = client;
					if (callback != null)
					{
						callback(client);
					}
					else
					{
						Debug.Log("Initialize Nearby callback is null");
					}
				});
			}
			else if (callback != null)
			{
				Debug.Log("Nearby Already initialized: calling callback directly");
				callback(sNearbyConnectionClient);
			}
			else
			{
				Debug.Log("Nearby Already initialized");
			}
		}

		public static PlayGamesPlatform Activate()
		{
			GooglePlayGames.OurUtils.Logger.d("Activating PlayGamesPlatform.");
			UnityEngine.Social.Active = Instance;
			GooglePlayGames.OurUtils.Logger.d("PlayGamesPlatform activated: " + UnityEngine.Social.Active);
			return Instance;
		}

		public void AddIdMapping(string fromId, string toId)
		{
			mIdMap[fromId] = toId;
		}

		public void Authenticate(Action<bool> callback)
		{
			Authenticate(callback, false);
		}

		public void Authenticate(Action<bool> callback, bool silent)
		{
			if (mClient == null)
			{
				GooglePlayGames.OurUtils.Logger.d("Creating platform-specific Play Games client.");
				mClient = PlayGamesClientFactory.GetPlatformPlayGamesClient(mConfiguration);
			}
			mClient.Authenticate(callback, silent);
		}

		public void Authenticate(ILocalUser unused, Action<bool> callback)
		{
			Authenticate(callback, false);
		}

		public bool IsAuthenticated()
		{
			return mClient != null && mClient.IsAuthenticated();
		}

		public void SignOut()
		{
			if (mClient != null)
			{
				mClient.SignOut();
			}
		}

		public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform.LoadUsers is not implemented.");
			if (callback != null)
			{
				callback(new IUserProfile[0]);
			}
		}

		public string GetUserId()
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("GetUserId() can only be called after authentication.");
				return "0";
			}
			return mClient.GetUserId();
		}

		public string GetIdToken()
		{
			if (mClient != null)
			{
				return mClient.GetIdToken();
			}
			return null;
		}

		public string GetAccessToken()
		{
			if (mClient != null)
			{
				return mClient.GetAccessToken();
			}
			return null;
		}

		public Achievement GetAchievement(string achievementId)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("GetAchievement can only be called after authentication.");
				return null;
			}
			return mClient.GetAchievement(achievementId);
		}

		public string GetUserDisplayName()
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("GetUserDisplayName can only be called after authentication.");
				return string.Empty;
			}
			return mClient.GetUserDisplayName();
		}

		public string GetUserImageUrl()
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("GetUserImageUrl can only be called after authentication.");
				return null;
			}
			return mClient.GetUserImageUrl();
		}

		public void ReportProgress(string achievementID, double progress, Action<bool> callback)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("ReportProgress can only be called after authentication.");
				if (callback != null)
				{
					callback(false);
				}
				return;
			}
			GooglePlayGames.OurUtils.Logger.d("ReportProgress, " + achievementID + ", " + progress);
			achievementID = MapId(achievementID);
			if (progress < 1E-06)
			{
				GooglePlayGames.OurUtils.Logger.d("Progress 0.00 interpreted as request to reveal.");
				mClient.RevealAchievement(achievementID, callback);
				return;
			}
			bool flag = false;
			int num = 0;
			int num2 = 0;
			Achievement achievement = mClient.GetAchievement(achievementID);
			if (achievement == null)
			{
				GooglePlayGames.OurUtils.Logger.w("Unable to locate achievement " + achievementID);
				GooglePlayGames.OurUtils.Logger.w("As a quick fix, assuming it's standard.");
				flag = false;
			}
			else
			{
				flag = achievement.IsIncremental;
				num = achievement.CurrentSteps;
				num2 = achievement.TotalSteps;
				GooglePlayGames.OurUtils.Logger.d("Achievement is " + ((!flag) ? "STANDARD" : "INCREMENTAL"));
				if (flag)
				{
					GooglePlayGames.OurUtils.Logger.d("Current steps: " + num + "/" + num2);
				}
			}
			if (flag)
			{
				GooglePlayGames.OurUtils.Logger.d("Progress " + progress + " interpreted as incremental target (approximate).");
				if (progress >= 0.0 && progress <= 1.0)
				{
					GooglePlayGames.OurUtils.Logger.w("Progress " + progress + " is less than or equal to 1. You might be trying to use values in the range of [0,1], while values are expected to be within the range [0,100]. If you are using the latter, you can safely ignore this message.");
				}
				int num3 = (int)(progress / 100.0 * (double)num2);
				int num4 = num3 - num;
				GooglePlayGames.OurUtils.Logger.d("Target steps: " + num3 + ", cur steps:" + num);
				GooglePlayGames.OurUtils.Logger.d("Steps to increment: " + num4);
				if (num4 > 0)
				{
					mClient.IncrementAchievement(achievementID, num4, callback);
				}
			}
			else if (progress >= 100.0)
			{
				GooglePlayGames.OurUtils.Logger.d("Progress " + progress + " interpreted as UNLOCK.");
				mClient.UnlockAchievement(achievementID, callback);
			}
			else
			{
				GooglePlayGames.OurUtils.Logger.d("Progress " + progress + " not enough to unlock non-incremental achievement.");
			}
		}

		public void IncrementAchievement(string achievementID, int steps, Action<bool> callback)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("IncrementAchievement can only be called after authentication.");
				if (callback != null)
				{
					callback(false);
				}
			}
			else
			{
				GooglePlayGames.OurUtils.Logger.d("IncrementAchievement: " + achievementID + ", steps " + steps);
				achievementID = MapId(achievementID);
				mClient.IncrementAchievement(achievementID, steps, callback);
			}
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform.LoadAchievementDescriptions is not implemented.");
			if (callback != null)
			{
				callback(new IAchievementDescription[0]);
			}
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform.LoadAchievements is not implemented.");
			if (callback != null)
			{
				callback(new IAchievement[0]);
			}
		}

		public IAchievement CreateAchievement()
		{
			return new PlayGamesAchievement();
		}

		public void ReportScore(long score, string board, Action<bool> callback)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("ReportScore can only be called after authentication.");
				if (callback != null)
				{
					callback(false);
				}
			}
			else
			{
				GooglePlayGames.OurUtils.Logger.d("ReportScore: score=" + score + ", board=" + board);
				string leaderboardId = MapId(board);
				mClient.SubmitScore(leaderboardId, score, callback);
			}
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform.LoadScores not implemented.");
			if (callback != null)
			{
				callback(new IScore[0]);
			}
		}

		public ILeaderboard CreateLeaderboard()
		{
			GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform.CreateLeaderboard not implemented. Returning null.");
			return null;
		}

		public void ShowAchievementsUI()
		{
			ShowAchievementsUI(null);
		}

		public void ShowAchievementsUI(Action<UIStatus> callback)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("ShowAchievementsUI can only be called after authentication.");
				return;
			}
			GooglePlayGames.OurUtils.Logger.d("ShowAchievementsUI callback is " + callback);
			mClient.ShowAchievementsUI(callback);
		}

		public void ShowLeaderboardUI()
		{
			GooglePlayGames.OurUtils.Logger.d("ShowLeaderboardUI with default ID");
			ShowLeaderboardUI(MapId(mDefaultLbUi), null);
		}

		public void ShowLeaderboardUI(string lbId)
		{
			if (lbId != null)
			{
				lbId = MapId(lbId);
			}
			mClient.ShowLeaderboardUI(lbId, null);
		}

		public void ShowLeaderboardUI(string lbId, Action<UIStatus> callback)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("ShowLeaderboardUI can only be called after authentication.");
				return;
			}
			GooglePlayGames.OurUtils.Logger.d("ShowLeaderboardUI, lbId=" + lbId + " callback is " + callback);
			mClient.ShowLeaderboardUI(lbId, callback);
		}

		public void SetDefaultLeaderboardForUI(string lbid)
		{
			GooglePlayGames.OurUtils.Logger.d("SetDefaultLeaderboardForUI: " + lbid);
			if (lbid != null)
			{
				lbid = MapId(lbid);
			}
			mDefaultLbUi = lbid;
		}

		public void LoadFriends(ILocalUser user, Action<bool> callback)
		{
			GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform.LoadFriends not implemented.");
			if (callback != null)
			{
				callback(false);
			}
		}

		public void LoadScores(ILeaderboard board, Action<bool> callback)
		{
			GooglePlayGames.OurUtils.Logger.w("PlayGamesPlatform.LoadScores not implemented.");
			if (callback != null)
			{
				callback(false);
			}
		}

		public bool GetLoading(ILeaderboard board)
		{
			return false;
		}

		public void LoadState(int slot, OnStateLoadedListener listener)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("LoadState can only be called after authentication.");
				if (listener != null)
				{
					listener.OnStateLoaded(false, slot, null);
				}
			}
			else
			{
				mClient.LoadState(slot, listener);
			}
		}

		public void UpdateState(int slot, byte[] data, OnStateLoadedListener listener)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.e("UpdateState can only be called after authentication.");
				if (listener != null)
				{
					listener.OnStateSaved(false, slot);
				}
			}
			else
			{
				mClient.UpdateState(slot, data, listener);
			}
		}

		public void RegisterInvitationDelegate(InvitationReceivedDelegate deleg)
		{
			mClient.RegisterInvitationDelegate(deleg);
		}

		private string MapId(string id)
		{
			if (id == null)
			{
				return null;
			}
			if (mIdMap.ContainsKey(id))
			{
				string text = mIdMap[id];
				GooglePlayGames.OurUtils.Logger.d("Mapping alias " + id + " to ID " + text);
				return text;
			}
			return id;
		}
	}
}
