using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

public class NekkiConsolePanel : ExtentionBehaviour
{
	public delegate void OnConsoleActiveDelegate(bool state);

	[SerializeField]
	private GameObject _Window;

	[SerializeField]
	private UITextList _TextList;

	[SerializeField]
	private UIInput _Input;

	[SerializeField]
	private UITweener _Tweener;

	private static NekkiConsolePanel _Panel = null;

	private bool _IsTweening;

	private string _Content = string.Empty;

	protected bool _IsInitialized;

	private static readonly List<string> _OutputList = new List<string>();

	private static readonly List<string> _CommandList = new List<string>();

	private int _LastCommandIndex = -1;

	public static NekkiConsolePanel Instance
	{
		get
		{
			return _Panel;
		}
	}

	private bool IsActive
	{
		get
		{
			return _Window.activeSelf;
		}
	}

	public static bool panelExists
	{
		get
		{
			return _Panel != null;
		}
	}

	public static event OnConsoleActiveDelegate OnConsoleActive;

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_Panel = null;
	}

	public static void Log(string p_string)
	{
		if (_Panel != null)
		{
			_Panel.AddText(p_string);
		}
	}

	public static void Clear()
	{
		if (_Panel != null)
		{
			_Panel._TextList.Clear();
		}
	}

	public void OnSubmit()
	{
		string text = NGUIText.StripSymbols(_Input.value);
		string text2 = NekkiConsole.ExecuteCommand(text);
		if (!string.IsNullOrEmpty(text))
		{
			if (!_CommandList.Contains(text))
			{
				_CommandList.Add(text);
			}
			AddText(string.Format("[b]{0}[/b]", text));
			string[] array = text2.Split('\n');
			foreach (string text3 in array)
			{
				AddText(text3);
			}
			_Input.value = string.Empty;
			_Input.isSelected = true;
		}
	}

	public void OnChange()
	{
		if (_IsTweening)
		{
			_Input.value = _Content;
		}
		else
		{
			_Content = _Input.value;
		}
	}

	public void OnButton()
	{
		_IsTweening = true;
		switch (_Tweener.direction)
		{
		case Direction.Forward:
			_Tweener.PlayReverse();
			break;
		case Direction.Reverse:
			if (!IsActive)
			{
				Activate(true);
			}
			_Tweener.PlayForward();
			break;
		}
	}

	public void OnTweenerFinished()
	{
		_IsTweening = false;
		if (_Tweener.direction == Direction.Reverse)
		{
			Activate(false);
		}
	}

	protected void Activate(bool isActive)
	{
		_Window.SetActive(isActive);
		_Input.isSelected = isActive;
		if (_IsInitialized && NekkiConsolePanel.OnConsoleActive != null)
		{
			NekkiConsolePanel.OnConsoleActive(isActive);
		}
	}

	private void AddText(string text)
	{
		_TextList.Add(text);
		_OutputList.Add(text);
	}

	private void Start()
	{
		_Panel = this;
		_Window.SetActive(false);
		_Tweener.PlayReverse();
		foreach (string output in _OutputList)
		{
			_TextList.Add(output);
		}
		_IsInitialized = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			OnButton();
		}
		if (IsActive && _CommandList.Count > 0)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				_LastCommandIndex--;
				_LastCommandIndex = ((_LastCommandIndex >= 0) ? _LastCommandIndex : (_LastCommandIndex = _CommandList.Count - 1));
				_Input.value = _CommandList[_LastCommandIndex];
				_Input.isSelected = true;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				_LastCommandIndex++;
				_LastCommandIndex = ((_LastCommandIndex < _CommandList.Count) ? _LastCommandIndex : (_LastCommandIndex = 0));
				_Input.value = _CommandList[_LastCommandIndex];
				_Input.isSelected = true;
			}
		}
	}
}
