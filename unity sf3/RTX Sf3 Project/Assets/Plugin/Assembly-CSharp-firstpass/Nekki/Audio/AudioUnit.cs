using UnityEngine;

namespace Nekki.Audio
{
	public class AudioUnit : MonoBehaviour
	{
		private PlayCommand _cmd;

		private AudioSource _source;

		private Chanel _parent;

		private bool _SmoothlyStop;

		internal bool Paused { get; private set; }

		internal bool Idle
		{
			get
			{
				return !_source || ((bool)_source && !_source.isPlaying && !Paused);
			}
		}

		internal void Init(Chanel chanel, PlayCommand cmd, AudioClip clip)
		{
			_cmd = cmd;
			if (!_source)
			{
				_source = base.gameObject.AddComponent<AudioSource>();
				_source.spatialBlend = 0f;
			}
			_source.clip = clip;
			_source.loop = cmd.Loop;
			_source.volume = chanel.MasterVolume * cmd.Volume;
			_parent = chanel;
			Paused = false;
			_source.Play();
		}

		public void Pause()
		{
			if ((bool)_source)
			{
				_source.Pause();
				Paused = true;
			}
		}

		public void UnPause()
		{
			if ((bool)_source)
			{
				_source.Play();
				_source.volume = _parent.MasterVolume * _cmd.Volume;
				Paused = false;
			}
		}

		public void Stop(bool isSmoothly = false)
		{
			if ((bool)_source)
			{
				if (!isSmoothly)
				{
					_source.Stop();
					Paused = false;
				}
				else
				{
					_SmoothlyStop = true;
				}
			}
		}

		internal void Update()
		{
			if (Paused || Idle)
			{
				return;
			}
			if (_SmoothlyStop)
			{
				float num = _source.volume * 0.9f;
				if ((double)num < 0.05)
				{
					Stop();
					_SmoothlyStop = false;
				}
				else
				{
					_source.volume = num;
				}
			}
			else
			{
				_source.volume = _parent.MasterVolume * _cmd.Volume;
			}
		}

		internal void Free()
		{
			if (_parent != null)
			{
				_parent.FreeUnit(this);
			}
		}
	}
}
