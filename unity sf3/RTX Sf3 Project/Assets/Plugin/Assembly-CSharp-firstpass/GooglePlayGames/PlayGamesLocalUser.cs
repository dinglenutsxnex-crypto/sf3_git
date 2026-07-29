using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames
{
	public class PlayGamesLocalUser : PlayGamesUserProfile, ILocalUser, IUserProfile
	{
		internal PlayGamesPlatform mPlatform;

		private WWW mAvatarUrl;

		private Texture2D mImage;

		public IUserProfile[] friends
		{
			get
			{
				return new IUserProfile[0];
			}
		}

		public bool authenticated
		{
			get
			{
				return mPlatform.IsAuthenticated();
			}
		}

		public bool underage
		{
			get
			{
				return true;
			}
		}

		public new string userName
		{
			get
			{
				return (!authenticated) ? string.Empty : mPlatform.GetUserDisplayName();
			}
		}

		public new string id
		{
			get
			{
				return (!authenticated) ? string.Empty : mPlatform.GetUserId();
			}
		}

		public string idToken
		{
			get
			{
				return (!authenticated) ? string.Empty : mPlatform.GetIdToken();
			}
		}

		public new bool isFriend
		{
			get
			{
				return true;
			}
		}

		public new UserState state
		{
			get
			{
				return UserState.Online;
			}
		}

		public new Texture2D image
		{
			get
			{
				return LoadImage();
			}
		}

		internal PlayGamesLocalUser(PlayGamesPlatform plaf)
		{
			mPlatform = plaf;
			mAvatarUrl = null;
			mImage = null;
		}

		public void Authenticate(Action<bool, string> callback)
		{
		}

		public void Authenticate(Action<bool> callback)
		{
			mPlatform.Authenticate(callback);
		}

		public void Authenticate(Action<bool> callback, bool silent)
		{
			mPlatform.Authenticate(callback, silent);
		}

		public void LoadFriends(Action<bool> callback)
		{
			if (callback != null)
			{
				callback(false);
			}
		}

		private Texture2D LoadImage()
		{
			string userImageUrl = mPlatform.GetUserImageUrl();
			if (!string.IsNullOrEmpty(userImageUrl))
			{
				if (mAvatarUrl == null || mAvatarUrl.url != userImageUrl)
				{
					mAvatarUrl = new WWW(userImageUrl);
					mImage = null;
				}
				if (mImage != null)
				{
					return mImage;
				}
				if (mAvatarUrl.isDone)
				{
					mImage = mAvatarUrl.texture;
					return mImage;
				}
			}
			return null;
		}
	}
}
