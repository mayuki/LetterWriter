using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using LetterWriter;
using LetterWriter.Markup;

namespace LetterWriter.Unity
{
    [ExecuteInEditMode]
    public class LetterWriterText : Graphic, ILayoutElement
    {
        private bool _requireReformatText = true;
        private Rect _previousRect = Rect.MinMaxRect(0, 0, 0, 0);
        private TextLine[] _formattedTextLines;

        private RectTransform _cachedRectTransform;
        public RectTransform CachedRectTransform
        {
            get { return this._cachedRectTransform ?? (this._cachedRectTransform = this.GetComponent<RectTransform>()); }
        }

        [SerializeField]
        private Font _font;
        public Font Font
        {
            get { return this._font; }
            set { this._font = value; this.MarkAsReformatRequired(); }
        }

        [SerializeField]
        [Multiline]
        private string _text;
        public string Text
        {
            get { return this._text; }
            set { this._text = value; this.MarkAsReformatRequired(); }
        }

        [SerializeField]
        private int _fontSize = 24;
        public int FontSize
        {
            get { return this._fontSize; }
            set { this._fontSize = value; this.MarkAsReformatRequired(); }
        }

        [SerializeField]
        private int _lineSpacing = 0;
        public int LineSpacing
        {
            get { return this._lineSpacing; }
            set { this._lineSpacing = value; this.MarkAsReformatRequired(); }
        }

        [SerializeField]
        private int _spacing = 0;
        public int Spacing
        {
            get { return this._spacing; }
            set { this._spacing = value; this.MarkAsReformatRequired(); }
        }

        [SerializeField]
        private float _lineHeight = 1;
        public float LineHeight
        {
            get { return this._lineHeight; }
            set { this._lineHeight = value; this.MarkAsReformatRequired(); }
        }

        [SerializeField]
        private int _visibleLength = -1;
        public int VisibleLength
        {
            get { return this._visibleLength; }
            set { this._visibleLength = value; this.MarkAsReformatRequired(); }
        }

        [SerializeField]
        private HorizontalWrapMode _horizontalOverflow = HorizontalWrapMode.Wrap;
        public HorizontalWrapMode HorizontalOverflow
        {
            get { return this._horizontalOverflow; }
            set { this._horizontalOverflow = value; }
        }

        [SerializeField]
        private VerticalWrapMode _verticalOverflow = VerticalWrapMode.Overflow;
        public VerticalWrapMode VerticalOverflow
        {
            get { return this._verticalOverflow; }
            set { this._verticalOverflow = value; }
        }

        public override Material defaultMaterial
        {
            get
            {
                return Canvas.GetDefaultCanvasTextMaterial();
            }
        }

        public override Texture mainTexture
        {
            get
            {
                return this.Font.material.mainTexture;
            }
        }

        protected void MarkAsReformatRequired()
        {
            this._requireReformatText = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this.MarkAsReformatRequired();
            Font.textureRebuilt += OnFontTextureRebuilt;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Font.textureRebuilt -= OnFontTextureRebuilt;
        }

        protected virtual void OnFontTextureRebuilt(Font font)
        {
            this.MarkAsReformatRequired();
        }

        protected virtual void UpdateText()
        {
            this._formattedTextLines = this.FormatText((this.HorizontalOverflow == HorizontalWrapMode.Wrap) ? this.rectTransform.rect.width : 99999f);
        }

        protected virtual TextLine[] FormatText(float width)
        {
            var textSource = new UnityMarkupParser().Parse(this.Text);
            var textLineBreakState = new TextLineBreakState();
            var textFormatter = new UnityTextFormatter(this.Font, this.FontSize, this.color);

            var textLines = new List<TextLine>();
            while (true)
            {
                var textLine = textFormatter.FormatLine(textSource, (int)width, textLineBreakState);
                if (textLine == null)
                    break;

                textLines.Add(textLine);
            }

            return textLines.ToArray();
        }

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            if (this.Font == null)
                return;

            // 何か変化があったら再フォーマットする
            if (this._requireReformatText || this._previousRect != this.CachedRectTransform.rect)
            {
                this.UpdateText();
                this._requireReformatText = false;
            }

            // 開始位置
            var x = this.rectTransform.rect.xMin;
            var y = this.rectTransform.rect.yMax;

            var leadingBase = ((this.Font.lineHeight - (float) this.Font.fontSize)/this.Font.fontSize) / 2;

            y += (leadingBase*this.FontSize); // 一行目の分、少し上に上げておく

