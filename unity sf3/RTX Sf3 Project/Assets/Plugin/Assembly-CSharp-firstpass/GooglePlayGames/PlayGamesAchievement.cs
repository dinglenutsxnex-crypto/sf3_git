using System;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	internal class PlayGamesAchievement : IAchievement
	{
		private readonly ReportProgress mProgressCallback;

		private string mId = string.Empty;

		private double mPercentComplete;

		private static readonly DateTime Thesentinel = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		public string id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

		public double percentCompleted
		{
			get
			{
				return mPercentComplete;
			}
			set
			{
				mPercentComplete = value;
			}
		}

		public bool completed
		{
			get
			{
				return false;
			}
		}

		public bool hidden
		{
			get
			{
				return false;
			}
		}

		public DateTime lastReportedDate
		{
			get
			{
				return Thesentinel;
			}
		}

		internal PlayGamesAchievement()
			: this(PlayGamesPlatform.Instance.ReportProgress)
		{
		}

		internal PlayGamesAchievement(ReportProgress progressCallback)
		{
			mProgressCallback = progressCallback;
		}

		public void ReportProgress(Action<bool> callback)
		{
			mProgressCallback(mId, mPercentComplete, callback);
		}
	}
}
