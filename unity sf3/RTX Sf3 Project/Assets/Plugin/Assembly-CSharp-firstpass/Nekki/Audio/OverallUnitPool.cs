using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Audio
{
	internal class OverallUnitPool
	{
		public static int MAX_CHANELS = 16;

		private static readonly List<AudioUnit> _sources = new List<AudioUnit>();

		internal static void Init(AudioManager manager)
		{
			GameObject gameObject = new GameObject(string.Format("_pool ({0} max)", MAX_CHANELS));
			gameObject.transform.parent = manager.transform;
			for (int i = 0; i < MAX_CHANELS; i++)
			{
				_sources.Add(gameObject.AddComponent<AudioUnit>());
			}
		}

		internal static AudioUnit GetIdle()
		{
			for (int i = 0; i < _sources.Count; i++)
			{
				if (_sources[i].Idle)
				{
					_sources[i].Free();
					return _sources[i];
				}
			}
			return null;
		}
	}
}
