using System;
using UnityEngine;

namespace Nekki.Audio
{
	public class AudioSettings
	{
		public delegate void VolumeChanged(float volume);

		private float _masterVolume;

		private float _soundsVolume;

		private float _musicVolume;

		private bool _muted;

		public bool Muted
		{
			get
			{
				return _muted;
			}
			set
			{
				if (value != _muted)
				{
					_muted = value;
					OnSoundsVolumeChanged(SoundVolume);
					OnMusicVolumeChanged(SoundVolume);
					PlayerPrefs.SetInt("_muted", _muted ? 1 : 0);
					PlayerPrefs.Save();
				}
			}
		}

		public float MasterVolume
		{
			get
			{
				return (!Muted) ? _masterVolume : 0f;
			}
			set
			{
				value = Mathf.Clamp01(value);
				if (!(Math.Abs(value - _masterVolume) < 0.01f))
				{
					_masterVolume = value;
					OnSoundsVolumeChanged(SoundVolume);
					OnMusicVolumeChanged(SoundVolume);
					PlayerPrefs.SetFloat("_masterVolume", _masterVolume);
					PlayerPrefs.Save();
				}
			}
		}

		public float SoundVolume
		{
			get
			{
				return _soundsVolume * MasterVolume;
			}
			set
			{
				value = Mathf.Clamp01(value);
				if (!(Math.Abs(value - _soundsVolume) < 0.01f))
				{
					_soundsVolume = Mathf.Clamp01(value);
					OnSoundsVolumeChanged(SoundVolume);
					PlayerPrefs.SetFloat("_soundsVolume", _soundsVolume);
					PlayerPrefs.Save();
				}
			}
		}

		public float MusicVolume
		{
			get
			{
				return _musicVolume * MasterVolume;
			}
			set
			{
				value = Mathf.Clamp01(value);
				if (!(Math.Abs(value - _musicVolume) < 0.01f))
				{
					_musicVolume = Mathf.Clamp01(value);
					OnMusicVolumeChanged(SoundVolume);
					PlayerPrefs.SetFloat("_musicVolume", _musicVolume);
					PlayerPrefs.Save();
				}
			}
		}

		public event VolumeChanged SoundsVolumeChanged;

		public event VolumeChanged MusicVolumeChanged;

		internal AudioSettings()
		{
			_masterVolume = PlayerPrefs.GetFloat("_masterVolume", 1f);
			_musicVolume = PlayerPrefs.GetFloat("_musicVolume", 1f);
			_soundsVolume = PlayerPrefs.GetFloat("_soundsVolume", 1f);
			_muted = PlayerPrefs.GetInt("_muted", 0) == 1;
		}

		protected virtual void OnSoundsVolumeChanged(float volume)
		{
			if (this.SoundsVolumeChanged != null)
			{
				this.SoundsVolumeChanged(volume);
			}
		}

		protected virtual void OnMusicVolumeChanged(float volume)
		{
			if (this.MusicVolumeChanged != null)
			{
				this.MusicVolumeChanged(volume);
			}
		}
	}
}
