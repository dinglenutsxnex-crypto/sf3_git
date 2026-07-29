using System.Collections.Generic;
using UnityEngine;

namespace Nekki.Audio
{
	internal class Chanel
	{
		private readonly Dictionary<string, AudioUnit> _actve = new Dictionary<string, AudioUnit>();

		private Dictionary<string, AudioClip> _clips;

		public bool IsMute;

		public bool IsMusic { get; private set; }

		public bool IsSound
		{
			get
			{
				return !IsMusic;
			}
		}

		public int ID { get; private set; }

		public bool IsPlaying
		{
			get
			{
				return _actve.Count != 0;
			}
		}

		public float MasterVolume { get; set; }

		internal Chanel(int id, bool isMusic, Dictionary<string, AudioClip> clips)
		{
			IsMusic = isMusic;
			ID = id;
			_clips = clips;
			IsMute = false;
			MasterVolume = 1f;
		}

		internal void Play(PlayCommand command)
		{
			if (IsMute)
			{
				return;
			}
			AudioClip audioClip = ((!_clips.ContainsKey(command.Sound)) ? null : _clips[command.Sound]);
			if (!audioClip)
			{
				return;
			}
			if (!command.MultiSource)
			{
				FreeAllUnit();
			}
			if (_actve.ContainsKey(command.Sound))
			{
				_actve[command.Sound].Init(this, command, audioClip);
				return;
			}
			AudioUnit idle = OverallUnitPool.GetIdle();
			if ((bool)idle)
			{
				idle.Init(this, command, audioClip);
				_actve.Add(command.Sound, idle);
			}
		}

		internal void Pause(bool pause)
		{
			if (IsMute)
			{
				return;
			}
			foreach (AudioUnit value in _actve.Values)
			{
				if (pause)
				{
					value.Pause();
				}
				else
				{
					value.UnPause();
				}
			}
		}

		internal void Pause(bool pause, string sound)
		{
			if (_actve.ContainsKey(sound))
			{
				if (pause)
				{
					_actve[sound].Pause();
				}
				else
				{
					_actve[sound].UnPause();
				}
			}
		}

		public void FreeUnit(AudioUnit audioUnit)
		{
			foreach (KeyValuePair<string, AudioUnit> item in _actve)
			{
				if (item.Value == audioUnit)
				{
					_actve.Remove(item.Key);
					break;
				}
			}
		}

		public void FreeAllUnit(bool isSmoothly = false)
		{
			foreach (AudioUnit value in _actve.Values)
			{
				value.Stop(isSmoothly);
			}
			_actve.Clear();
		}

		public void Mute()
		{
			Pause(true);
			IsMute = true;
		}

		public void UnMute()
		{
			IsMute = false;
			Pause(false);
		}
	}
}
