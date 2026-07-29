using System;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal class LeaderboardManager
	{
		private readonly GooglePlayGames.Native.PInvoke.GameServices mServices;

		internal LeaderboardManager(GooglePlayGames.Native.PInvoke.GameServices services)
		{
			mServices = Misc.CheckNotNull(services);
		}

		internal void SubmitScore(string leaderboardId, long score)
		{
			Misc.CheckNotNull(leaderboardId);
			Logger.d("Native Submitting score: " + score + " for lb " + leaderboardId);
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_SubmitScore(mServices.AsHandle(), leaderboardId, (ulong)score, string.Empty);
		}

		internal void ShowAllUI(Action<CommonErrorStatus.UIStatus> callback)
		{
			Misc.CheckNotNull(callback);
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ShowAllUI(mServices.AsHandle(), Callbacks.InternalShowUICallback, Callbacks.ToIntPtr(callback));
		}

		internal void ShowUI(string leaderboardId, Action<CommonErrorStatus.UIStatus> callback)
		{
			Misc.CheckNotNull(callback);
			GooglePlayGames.Native.Cwrapper.LeaderboardManager.LeaderboardManager_ShowUI(mServices.AsHandle(), leaderboardId, Callbacks.InternalShowUICallback, Callbacks.ToIntPtr(callback));
		}
	}
}
