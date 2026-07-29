using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneConfig : MonoBehaviour
{
	[Serializable]
	public class BaseProperty
	{
		public string Name;

		public int Index;

		public BaseProperty()
		{
			Name = "Property";
		}

		public BaseProperty(string name)
		{
			Name = name;
		}
	}

	[Serializable]
	public class NoneProperty : BaseProperty
	{
		public int Value;
	}

	[Serializable]
	public class IntProperty : BaseProperty
	{
		public int Value;
	}

	[Serializable]
	public class FloatProperty : BaseProperty
	{
		public float Value;
	}

	[Serializable]
	public class StringProperty : BaseProperty
	{
		public string Value;
	}

	[Serializable]
	public class BoolProperty : BaseProperty
	{
		public bool Value;
	}

	[Serializable]
	public class IntArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<int> Value;
	}

	[Serializable]
	public class FloatArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<float> Value;
	}

	[Serializable]
	public class StringArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<string> Value;
	}

	[Serializable]
	public class BoolArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<bool> Value;
	}

	[Serializable]
	public class PropertyArrayProperty : BaseProperty
	{
		[SerializeField]
		public List<BaseProperty> Value;
	}

	public string SceneName;

	[SerializeField]
	public List<NoneProperty> NoneProperties;

	[SerializeField]
	public List<IntProperty> IntProperties;

	[SerializeField]
	public List<FloatProperty> FloatProperties;

	[SerializeField]
	public List<StringProperty> StringProperties;

	[SerializeField]
	public List<BoolProperty> BoolProperties;

	[SerializeField]
	public List<IntArrayProperty> IntArrayProperties;

	[SerializeField]
	public List<FloatArrayProperty> FloatArrayProperties;

	[SerializeField]
	public List<StringArrayProperty> StringArrayProperties;

	[SerializeField]
	public List<BoolArrayProperty> BoolArrayProperties;

	[SerializeField]
	public List<PropertyArrayProperty> PropertyArrayProperties;

	public bool IsConfig;

	private static SceneConfig _instance;

	private float _leftBorderX;

	private float _rightBorderX;

	private float _centerX;

	private Vector3 _spawnPointEnemy;

	private Vector3 _spawnPointPlayer;

	private float _maxDistBetweenModels;

	private float _locationRightBorder;

	private float _locationLeftBorder;

	private float _camZOffset;

	public static SceneConfig Instance
	{
		get
		{
			return _instance;
		}
	}

	public static bool IsPresent
	{
		get
		{
			return _instance;
		}
	}

	public static float LeftBorderX
	{
		get
		{
			return _instance._leftBorderX;
		}
		set
		{
			_instance._leftBorderX = value;
		}
	}

	public static float RightBorderX
	{
		get
		{
			return _instance._rightBorderX;
		}
		set
		{
			_instance._rightBorderX = value;
		}
	}

	public static float CenterX
	{
		get
		{
			return _instance._centerX;
		}
		private set
		{
			_instance._centerX = value;
		}
	}

	public static Vector3 SpawnPointEnemy
	{
		get
		{
			return _instance._spawnPointEnemy;
		}
		private set
		{
			_instance._spawnPointEnemy = value;
		}
	}

	public static Vector3 SpawnPointPlayer
	{
		get
		{
			return _instance._spawnPointPlayer;
		}
		private set
		{
			_instance._spawnPointPlayer = value;
		}
	}

	public static float PointFloor
	{
		get
		{
			return _instance._spawnPointPlayer.y;
		}
	}

	public static float MaxDistBetweenModels
	{
		get
		{
			return _instance._maxDistBetweenModels;
		}
		private set
		{
			_instance._maxDistBetweenModels = value;
		}
	}

	public static float LocationRightBorder
	{
		get
		{
			return _instance._locationRightBorder;
		}
		private set
		{
			_instance._locationRightBorder = value;
		}
	}

	public static float LocationLeftBorder
	{
		get
		{
			return _instance._locationLeftBorder;
		}
		private set
		{
			_instance._locationLeftBorder = value;
		}
	}

	public static float CamZOffset
	{
		get
		{
			return _instance._camZOffset;
		}
		private set
		{
			_instance._camZOffset = value;
		}
	}

	public int GetInt(string property)
	{
		for (int i = 0; i < IntProperties.Count; i++)
		{
			if (IntProperties[i].Name.Equals(property))
			{
				return IntProperties[i].Value;
			}
		}
		return 0;
	}

	public float GetFloat(string property)
	{
		for (int i = 0; i < FloatProperties.Count; i++)
		{
			if (FloatProperties[i].Name.Equals(property))
			{
				return FloatProperties[i].Value;
			}
		}
		return 0f;
	}

	public string GetString(string property)
	{
		for (int i = 0; i < StringProperties.Count; i++)
		{
			if (StringProperties[i].Name.Equals(property))
			{
				return StringProperties[i].Value;
			}
		}
		return string.Empty;
	}

	public bool GetBool(string property)
	{
		for (int i = 0; i < BoolProperties.Count; i++)
		{
			if (BoolProperties[i].Name.Equals(property))
			{
				return BoolProperties[i].Value;
			}
		}
		return false;
	}

	public List<int> GetIntArray(string property)
	{
		for (int i = 0; i < IntArrayProperties.Count; i++)
		{
			if (IntArrayProperties[i].Name.Equals(property))
			{
				return IntArrayProperties[i].Value;
			}
		}
		return new List<int>();
	}

	public List<float> GetFloatArray(string property)
	{
		for (int i = 0; i < FloatArrayProperties.Count; i++)
		{
			if (FloatArrayProperties[i].Name.Equals(property))
			{
				return FloatArrayProperties[i].Value;
			}
		}
		return new List<float>();
	}

	public List<string> GetStringArray(string property)
	{
		for (int i = 0; i < StringArrayProperties.Count; i++)
		{
			if (StringArrayProperties[i].Name.Equals(property))
			{
				return StringArrayProperties[i].Value;
			}
		}
		return new List<string>();
	}

	public List<bool> GetBoolArray(string property)
	{
		for (int i = 0; i < BoolArrayProperties.Count; i++)
		{
			if (BoolArrayProperties[i].Name.Equals(property))
			{
				return BoolArrayProperties[i].Value;
			}
		}
		return new List<bool>();
	}

	private void Awake()
	{
		_instance = this;
		if (!IsConfig)
		{
			float locationLeftBorder = (LeftBorderX = base.transform.Find(GetString("LeftBorder")).position.x);
			LocationLeftBorder = locationLeftBorder;
			locationLeftBorder = (RightBorderX = base.transform.Find(GetString("RightBorder")).position.x);
			LocationRightBorder = locationLeftBorder;
			CenterX = (RightBorderX + LeftBorderX) / 2f;
			SpawnPointEnemy = base.transform.Find(GetString("SpawnPointA")).position;
			SpawnPointPlayer = base.transform.Find(GetString("SpawnPointB")).position;
			MaxDistBetweenModels = GetFloat("MaxDistBetweenModels");
			CamZOffset = GetFloat("CamZOffset");
		}
	}
}
