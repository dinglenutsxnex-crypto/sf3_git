using GooglePlayGames.BasicApi;

namespace GooglePlayGames.Native
{
	internal interface AppStateClient
	{
		void LoadState(int slot, OnStateLoadedListener listener);

		void UpdateState(int slot, byte[] data, OnStateLoadedListener listener);
	}
}
