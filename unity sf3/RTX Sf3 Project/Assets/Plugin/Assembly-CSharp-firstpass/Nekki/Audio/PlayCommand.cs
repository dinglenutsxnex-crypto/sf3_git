using UnityEngine;

namespace Nekki.Audio
{
	public class PlayCommand
	{
		private float _volume;

		private AudioSettings _settings;

		public int ChanelID { get; private set; }

		public string Sound { get; private set; }

		public bool Loop { get; private set; }

		public bool MultiSource { get; private set; }

		public bool IsMusic { get; private set; }

		public float Volume
		{
			get
			{
				return _volume * ((!IsMusic) ? _settings.SoundVolume : _settings.MusicVolume);
			}
			set
			{
				_volume = Mathf.Clamp01(value);
			}
		}

		public PlayCommand(int chanelID, string sound, bool loop, bool multiSource, float volume)
		{
			Volume = volume;
			MultiSource = multiSource;
			Loop = loop;
			Sound = sound;
			ChanelID = chanelID;
		}

		internal void UpdateType(bool isMusic)
		{
			IsMusic = isMusic;
		}

		internal void SetCurrentSettings(AudioSettings settings)
		{
			_settings = settings;
		}
	}
}
