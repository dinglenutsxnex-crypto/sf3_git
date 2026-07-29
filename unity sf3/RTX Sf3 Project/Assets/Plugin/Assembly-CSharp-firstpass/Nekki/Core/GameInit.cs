using System;
using Nekki.Utils;
using UnityEngine;

namespace Nekki.Core
{
	public abstract class GameInit
	{
		public delegate void OnInitializeDoneEventHandler();

		private static bool canContinueInit = true;

		public static bool CanContinueInit
		{
			get
			{
				return canContinueInit;
			}
			set
			{
				canContinueInit = value;
				Debug.LogWarning("[" + Time.time + "] canContinueInit == " + value);
			}
		}

		public event OnInitializeDoneEventHandler InitializeDone;

		private void OnInitializeDone()
		{
			OnInitializeDoneEventHandler initializeDone = this.InitializeDone;
			if (initializeDone != null)
			{
				initializeDone();
			}
		}

		protected void OnNekkiAssetDownloaderInitDone()
		{
			OnInitializeDone();
		}

		public virtual void Subscribe(params Action[] actions)
		{
			foreach (Action action2 in actions)
			{
				Action action = action2;
				InitializeDone += delegate
				{
					action();
				};
			}
		}

		public virtual void Initialize()
		{
			CanContinueInit = true;
			GlobalTimer.Init();
			Application.targetFrameRate = 60;
			OnNekkiAssetDownloaderInitDone();
		}

		public abstract void Init(params Action[] actions);
	}
}
