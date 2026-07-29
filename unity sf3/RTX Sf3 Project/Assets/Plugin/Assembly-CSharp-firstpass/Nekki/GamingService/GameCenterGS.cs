using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Nekki.GamingService
{
	public class GameCenterGS : GamingService
	{
		private bool _wasAutorized;

		private bool _wasAuthSuccessful;

		public override void authorize(Action<bool> callback)
		{
			if (_wasAutorized)
			{
				callback(_wasAuthSuccessful);
				return;
			}
			Authorize(delegate(bool success)
			{
				_wasAutorized = true;
				_wasAuthSuccessful = success;
				callback(_wasAuthSuccessful);
			});
		}

		private static void Authorize(Action<bool> callback)
		{
			try
			{
				UnityEngine.Social.localUser.Authenticate(delegate(bool success)
				{
					if (!success)
					{
						callback(false);
					}
					else
					{
						Debug.Log("Authentication successful");
						string message = "Username: " + UnityEngine.Social.localUser.userName + "\nUser ID: " + UnityEngine.Social.localUser.id + "\nIsUnderage: " + UnityEngine.Social.localUser.underage;
						Debug.Log(message);
						_GameCenterSetup();
						iOSBridge b = iOSBridge.create("_GameCenterBridge");
						b.addCallback("onJsonFinish", delegate
						{
							b.removeInstance();
							callback(true);
						});
					}
				});
			}
			catch (Exception)
			{
				callback(false);
			}
		}

		public override string getAccount()
		{
			return UnityEngine.Social.localUser.userName;
		}

		public override bool isAutorized()
		{
			return UnityEngine.Social.localUser.authenticated;
		}

		public override void logout()
		{
		}

		public override string getVerificationToken()
		{
			return _GameCenterGetVerificationJson();
		}

		[DllImport("__Internal")]
		private static extern bool _GameCenterSetup();

		[DllImport("__Internal")]
		private static extern string _GameCenterGetVerificationJson();
	}
}
