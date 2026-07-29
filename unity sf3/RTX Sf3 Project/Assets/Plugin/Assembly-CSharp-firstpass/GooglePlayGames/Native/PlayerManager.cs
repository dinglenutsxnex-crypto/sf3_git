using System;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal class PlayerManager
	{
		internal class FetchSelfResponse : BaseReferenceHolder
		{
			internal FetchSelfResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus Status()
			{
				return GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelfResponse_GetStatus(SelfPtr());
			}

			internal NativePlayer Self()
			{
				return new NativePlayer(GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelfResponse_GetData(SelfPtr()));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelfResponse_Dispose(SelfPtr());
			}

			internal static FetchSelfResponse FromPointer(IntPtr selfPointer)
			{
				if (PInvokeUtilities.IsNull(selfPointer))
				{
					return null;
				}
				return new FetchSelfResponse(selfPointer);
			}
		}

		private readonly GooglePlayGames.Native.PInvoke.GameServices mGameServices;

		internal PlayerManager(GooglePlayGames.Native.PInvoke.GameServices services)
		{
			mGameServices = Misc.CheckNotNull(services);
		}

		internal void FetchSelf(Action<FetchSelfResponse> callback)
		{
			GooglePlayGames.Native.Cwrapper.PlayerManager.PlayerManager_FetchSelf(mGameServices.AsHandle(), Types.DataSource.CACHE_OR_NETWORK, InternalFetchSelfCallback, Callbacks.ToIntPtr(callback, FetchSelfResponse.FromPointer));
		}

		[MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.PlayerManager.FetchSelfCallback))]
		private static void InternalFetchSelfCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("PlayerManager#InternalFetchSelfCallback", Callbacks.Type.Temporary, response, data);
		}
	}
}
