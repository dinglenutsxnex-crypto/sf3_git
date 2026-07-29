using UnityEngine;

[RequireComponent(typeof(UIPopupList))]
[AddComponentMenu("NGUI/Interaction/Language Selection")]
public class LanguageSelection : MonoBehaviour
{
	private UIPopupList mList;

	private void Start()
	{
		mList = GetComponent<UIPopupList>();
		if (NguiLocalization.knownLanguages != null)
		{
			mList.items.Clear();
			int i = 0;
			for (int num = NguiLocalization.knownLanguages.Length; i < num; i++)
			{
				mList.items.Add(NguiLocalization.knownLanguages[i]);
			}
			mList.value = NguiLocalization.language;
		}
		EventDelegate.Add(mList.onChange, OnChange);
	}

	private void OnChange()
	{
		NguiLocalization.language = UIPopupList.current.value;
	}
}