            foreach (var textLine in this._formattedTextLines)
            {
                var lineHeight = (this.FontSize + (leadingBase * this.FontSize));

                // 上に突き抜けてる分を計算してあげないと…
                if (textLine.PlacedGlyphs.Any(p => p.Y < 0))
                {
                    lineHeight += textLine.PlacedGlyphs.Where(p => p.Y < 0).Max(p => p.Glyph.Height);
                }

                // オーバーフロー
                if (this.VerticalOverflow == VerticalWrapMode.Truncate && this.rectTransform.rect.yMin > (y - lineHeight))
                {
                    break;
                }

                foreach (var placedGlyph in textLine.PlacedGlyphs.Where(p => p != GlyphPlacement.Empty && (p.Index < this._visibleLength || this._visibleLength == -1)))
                {
                    var glyph = (UnityGlyph)placedGlyph.Glyph;
                    var uiVertexes = glyph.BaseVertices;

                    uiVertexes[0].position.x += placedGlyph.X + x;
                    uiVertexes[0].position.y += -placedGlyph.Y + y - lineHeight;

                    uiVertexes[1].position.x += placedGlyph.X + x;
                    uiVertexes[1].position.y += -placedGlyph.Y + y - lineHeight;

                    uiVertexes[2].position.x += placedGlyph.X + x;
                    uiVertexes[2].position.y += -placedGlyph.Y + y - lineHeight;

                    uiVertexes[3].position.x += placedGlyph.X + x;
                    uiVertexes[3].position.y += -placedGlyph.Y + y - lineHeight;

                    vbo.AddRange(uiVertexes);
                }

                y -= lineHeight * this.LineHeight;
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            this.Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
#endif

        #region ILayoutElement Implementation

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        public float minHeight { get { return 0; } }
        public float minWidth { get { return 0; } }

        /// <summary>
        /// 幅に対する制限がないものとして最大の横幅(1行の最大長)
        /// </summary>
        public float preferredWidth
        {
            get
            {
                var formattedLines = this.FormatText(99999f); // 超巨大ということにしておく
                var firstLine = formattedLines.FirstOrDefault();
                if (firstLine == null) return 0;

                return firstLine.PlacedGlyphs.Max(x => x.X + x.Glyph.AdvanceWidth);
            }
        }
        public float flexibleWidth { get { return -1; } }

        /// <summary>
        /// 現在の幅を固定した想定で必要となる高さ(折り返しも含まれる)
        /// </summary>
        public float preferredHeight
        {
            get
            {
                if (this.rectTransform.rect.width < 0)
                    return 0;

                var formattedLines = this.FormatText(this.rectTransform.rect.width);
                var height = 0f;
                var leadingBase = ((this.Font.lineHeight - (float) this.Font.fontSize)/this.Font.fontSize)/2;

                height -= (leadingBase * this.FontSize); // 一行目の分、少し上に上げておく

                foreach (var textLine in formattedLines)
                {
                    var lineHeight = (this.FontSize + (leadingBase * this.FontSize));

                    // 上に突き抜けてる分を計算してあげないと…
                    if (textLine.PlacedGlyphs.Any(p => p.Y < 0))
                    {
                        lineHeight += textLine.PlacedGlyphs.Where(p => p.Y < 0).Max(p => p.Glyph.Height);
                    }

                    height += lineHeight * this.LineHeight;
                }

                height += (leadingBase * this.FontSize);

                return height;
            }
        }
        public float flexibleHeight { get { return -1; } }
        public int layoutPriority { get { return 0; } }

        #endregion
    }

    public class UnityMarkupParser : LetterWriter.Markup.LetterWriterMarkupParser
    {
        private Dictionary<string, Color> _colorTable = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            {"red", new Color(1f, 0.0f, 0.0f, 1f)},
            {"green", new Color(0.0f, 1f, 0.0f, 1f)},
            {"blue", new Color(0.0f, 0.0f, 1f, 1f)},
            {"white", new Color(1f, 1f, 1f, 1f)},
            {"black", new Color(0.0f, 0.0f, 0.0f, 1f)},
            {"yellow", new Color(1f, 0.9215686f, 0.01568628f, 1f)},
            {"cyan", new Color(0.0f, 1f, 1f, 1f)},
            {"magenta", new Color(1f, 0.0f, 1f, 1f)},
            {"gray", new Color(0.5f, 0.5f, 0.5f, 1f)},
            {"grey", new Color(0.5f, 0.5f, 0.5f, 1f)},
            {"clear", new Color(0.0f, 0.0f, 0.0f, 0.0f)},
        };


