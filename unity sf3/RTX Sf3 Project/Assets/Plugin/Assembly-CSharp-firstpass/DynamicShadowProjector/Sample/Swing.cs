using System;
using UnityEngine;

namespace DynamicShadowProjector.Sample
{
	public class Swing : MonoBehaviour
	{
		public float m_minAngle = -30f;

		public float m_maxAngle = 30f;

		public float m_swingSpeed = 0.1f;

		private Quaternion m_initialRotation;

		private float m_swing;

		private void Start()
		{
			m_initialRotation = base.transform.rotation;
			m_swing = 0f;
		}

		private void Update()
		{
			m_swing += m_swingSpeed * Time.deltaTime;
			m_swing -= Mathf.Floor(m_swing);
			float angle = Mathf.Lerp(m_minAngle, m_maxAngle, 0.5f - 0.5f * Mathf.Cos((float)Math.PI * 2f * m_swing));
			base.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * m_initialRotation;
		}
	}
}
