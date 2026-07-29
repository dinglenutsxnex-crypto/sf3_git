using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Event System (UICamera)")]
[RequireComponent(typeof(Camera))]
public class NekkiUICamera : UICamera
{
	public bool onlyLeftBtnTrigger;

	public bool renderCameraToTexture;

	public Transform renderCameraParent;

	public int renderCameraDepth;

	private RenderTexture targetTexture;

	private GameObject textureObject;

	[SerializeField]
	private UITexture texture;

	[SerializeField]
	private bool useCustomTextureSize;

	[SerializeField]
	private int textureWidth;

	[SerializeField]
	private int textureHeight;

	private int GetRenderTextureWidth()
	{
		if (useCustomTextureSize)
		{
			return textureWidth;
		}
		return Screen.width;
	}

	private int GetRenderTextureHeight()
	{
		if (useCustomTextureSize)
		{
			return textureHeight;
		}
		return Screen.height;
	}

	private int GetUITextureWidth()
	{
		if (useCustomTextureSize)
		{
			return textureWidth;
		}
		return NekkiUIRoot.Instance.ScreenWidth;
	}

	private int GetUITextureHeight()
	{
		if (useCustomTextureSize)
		{
			return textureHeight;
		}
		return NekkiUIRoot.Instance.ScreenHeight;
	}

	public override void ProcessTouch(bool pressed, bool unpressed)
	{
		if (Input.touchCount != 0 || ((!pressed || (!Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2)) || !onlyLeftBtnTrigger) && (!unpressed || (!Input.GetMouseButtonUp(1) && !Input.GetMouseButtonUp(2)) || !onlyLeftBtnTrigger)))
		{
			base.ProcessTouch(pressed, unpressed);
		}
	}

	public void RenderAndDisable()
	{
		base.cachedCamera.enabled = true;
		base.cachedCamera.Render();
		StartCoroutine(DisableCamera());
	}

	private IEnumerator DisableCamera()
	{
		yield return new WaitForEndOfFrame();
		base.cachedCamera.enabled = false;
	}

	protected override void Awake()
	{
		base.Awake();
		if (renderCameraToTexture)
		{
			InitRenderTexture();
			InitTextureObject();
			InitTexture();
		}
	}

	private void InitRenderTexture()
	{
		targetTexture = new RenderTexture(GetRenderTextureWidth(), GetRenderTextureHeight(), 16);
		targetTexture.filterMode = FilterMode.Point;
		base.cachedCamera.targetTexture = targetTexture;
		base.cachedCamera.clearFlags = CameraClearFlags.Color;
		Rect rect = base.cachedCamera.rect;
		base.cachedCamera.rect = new Rect(0f, 0f, 1f, 1f);
		base.cachedCamera.Render();
		base.cachedCamera.rect = rect;
	}

	private void InitTextureObject()
	{
		textureObject = new GameObject(base.gameObject.name + "_renderTexture");
		textureObject.layer = LayerMask.NameToLayer("NGUI");
	}

	private void InitTexture()
	{
		if (texture == null)
		{
			texture = textureObject.AddComponent<UITexture>();
			base.cachedCamera.aspect = (float)GetRenderTextureWidth() / (float)GetRenderTextureHeight();
		}
		texture.mainTexture = base.cachedCamera.targetTexture;
		texture.depth = renderCameraDepth;
		texture.transform.parent = renderCameraParent.transform;
		texture.width = GetUITextureWidth();
		texture.height = GetUITextureHeight();
	}

	private void OnDestroy()
	{
		if (renderCameraToTexture)
		{
			Object.Destroy(targetTexture);
			if (texture != null)
			{
				Object.Destroy(texture.gameObject);
			}
		}
	}
}
