using System.Collections.Generic;
using UnityEngine;

public class MultiTweenTransition : MonoBehaviour
{
	public UITweener[] Tweens;

	private bool _initDone;

	public bool Automatic = true;

	internal void Start()
	{
		if (Automatic)
		{
			List<UITweener> list = new List<UITweener>();
			list.AddRange(GetComponentsInChildren<UITweener>());
			UITweener[] components = GetComponents<UITweener>();
			for (int i = 0; i < components.Length; i++)
			{
				if (!list.Contains(components[i]))
				{
					list.Add(components[i]);
				}
			}
			Tweens = list.ToArray();
		}
		_initDone = true;
	}

	public void TweenOut()
	{
		if (!_initDone)
		{
			Start();
		}
		for (int i = 0; i < Tweens.Length; i++)
		{
			Tweens[i].PlayReverse();
		}
	}

	public void TweenIn()
	{
		if (!_initDone)
		{
			Start();
		}
		for (int i = 0; i < Tweens.Length; i++)
		{
			Tweens[i].PlayForward();
		}
	}
}
