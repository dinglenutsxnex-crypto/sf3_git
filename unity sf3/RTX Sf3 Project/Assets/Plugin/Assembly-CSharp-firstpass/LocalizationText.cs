using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
	[SerializeField]
	private string alias;

	[SerializeField]
	private string[] args;

	private UnityEngine.UI.Text textComponent;

	public void SetAlias(string mewAlias)
	{
		alias = mewAlias;
		UpdateText();
	}

	public void SetAlias(string newAlias, string[] newArgs)
	{
		args = newArgs;
		SetAlias(newAlias);
	}

	public string GetAlias()
	{
		return alias;
	}

	public void SetColor(Color color)
	{
		textComponent.color = color;
	}

	private void Awake()
	{
		Localization.LanguageSwitched += UpdateText;
		UpdateText();
	}

	private void InitTextComponent()
	{
		if (!(textComponent != null))
		{
			textComponent = GetComponent<UnityEngine.UI.Text>();
			if (textComponent == null)
			{
				Debug.LogError("Text component not found");
			}
		}
	}

	private void Destroy()
	{
		Localization.LanguageSwitched -= UpdateText;
	}

	private void UpdateText()
	{
		InitTextComponent();
		string text = Localization.Get(alias).String;
		if (args.Length > 0)
		{
			text = string.Format(text, args);
		}
		textComponent.text = text;
	}
}
