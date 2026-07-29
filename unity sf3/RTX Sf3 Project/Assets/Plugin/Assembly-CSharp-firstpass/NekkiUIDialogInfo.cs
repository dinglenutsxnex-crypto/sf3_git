using UnityEngine;

public class NekkiUIDialogInfo : MonoBehaviour
{
	[SerializeField]
	private UIWidget _modalWidget;

	[SerializeField]
	private UIWidget _bg;

	[SerializeField]
	private UIWidget _modalBg;

	[SerializeField]
	private UIPanel _contentPanel;

	public UIWidget ModalWidget
	{
		get
		{
			return _modalWidget;
		}
	}

	public UIWidget Bg
	{
		get
		{
			return _bg;
		}
	}

	public UIWidget ModalBg
	{
		get
		{
			return _modalBg;
		}
	}

	public UIPanel ContentPanel
	{
		get
		{
			return _contentPanel;
		}
	}
}
