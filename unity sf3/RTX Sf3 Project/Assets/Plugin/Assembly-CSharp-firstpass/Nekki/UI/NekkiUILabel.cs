using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nekki.UI
{
	public class NekkiUILabel : UILabel, IHasAlias
	{
		public enum Types
		{
			Localized = 0,
			Simple = 1
		}

		public class SubUnit
		{
			public UILabel Label;

			public NekkiUISprite Sprite;

			protected ImageData data;

			public GameObject Obj
			{
				get
				{
					if ((bool)Label)
					{
						return Label.gameObject;
					}
					if ((bool)Sprite)
					{
						return Sprite.gameObject;
					}
					return null;
				}
			}

			public Pivot Pivot
			{
				get
				{
					if ((bool)Label)
					{
						return Label.pivot;
					}
					if ((bool)Sprite)
					{
						return Sprite.pivot;
					}
					return Pivot.Center;
				}
			}

			public float Width
			{
				get
				{
					return UnscaledWidth * Obj.transform.localScale.x;
				}
			}

			public float UnscaledWidth
			{
				get
				{
					if ((bool)Label)
					{
						return Label.printedSize.x;
					}
					if ((bool)Sprite)
					{
						return Sprite.width;
					}
					return 0f;
				}
			}

			public float Height
			{
				get
				{
					return UnscaledHeight * Obj.transform.localScale.y;
				}
			}

			public float UnscaledHeight
			{
				get
				{
					if ((bool)Label)
					{
						return Label.printedSize.y;
					}
					if ((bool)Sprite)
					{
						return Sprite.height;
					}
					return 0f;
				}
			}

			public ImageData Data
			{
				get
				{
					return data;
				}
			}

			public MonoBehaviour Unit
			{
				get
				{
					if ((bool)Label)
					{
						return Label;
					}
					return Sprite;
				}
			}

			public SubUnit(UILabel label)
			{
				Label = label;
			}

			public SubUnit(NekkiUISprite sprite, ImageData data)
			{
				Sprite = sprite;
				this.data = data;
			}
		}

		private class Line
		{
			public NekkiUILabel Parent;

			private Vector2 _verticalBounds = Vector2.zero;

			protected LineData data;

			public List<SubUnit> SubUnits = new List<SubUnit>();

			public LineData Data
			{
				get
				{
					return data;
				}
			}

			public Vector2 VerticalBounds
			{
				get
				{
					if (_verticalBounds != Vector2.zero || SubUnits.Count == 0)
					{
						return _verticalBounds;
					}
					_verticalBounds = new Vector2(float.MaxValue, float.MinValue);
					for (int i = 0; i < SubUnits.Count; i++)
					{
						float num = (0f - SubUnits[i].UnscaledHeight) / 2f + GetSubUnitOffsetY(i);
						if (num < _verticalBounds.x)
						{
							_verticalBounds.x = num;
						}
						num = SubUnits[i].UnscaledHeight / 2f + GetSubUnitOffsetY(i);
						if (num > _verticalBounds.y)
						{
							_verticalBounds.y = num;
						}
					}
					_verticalBounds.y += data.PaddingTop;
					return _verticalBounds;
				}
			}

			public Vector3 Position
			{
				get
				{
					if (SubUnits.Count > 0)
					{
						return new Vector3(SubUnits[0].Obj.transform.localPosition.x, SubUnits[0].Obj.transform.localPosition.y + (VerticalBounds.y + VerticalBounds.x) * SubUnits[0].Obj.transform.localScale.y / 2f);
					}
					return Vector3.zero;
				}
			}

			public float Width
			{
				get
				{
					return UnscaledWidth * SubUnits[0].Obj.transform.localScale.x;
				}
			}

			public float Height
			{
				get
				{
					return UnscaledHeight * SubUnits[0].Obj.transform.localScale.y;
				}
			}

			public float UnscaledWidth
			{
				get
				{
					float num = 0f;
					for (int i = 0; i < SubUnits.Count; i++)
					{
						num += SubUnits[i].UnscaledWidth;
					}
					return num + Parent.Margin.x * (float)(SubUnits.Count - 1);
				}
			}

			public float UnscaledHeight
			{
				get
				{
					return VerticalBounds.y - VerticalBounds.x;
				}
			}

			public float Scale
			{
				get
				{
					return SubUnits[0].Obj.transform.localScale.y;
				}
			}

			public Line(NekkiUILabel parent, LineData lineData)
			{
				Parent = parent;
				data = lineData;
			}

			protected float GetSubUnitOffsetY(int index)
			{
				return (SubUnits[index].Data != null) ? ((!SubUnits[index].Data.UseDefaultImageOffset) ? SubUnits[index].Data.CustomOffsetY : Parent.DefaultImageOffsetY) : 0;
			}

			public void Add(SubUnit unit)
			{
				for (int i = 0; i < SubUnits.Count; i++)
				{
					if (SubUnits[i].Obj == unit.Obj)
					{
						SubUnits.RemoveAt(i);
						break;
					}
				}
				SubUnits.Add(unit);
			}

			public bool Remove(UILabel subLabel)
			{
				for (int i = 0; i < SubUnits.Count; i++)
				{
					if (SubUnits[i].Unit == subLabel)
					{
						SubUnits.Remove(SubUnits[i]);
						return true;
					}
				}
				return false;
			}

			public void MoveHorizontal(float delta)
			{
				for (int i = 0; i < SubUnits.Count; i++)
				{
					SubUnits[i].Obj.transform.localPosition += Vector3.left * delta;
				}
			}

			public void MoveVertical(float delta)
			{
				for (int i = 0; i < SubUnits.Count; i++)
				{
					SubUnits[i].Obj.transform.localPosition += Vector3.down * delta;
				}
			}

			public void Clear(bool editorUsePool)
			{
				for (int i = 0; i < SubUnits.Count; i++)
				{
					if ((bool)SubUnits[i].Obj)
					{
						if (Parent.Pool == null)
						{
							Parent.Pool = new ObjectPool();
						}
						if ((bool)SubUnits[i].Label)
						{
							Parent.Pool.Push(SubUnits[i].Label);
						}
						if ((bool)SubUnits[i].Sprite)
						{
							Parent.Pool.Push(SubUnits[i].Sprite);
						}
					}
				}
				SubUnits.Clear();
			}

			public void LineUp()
			{
				for (int i = 0; i < SubUnits.Count; i++)
				{
					float y = GetSubUnitOffsetY(i) * SubUnits[i].Obj.transform.localScale.x;
					if (i == 0)
					{
						SubUnits[i].Obj.transform.localPosition = new Vector3(GetXOffset() * SubUnits[i].Obj.transform.localScale.x, y);
					}
					else if ((bool)SubUnits[i - 1].Label && string.IsNullOrEmpty(SubUnits[i - 1].Label.text))
					{
						SubUnits[i].Obj.transform.localPosition = new Vector3(SubUnits[i - 1].Obj.transform.localPosition.x + Parent.Margin.x * SubUnits[i].Obj.transform.localScale.x, y);
					}
					else
					{
						SubUnits[i].Obj.transform.localPosition = new Vector3(SubUnits[i - 1].Width + SubUnits[i - 1].Obj.transform.localPosition.x + Parent.Margin.x * SubUnits[i].Obj.transform.localScale.x, y);
					}
				}
			}

			public void Shrink(float scale)
			{
				if (SubUnits.Count > 0 && SubUnits[0].Obj.transform.localScale.x != scale)
				{
					for (int i = 0; i < SubUnits.Count; i++)
					{
						SubUnits[i].Obj.transform.localScale = new Vector3(scale, scale, scale);
					}
				}
			}

			private float GetXOffset()
			{
				float result = Parent.OffsetX;
				if (Parent.overflowMethod == Overflow.ShrinkContent)
				{
					switch (Parent.pivot)
					{
					case Pivot.Left:
						if (Parent.OffsetX < 0)
						{
							result = 0f;
						}
						break;
					case Pivot.Right:
						if (Parent.OffsetX > 0)
						{
							result = 0f;
						}
						break;
					}
				}
				return result;
			}
		}

		private class Lines
		{
			public NekkiUILabel Parent;

			public List<Line> Units = new List<Line>();

			public float Height
			{
				get
				{
					return UnscaledHeight * Units[0].SubUnits[0].Obj.transform.localScale.y;
				}
			}

			public float Width
			{
				get
				{
					return UnscaledWidth * Units[0].SubUnits[0].Obj.transform.localScale.x;
				}
			}

			public float UnscaledWidth
			{
				get
				{
					float num = 0f;
					for (int i = 0; i < Units.Count; i++)
					{
						float unscaledWidth = Units[i].UnscaledWidth;
						if (unscaledWidth > num)
						{
							num = unscaledWidth;
						}
					}
					return num;
				}
			}

			public float UnscaledHeight
			{
				get
				{
					float num = 0f;
					for (int i = 0; i < Units.Count; i++)
					{
						num += Units[i].UnscaledHeight;
					}
					return num + (float)(Units.Count - 1) * Parent.Margin.y;
				}
			}

			public Lines(NekkiUILabel label)
			{
				Parent = label;
			}

			public void NewLine(LineData lineData)
			{
				Units.Add(new Line(Parent, lineData));
			}

			public void Clear(bool editorUsePool = false)
			{
				for (int i = 0; i < Units.Count; i++)
				{
					Units[i].Clear(editorUsePool);
				}
				Units.Clear();
			}

			private void AddUnit(UILabel label)
			{
				Units[Units.Count - 1].Add(new SubUnit(label));
			}

			private void AddUnit(NekkiUISprite sprite, ImageData data)
			{
				Units[Units.Count - 1].Add(new SubUnit(sprite, data));
			}

			public void LineUp()
			{
				switch (Parent.pivot)
				{
				case Pivot.TopLeft:
					LineUpHorizontalLeft();
					break;
				case Pivot.Top:
					LineUpHorizontalCenter();
					break;
				case Pivot.TopRight:
					LineUpHorizontalRight();
					break;
				case Pivot.Left:
					LineUpHorizontalLeft();
					break;
				case Pivot.Center:
					LineUpHorizontalCenter();
					break;
				case Pivot.Right:
					LineUpHorizontalRight();
					break;
				case Pivot.BottomLeft:
					LineUpHorizontalLeft();
					break;
				case Pivot.Bottom:
					LineUpHorizontalCenter();
					break;
				case Pivot.BottomRight:
					LineUpHorizontalRight();
					break;
				}
			}

			private void LineUpRows()
			{
				float num = 1f;
				if (Parent.overflowMethod == Overflow.ShrinkContent)
				{
					num = Mathf.Min((float)Parent.width / (UnscaledWidth + (float)Mathf.Abs(Parent.OffsetX)), (float)Parent.height / UnscaledHeight, 1f);
				}
				for (int i = 0; i < Units.Count; i++)
				{
					Units[i].Parent = Parent;
					Units[i].Shrink(num);
					Units[i].LineUp();
					if (i > 0)
					{
						Units[i].MoveVertical(Units[i].Position.y - Units[i - 1].Position.y + Units[i - 1].Height / 2f + Units[i].Height / 2f + Parent.Margin.y * num);
					}
				}
				float num2 = Units[0].Position.y + Units[0].Height / 2f;
				switch (Parent.pivot)
				{
				case Pivot.TopLeft:
				case Pivot.Top:
				case Pivot.TopRight:
				{
					for (int k = 0; k < Units.Count; k++)
					{
						Units[k].MoveVertical(num2);
					}
					break;
				}
				case Pivot.Left:
				case Pivot.Center:
				case Pivot.Right:
				{
					for (int l = 0; l < Units.Count; l++)
					{
						Units[l].MoveVertical(num2 - Height / 2f);
					}
					break;
				}
				case Pivot.BottomLeft:
				case Pivot.Bottom:
				case Pivot.BottomRight:
				{
					for (int j = 0; j < Units.Count; j++)
					{
						Units[j].MoveVertical(num2 - Height);
					}
					break;
				}
				}
			}

			private void LineUpHorizontalLeft()
			{
				LineUpRows();
				for (int i = 0; i < Units.Count; i++)
				{
					Units[i].MoveHorizontal(0f);
				}
			}

			private void LineUpHorizontalRight()
			{
				LineUpRows();
				for (int i = 0; i < Units.Count; i++)
				{
					Units[i].MoveHorizontal(Units[i].Width);
				}
			}

			private void LineUpHorizontalCenter()
			{
				LineUpRows();
				for (int i = 0; i < Units.Count; i++)
				{
					Units[i].MoveHorizontal((Units[i].Width + (float)Parent.OffsetX * Units[i].Scale) / 2f);
				}
			}

			public void Remove(UILabel subLabel)
			{
				for (int i = 0; i < Units.Count && !Units[i].Remove(subLabel); i++)
				{
				}
			}

			public UILabel InsertLabel()
			{
				UILabel uILabel = ((Parent.Pool == null) ? null : Parent.Pool.GetLabel);
				if (!uILabel)
				{
					Transform transform = new GameObject("_subLabel").transform;
					transform.gameObject.layer = Parent.gameObject.layer;
					transform.parent = Parent.transform;
					transform.localPosition = Vector3.zero;
					transform.localScale = Vector3.one;
					transform.localEulerAngles = Vector3.zero;
					uILabel = transform.gameObject.AddComponent<UILabel>();
				}
				uILabel.bitmapFont = Parent.bitmapFont;
				uILabel.trueTypeFont = Parent.trueTypeFont;
				uILabel.fontSize = Parent.fontSize;
				uILabel.fontStyle = Parent.fontStyle;
				uILabel.ambigiousFont = Parent.ambigiousFont;
				uILabel.alignment = Parent.alignment;
				uILabel.applyGradient = Parent.applyGradient;
				uILabel.gradientTop = Parent.gradientTop;
				uILabel.gradientBottom = Parent.gradientBottom;
				uILabel.effectStyle = Parent.effectStyle;
				uILabel.effectColor = Parent.effectColor;
				uILabel.effectDistance = Parent.effectDistance;
				uILabel.spacingX = Parent.spacingX;
				uILabel.spacingY = Parent.spacingY;
				uILabel.maxLineCount = Parent.maxLineCount;
				uILabel.symbolStyle = Parent.symbolStyle;
				uILabel.color = Parent.color;
				uILabel.depth = Parent.depth;
				uILabel.aspectRatio = Parent.aspectRatio;
				uILabel.keepAspectRatio = Parent.keepAspectRatio;
				uILabel.keepCrispWhenShrunk = Parent.keepCrispWhenShrunk;
				uILabel.pivot = Pivot.Left;
				uILabel.overflowMethod = Overflow.ResizeFreely;
				AddUnit(uILabel);
				return uILabel;
			}

			public NekkiUISprite InsertImage(string sprite, float scale, bool rowSize, ImageData data)
			{
				NekkiUISprite nekkiUISprite = ((Parent.Pool == null) ? null : Parent.Pool.GetSprite);
				if (!nekkiUISprite)
				{
					Transform transform = new GameObject("_subSprite").transform;
					transform.gameObject.layer = Parent.gameObject.layer;
					transform.parent = Parent.transform;
					transform.localPosition = Vector3.zero;
					transform.localScale = Vector3.one;
					nekkiUISprite = transform.gameObject.AddComponent<NekkiUISprite>();
				}
				nekkiUISprite.pivot = Pivot.Left;
				nekkiUISprite.atlas = Parent.Images;
				nekkiUISprite.SetAtlas(Parent.High, Parent.Low);
				nekkiUISprite.spriteName = sprite;
				nekkiUISprite.depth = Parent.depth;
				nekkiUISprite.MakePixelPerfect();
				if (rowSize)
				{
					float num = (float)nekkiUISprite.height / Parent.printedSize.y;
					nekkiUISprite.width = (int)((float)nekkiUISprite.width / num);
					nekkiUISprite.height = (int)((float)nekkiUISprite.height / num);
				}
				else if (Math.Abs(scale - 100f) > 0.1f)
				{
					nekkiUISprite.width = (int)((float)nekkiUISprite.width / 100f * scale);
					nekkiUISprite.height = (int)((float)nekkiUISprite.height / 100f * scale);
				}
				AddUnit(nekkiUISprite, data);
				return nekkiUISprite;
			}
		}

		public class ObjectPool
		{
			public List<UILabel> Labels;

			public List<NekkiUISprite> Sprites;

			public NekkiUISprite GetSprite
			{
				get
				{
					if (Sprites != null && Sprites.Count > 0)
					{
						NekkiUISprite nekkiUISprite = Sprites[0];
						nekkiUISprite.gameObject.SetActive(true);
						Sprites.RemoveAt(0);
						return nekkiUISprite;
					}
					return null;
				}
			}

			public UILabel GetLabel
			{
				get
				{
					if (Labels != null && Labels.Count > 0)
					{
						UILabel uILabel = Labels[0];
						uILabel.gameObject.SetActive(true);
						Labels.RemoveAt(0);
						return uILabel;
					}
					return null;
				}
			}

			public void Push(UILabel label)
			{
				if ((bool)label)
				{
					if (Labels == null)
					{
						Labels = new List<UILabel>();
					}
					if (!Labels.Contains(label))
					{
						label.text = string.Empty;
						label.transform.localPosition = Vector3.zero;
						label.transform.localScale = Vector3.one;
						label.gameObject.SetActive(false);
						Labels.Add(label);
					}
				}
			}

			public void Push(NekkiUISprite sprite)
			{
				if ((bool)sprite)
				{
					if (Sprites == null)
					{
						Sprites = new List<NekkiUISprite>();
					}
					if (!Sprites.Contains(sprite))
					{
						sprite.transform.localPosition = Vector3.zero;
						sprite.transform.localScale = Vector3.one;
						sprite.gameObject.SetActive(false);
						Sprites.Add(sprite);
					}
				}
			}
		}

		[Serializable]
		public class ImageData
		{
			[SerializeField]
			private int _customOffsetY;

			[NonSerialized]
			public NekkiUISprite Sprite;

			public int sectionID;

			public bool UseDefaultImageOffset = true;

			public int CustomOffsetY
			{
				get
				{
					return _customOffsetY;
				}
				set
				{
					_customOffsetY = value;
					if (Application.isPlaying)
					{
						UseDefaultImageOffset = false;
					}
				}
			}
		}

		[Serializable]
		public class LineData
		{
			private int _paddingTop;

			public int PaddingTop
			{
				get
				{
					return _paddingTop;
				}
				set
				{
					_paddingTop = value;
				}
			}
		}

		private Lines _lines;

		[SerializeField]
		private Vector2 _margin = Vector2.zero;

		[SerializeField]
		private Types _type = Types.Simple;

		[SerializeField]
		private string _alias;

		private bool _initDone;

		[SerializeField]
		protected UIAtlas Low;

		[SerializeField]
		protected UIAtlas High;

		[NonSerialized]
		public ObjectPool Pool;

		[SerializeField]
		public UIAtlas Images;

		[NonSerialized]
		public LocaleImport.LocaleString LocaleString;

		[SerializeField]
		public string[] LastReplacement;

		[SerializeField]
		public List<ImageData> ImagesInfo;

		public int DefaultImageOffsetY;

		public int OffsetX;

		public new string text
		{
			get
			{
				return base.text;
			}
			set
			{
				string arg = base.text;
				base.text = value;
				if (this.OnTextChangeEvent != null)
				{
					this.OnTextChangeEvent(arg, base.text);
				}
			}
		}

		public int MaxSymbols { get; set; }

		public new Vector3 localSize
		{
			get
			{
				if (LocaleString.Simple)
				{
					return base.localSize;
				}
				float num = 0f;
				float num2 = 0f;
				foreach (Line unit in _lines.Units)
				{
					if (unit.Width > num2)
					{
						num2 = unit.Width;
					}
					num += unit.Height;
				}
				return new Vector2(num2, num);
			}
		}

		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				if (_alias == value)
				{
					return;
				}
				if (Type == Types.Simple)
				{
					Debug.LogError("cant set alias for a plain text!");
					return;
				}
				_alias = value;
				Clear();
				if (string.IsNullOrEmpty(_alias))
				{
					text = string.Empty;
				}
				else if (Localization.Contains(_alias, true))
				{
					LocaleString = ((MaxSymbols != 0) ? Localization.Get(_alias, MaxSymbols, LastReplacement) : Localization.Get(_alias));
					if (LocaleString.Simple)
					{
						text = LocaleString;
						if (text.Contains("\\n"))
						{
							text = text.Replace("\\n", "\n");
						}
						return;
					}
					ImagesInfo = new List<ImageData>();
					foreach (LocaleImport.LocaleString.Section section in LocaleString.Sections)
					{
						if (section.SectionType == LocaleImport.LocaleString.SectionTypes.Image)
						{
							ImageData imageData = new ImageData();
							imageData.sectionID = section.ID;
							ImagesInfo.Add(imageData);
						}
					}
					ImagesInfo.Sort((ImageData a, ImageData b) => a.sectionID.CompareTo(b.sectionID));
					text = string.Empty;
				}
				else
				{
					text = "Error_" + _alias;
				}
			}
		}

		public Types Type
		{
			get
			{
				return _type;
			}
			set
			{
				if (_type != value)
				{
					_type = value;
					switch (_type)
					{
					case Types.Localized:
						text = string.Empty;
						break;
					case Types.Simple:
						_alias = string.Empty;
						Clear();
						break;
					}
				}
			}
		}

		public Vector2 Margin
		{
			get
			{
				return _margin;
			}
			set
			{
				if (!(Vector2.Distance(_margin, value) < 0.01f))
				{
					_margin = value;
					if (ImagesCreated)
					{
						Format(LastReplacement);
					}
				}
			}
		}

		protected bool ImagesCreated
		{
			get
			{
				return ImagesInfo != null && ImagesInfo.Count > 0 && ImagesInfo[0].Sprite != null;
			}
		}

		public new Overflow overflowMethod
		{
			get
			{
				return base.overflowMethod;
			}
			set
			{
				if (base.overflowMethod == Overflow.ResizeFreely && value != Overflow.ResizeFreely && ImagesCreated)
				{
					base.overflowMethod = value;
					ResetSize();
				}
				else
				{
					base.overflowMethod = value;
				}
			}
		}

		public event Action<string, string> OnTextChangeEvent;

		protected override void OnStart()
		{
			base.OnStart();
			Init(true);
		}

		protected void OnDestroy()
		{
			Localization.LanguageSwitched -= OnLanguageSwitch;
		}

		public void OnLanguageSwitch()
		{
			if (Type == Types.Localized)
			{
				if (LastReplacement != null && LastReplacement.Length > 0)
				{
					Format(LastReplacement);
					return;
				}
				string alias = _alias;
				_alias = string.Empty;
				Alias = alias;
			}
		}

		public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
		{
			if (overflowMethod == Overflow.ShrinkContent && ImagesCreated)
			{
				LineUp();
			}
			base.OnFill(verts, uvs, cols);
		}

		protected virtual void Init(bool format)
		{
			if (!_initDone)
			{
				_initDone = true;
				if ((bool)High && (bool)Low)
				{
					Images = ((!SystemProperties.IsHighResolution) ? Low : High);
				}
				if (format)
				{
					OnLanguageSwitch();
				}
				Localization.LanguageSwitched += OnLanguageSwitch;
			}
		}

		protected virtual void Clear()
		{
			if (_lines != null)
			{
				_lines.Clear();
			}
			LocaleString = null;
			if (MaxSymbols == 0)
			{
				LastReplacement = null;
			}
			ImagesInfo = null;
		}

		protected void RefreshPool()
		{
			Pool = new ObjectPool();
			UILabel[] componentsInChildren = GetComponentsInChildren<UILabel>(true);
			NekkiUISprite[] componentsInChildren2 = GetComponentsInChildren<NekkiUISprite>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != this)
				{
					Pool.Push(componentsInChildren[i]);
				}
			}
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].name == "_subSprite")
				{
					Pool.Push(componentsInChildren2[j]);
				}
			}
		}

		protected virtual void ResetSize()
		{
			base.width = GetLinesWidth();
			base.height = GetLinesHeight();
		}

		public void SetImagesAtlas(UIAtlas high, UIAtlas low)
		{
			Low = low ?? Low;
			High = high ?? High;
		}

		public ImageData GetImageDataByID(int sectionID)
		{
			if (ImagesInfo == null)
			{
				return null;
			}
			foreach (ImageData item in ImagesInfo)
			{
				if (item.sectionID == sectionID)
				{
					return item;
				}
			}
			ImageData imageData = new ImageData();
			imageData.sectionID = sectionID;
			ImagesInfo.Add(imageData);
			return imageData;
		}

		public void Format(params object[] replacement)
		{
			Init(false);
			if (!Localization.Contains(_alias, true))
			{
				text = "ERROR_" + _alias;
			}
			if (_lines == null)
			{
				_lines = new Lines(this);
				RefreshPool();
			}
			else
			{
				_lines.Clear();
			}
			_lines.Parent = this;
			_lines.NewLine(new LineData());
			LastReplacement = new string[replacement.Length];
			for (int i = 0; i < LastReplacement.Length; i++)
			{
				if (replacement[i] == null)
				{
					Debug.LogError("Image is not set");
					return;
				}
				LastReplacement[i] = replacement[i].ToString();
			}
			LocaleString = ((MaxSymbols != 0) ? Localization.Get(_alias, MaxSymbols, LastReplacement) : Localization.Get(_alias));
			string[] array = LocaleString.CompileWith(replacement, this);
			if (array.Length == 0)
			{
				return;
			}
			if (LocaleString.Simple || (array.Length == 1 && !LocaleString.ContainsImageRef))
			{
				text = array[0];
				return;
			}
			text = string.Empty;
			foreach (LocaleImport.LocaleString.Section section in LocaleString.Sections)
			{
				if (section.ContainsPosition(-1))
				{
					if (replacement.Length <= section.ID)
					{
						Debug.LogError("Image name is not set");
						return;
					}
					ImageData imageDataByID = GetImageDataByID(section.ID);
					imageDataByID.Sprite = _lines.InsertImage(replacement[section.ID].ToString(), section.GetPositionAtIndex(-1).Scale, section.GetPositionAtIndex(-1).ScaleToRow, imageDataByID);
					break;
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].Contains("\n") || array[j].Contains("\0"))
				{
					string[] array2 = array[j].Split('\n');
					for (int k = 0; k < array2.Length; k++)
					{
						UILabel uILabel = _lines.InsertLabel();
						uILabel.text = array2[k];
						if (k >= array2.Length - 1)
						{
							continue;
						}
						LineData lineData = new LineData();
						try
						{
							if (array2[k + 1].Substring(0, 6) == "[line:")
							{
								int num = array2[k + 1].IndexOf(']');
								int paddingTop = int.Parse(array2[k + 1].Substring(6, array2[k + 1].IndexOf(']') - 6));
								lineData.PaddingTop = paddingTop;
								array2[k + 1] = array2[k + 1].Substring(num + 1);
							}
						}
						catch (Exception)
						{
						}
						_lines.NewLine(lineData);
					}
				}
				else
				{
					UILabel uILabel2 = _lines.InsertLabel();
					uILabel2.text = array[j];
				}
				foreach (LocaleImport.LocaleString.Section section2 in LocaleString.Sections)
				{
					if (section2.Positions.Count > 0 && section2.ContainsPosition(j))
					{
						if (replacement.Length <= section2.ID)
						{
							Debug.LogError("Image name is not set");
							return;
						}
						ImageData imageDataByID2 = GetImageDataByID(section2.ID);
						imageDataByID2.Sprite = _lines.InsertImage(replacement[section2.ID].ToString(), section2.GetPositionAtIndex(j).Scale, section2.GetPositionAtIndex(j).ScaleToRow, imageDataByID2);
					}
				}
			}
			LineUp();
		}

		public void LineUp()
		{
			if (_lines != null)
			{
				_lines.LineUp();
			}
		}

		public int GetLinesWidth()
		{
			return Mathf.CeilToInt(_lines.UnscaledWidth);
		}

		public int GetLinesHeight()
		{
			return Mathf.CeilToInt(_lines.UnscaledHeight);
		}
	}
}
