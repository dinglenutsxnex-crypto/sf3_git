using UnityEngine;

namespace UnityAndroidNotifications
{
	public class LocalNotification
	{
		private static string fullClassName = "ru.nekki.androidlocalnotifications.LocalNotificationsController";

		public static void Init()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("Init");
				androidJavaClass.CallStatic("RemovePrevNotifications");
			}
			else
			{
				Debug.Log("pluginClass is null!");
			}
		}

		public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
			if (androidJavaClass != null)
			{
				Debug.Log("pluginClass not null");
				androidJavaClass.CallStatic("SetNotification", id, delay * 1000, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, "app_icon", "app_icon", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b);
			}
			else
			{
				Debug.Log("pluginClass is null!");
			}
		}

		public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("SetRepeatingNotification", id, delay * 1000, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, "app_icon", "app_icon", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b);
			}
		}

		public static void CancelNotification(int id)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("CancelNotification", id);
			}
		}

		public static void CancelAllNotifications()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("CancelAllNotifications");
			}
		}

		public static void SavePreferences()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("SavePreferences");
			}
		}
	}
}
