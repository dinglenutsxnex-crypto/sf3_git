using System;
using System.Collections.Generic;
using UnityEngine;

public class NekkiUIElements : ExtentionBehaviour
{
	[Serializable]
	public class Element
	{
		[SerializeField]
		public string Name;

		[SerializeField]
		public MonoBehaviour Widget;
	}

	[SerializeField]
	public List<Element> Elements;

	public MonoBehaviour this[string index]
	{
		get
		{
			if (Elements != null)
			{
				for (int i = 0; i < Elements.Count; i++)
				{
					if (Elements[i].Name.Equals(index))
					{
						return Elements[i].Widget;
					}
				}
			}
			return null;
		}
	}

	public UIButton GetButton(string element)
	{
		return Get<UIButton>(element);
	}

	public UIGrid GetGrid(string element)
	{
		return Get<UIGrid>(element);
	}

	public T Get<T>(string element) where T : MonoBehaviour
	{
		T val = this[element] as T;
		if (!val && (bool)this[element])
		{
			val = this[element].GetComponent<T>();
		}
		return val;
	}
}
