using UnityEngine;

public class NekkiRectTransformUtility : MonoBehaviour
{
	public static Rect RectTransformToScreenSpace(RectTransform rectTransform)
	{
		Canvas canvas = NekkiUIRoot.Instance.canvas;
		Vector3[] array = new Vector3[4];
		Vector3[] array2 = new Vector3[2];
		rectTransform.GetWorldCorners(array);
		if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
		{
			array2[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, array[0]);
			array2[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, array[2]);
		}
		else
		{
			array2[0] = RectTransformUtility.WorldToScreenPoint(null, array[0]);
			array2[1] = RectTransformUtility.WorldToScreenPoint(null, array[2]);
		}
		return new Rect(array2[0], array2[1] - array2[0]);
	}
}
