namespace GooglePlayGames.BasicApi
{
	public class Achievement
	{
		private string mId = string.Empty;

		private bool mIsIncremental;

		private bool mIsRevealed;

		private bool mIsUnlocked;

		private int mCurrentSteps;

		private int mTotalSteps;

		private string mDescription = string.Empty;

		private string mName = string.Empty;

		public bool IsIncremental
		{
			get
			{
				return mIsIncremental;
			}
			set
			{
				mIsIncremental = value;
			}
		}

		public int CurrentSteps
		{
			get
			{
				return mCurrentSteps;
			}
			set
			{
				mCurrentSteps = value;
			}
		}

		public int TotalSteps
		{
			get
			{
				return mTotalSteps;
			}
			set
			{
				mTotalSteps = value;
			}
		}

		public bool IsUnlocked
		{
			get
			{
				return mIsUnlocked;
			}
			set
			{
				mIsUnlocked = value;
			}
		}

		public bool IsRevealed
		{
			get
			{
				return mIsRevealed;
			}
			set
			{
				mIsRevealed = value;
			}
		}

		public string Id
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

		public string Description
		{
			get
			{
				return mDescription;
			}
			set
			{
				mDescription = value;
			}
		}

		public string Name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}

		public override string ToString()
		{
			return string.Format("[Achievement] id={0}, name={1}, desc={2}, type={3}, revealed={4}, unlocked={5}, steps={6}/{7}", mId, mName, mDescription, (!mIsIncremental) ? "STANDARD" : "INCREMENTAL", mIsRevealed, mIsUnlocked, mCurrentSteps, mTotalSteps);
		}
	}
}
