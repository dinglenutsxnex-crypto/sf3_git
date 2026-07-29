using UnityEngine;

namespace Nekki.UI
{
	public class NekkiUIOverscrollHandler : MonoBehaviour
	{
		public UIScrollView ScrollView;

		public float Top;

		public float Bottom;

		public float Left;

		public float Right;

		private Transform _transform;

		private UIPanel _panel;

		private void Start()
		{
			_transform = ScrollView.transform;
			_panel = ScrollView.panel;
		}

		private void Update()
		{
			if (!ScrollView.isDragging)
			{
				return;
			}
			Bounds bounds = ScrollView.bounds;
			Vector3 vector = _panel.CalculateConstrainOffset(bounds.min, bounds.max);
			Vector3 vector2 = _panel.clipOffset;
			Vector3 localPosition = _transform.localPosition;
			if (ScrollView.movement != 0)
			{
				if (vector.y > Top)
				{
					localPosition.y += vector.y - Top;
					vector2.y = 0f - localPosition.y;
					_transform.localPosition = localPosition;
					_panel.clipOffset = vector2;
					Vector3 currentMomentum = ScrollView.currentMomentum;
					currentMomentum.y = 0f;
					ScrollView.currentMomentum = currentMomentum;
				}
				else if (vector.y < 0f - Bottom)
				{
					localPosition.y += vector.y + Bottom;
					vector2.y = 0f - localPosition.y;
					_transform.localPosition = localPosition;
					_panel.clipOffset = vector2;
					Vector3 currentMomentum2 = ScrollView.currentMomentum;
					currentMomentum2.y = 0f;
					ScrollView.currentMomentum = currentMomentum2;
				}
			}
			if (ScrollView.movement != UIScrollView.Movement.Vertical)
			{
				if (vector.x > Right)
				{
					localPosition.x += vector.x - Right;
					vector2.x = 0f - localPosition.x;
					_transform.localPosition = localPosition;
					_panel.clipOffset = vector2;
					Vector3 currentMomentum3 = ScrollView.currentMomentum;
					currentMomentum3.x = 0f;
					ScrollView.currentMomentum = currentMomentum3;
				}
				else if (vector2.x < 0f - Left)
				{
					localPosition.x += vector.x + Left;
					vector2.x = 0f - localPosition.x;
					_transform.localPosition = localPosition;
					_panel.clipOffset = vector2;
					Vector3 currentMomentum4 = ScrollView.currentMomentum;
					currentMomentum4.x = 0f;
					ScrollView.currentMomentum = currentMomentum4;
				}
			}
		}
	}
}
