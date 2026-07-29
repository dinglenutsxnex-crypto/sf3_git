using System.Collections.Generic;
using UnityEngine;

public class AtlasManager : MonoBehaviour
{
	private static Dictionary<string, UIAtlas> _atlases = new Dictionary<string, UIAtlas>();

	public static UIAtlas Get(string atlas)
	{
		if (string.IsNullOrEmpty(atlas))
		{
			return null;
		}
		if (!_atlases.ContainsKey(atlas))
		{
			_atlases.Add(atlas, null);
		}
		if (!_atlases[atlas])
		{
			GameObject prefab = GlobalLoad.GetPrefab(string.Format("Atlases/{0}", atlas));
			if ((bool)prefab)
			{
				_atlases[atlas] = prefab.GetComponent<UIAtlas>();
			}
			else
			{
				Debug.LogError("!there is no atlas " + atlas);
			}
		}
		return _atlases[atlas];
	}
}
