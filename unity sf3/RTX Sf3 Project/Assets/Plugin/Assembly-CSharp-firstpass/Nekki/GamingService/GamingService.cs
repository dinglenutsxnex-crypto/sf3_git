using System;

namespace Nekki.GamingService
{
	public class GamingService
	{
		protected bool DEBUG_LOG = true;

		private static GamingService _instance;

		public static GamingService Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GamingService();
				}
				return _instance;
			}
		}

		public virtual void authorize(Action<bool> callback)
		{
			callback(false);
		}

		public virtual string getAccount()
		{
			return string.Empty;
		}

		public virtual bool isAutorized()
		{
			return false;
		}

		public virtual void logout()
		{
		}

		public virtual string getVerificationToken()
		{
			return string.Empty;
		}
	}
}
