using System;
using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi.Quests;
using GooglePlayGames.BasicApi.SavedGame;

namespace GooglePlayGames.BasicApi
{
	public interface IPlayGamesClient
	{
		void Authenticate(Action<bool> callback, bool silent);

		bool IsAuthenticated();

		void SignOut();

		string GetUserId();

		string GetUserDisplayName();

		string GetAccessToken();

		string GetIdToken();

		string GetUserImageUrl();

		Achievement GetAchievement(string achievementId);

		void UnlockAchievement(string achievementId, Action<bool> successOrFailureCalllback);

		void RevealAchievement(string achievementId, Action<bool> successOrFailureCalllback);

		void IncrementAchievement(string achievementId, int steps, Action<bool> successOrFailureCalllback);

		void ShowAchievementsUI(Action<UIStatus> callback);

		void ShowLeaderboardUI(string leaderboardId, Action<UIStatus> callback);

		void SubmitScore(string leaderboardId, long score, Action<bool> successOrFailureCalllback);

		void LoadState(int slot, OnStateLoadedListener listener);

		void UpdateState(int slot, byte[] data, OnStateLoadedListener listener);

		IRealTimeMultiplayerClient GetRtmpClient();

		ITurnBasedMultiplayerClient GetTbmpClient();

		ISavedGameClient GetSavedGameClient();

		IEventsClient GetEventsClient();

		IQuestsClient GetQuestsClient();

		void RegisterInvitationDelegate(InvitationReceivedDelegate invitationDelegate);
	}
}