        protected override TextRun[] VisitMarkupElement(Element element, string tagNameUpper)
        {
            if (tagNameUpper == "RUBY")
            {
                Color? color = null;
                float? scale = null;

                if (element.Attributes.ContainsKey("color"))
                {
                    color = this._colorTable[element.Attributes["Color"]];
                }
                if (element.Attributes.ContainsKey("scale"))
                {
                    var tmpSize = 0f;
                    if (Single.TryParse(element.Attributes["Scale"], out tmpSize))
                    {
                        scale = tmpSize;
                    }
                }

                return
                        new TextRun[] { new UnityTextModifier() { RubyColor = color, RubyFontScale = scale } }
                        .Concat(base.VisitMarkupElement(element, tagNameUpper))
                        .Concat(new TextRun[] { new TextEndOfSegment() })
                        .ToArray();
            }

            if (tagNameUpper == "COLOR")
            {
                if (!_colorTable.ContainsKey(element.Attributes["Value"]))
                    return base.VisitMarkupElement(element, tagNameUpper);

                return
                    new TextRun[]
                    {
                        new UnityTextModifier() { Color = this._colorTable[element.Attributes["Value"]] }
                    }
                    .Concat(base.VisitMarkupElement(element, tagNameUpper))
                    .Concat(new TextRun[] { new TextEndOfSegment() })
                    .ToArray();
            }
            if (tagNameUpper == "SIZE")
            {
                var size = 0;
                if (Int32.TryParse(element.Attributes["Value"], out size))
                {
                    return
                        new TextRun[]
                        {
                            new UnityTextModifier() { FontSize = size }
                        }
                        .Concat(base.VisitMarkupElement(element, tagNameUpper))
                        .Concat(new TextRun[] { new TextEndOfSegment() })
                        .ToArray();
                }
                else
                {
                    return base.VisitMarkupElement(element, tagNameUpper);
                }
            }

            if (tagNameUpper == "B")
            {
                return new TextRun[]
                    {
                        new UnityTextModifier() { FontStyle = FontStyle.Bold }
                    }
                    .Concat(base.VisitMarkupElement(element, tagNameUpper))
                    .Concat(new TextRun[] { new TextEndOfSegment() })
                    .ToArray();
            }

            return base.VisitMarkupElement(element, tagNameUpper);
        }
    }

    public class UnityTextFormatter : LetterWriter.TextFormatter
    {
        public Font Font { get; private set; }
        public int FontSize { get; private set; }
        public Color Color { get; private set; }

        public UnityTextFormatter(Font font, int fontSize, Color color)
        {
            this.Font = font;
            this.FontSize = fontSize;
            this.Color = color;

            this.Initialize();
        }

        public override GlyphProvider CreateGlyphProvider()
        {
            return new UnityGlyphProvider(this.Font);
        }

        public override TextModifierScope CreateTextModifierScope(TextModifierScope parent, TextModifier textModifier)
        {
            return new UnityTextModifierScope((UnityTextModifierScope)parent, (UnityTextModifier)textModifier ?? new UnityTextModifier { FontSize = this.FontSize, FontStyle = FontStyle.Normal, Spacing = 0, RubyFontScale = 0.5f, Color = this.Color });
        }
    }

    public class UnityTextModifier : TextModifier
    {
        public Color? RubyColor { get; set; }
        public float? RubyFontScale { get; set; }
        public FontStyle? RubyFontStyle { get; set; }

        public int? FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }

