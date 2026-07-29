using System;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native
{
	internal class UnsupportedAppStateClient : AppStateClient
	{
		private readonly string mMessage;

		internal UnsupportedAppStateClient(string message)
		{
			mMessage = Misc.CheckNotNull(message);
		}

		public void LoadState(int slot, OnStateLoadedListener listener)
		{
			throw new NotImplementedException(mMessage);
		}

		public void UpdateState(int slot, byte[] data, OnStateLoadedListener listener)
		{
			throw new NotImplementedException(mMessage);
		}
	}
}
