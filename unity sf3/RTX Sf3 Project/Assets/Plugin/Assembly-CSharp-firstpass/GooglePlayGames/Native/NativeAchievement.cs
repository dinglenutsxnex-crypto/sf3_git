using System;
using System.Runtime.InteropServices;
using System.Text;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;

namespace GooglePlayGames.Native
{
	internal class NativeAchievement : BaseReferenceHolder
	{
		internal NativeAchievement(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal uint CurrentSteps()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_CurrentSteps(SelfPtr());
		}

		internal string Description()
		{
			return PInvokeUtilities.OutParamsToString((StringBuilder out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Description(SelfPtr(), out_string, out_size));
		}

		internal string Id()
		{
			return PInvokeUtilities.OutParamsToString((StringBuilder out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Id(SelfPtr(), out_string, out_size));
		}

		internal string Name()
		{
			return PInvokeUtilities.OutParamsToString((StringBuilder out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Name(SelfPtr(), out_string, out_size));
		}

		internal Types.AchievementState State()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_State(SelfPtr());
		}

		internal uint TotalSteps()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_TotalSteps(SelfPtr());
		}

		internal Types.AchievementType Type()
		{
			return GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Type(SelfPtr());
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.Achievement.Achievement_Dispose(selfPointer);
		}

		internal GooglePlayGames.BasicApi.Achievement AsAchievement()
		{
			GooglePlayGames.BasicApi.Achievement achievement = new GooglePlayGames.BasicApi.Achievement();
			achievement.Id = Id();
			achievement.Name = Name();
			achievement.Description = Description();
			if (Type() == Types.AchievementType.INCREMENTAL)
			{
				achievement.IsIncremental = true;
				achievement.CurrentSteps = (int)CurrentSteps();
				achievement.TotalSteps = (int)TotalSteps();
			}
			achievement.IsRevealed = State() == Types.AchievementState.REVEALED;
			achievement.IsUnlocked = State() == Types.AchievementState.UNLOCKED;
			return achievement;
		}
	}
}
