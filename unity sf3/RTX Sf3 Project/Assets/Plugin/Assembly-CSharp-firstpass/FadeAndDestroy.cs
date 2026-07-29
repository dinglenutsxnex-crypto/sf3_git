using UnityEngine;

[RequireComponent(typeof(SlicedSprite))]
public class FadeAndDestroy : MonoBehaviour
{
	public float m_FadeDelay = 1f;

	public float m_FadeTime = 1f;

	public bool m_WaitUntilStationary;

	public float m_StationaryVelocity = 0.2f;

	private SlicedSprite m_SlicedSprite;

	private Rigidbody2D m_RigidBody;

	private Material m_Material;

	private Color m_InitialColor;

	private float m_Timer;

	private void Awake()
	{
		m_SlicedSprite = GetComponent<SlicedSprite>();
		m_RigidBody = m_SlicedSprite.GetComponent<Rigidbody2D>();
		m_Material = m_SlicedSprite.GetComponent<Renderer>().material;
		m_InitialColor = m_Material.color;
	}

	private void Update()
	{
		if (!m_WaitUntilStationary || m_RigidBody.velocity.sqrMagnitude < m_StationaryVelocity * m_StationaryVelocity)
		{
			m_Timer += Time.deltaTime;
			if (m_FadeTime > 0f)
			{
				Color initialColor = m_InitialColor;
				initialColor.a = 1f - Mathf.Clamp01((m_Timer - m_FadeDelay) / m_FadeTime);
				m_SlicedSprite.GetComponent<Renderer>().material.color = initialColor;
			}
			if (m_Timer - m_FadeDelay >= m_FadeTime)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
