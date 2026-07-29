using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class SlicedSprite : MonoBehaviour
{
	private MeshRenderer _meshRenderer;

	private MeshFilter _meshFilter;

	private Transform _transform;

	private Vector2 _minCoords;

	private Vector2 _maxCoords;

	private Vector2 _centroid;

	private Vector2 _uvOffset;

	private Bounds _spriteBounds;

	private int _parentInstanceID;

	private int _cutsSinceParentObject;

	private bool _rotated;

	private bool _vFlipped;

	private bool _hFlipped;

	public MeshRenderer MeshRenderer
	{
		get
		{
			return _meshRenderer;
		}
	}

	public Vector2 MinCoords
	{
		get
		{
			return _minCoords;
		}
	}

	public Vector2 MaxCoords
	{
		get
		{
			return _maxCoords;
		}
	}

	public Bounds SpriteBounds
	{
		get
		{
			return _spriteBounds;
		}
	}

	public int ParentInstanceID
	{
		get
		{
			return _parentInstanceID;
		}
	}

	public int CutsSinceParentObject
	{
		get
		{
			return _cutsSinceParentObject;
		}
	}

	public bool Rotated
	{
		get
		{
			return _rotated;
		}
	}

	public bool HFlipped
	{
		get
		{
			return _hFlipped;
		}
	}

	public bool VFlipped
	{
		get
		{
			return _vFlipped;
		}
	}

	private void Awake()
	{
		_transform = base.transform;
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_parentInstanceID = base.gameObject.GetInstanceID();
		_minCoords = new Vector2(0f, 0f);
		_maxCoords = new Vector2(1f, 1f);
		if ((bool)_meshFilter.mesh)
		{
			_spriteBounds = _meshFilter.mesh.bounds;
		}
	}

	public void InitFromSlicedSprite(SlicedSprite slicedSprite, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		slicedSprite._meshRenderer.GetPropertyBlock(materialPropertyBlock);
		_meshRenderer.SetPropertyBlock(materialPropertyBlock);
		InitSprite(slicedSprite.gameObject, slicedSprite.MeshRenderer, ref polygon, ref polygonPoints, slicedSprite.MinCoords, slicedSprite.MaxCoords, slicedSprite.SpriteBounds, slicedSprite._meshRenderer.sharedMaterial, slicedSprite.Rotated, slicedSprite.HFlipped, slicedSprite.VFlipped, slicedSprite._centroid, slicedSprite._uvOffset, isConcave);
		_parentInstanceID = slicedSprite.GetInstanceID();
		_cutsSinceParentObject = slicedSprite.CutsSinceParentObject + 1;
	}

	public void InitFromUnitySprite(SpriteRenderer unitySprite, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave)
	{
		Sprite sprite = unitySprite.sprite;
		Vector2 pivotVector = GetPivotVector(unitySprite.transform, unitySprite.bounds, unitySprite.bounds.size);
		Texture2D texture = sprite.texture;
		Material sharedMaterial = unitySprite.sharedMaterial;
		SetMaterialPropertyBlockTexture(texture);
		Rect textureRect = sprite.textureRect;
		Vector2 vector = new Vector2(texture.width, texture.height);
		Vector2 vector2 = new Vector2(textureRect.xMin / vector.x, textureRect.yMin / vector.y);
		InitSprite(maxCoords: new Vector2(textureRect.xMax / vector.x, textureRect.yMax / vector.y), parentObject: unitySprite.gameObject, parentRenderer: unitySprite.GetComponent<Renderer>(), polygon: ref polygon, polygonPoints: ref polygonPoints, minCoords: vector2, spriteBounds: sprite.bounds, material: sharedMaterial, rotated: false, hFlipped: false, vFlipped: false, parentCentroid: Vector2.zero, uvOffset: pivotVector, isConcave: isConcave);
		_parentInstanceID = unitySprite.gameObject.GetInstanceID();
	}

	public void InitFromNGUI<T>(T nguiBasic, Material material, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave) where T : UIBasicSprite
	{
		Bounds bounds = new Bounds(nguiBasic.localCenter, nguiBasic.localSize);
		Vector2 pivotVector = GetPivotVector(nguiBasic.transform, bounds, nguiBasic.localSize);
		SetMaterialPropertyBlockTexture(nguiBasic.mainTexture);
		Vector2 vector = Vector2.zero;
		Vector2 vector2 = Vector2.one;
		if (nguiBasic is UISprite)
		{
			UISpriteData atlasSprite = (nguiBasic as UISprite).GetAtlasSprite();
			Rect rect = new Rect(atlasSprite.x, atlasSprite.y, atlasSprite.width, atlasSprite.height);
			Vector2 vector3 = new Vector2(nguiBasic.mainTexture.width, nguiBasic.mainTexture.height);
			vector = new Vector2(rect.xMin / vector3.x, 1f - rect.yMax / vector3.y);
			vector2 = new Vector2(rect.xMax / vector3.x, 1f - rect.yMin / vector3.y);
		}
		InitSprite(nguiBasic.gameObject, nguiBasic.GetComponent<Renderer>(), ref polygon, ref polygonPoints, vector, vector2, bounds, material, false, false, false, Vector2.zero, pivotVector, isConcave);
		_parentInstanceID = nguiBasic.gameObject.GetInstanceID();
	}

	private Vector2 GetPivotVector(Transform transform, Bounds bounds, Vector2 size)
	{
		Vector2 vector = bounds.min;
		Vector2 vector2 = bounds.max;
		Vector2 vector3 = transform.position;
		Vector2 vector4 = transform.lossyScale;
		Vector2 zero = Vector2.zero;
		if (vector4.x < 0f)
		{
			zero.x = vector2.x - vector3.x;
		}
		else
		{
			zero.x = vector3.x - vector.x;
		}
		if (vector4.y < 0f)
		{
			zero.y = vector2.y - vector3.y;
		}
		else
		{
			zero.y = vector3.y - vector.y;
		}
		Vector2 vector5 = new Vector2(zero.x / size.x, zero.y / size.y);
		return vector5 - new Vector2(0.5f, 0.5f);
	}

	private void SetMaterialPropertyBlockTexture(Texture texture)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetTexture("_MainTex", texture);
		_meshRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	private void InitSprite(GameObject parentObject, Renderer parentRenderer, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, Vector3 minCoords, Vector3 maxCoords, Bounds spriteBounds, Material material, bool rotated, bool hFlipped, bool vFlipped, Vector2 parentCentroid, Vector2 uvOffset, bool isConcave)
	{
		_minCoords = minCoords;
		_maxCoords = maxCoords;
		_spriteBounds = spriteBounds;
		_vFlipped = vFlipped;
		_hFlipped = hFlipped;
		_rotated = rotated;
		_spriteBounds = spriteBounds;
		_uvOffset = uvOffset;
		base.gameObject.tag = parentObject.tag;
		base.gameObject.layer = parentObject.layer;
		Mesh mesh = new Mesh();
		mesh.name = "SlicedSpriteMesh";
		_meshFilter.mesh = mesh;
		int num = polygonPoints.Length;
		Vector3[] array = new Vector3[num];
		Color[] array2 = new Color[num];
		Vector2[] array3 = new Vector2[num];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = polygonPoints[i];
			array2[i] = Color.white;
		}
		Vector2 vector = maxCoords - minCoords;
		Vector3 size = spriteBounds.size;
		Vector2 vector2 = new Vector2(1f / size.x, 1f / size.y);
		for (int j = 0; j < num; j++)
		{
			Vector2 vector3 = polygonPoints[j] + parentCentroid;
			float num2 = 0.5f + (vector3.x * vector2.x + uvOffset.x);
			float num3 = 0.5f + (vector3.y * vector2.y + uvOffset.y);
			if (hFlipped)
			{
				num2 = 1f - num2;
			}
			if (vFlipped)
			{
				num3 = 1f - num3;
			}
			Vector2 vector4 = default(Vector2);
			if (rotated)
			{
				vector4.y = maxCoords.y - vector.y * (1f - num2);
				vector4.x = minCoords.x + vector.x * num3;
			}
			else
			{
				vector4.x = minCoords.x + vector.x * num2;
				vector4.y = minCoords.y + vector.y * num3;
			}
			array3[j] = vector4;
		}
		int[] array4;
		if (isConcave)
		{
			List<Vector2> points = new List<Vector2>(polygonPoints);
			array4 = SpriteSlicer2D.Triangulate(ref points);
		}
		else
		{
			int num4 = 0;
			array4 = new int[num * 3];
			for (int k = 1; k < num - 1; k++)
			{
				array4[num4++] = 0;
				array4[num4++] = k + 1;
				array4[num4++] = k;
			}
		}
		mesh.Clear();
		mesh.vertices = array;
		mesh.uv = array3;
		mesh.triangles = array4;
		mesh.colors = array2;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		Vector2 vector5 = Vector3.zero;
		if (SpriteSlicer2D.CentreChildSprites)
		{
			vector5 = mesh.bounds.center;
			for (int l = 0; l < num; l++)
			{
				array[l] -= (Vector3)vector5;
			}
			for (int m = 0; m < num; m++)
			{
				polygonPoints[m] -= vector5;
			}
			_centroid = vector5 + parentCentroid;
			polygon.points = polygonPoints;
			mesh.vertices = array;
			mesh.RecalculateBounds();
		}
		Transform transform = parentObject.transform;
		_transform.parent = transform.parent;
		_transform.position = transform.position + transform.rotation * vector5;
		_transform.rotation = transform.rotation;
		_transform.localScale = transform.localScale;
		_meshRenderer.material = material;
		if (parentRenderer != null)
		{
			_meshRenderer.sortingLayerID = parentRenderer.sortingLayerID;
			_meshRenderer.sortingOrder = parentRenderer.sortingOrder;
		}
		TexturesUtils.AddTexture(material.mainTexture);
	}

	private void OnDestroy()
	{
		TexturesUtils.ReleaseTexture(_meshRenderer.material.mainTexture);
	}
}
