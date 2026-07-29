using System;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteSlicer2D
{
	private enum LineSide
	{
		Left = 0,
		Right = 1,
		On = 2
	}

	private struct SpriteSlicerLine
	{
		public Vector2 p1;

		public Vector2 p2;

		public SpriteSlicerLine(Vector2 a, Vector2 b)
		{
			p1 = a;
			p2 = b;
		}
	}

	public class Polygon
	{
		public List<Vector2> points = new List<Vector2>();
	}

	private class LinkedPolygonPoint
	{
		public Vector2 position;

		public LineSide sliceSide;

		public LinkedPolygonPoint next;

		public LinkedPolygonPoint prev;

		public float distOnLine;

		public bool visited;

		public LinkedPolygonPoint(Vector2 startPos, LineSide side)
		{
			position = startPos;
			sliceSide = side;
		}
	}

	public class VectorComparer : IComparer<Vector2>
	{
		public int Compare(Vector2 vectorA, Vector2 vectorB)
		{
			if (vectorA.x > vectorB.x)
			{
				return 1;
			}
			if (vectorA.x < vectorB.x)
			{
				return -1;
			}
			return 0;
		}
	}

	private class EdgeComparer : IComparer<LinkedPolygonPoint>
	{
		private SpriteSlicerLine _line;

		public EdgeComparer(SpriteSlicerLine ln)
		{
			_line = ln;
		}

		public int Compare(LinkedPolygonPoint edgeA, LinkedPolygonPoint edgeB)
		{
			float num = DotProduct(_line.p1, _line.p2, edgeA.position);
			float num2 = DotProduct(_line.p1, _line.p2, edgeB.position);
			if (num < num2)
			{
				return -1;
			}
			return 1;
		}
	}

	private static bool _debugLoggingEnabled = false;

	private static bool _centreChildSprites = false;

	private static List<SpriteSlicer2DSliceInfo> _subSlicesCont = new List<SpriteSlicer2DSliceInfo>();

	private static List<LinkedPolygonPoint> _concavePolygonPoints = new List<LinkedPolygonPoint>();

	private static List<LinkedPolygonPoint> _concavePolygonIntersectionPoints = new List<LinkedPolygonPoint>();

	private static List<Polygon> _concaveSlicePolygonResults = new List<Polygon>();

	private static VectorComparer _vectorComparer = new VectorComparer();

	public static bool DebugLoggingEnabled
	{
		get
		{
			return _debugLoggingEnabled;
		}
		set
		{
			_debugLoggingEnabled = value;
		}
	}

	public static bool CentreChildSprites
	{
		get
		{
			return _centreChildSprites;
		}
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint)
	{
		LayerMask layerMask = -1;
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, true, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, LayerMask layerMask)
	{
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, true, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, string tag)
	{
		LayerMask layerMask = -1;
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, true, -1, ref slicedObjectInfo, layerMask, tag);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, string tag)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, tag);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask)
	{
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask)
	{
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
	}

	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite)
	{
		if ((bool)sprite)
		{
			LayerMask layerMask = -1;
			List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, true, -1, ref slicedObjectInfo, layerMask, null);
		}
	}

	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		if ((bool)sprite)
		{
			LayerMask layerMask = -1;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
		}
	}

	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		if ((bool)sprite)
		{
			LayerMask layerMask = -1;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
		}
	}

	public static void ExplodeSprite(GameObject sprite, int numCuts, float explosionForce)
	{
		if ((bool)sprite)
		{
			List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
			if (explosionForce != 0f)
			{
				slicedObjectInfo = new List<SpriteSlicer2DSliceInfo>();
			}
			ExplodeSprite(sprite, numCuts, explosionForce, true, ref slicedObjectInfo);
		}
	}

	public static void ExplodeSprite(GameObject sprite, int numCuts, float explosionForce, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		Bounds spriteBounds;
		if (!sprite || !GetSpriteBounds(sprite, out spriteBounds))
		{
			return;
		}
		LayerMask layerMask = -1;
		int instanceID = sprite.GetInstanceID();
		Vector3 position = sprite.transform.position;
		float num = spriteBounds.size.x + spriteBounds.size.y;
		for (int i = 0; i < numCuts; i++)
		{
			float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			Vector3 vector = new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f) * num;
			f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			Vector3 vector2 = new Vector3(Mathf.Sin(f) * (spriteBounds.size.x * 0.25f), Mathf.Cos(f) * (spriteBounds.size.y * 0.25f), 0f);
			Vector3 worldStartPoint = position + vector + vector2;
			Vector3 worldEndPoint = position - vector + vector2;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, null, instanceID, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
			_subSlicesCont.Clear();
			for (int j = 0; j < slicedObjectInfo.Count; j++)
			{
				for (int k = 0; k < slicedObjectInfo[j].ChildObjects.Count; k++)
				{
					SliceSpritesInternal(worldStartPoint, worldEndPoint, slicedObjectInfo[j].ChildObjects[k], 0, destroySlicedObjects, -1, ref _subSlicesCont, layerMask, null);
				}
			}
			slicedObjectInfo.AddRange(_subSlicesCont);
		}
		if (slicedObjectInfo == null)
		{
			return;
		}
		for (int l = 0; l < slicedObjectInfo.Count; l++)
		{
			for (int m = 0; m < slicedObjectInfo[l].ChildObjects.Count; m++)
			{
				Rigidbody2D component = slicedObjectInfo[l].ChildObjects[m].GetComponent<Rigidbody2D>();
				if ((bool)component)
				{
					component.AddForceAtPosition(new Vector2(0f, 1f) * explosionForce, position);
				}
			}
		}
	}

	public static void ShatterSprite(GameObject spriteObject, float explosionForce)
	{
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		ShatterSprite(spriteObject, explosionForce, true, ref slicedObjectInfo);
	}

	public static void ShatterSprite(GameObject spriteObject, float explosionForce, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		Rigidbody2D component = spriteObject.GetComponent<Rigidbody2D>();
		if (!component)
		{
			Debug.LogWarning("Could not shatter sprite - no attached rigidbody");
			return;
		}
		SlicedSprite slicedSprite = null;
		SpriteRenderer spriteRenderer = null;
		spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
		{
			slicedSprite = spriteObject.GetComponent<SlicedSprite>();
			if (slicedSprite == null)
			{
				return;
			}
		}
		List<Vector2> points = null;
		PolygonCollider2D component2 = spriteObject.GetComponent<PolygonCollider2D>();
		PhysicsMaterial2D physicsMaterial = null;
		if ((bool)component2)
		{
			points = new List<Vector2>(component2.points);
			physicsMaterial = component2.sharedMaterial;
		}
		else
		{
			BoxCollider2D component3 = spriteObject.GetComponent<BoxCollider2D>();
			if ((bool)component3)
			{
				points = new List<Vector2>(4);
				points.Add(new Vector2((0f - component3.size.x) * 0.5f, (0f - component3.size.y) * 0.5f));
				points.Add(new Vector2(component3.size.x * 0.5f, (0f - component3.size.y) * 0.5f));
				points.Add(new Vector2(component3.size.x * 0.5f, component3.size.y * 0.5f));
				points.Add(new Vector2((0f - component3.size.x) * 0.5f, component3.size.y * 0.5f));
				physicsMaterial = component3.sharedMaterial;
			}
			else
			{
				CircleCollider2D component4 = spriteObject.GetComponent<CircleCollider2D>();
				if ((bool)component4)
				{
					int num = 32;
					float num2 = (float)Math.PI * 2f / (float)num;
					points = new List<Vector2>(num);
					for (int i = 0; i < num; i++)
					{
						float f = num2 * (float)i;
						points.Add(new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * component4.radius);
					}
					physicsMaterial = component4.sharedMaterial;
				}
			}
		}
		if (points == null || points.Count <= 3)
		{
			return;
		}
		SpriteSlicer2DSliceInfo spriteSlicer2DSliceInfo = null;
		if (slicedObjectInfo != null)
		{
			spriteSlicer2DSliceInfo = new SpriteSlicer2DSliceInfo();
			slicedObjectInfo.Add(spriteSlicer2DSliceInfo);
			if (!destroySlicedObjects)
			{
				spriteSlicer2DSliceInfo.SlicedObject = spriteObject;
			}
		}
		Vector2 vector = spriteObject.transform.position;
		int[] array = Triangulate(ref points);
		List<Vector2> list = new List<Vector2>(3);
		list.Add(Vector2.zero);
		list.Add(Vector2.zero);
		list.Add(Vector2.zero);
		float parentArea = Mathf.Abs(Area(ref points));
		for (int j = 0; j < array.Length; j += 3)
		{
			list[0] = points[array[j]];
			list[1] = points[array[j + 1]];
			list[2] = points[array[j + 2]];
			Vector2[] vertices = list.ToArray();
			float area = GetArea(ref vertices);
			SlicedSprite slicedSprite2;
			PolygonCollider2D polygonCollider;
			CreateChildSprite(component, physicsMaterial, ref vertices, parentArea, area, out slicedSprite2, out polygonCollider);
			slicedSprite2.gameObject.name = spriteObject.name + "_child";
			if ((bool)slicedSprite)
			{
				slicedSprite2.InitFromSlicedSprite(slicedSprite, ref polygonCollider, ref vertices, false);
			}
			else if ((bool)spriteRenderer)
			{
				slicedSprite2.InitFromUnitySprite(spriteRenderer, ref polygonCollider, ref vertices, false);
			}
			Vector2 vector2 = vector + (vertices[0] + vertices[1] + vertices[2]) * 0.33f;
			Vector2 vector3 = vector2 - vector;
			vector3.Normalize();
			slicedSprite2.GetComponent<Rigidbody2D>().AddForceAtPosition(vector3 * explosionForce, vector);
			if (spriteSlicer2DSliceInfo != null)
			{
				spriteSlicer2DSliceInfo.ChildObjects.Add(slicedSprite2.gameObject);
			}
		}
		if (destroySlicedObjects)
		{
			UnityEngine.Object.Destroy(component.gameObject);
		}
		else
		{
			component.gameObject.SetActive(false);
		}
	}

	private static bool GetSpriteBounds(GameObject sprite, out Bounds spriteBounds)
	{
		spriteBounds = default(Bounds);
		bool result = false;
		SpriteRenderer component = sprite.GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			spriteBounds = component.sprite.bounds;
			result = true;
		}
		else
		{
			SlicedSprite component2 = sprite.GetComponent<SlicedSprite>();
			if (component2 != null)
			{
				spriteBounds = component2.SpriteBounds;
				result = true;
			}
		}
		return result;
	}

	private static void SliceSpritesInternal(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject spriteObject, int spriteInstanceID, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask, string tag)
	{
		Vector3 vector = Vector3.Normalize(worldEndPoint - worldStartPoint);
		float distance = Vector3.Distance(worldStartPoint, worldEndPoint);
		RaycastHit2D[] array = Physics2D.RaycastAll(worldStartPoint, vector, distance, layerMask.value);
		RaycastHit2D[] array2 = Physics2D.RaycastAll(worldEndPoint, -vector, distance, layerMask.value);
		if (array.Length != array2.Length)
		{
			return;
		}
		for (int i = 0; i < array.Length && i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D = array[i];
			int num = -1;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].collider == raycastHit2D.collider)
				{
					num = j;
					break;
				}
			}
			if (num == -1)
			{
				continue;
			}
			RaycastHit2D raycastHit2D2 = array2[num];
			if (!(raycastHit2D.rigidbody == raycastHit2D2.rigidbody))
			{
				continue;
			}
			Rigidbody2D rigidbody = raycastHit2D.rigidbody;
			Transform transform = raycastHit2D.transform;
			if (!rigidbody || rigidbody.gameObject.isStatic || (spriteObject != null && rigidbody.gameObject != spriteObject) || (tag != null && rigidbody.tag != tag))
			{
				continue;
			}
			SlicedSprite slicedSprite = null;
			SpriteRenderer spriteRenderer = null;
			UIBasicSprite uIBasicSprite = null;
			spriteRenderer = rigidbody.GetComponent<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				uIBasicSprite = rigidbody.GetComponent<UIBasicSprite>();
				if (uIBasicSprite == null)
				{
					slicedSprite = rigidbody.GetComponent<SlicedSprite>();
					if (slicedSprite == null || (maxCutDepth >= 0 && slicedSprite.CutsSinceParentObject >= maxCutDepth))
					{
						continue;
					}
				}
			}
			if (spriteInstanceID != 0 && rigidbody.gameObject.GetInstanceID() != spriteInstanceID && (slicedSprite == null || slicedSprite.ParentInstanceID != spriteInstanceID))
			{
				continue;
			}
			Vector3 vector2 = transform.InverseTransformPoint(worldStartPoint);
			Vector3 vector3 = transform.InverseTransformPoint(worldEndPoint);
			Vector3 vector4 = transform.InverseTransformPoint(raycastHit2D.point);
			Vector3 vector5 = transform.InverseTransformPoint(raycastHit2D2.point);
			List<Vector2> polygonPoints = null;
			PolygonCollider2D component = rigidbody.GetComponent<PolygonCollider2D>();
			PhysicsMaterial2D physicsMaterial = null;
			if ((bool)component)
			{
				polygonPoints = new List<Vector2>(component.points);
				physicsMaterial = component.sharedMaterial;
			}
			else
			{
				BoxCollider2D component2 = rigidbody.GetComponent<BoxCollider2D>();
				if ((bool)component2)
				{
					polygonPoints = new List<Vector2>(4);
					Vector2 offset = component2.offset;
					polygonPoints.Add(offset + new Vector2((0f - component2.size.x) * 0.5f, (0f - component2.size.y) * 0.5f));
					polygonPoints.Add(offset + new Vector2(component2.size.x * 0.5f, (0f - component2.size.y) * 0.5f));
					polygonPoints.Add(offset + new Vector2(component2.size.x * 0.5f, component2.size.y * 0.5f));
					polygonPoints.Add(offset + new Vector2((0f - component2.size.x) * 0.5f, component2.size.y * 0.5f));
					physicsMaterial = component2.sharedMaterial;
				}
				else
				{
					CircleCollider2D component3 = rigidbody.GetComponent<CircleCollider2D>();
					if ((bool)component3)
					{
						int num2 = 32;
						float num3 = (float)Math.PI * 2f / (float)num2;
						polygonPoints = new List<Vector2>(num2);
						Vector2 offset2 = component3.offset;
						for (int k = 0; k < num2; k++)
						{
							float f = num3 * (float)k;
							polygonPoints.Add(offset2 + new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * component3.radius);
						}
						physicsMaterial = component3.sharedMaterial;
					}
				}
			}
			if (polygonPoints == null)
			{
				continue;
			}
			if (IsPointInsidePolygon(vector2, ref polygonPoints) || IsPointInsidePolygon(vector3, ref polygonPoints))
			{
				if (_debugLoggingEnabled && spriteObject != null)
				{
					Debug.LogWarning("Failed to slice " + rigidbody.gameObject.name + " - start or end cut point is inside the collision mesh");
				}
				continue;
			}
			SpriteSlicer2DSliceInfo spriteSlicer2DSliceInfo = new SpriteSlicer2DSliceInfo();
			spriteSlicer2DSliceInfo.SlicedObject = rigidbody.gameObject;
			spriteSlicer2DSliceInfo.SliceEnterWorldPosition = raycastHit2D.point;
			spriteSlicer2DSliceInfo.SliceExitWorldPosition = raycastHit2D2.point;
			float parentArea = Mathf.Abs(Area(ref polygonPoints));
			if (IsConvex(ref polygonPoints))
			{
				int count = polygonPoints.Count;
				List<Vector2> vertices = new List<Vector2>(count);
				List<Vector2> vertices2 = new List<Vector2>(count);
				vertices.Add(vector4);
				vertices.Add(vector5);
				vertices2.Add(vector4);
				vertices2.Add(vector5);
				for (int l = 0; l < count; l++)
				{
					Vector2 vector6 = polygonPoints[l];
					float num4 = CalculateDeterminant2x3(vector2, vector3, vector6);
					if (num4 > 0f)
					{
						vertices.Add(vector6);
					}
					else
					{
						vertices2.Add(vector6);
					}
				}
				Vector2[] vertices3 = ArrangeVertices(ref vertices);
				Vector2[] vertices4 = ArrangeVertices(ref vertices2);
				float area = GetArea(ref vertices3);
				float area2 = GetArea(ref vertices4);
				if (!AreVerticesAcceptable(ref vertices3, area, true) || !AreVerticesAcceptable(ref vertices4, area2, true))
				{
					continue;
				}
				SlicedSprite slicedSprite2;
				PolygonCollider2D polygonCollider;
				CreateChildSprite(rigidbody, physicsMaterial, ref vertices3, parentArea, area, out slicedSprite2, out polygonCollider);
				slicedSprite2.gameObject.name = rigidbody.gameObject.name + "_child1";
				SlicedSprite slicedSprite3;
				PolygonCollider2D polygonCollider2;
				CreateChildSprite(rigidbody, physicsMaterial, ref vertices4, parentArea, area2, out slicedSprite3, out polygonCollider2);
				slicedSprite3.gameObject.name = rigidbody.gameObject.name + "_child2";
				if ((bool)slicedSprite)
				{
					slicedSprite2.InitFromSlicedSprite(slicedSprite, ref polygonCollider, ref vertices3, false);
					slicedSprite3.InitFromSlicedSprite(slicedSprite, ref polygonCollider2, ref vertices4, false);
				}
				else if ((bool)spriteRenderer)
				{
					slicedSprite2.InitFromUnitySprite(spriteRenderer, ref polygonCollider, ref vertices3, false);
					slicedSprite3.InitFromUnitySprite(spriteRenderer, ref polygonCollider2, ref vertices4, false);
				}
				else if ((bool)uIBasicSprite)
				{
					Material nGUIMaterilal = GetNGUIMaterilal(uIBasicSprite);
					slicedSprite2.InitFromNGUI(uIBasicSprite, nGUIMaterilal, ref polygonCollider, ref vertices3, false);
					slicedSprite3.InitFromNGUI(uIBasicSprite, nGUIMaterilal, ref polygonCollider2, ref vertices4, false);
				}
				spriteSlicer2DSliceInfo.ChildObjects.Add(slicedSprite2.gameObject);
				spriteSlicer2DSliceInfo.ChildObjects.Add(slicedSprite3.gameObject);
			}
			else
			{
				Polygon polygon = new Polygon();
				SpriteSlicerLine line = new SpriteSlicerLine(vector2, vector3);
				polygon.points = polygonPoints;
				SliceConcave(polygon, line);
				for (int m = 0; m < _concaveSlicePolygonResults.Count; m++)
				{
					Vector2[] vertices5 = _concaveSlicePolygonResults[m].points.ToArray();
					float area3 = GetArea(ref vertices5);
					if (AreVerticesAcceptable(ref vertices5, area3, false))
					{
						SlicedSprite slicedSprite4;
						PolygonCollider2D polygonCollider3;
						CreateChildSprite(rigidbody, physicsMaterial, ref vertices5, parentArea, area3, out slicedSprite4, out polygonCollider3);
						slicedSprite4.gameObject.name = rigidbody.gameObject.name + "_child" + m;
						if ((bool)slicedSprite)
						{
							slicedSprite4.InitFromSlicedSprite(slicedSprite, ref polygonCollider3, ref vertices5, true);
						}
						else if ((bool)spriteRenderer)
						{
							slicedSprite4.InitFromUnitySprite(spriteRenderer, ref polygonCollider3, ref vertices5, true);
						}
						else if ((bool)uIBasicSprite)
						{
							slicedSprite4.InitFromNGUI(uIBasicSprite, GetNGUIMaterilal(uIBasicSprite), ref polygonCollider3, ref vertices5, true);
						}
						spriteSlicer2DSliceInfo.ChildObjects.Add(slicedSprite4.gameObject);
					}
				}
			}
			if (spriteSlicer2DSliceInfo.ChildObjects.Count <= 0)
			{
				continue;
			}
			rigidbody.gameObject.SendMessage("OnSpriteSliced", spriteSlicer2DSliceInfo, SendMessageOptions.DontRequireReceiver);
			if (slicedObjectInfo != null)
			{
				if (destroySlicedObjects)
				{
					spriteSlicer2DSliceInfo.SlicedObject = null;
				}
				slicedObjectInfo.Add(spriteSlicer2DSliceInfo);
			}
			if (destroySlicedObjects)
			{
				UnityEngine.Object.Destroy(rigidbody.gameObject);
			}
			else
			{
				rigidbody.gameObject.SetActive(false);
			}
		}
	}

	private static void CreateChildSprite(Rigidbody2D parentRigidBody, PhysicsMaterial2D physicsMaterial, ref Vector2[] spriteVertices, float parentArea, float childArea, out SlicedSprite slicedSprite, out PolygonCollider2D polygonCollider)
	{
		GameObject gameObject = new GameObject();
		slicedSprite = gameObject.AddComponent<SlicedSprite>();
		Rigidbody2D component = slicedSprite.GetComponent<Rigidbody2D>();
		component.mass = parentRigidBody.mass * (childArea / parentArea);
		component.drag = parentRigidBody.drag;
		component.angularDrag = parentRigidBody.angularDrag;
		component.gravityScale = parentRigidBody.gravityScale;
		component.constraints = parentRigidBody.constraints;
		component.isKinematic = parentRigidBody.isKinematic;
		component.interpolation = parentRigidBody.interpolation;
		component.sleepMode = parentRigidBody.sleepMode;
		component.collisionDetectionMode = parentRigidBody.collisionDetectionMode;
		component.velocity = parentRigidBody.velocity;
		component.angularVelocity = parentRigidBody.angularVelocity;
		polygonCollider = slicedSprite.GetComponent<PolygonCollider2D>();
		polygonCollider.points = spriteVertices;
		polygonCollider.sharedMaterial = physicsMaterial;
	}

	private static Material GetNGUIMaterilal<T>(T nguiBasic) where T : UIBasicSprite
	{
		Material material = nguiBasic.material;
		if (material == null)
		{
			material = new Material(Shader.Find("Sprites/Default"));
			material.SetTexture("_MainTex", nguiBasic.mainTexture);
		}
		return material;
	}

	private static LineSide GetSideOfLine(SpriteSlicerLine line, Vector2 pt)
	{
		float num = (pt.x - line.p1.x) * (line.p2.y - line.p1.y) - (pt.y - line.p1.y) * (line.p2.x - line.p1.x);
		return (num > float.Epsilon) ? LineSide.Right : ((!(num < -1E-45f)) ? LineSide.On : LineSide.Left);
	}

	private static float CalculateDeterminant2x3(Vector2 start, Vector2 end, Vector2 point)
	{
		return start.x * end.y + end.x * point.y + point.x * start.y - start.y * end.x - end.y * point.x - point.y * start.x;
	}

	public static float CalculateDeterminant2x2(Vector2 vectorA, Vector2 vectorB)
	{
		return vectorA.x * vectorB.y - vectorA.y * vectorB.x;
	}

	public static float DotProduct(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
	{
		return (point.x - lineStart.x) * (lineEnd.x - lineStart.x) + (point.y - lineStart.y) * (lineEnd.y - lineStart.y);
	}

	public static int[] Triangulate(ref List<Vector2> points)
	{
		List<int> list = new List<int>();
		int count = points.Count;
		if (count < 3)
		{
			return list.ToArray();
		}
		int[] array = new int[count];
		if (Area(ref points) > 0f)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				array[j] = count - 1 - j;
			}
		}
		int num = count;
		int num2 = 2 * num;
		int num3 = 0;
		int num4 = num - 1;
		while (num > 2)
		{
			if (num2-- <= 0)
			{
				return list.ToArray();
			}
			int num5 = num4;
			if (num <= num5)
			{
				num5 = 0;
			}
			num4 = num5 + 1;
			if (num <= num4)
			{
				num4 = 0;
			}
			int num6 = num4 + 1;
			if (num <= num6)
			{
				num6 = 0;
			}
			if (Snip(ref points, num5, num4, num6, num, array))
			{
				int item = array[num5];
				int item2 = array[num4];
				int item3 = array[num6];
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				num3++;
				int num7 = num4;
				for (int k = num4 + 1; k < num; k++)
				{
					array[num7] = array[k];
					num7++;
				}
				num--;
				num2 = 2 * num;
			}
		}
		list.Reverse();
		return list.ToArray();
	}

	private static float Area(ref List<Vector2> points)
	{
		int count = points.Count;
		float num = 0f;
		int index = count - 1;
		int num2 = 0;
		while (num2 < count)
		{
			Vector2 vector = points[index];
			Vector2 vector2 = points[num2];
			num += vector.x * vector2.y - vector2.x * vector.y;
			index = num2++;
		}
		return num * 0.5f;
	}

	private static bool Snip(ref List<Vector2> points, int u, int v, int w, int n, int[] V)
	{
		Vector2 a = points[V[u]];
		Vector2 b = points[V[v]];
		Vector2 c = points[V[w]];
		if (Mathf.Epsilon > (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Vector2 p = points[V[i]];
				if (InsideTriangle(a, b, c, p))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float num = C.x - B.x;
		float num2 = C.y - B.y;
		float num3 = A.x - C.x;
		float num4 = A.y - C.y;
		float num5 = B.x - A.x;
		float num6 = B.y - A.y;
		float num7 = P.x - A.x;
		float num8 = P.y - A.y;
		float num9 = P.x - B.x;
		float num10 = P.y - B.y;
		float num11 = P.x - C.x;
		float num12 = P.y - C.y;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		return num13 >= 0f && num15 >= 0f && num14 >= 0f;
	}

	private static Vector2[] ArrangeVertices(ref List<Vector2> vertices)
	{
		int count = vertices.Count;
		int index = 1;
		int num = count - 1;
		vertices.Sort(_vectorComparer);
		List<Vector2> list = new List<Vector2>(count);
		for (int i = 0; i < count; i++)
		{
			list.Add(Vector2.zero);
		}
		Vector2 vector = vertices[0];
		Vector2 vector2 = vertices[count - 1];
		list[0] = vector;
		for (int j = 1; j < count - 1; j++)
		{
			float num2 = CalculateDeterminant2x3(vector, vector2, vertices[j]);
			if (num2 < 0f)
			{
				list[index++] = vertices[j];
			}
			else
			{
				list[num--] = vertices[j];
			}
		}
		list[index] = vector2;
		return list.ToArray();
	}

	private static float GetArea(ref Vector2[] vertices)
	{
		int num = vertices.Length;
		float num2 = vertices[0].y * (vertices[num - 1].x - vertices[1].x);
		for (int i = 1; i < num; i++)
		{
			num2 += vertices[i].y * (vertices[i - 1].x - vertices[(i + 1) % num].x);
		}
		return Mathf.Abs(num2 * 0.5f);
	}

	public static bool IsConvex(ref Vector2[] vertices)
	{
		int num = vertices.Length;
		Vector3 vector = vertices[0] - vertices[num - 1];
		Vector3 vector2 = vertices[1] - vertices[0];
		float num2 = CalculateDeterminant2x2(vector, vector2);
		float num3;
		for (int i = 1; i < num - 1; i++)
		{
			vector = vector2;
			vector2 = vertices[i + 1] - vertices[i];
			num3 = CalculateDeterminant2x2(vector, vector2);
			if (num2 * num3 < 0f)
			{
				return false;
			}
		}
		vector = vector2;
		vector2 = vertices[0] - vertices[num - 1];
		num3 = CalculateDeterminant2x2(vector, vector2);
		if (num2 * num3 < 0f)
		{
			return false;
		}
		return true;
	}

	public static bool IsConvex(ref List<Vector2> vertices)
	{
		int count = vertices.Count;
		Vector3 vector = vertices[0] - vertices[count - 1];
		Vector3 vector2 = vertices[1] - vertices[0];
		float num = CalculateDeterminant2x2(vector, vector2);
		float num2;
		for (int i = 1; i < count - 1; i++)
		{
			vector = vector2;
			vector2 = vertices[i + 1] - vertices[i];
			num2 = CalculateDeterminant2x2(vector, vector2);
			if (num * num2 < 0f)
			{
				return false;
			}
		}
		vector = vector2;
		vector2 = vertices[0] - vertices[count - 1];
		num2 = CalculateDeterminant2x2(vector, vector2);
		if (num * num2 < 0f)
		{
			return false;
		}
		return true;
	}

	private static bool AreVerticesAcceptable(ref Vector2[] vertices, float area, bool failOnConcave)
	{
		if (vertices.Length < 3)
		{
			if (_debugLoggingEnabled)
			{
				Debug.LogWarning("Vertices rejected - insufficient vertices");
			}
			return false;
		}
		if (area < 0.0001f)
		{
			if (_debugLoggingEnabled)
			{
				Debug.LogWarning("Vertices rejected - below minimum area");
			}
			return false;
		}
		if (failOnConcave && !IsConvex(ref vertices))
		{
			if (_debugLoggingEnabled)
			{
				Debug.LogWarning("Vertices rejected - shape is not convex");
			}
			return false;
		}
		return true;
	}

	private static void SliceConcave(Polygon poly, SpriteSlicerLine line)
	{
		SplitEdges(poly, line);
		SortEdges(line);
		SplitPolygon();
		CollectPolys();
	}

	private static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
	{
		float num = pe1.y - ps1.y;
		float num2 = ps1.x - pe1.x;
		float num3 = num * ps1.x + num2 * ps1.y;
		float num4 = pe2.y - ps2.y;
		float num5 = ps2.x - pe2.x;
		float num6 = num4 * ps2.x + num5 * ps2.y;
		float num7 = num * num5 - num4 * num2;
		if (num7 == 0f)
		{
			Debug.LogError("Lines are parallel!");
		}
		float num8 = 1f / num7;
		return new Vector2((num5 * num3 - num2 * num6) * num8, (num * num6 - num4 * num3) * num8);
	}

	private static void SplitEdges(Polygon poly, SpriteSlicerLine line)
	{
		_concavePolygonPoints.Clear();
		_concavePolygonIntersectionPoints.Clear();
		for (int i = 0; i < poly.points.Count; i++)
		{
			SpriteSlicerLine spriteSlicerLine = new SpriteSlicerLine(poly.points[i], poly.points[(i + 1) % poly.points.Count]);
			LineSide sideOfLine = GetSideOfLine(line, spriteSlicerLine.p1);
			LineSide sideOfLine2 = GetSideOfLine(line, spriteSlicerLine.p2);
			_concavePolygonPoints.Add(new LinkedPolygonPoint(poly.points[i], sideOfLine));
			if (sideOfLine == LineSide.On)
			{
				_concavePolygonIntersectionPoints.Add(_concavePolygonPoints[_concavePolygonPoints.Count - 1]);
			}
			else if (sideOfLine != sideOfLine2 && sideOfLine2 != LineSide.On)
			{
				Vector2 startPos = LineIntersectionPoint(spriteSlicerLine.p1, spriteSlicerLine.p2, line.p1, line.p2);
				_concavePolygonPoints.Add(new LinkedPolygonPoint(startPos, LineSide.On));
				_concavePolygonIntersectionPoints.Add(_concavePolygonPoints[_concavePolygonPoints.Count - 1]);
			}
		}
		for (int j = 0; j < _concavePolygonPoints.Count - 1; j++)
		{
			int index = (j + 1) % _concavePolygonPoints.Count;
			LinkedPolygonPoint linkedPolygonPoint = _concavePolygonPoints[j];
			(linkedPolygonPoint.next = _concavePolygonPoints[index]).prev = linkedPolygonPoint;
		}
		_concavePolygonPoints[_concavePolygonPoints.Count - 1].next = _concavePolygonPoints[0];
		_concavePolygonPoints[0].prev = _concavePolygonPoints[_concavePolygonPoints.Count - 1];
	}

	private static void SortEdges(SpriteSlicerLine line)
	{
		EdgeComparer comparer = new EdgeComparer(line);
		_concavePolygonIntersectionPoints.Sort(comparer);
		for (int i = 1; i < _concavePolygonIntersectionPoints.Count; i++)
		{
			_concavePolygonIntersectionPoints[i].distOnLine = (_concavePolygonIntersectionPoints[i].position - _concavePolygonIntersectionPoints[0].position).magnitude;
		}
	}

	private static void SplitPolygon()
	{
		LinkedPolygonPoint linkedPolygonPoint = null;
		for (int i = 0; i < _concavePolygonIntersectionPoints.Count; i++)
		{
			LinkedPolygonPoint linkedPolygonPoint2 = linkedPolygonPoint;
			linkedPolygonPoint = null;
			while (linkedPolygonPoint2 == null && i < _concavePolygonIntersectionPoints.Count)
			{
				LinkedPolygonPoint linkedPolygonPoint3 = _concavePolygonIntersectionPoints[i];
				LineSide sliceSide = linkedPolygonPoint3.sliceSide;
				LineSide sliceSide2 = linkedPolygonPoint3.prev.sliceSide;
				LineSide sliceSide3 = linkedPolygonPoint3.next.sliceSide;
				if (sliceSide != LineSide.On)
				{
					Debug.LogError("Current side should be ON");
				}
				if ((sliceSide2 == LineSide.Left && sliceSide3 == LineSide.Right) || (sliceSide2 == LineSide.Left && sliceSide3 == LineSide.On && linkedPolygonPoint3.next.distOnLine < linkedPolygonPoint3.distOnLine) || (sliceSide2 == LineSide.On && sliceSide3 == LineSide.Right && linkedPolygonPoint3.prev.distOnLine < linkedPolygonPoint3.distOnLine))
				{
					linkedPolygonPoint2 = linkedPolygonPoint3;
				}
				i++;
			}
			LinkedPolygonPoint linkedPolygonPoint4 = null;
			while (linkedPolygonPoint4 == null && i < _concavePolygonIntersectionPoints.Count)
			{
				LinkedPolygonPoint linkedPolygonPoint5 = _concavePolygonIntersectionPoints[i];
				LineSide sliceSide4 = linkedPolygonPoint5.sliceSide;
				LineSide sliceSide5 = linkedPolygonPoint5.prev.sliceSide;
				LineSide sliceSide6 = linkedPolygonPoint5.next.sliceSide;
				if (sliceSide4 != LineSide.On)
				{
					Debug.LogError("Current side should be ON");
				}
				if ((sliceSide5 == LineSide.Right && sliceSide6 == LineSide.Left) || (sliceSide5 == LineSide.On && sliceSide6 == LineSide.Left) || (sliceSide5 == LineSide.Right && sliceSide6 == LineSide.On) || (sliceSide5 == LineSide.Right && sliceSide6 == LineSide.Right) || (sliceSide5 == LineSide.Left && sliceSide6 == LineSide.Left))
				{
					linkedPolygonPoint4 = linkedPolygonPoint5;
				}
				else
				{
					i++;
				}
			}
			if (linkedPolygonPoint2 != null && linkedPolygonPoint4 != null)
			{
				LinkPoints(linkedPolygonPoint2, linkedPolygonPoint4);
				VerifyPolygons();
				if (linkedPolygonPoint2.prev.prev.sliceSide == LineSide.Left)
				{
					linkedPolygonPoint = linkedPolygonPoint2.prev;
				}
				else if (linkedPolygonPoint4.next.sliceSide == LineSide.Right)
				{
					linkedPolygonPoint = linkedPolygonPoint4;
				}
			}
		}
	}

	private static void CollectPolys()
	{
		_concaveSlicePolygonResults.Clear();
		foreach (LinkedPolygonPoint concavePolygonPoint in _concavePolygonPoints)
		{
			if (!concavePolygonPoint.visited)
			{
				Polygon polygon = new Polygon();
				LinkedPolygonPoint linkedPolygonPoint = concavePolygonPoint;
				do
				{
					linkedPolygonPoint.visited = true;
					polygon.points.Add(linkedPolygonPoint.position);
					linkedPolygonPoint = linkedPolygonPoint.next;
				}
				while (linkedPolygonPoint != concavePolygonPoint);
				_concaveSlicePolygonResults.Add(polygon);
			}
		}
	}

	private static void VerifyPolygons()
	{
		foreach (LinkedPolygonPoint concavePolygonPoint in _concavePolygonPoints)
		{
			LinkedPolygonPoint linkedPolygonPoint = concavePolygonPoint;
			int num = 0;
			do
			{
				if (num >= _concavePolygonPoints.Count)
				{
					Debug.LogError("Invalid polygon cycle detected");
					break;
				}
				linkedPolygonPoint = linkedPolygonPoint.next;
				num++;
			}
			while (linkedPolygonPoint != concavePolygonPoint);
		}
	}

	private static void LinkPoints(LinkedPolygonPoint srcEdge, LinkedPolygonPoint dstEdge)
	{
		_concavePolygonPoints.Add(new LinkedPolygonPoint(srcEdge.position, srcEdge.sliceSide));
		LinkedPolygonPoint linkedPolygonPoint = _concavePolygonPoints[_concavePolygonPoints.Count - 1];
		_concavePolygonPoints.Add(new LinkedPolygonPoint(dstEdge.position, dstEdge.sliceSide));
		LinkedPolygonPoint linkedPolygonPoint2 = _concavePolygonPoints[_concavePolygonPoints.Count - 1];
		linkedPolygonPoint.next = dstEdge;
		linkedPolygonPoint.prev = srcEdge.prev;
		linkedPolygonPoint2.next = srcEdge;
		linkedPolygonPoint2.prev = dstEdge.prev;
		srcEdge.prev.next = linkedPolygonPoint;
		srcEdge.prev = linkedPolygonPoint2;
		dstEdge.prev.next = linkedPolygonPoint2;
		dstEdge.prev = linkedPolygonPoint;
	}

	private static bool IsPointInsidePolygon(Vector2 pos, ref List<Vector2> polygonPoints)
	{
		int num = 0;
		int count = polygonPoints.Count;
		for (int i = 0; i < count; i++)
		{
			int num2 = i + 1;
			if (num2 >= count)
			{
				num2 = 0;
			}
			Vector2 vector = polygonPoints[i];
			Vector2 vector2 = polygonPoints[num2];
			if (vector.y <= pos.y)
			{
				if (vector2.y > pos.y)
				{
					float num3 = (vector2.x - vector.x) * (pos.y - vector.y) - (pos.x - vector.x) * (vector2.y - vector.y);
					if (num3 > 0f)
					{
						num++;
					}
				}
			}
			else if (vector2.y <= pos.y)
			{
				float num4 = (vector2.x - vector.x) * (pos.y - vector.y) - (pos.x - vector.x) * (vector2.y - vector.y);
				if (num4 < 0f)
				{
					num--;
				}
			}
		}
		return num != 0;
	}
}
