using System.Collections.Generic;
using UnityEngine;

public class TexturesUtils
{
	private static readonly Dictionary<string, Sprite[]> AtlasesCache = new Dictionary<string, Sprite[]>();

	private static readonly Dictionary<string, string> AtlasesNames = new Dictionary<string, string>();

	private static readonly Dictionary<int, int> TexturesLoaded = new Dictionary<int, int>();

	private static readonly List<Texture> TexturesToUnload = new List<Texture>();

	public static void Init()
	{
		Routiner.AddUpdate(Update);
	}

	public static Sprite CreateSprite(Texture2D texture)
	{
		return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}

	public static int GetCountTexture(Texture texture)
	{
		if (texture != null)
		{
			int instanceID = texture.GetInstanceID();
			if (TexturesLoaded.ContainsKey(instanceID))
			{
				return TexturesLoaded[instanceID];
			}
			if (TexturesToUnload.Contains(texture))
			{
				return 0;
			}
		}
		return -1;
	}

	public static void AddTexture(Texture texture)
	{
		if (texture == null)
		{
			return;
		}
		int instanceID = texture.GetInstanceID();
		if (TexturesLoaded.ContainsKey(instanceID))
		{
			TexturesLoaded[instanceID]++;
		}
		else
		{
			if (TexturesToUnload.Contains(texture))
			{
				TexturesToUnload.Remove(texture);
			}
			TexturesLoaded.Add(instanceID, 1);
		}
		Log("AddTexture " + texture.name + " " + GetCountTexture(texture));
	}

	public static void ReleaseTexture(Texture texture)
	{
		if (texture == null)
		{
			return;
		}
		int instanceID = texture.GetInstanceID();
		if (TexturesLoaded.ContainsKey(instanceID))
		{
			if (TexturesLoaded[instanceID] > 1)
			{
				TexturesLoaded[instanceID]--;
			}
			else
			{
				TexturesLoaded.Remove(instanceID);
				TexturesToUnload.Add(texture);
			}
			Log("ReleaseTexture " + texture.name + " " + GetCountTexture(texture));
		}
	}

	private static void Update()
	{
		if (TexturesToUnload.Count <= 0)
		{
			return;
		}
		Texture texture = TexturesToUnload[0];
		if (texture != null)
		{
			if (AtlasesNames.ContainsKey(texture.name))
			{
				AtlasesCache.Remove(AtlasesNames[texture.name]);
				AtlasesNames.Remove(texture.name);
				Log("UnloadAtlas " + texture.name);
			}
			Log("DestroyTexture " + texture.name);
			GlobalLoad.Unload(texture);
		}
		TexturesToUnload.Remove(texture);
	}

	public static Sprite GetSpriteFromAtlas(string internalName, string atlasPath, string spriteName)
	{
		return GetSpriteFromAtlas(LoadAtlas(internalName, atlasPath), spriteName);
	}

	public static Sprite GetSpriteFromAtlas(string atlasPath, string spriteName)
	{
		return GetSpriteFromAtlas(LoadAtlas(atlasPath, string.Empty), spriteName);
	}

	private static Sprite GetSpriteFromAtlas(Sprite[] sprites, string spriteName)
	{
		if (sprites != null && sprites.Length > 0)
		{
			foreach (Sprite sprite in sprites)
			{
				if (sprite.name.Equals(spriteName))
				{
					return sprite;
				}
			}
		}
		Log("Sprite From Atlas Not Found  - " + spriteName);
		return GlobalLoad.NoImageSprite;
	}

	private static Sprite[] LoadAtlas(string path, string name = "")
	{
		if (!AtlasesCache.ContainsKey(path))
		{
			Sprite[] array = ((!name.IsNullOrEmpty()) ? GlobalLoad.GetLoadObjectsInternal<Sprite>(path, name) : GlobalLoad.GetLoadObjects<Sprite>(path));
			if (array != null && array.Length > 0)
			{
				AtlasesCache.Add(path, array);
				AtlasesNames.Add(array[0].texture.name, path);
				Log("LoadAtlas " + path);
			}
			return array;
		}
		return AtlasesCache[path];
	}

	private static void Log(string message)
	{
	}
}
