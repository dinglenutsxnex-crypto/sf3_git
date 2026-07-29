using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteSlicer2DDemoManager : MonoBehaviour
{
	private struct MousePosition
	{
		public Vector3 m_WorldPosition;

		public float m_Time;
	}

	private List<SpriteSlicer2DSliceInfo> m_SlicedSpriteInfo = new List<SpriteSlicer2DSliceInfo>();

	private TrailRenderer m_TrailRenderer;

	public float m_MouseRecordInterval = 0.05f;

	public int m_MaxMousePositions = 5;

	public bool m_FadeFragments;

	private List<MousePosition> m_MousePositions = new List<MousePosition>();

	private float m_MouseRecordTimer;

	private void Start()
	{
		m_TrailRenderer = GetComponentInChildren<TrailRenderer>();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt)))
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vector.z = Camera.main.transform.position.z;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, new Vector3(0f, 0f, 0f), 0f);
			if ((bool)raycastHit2D.rigidbody && Input.GetKey(KeyCode.LeftControl))
			{
				SpriteSlicer2D.ExplodeSprite(raycastHit2D.rigidbody.gameObject, 16, 100f, true, ref m_SlicedSpriteInfo);
				if (m_SlicedSpriteInfo.Count == 0)
				{
					raycastHit2D.rigidbody.AddForce(new Vector2(0f, 400f));
				}
			}
		}
		else if (Input.GetMouseButton(0))
		{
			bool flag = false;
			m_MouseRecordTimer -= Time.deltaTime;
			if (m_MouseRecordTimer <= 0f)
			{
				MousePosition item = default(MousePosition);
				item.m_WorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				item.m_Time = Time.time;
				m_MousePositions.Add(item);
				m_MouseRecordTimer = m_MouseRecordInterval;
				flag = true;
				if (m_MousePositions.Count > m_MaxMousePositions)
				{
					m_MousePositions.RemoveAt(0);
				}
			}
			if (m_MousePositions.Count > 0 && Time.time - m_MousePositions[0].m_Time > m_MouseRecordInterval * (float)m_MaxMousePositions)
			{
				m_MousePositions.RemoveAt(0);
			}
			if (flag)
			{
				for (int i = 0; i < m_MousePositions.Count - 1; i++)
				{
					SpriteSlicer2D.SliceAllSprites(m_MousePositions[i].m_WorldPosition, m_MousePositions[m_MousePositions.Count - 1].m_WorldPosition, true, ref m_SlicedSpriteInfo);
					if (m_SlicedSpriteInfo.Count <= 0)
					{
						continue;
					}
					for (int j = 0; j < m_SlicedSpriteInfo.Count; j++)
					{
						for (int k = 0; k < m_SlicedSpriteInfo[j].ChildObjects.Count; k++)
						{
							Vector2 vector2 = m_MousePositions[m_MousePositions.Count - 1].m_WorldPosition - m_MousePositions[i].m_WorldPosition;
							vector2.Normalize();
							m_SlicedSpriteInfo[j].ChildObjects[k].GetComponent<Rigidbody2D>().AddForce(vector2 * 500f);
						}
					}
					m_MousePositions.Clear();
					break;
				}
			}
			if ((bool)m_TrailRenderer)
			{
				Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				position.z = -9f;
				m_TrailRenderer.transform.position = position;
			}
		}
		else
		{
			m_MousePositions.Clear();
		}
		for (int l = 0; l < m_SlicedSpriteInfo.Count; l++)
		{
			for (int m = 0; m < m_SlicedSpriteInfo[l].ChildObjects.Count; m++)
			{
				Vector3 position2 = m_SlicedSpriteInfo[l].ChildObjects[m].transform.position;
				position2.z = -1f;
				m_SlicedSpriteInfo[l].ChildObjects[m].transform.position = position2;
			}
		}
		if (m_FadeFragments)
		{
			for (int n = 0; n < m_SlicedSpriteInfo.Count; n++)
			{
				for (int num = 0; num < m_SlicedSpriteInfo[n].ChildObjects.Count; num++)
				{
					if (!m_SlicedSpriteInfo[n].ChildObjects[num].GetComponent<Rigidbody2D>().isKinematic)
					{
						m_SlicedSpriteInfo[n].ChildObjects[num].AddComponent<FadeAndDestroy>();
					}
				}
			}
		}
		m_SlicedSpriteInfo.Clear();
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(20f, 20f, 120f, 20f), "Reset Scene"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		m_FadeFragments = GUI.Toggle(new Rect(20f, 50f, 400f, 20f), m_FadeFragments, "Fade out sprites once destroyed");
		GUI.Label(new Rect(Screen.width / 2, 20f, 900f, 20f), "Left Mouse Button + Drag Cursor: Slice Objects");
		GUI.Label(new Rect(Screen.width / 2, 40f, 900f, 20f), "(Cuts objects intersected by the cursor movement vector)");
		GUI.Label(new Rect(Screen.width / 2, 80f, 900f, 20f), "Ctrl + Click Left Mouse Button: Explode Objects");
		GUI.Label(new Rect(Screen.width / 2, 100f, 900f, 20f), "(Randomly slices an objects multiple times, then applies an optional force)");
	}
}