        public Color? Color { get; set; }
    }

    public class UnityTextModifierScope : TextModifierScope<UnityTextModifier>, IRubyTextModifierScope
    {
        protected new UnityTextModifierScope Parent { get { return (UnityTextModifierScope)base.Parent; } }

        public Color? RubyColor
        {
            get { return this.TextModifier.RubyColor ?? ((this.Parent != null) ? this.Parent.RubyColor : null); }
            set { this.TextModifier.RubyColor = value; }
        }
        public FontStyle? RubyFontStyle
        {
            get { return this.TextModifier.RubyFontStyle ?? ((this.Parent != null) ? this.Parent.RubyFontStyle : null); }
            set { this.TextModifier.RubyFontStyle = value; }
        }
        public float? RubyFontScale
        {
            get { return this.TextModifier.RubyFontScale ?? ((this.Parent != null) ? this.Parent.RubyFontScale : null); }
            set { this.TextModifier.RubyFontScale = value; }
        }
        public int? FontSize
        {
            get { return this.TextModifier.FontSize ?? ((this.Parent != null) ? this.Parent.FontSize : null); }
            set { this.TextModifier.FontSize = value; }
        }
        public FontStyle? FontStyle
        {
            get { return this.TextModifier.FontStyle ?? ((this.Parent != null) ? this.Parent.FontStyle : null); }
            set { this.TextModifier.FontStyle = value; }
        }
        public Color? Color
        {
            get { return this.TextModifier.Color ?? ((this.Parent != null) ? this.Parent.Color : null); }
            set { this.TextModifier.Color = value; }
        }

        public UnityTextModifierScope(TextModifierScope<UnityTextModifier> parent, UnityTextModifier textModifier) : base(parent, textModifier)
        {
        }

        public override void Apply(UnityTextModifier textModifier)
        {
            base.Apply(textModifier);

            this.RubyColor = textModifier.RubyColor;
            this.RubyFontScale = textModifier.RubyFontScale;
            this.RubyFontStyle = textModifier.RubyFontStyle;
            this.FontSize = textModifier.FontSize;
            this.FontStyle = textModifier.FontStyle;
            this.Color = textModifier.Color;
        }

        public TextModifierScope RubyScope
        {
            get
            {
                return new UnityTextModifierScope(this, new UnityTextModifier() { Color = this.RubyColor, FontSize = (int)((this.FontSize ?? 1) * (this.RubyFontScale ?? 1)), FontStyle = this.RubyFontStyle});
            }
        }
    }


    public class UnityGlyphProvider : GlyphProvider<UnityTextModifierScope>
    {
        public Font Font { get; set; }

        public UnityGlyphProvider(Font font)
        {
            this.Font = font;
        }

        protected override IGlyph[] GetGlyphsFromStringCore(UnityTextModifierScope textModifierScope, string value)
        {
            var fontSize = textModifierScope.FontSize ?? 24;
            var fontStyle = textModifierScope.FontStyle ?? FontStyle.Normal;

            var textGenerator = new TextGenerator();
            if (!textGenerator.Populate(value + "…M", new TextGenerationSettings() { font = this.Font, fontSize = fontSize, fontStyle = fontStyle }))
            {
                throw new Exception("TextGenerator.Populate failed");
            }

            this.Font.RequestCharactersInTexture(value, fontSize, fontStyle);

            return value.Select(c =>
            {
                if (Char.IsControl(c)) return null;

                CharacterInfo characterInfo;
                if (!this.Font.GetCharacterInfo(c, out characterInfo, fontSize, fontStyle))
                {
                    throw new Exception("this.Font.GetCharacterInfo failed: " + c);
                }
                return new UnityGlyph(c, characterInfo, textModifierScope.Color, fontSize);
            }).Where(x => x != null).ToArray();
        }
    }

    public class UnityGlyph : Glyph
    {
        public CharacterInfo CharacterInfo { get; private set; }

        public Color? Color { get; private set; }

        public UnityGlyph(char character, CharacterInfo characterInfo, Color? color, int height) : base(character, characterInfo.advance, height)
        {
            this.CharacterInfo = characterInfo;
            this.Color = color;
        }

        public UIVertex[] BaseVertices
        {
            get
            {
                var verts = Enumerable.Range(0, 4).Select(_ => UIVertex.simpleVert).ToArray();

                // MEMO: なぜか高さはuGUIと微妙に違うサイズのが出てくる…。TextGeneratorが誤差であれになってるのではという気もしなくなくもなくもない。

                // 左下
                verts[0].uv0 = this.CharacterInfo.uvBottomLeft;
                verts[0].position = new Vector3(this.CharacterInfo.minX, this.CharacterInfo.minY);

                // 右下
                verts[1].uv0 = this.CharacterInfo.uvBottomRight;
                verts[1].position = new Vector3(this.CharacterInfo.maxX, this.CharacterInfo.minY);

                // 右上
                verts[2].uv0 = this.CharacterInfo.uvTopRight;
                verts[2].position = new Vector3(this.CharacterInfo.maxX, this.CharacterInfo.maxY);

                // 左上
                verts[3].uv0 = this.CharacterInfo.uvTopLeft;
                verts[3].position = new Vector3(this.CharacterInfo.minX, this.CharacterInfo.maxY);

                if (this.Color.HasValue)
                {
                    verts[0].color = this.Color.Value;
                    verts[1].color = this.Color.Value;
                    verts[2].color = this.Color.Value;
                    verts[3].color = this.Color.Value;
                }

                return verts;
            }
        }
    }
}
