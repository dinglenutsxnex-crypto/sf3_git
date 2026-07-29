using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Nekki.Audio
{
	public class AudioManager : MonoBehaviour
	{
		private const int DefaultMusicChanel = 0;

		private const int DefaultSoundChanel = 1;

		private static AudioManager _instance;

		private static readonly Dictionary<int, Chanel> Chanels = new Dictionary<int, Chanel>();

		private static List<int> _musicChanels;

		private static Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();

		private static Dictionary<string, float> _volumesByClips = new Dictionary<string, float>();

		private static bool _IsMute = false;

		private static AudioSettings _settings;

		public static void Init(string folder, int[] musicChanels, int[] soundChanels)
		{
			if ((bool)_instance)
			{
				Debug.LogWarning("AudioManager already exists!");
				return;
			}
			_musicChanels = new List<int>(musicChanels);
			_instance = new GameObject("_audioManager").AddComponent<AudioManager>();
			StaticObjectsManager.AddObject(_instance.gameObject, false);
			Load(folder);
			OverallUnitPool.Init(_instance);
			_settings = new AudioSettings();
		}

		public static void Init(string folder, int musicChanel, int[] soundChanels)
		{
			Init(folder, new int[1] { musicChanel }, soundChanels);
		}

		public static void Init(string folder, int musicChanel, int soundChanel)
		{
			Init(folder, new int[1] { musicChanel }, new int[1] { soundChanel });
		}

		public static void Init(string folder)
		{
			Init(folder, new int[1], new int[1] { 1 });
		}

		private static void Load(string folder)
		{
			if (Directory.Exists(folder))
			{
				string[] directories = Directory.GetDirectories(folder);
				for (int i = 0; i < directories.Length; i++)
				{
					Load(directories[i]);
				}
				List<string> list = new List<string>(Directory.GetFiles(folder, "*.xml"));
				for (int j = 0; j < list.Count; j++)
				{
					LoadXML(list[j], folder);
				}
			}
		}

		private static void LoadXML(string file, string folder)
		{
			if (!File.Exists(file))
			{
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(File.ReadAllText(file));
			}
			catch (Exception ex)
			{
				Debug.LogWarning("wrong xml: " + ex.Message);
				return;
			}
			XmlElement xmlElement = xmlDocument["Sounds"];
			if (xmlElement == null)
			{
				return;
			}
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.Attributes == null)
				{
					continue;
				}
				string value = childNode.Attributes["Name"].Value;
				string text = childNode.Attributes["File"].Value.Replace("\\", "/");
				float value2 = ((childNode.Attributes["Volume"] != null) ? float.Parse(childNode.Attributes["Volume"].Value) : 1f);
				if (!string.IsNullOrEmpty(text))
				{
					string clipPath = folder + "/" + text;
					AddAudioClip(value, clipPath);
					if (_volumesByClips.ContainsKey(value))
					{
						_volumesByClips[value] = value2;
					}
					else
					{
						_volumesByClips.Add(value, value2);
					}
				}
			}
		}

		private static void AddAudioClip(string sound, string clipPath)
		{
			if (!File.Exists(clipPath))
			{
				Debug.Log("No" + clipPath);
				return;
			}
			AudioClip audioClip = GeAudioClip(clipPath);
			if ((bool)audioClip)
			{
				if (!_clips.ContainsKey(sound))
				{
					_clips.Add(sound, audioClip);
				}
				else
				{
					_clips[sound] = audioClip;
				}
			}
		}

		private static AudioClip GeAudioClip(string path)
		{
			WWW wWW = new WWW(string.Format("file:///{0}", path));
			while (!wWW.isDone && string.IsNullOrEmpty(wWW.error))
			{
			}
			if (string.IsNullOrEmpty(wWW.error))
			{
				return wWW.GetAudioClip();
			}
			Debug.LogError(wWW.error);
			return null;
		}

		public static void AddAudio(AudioClip clip, string name, float volume)
		{
			if (_clips.ContainsKey(name))
			{
				_clips[name] = clip;
			}
			else
			{
				_clips.Add(name, clip);
			}
			if (_volumesByClips.ContainsKey(name))
			{
				_volumesByClips[name] = volume;
			}
			else
			{
				_volumesByClips.Add(name, volume);
			}
		}

		public static void UnloadAudio(string name)
		{
			if (_clips.ContainsKey(name))
			{
				_clips.Remove(name);
			}
			if (_volumesByClips.ContainsKey(name))
			{
				_volumesByClips.Remove(name);
			}
		}

		public static void Play(int chanelID, string sound, bool loop, bool multisource, float volume = 1f)
		{
			if (!_IsMute && _clips.ContainsKey(sound))
			{
				if (!Chanels.ContainsKey(chanelID))
				{
					Chanels.Add(chanelID, new Chanel(chanelID, IsMusicChanel(chanelID), _clips));
				}
				PlayCommand playCommand = new PlayCommand(chanelID, sound, loop, multisource, volume * _volumesByClips[sound]);
				playCommand.SetCurrentSettings(_settings);
				Chanels[chanelID].Play(playCommand);
			}
		}

		public static void Play(PlayCommand command)
		{
			if (_IsMute)
			{
				return;
			}
			if (!_instance)
			{
				Debug.LogWarning("you must init AudioManager first!");
				return;
			}
			command.SetCurrentSettings(_settings);
			if (Chanels.ContainsKey(command.ChanelID))
			{
				Chanels.Add(command.ChanelID, new Chanel(command.ChanelID, IsMusicChanel(command.ChanelID), _clips));
			}
			Chanels[command.ChanelID].Play(command);
		}

		public static void Mute()
		{
			Pause(true);
			_IsMute = true;
		}

		public static void UnMute()
		{
			Pause(false);
			_IsMute = false;
		}

		public static void Mute(int chanelID)
		{
			if (!Chanels.ContainsKey(chanelID))
			{
				Chanels.Add(chanelID, new Chanel(chanelID, IsMusicChanel(chanelID), _clips));
			}
			Chanels[chanelID].Mute();
		}

		public static void UnMute(int chanelID)
		{
			if (!Chanels.ContainsKey(chanelID))
			{
				Chanels.Add(chanelID, new Chanel(chanelID, IsMusicChanel(chanelID), _clips));
			}
			Chanels[chanelID].UnMute();
		}

		private static void Pause(bool pause, int chanelID, string soundName)
		{
			if (Chanels.ContainsKey(chanelID))
			{
				Chanels[chanelID].Pause(pause, soundName);
			}
		}

		public static void Pause(bool pause, int chanelID)
		{
			if (Chanels.ContainsKey(chanelID))
			{
				Chanels[chanelID].Pause(pause);
			}
		}

		private static void Pause(bool pause)
		{
			foreach (Chanel value in Chanels.Values)
			{
				value.Pause(pause);
			}
		}

		public static void Stop(int channelID, bool isSmoothly = false)
		{
			if (Chanels.ContainsKey(channelID))
			{
				Chanels[channelID].FreeAllUnit(isSmoothly);
			}
		}

		public static void SetVolume(float volume, int channelID)
		{
			if (!Chanels.ContainsKey(channelID))
			{
				Chanels.Add(channelID, new Chanel(channelID, IsMusicChanel(channelID), _clips));
			}
			Chanels[channelID].MasterVolume = volume;
		}

		internal void Start()
		{
			if ((bool)_instance && _instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private static bool IsMusicChanel(int p_value)
		{
			return _musicChanels.Contains(p_value);
		}

		public static bool IsPlaying(int channelID)
		{
			if (Chanels.ContainsKey(channelID))
			{
				return Chanels[channelID].IsPlaying;
			}
			return false;
		}
	}
}
