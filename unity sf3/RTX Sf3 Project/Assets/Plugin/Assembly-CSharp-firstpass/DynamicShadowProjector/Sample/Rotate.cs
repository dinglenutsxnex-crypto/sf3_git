using UnityEngine;

namespace DynamicShadowProjector.Sample
{
	public class Rotate : MonoBehaviour
	{
		public float m_rotateSpeed = 90f;

		private void Update()
		{
			base.transform.rotation = Quaternion.AngleAxis(m_rotateSpeed * Time.deltaTime, Vector3.up) * base.transform.rotation;
		}
	}
}
