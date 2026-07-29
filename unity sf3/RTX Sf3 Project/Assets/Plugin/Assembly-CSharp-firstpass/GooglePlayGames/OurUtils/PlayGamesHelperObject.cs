using System;
using System.Collections.Generic;
using UnityEngine;

namespace GooglePlayGames.OurUtils
{
	public class PlayGamesHelperObject : MonoBehaviour
	{
		private static PlayGamesHelperObject instance = null;

		private static bool sIsDummy = false;

		private static List<Action> sQueue = new List<Action>();

		private static volatile bool sQueueEmpty = true;

		private static Action<bool> sPauseCallback = null;

		private static Action<bool> sFocusCallback = null;

		public static void CreateObject()
		{
			if (!(instance != null))
			{
				if (Application.isPlaying)
				{
					GameObject gameObject = new GameObject("PlayGames_QueueRunner");
					StaticObjectsManager.AddObject(gameObject, false);
					instance = gameObject.AddComponent<PlayGamesHelperObject>();
				}
				else
				{
					instance = new PlayGamesHelperObject();
					sIsDummy = true;
				}
			}
		}

		public void Awake()
		{
			StaticObjectsManager.AddObject(base.gameObject, false);
		}

		public void OnDisable()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		public static void RunOnGameThread(Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (sIsDummy)
			{
				return;
			}
			lock (sQueue)
			{
				sQueue.Add(action);
				sQueueEmpty = false;
			}
		}

		public void Update()
		{
			if (!sIsDummy && !sQueueEmpty)
			{
				List<Action> list = new List<Action>();
				lock (sQueue)
				{
					list.AddRange(sQueue);
					sQueue.Clear();
					sQueueEmpty = true;
				}
				list.ForEach(delegate(Action a)
				{
					a();
				});
			}
		}

		public void OnApplicationFocus(bool focused)
		{
			if (sFocusCallback != null)
			{
				sFocusCallback(focused);
			}
		}

		public void OnApplicationPause(bool paused)
		{
			if (sPauseCallback != null)
			{
				sPauseCallback(paused);
			}
		}

		public static void SetFocusCallback(Action<bool> callback)
		{
			sFocusCallback = callback;
		}

		public static void SetPauseCallback(Action<bool> callback)
		{
			sPauseCallback = callback;
		}
	}
}
