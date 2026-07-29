using UnityEngine;

namespace Nekki.UI
{
	[RequireComponent(typeof(UIWidget))]
	public class NekkiUIHitAlphaTest : MonoBehaviour
	{
		public UIWidget alphaTestWidget;

		[Range(0.01f, 1f)]
		public float threshold = 0.5f;

		private void Start()
		{
			UIWidget component = GetComponent<UIWidget>();
			component.hitCheck = HitCheck;
			if (alphaTestWidget == null)
			{
				alphaTestWidget = component;
			}
		}

		private bool HitCheck(Vector3 worldPos)
		{
			Vector3 pos = alphaTestWidget.cachedTransform.InverseTransformPoint(worldPos);
			Texture2D tex = alphaTestWidget.mainTexture as Texture2D;
			for (int i = 0; i < alphaTestWidget.geometry.verts.size; i += 4)
			{
				if ((CheckTriangle(pos, alphaTestWidget.geometry.verts.buffer[i], alphaTestWidget.geometry.verts.buffer[i + 1], alphaTestWidget.geometry.verts.buffer[i + 2], alphaTestWidget.geometry.uvs.buffer[i], alphaTestWidget.geometry.uvs.buffer[i + 1], alphaTestWidget.geometry.uvs.buffer[i + 2], tex) || CheckTriangle(pos, alphaTestWidget.geometry.verts.buffer[i], alphaTestWidget.geometry.verts.buffer[i + 2], alphaTestWidget.geometry.verts.buffer[i + 3], alphaTestWidget.geometry.uvs.buffer[i], alphaTestWidget.geometry.uvs.buffer[i + 2], alphaTestWidget.geometry.uvs.buffer[i + 3], tex)) && (CheckTriangle(pos, alphaTestWidget.geometry.verts.buffer[i], alphaTestWidget.geometry.verts.buffer[i + 2], alphaTestWidget.geometry.verts.buffer[i + 1], alphaTestWidget.geometry.uvs.buffer[i], alphaTestWidget.geometry.uvs.buffer[i + 2], alphaTestWidget.geometry.uvs.buffer[i + 1], tex) || CheckTriangle(pos, alphaTestWidget.geometry.verts.buffer[i], alphaTestWidget.geometry.verts.buffer[i + 3], alphaTestWidget.geometry.verts.buffer[i + 2], alphaTestWidget.geometry.uvs.buffer[i], alphaTestWidget.geometry.uvs.buffer[i + 3], alphaTestWidget.geometry.uvs.buffer[i + 2], tex)))
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckTriangle(Vector3 pos, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3, Texture2D tex)
		{
			Vector3 vector = Vector3.Cross(v3 - v1, v2 - v1);
			if (vector.sqrMagnitude < 0.01f)
			{
				return false;
			}
			pos -= Vector3.Dot(pos - v1, vector) * vector;
			float magnitude = vector.magnitude;
			float num = Vector3.Cross(v2 - pos, v3 - pos).magnitude / magnitude;
			if (num < 0f || num > 1f)
			{
				return false;
			}
			float num2 = Vector3.Cross(v3 - pos, v1 - pos).magnitude / magnitude;
			if (num2 < 0f || num2 > 1f)
			{
				return false;
			}
			float num3 = 1f - num - num2;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			Vector2 vector2 = uv1 * num + uv2 * num2 + uv3 * num3;
			return tex.GetPixel((int)(vector2.x * (float)tex.width), (int)(vector2.y * (float)tex.height)).a >= threshold;
		}
	}
}
