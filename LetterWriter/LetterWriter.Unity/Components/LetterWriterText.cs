using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using LetterWriter;
using LetterWriter.Markup;
using LetterWriter.Unity.Markup;

namespace LetterWriter.Unity.Components
{
    [ExecuteInEditMode]
    public class LetterWriterText : Graphic, ILayoutElement
    {
        private bool _requireReformatText = true;
        private Rect _previousRect = Rect.MinMaxRect(0, 0, 0, 0);
        private TextLine[] _formattedTextLines;

        private static LetterWriterMarkupParser _markupParser = new UnityMarkupParser();

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

        [SerializeField]
        private bool _treatNewLineAsLineBreak = true;
        public bool TreatNewLineAsLineBreak
        {
            get { return this._treatNewLineAsLineBreak; }
            set { this._treatNewLineAsLineBreak = value; }
        }

        public override Material defaultMaterial
        {
            get
            {
                return Canvas.GetDefaultCanvasMaterial();
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

            this._cachedRectTransform = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Font.textureRebuilt -= OnFontTextureRebuilt;
            this._cachedRectTransform = null;
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
            _markupParser.TreatNewLineAsLineBreak = this.TreatNewLineAsLineBreak;

            var textSource = _markupParser.Parse(this.Text);
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

#if UNITY_5 && UNITY_5_2
        protected override void OnPopulateMesh(Mesh m)
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

            var leadingBase = ((this.Font.lineHeight - (float)this.Font.fontSize) / this.Font.fontSize) / 2;

            y += (leadingBase * this.FontSize); // 一行目の分、少し上に上げておく

            m.Clear();

            using (var vertexHelper = new VertexHelper(m))
            { 
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

                        vertexHelper.AddUIVertexQuad(uiVertexes);
                    }

                    y -= lineHeight * this.LineHeight;
                }

                vertexHelper.FillMesh(m);
            }
        }
#endif
#if UNITY_5 && !UNITY_5_2
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
#endif

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
                return this.GetPreferredWidth(this.rectTransform.rect.height);
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
                return this.GetPreferredHeight(this.rectTransform.rect.width);
            }
        }
        public float flexibleHeight { get { return -1; } }
        public int layoutPriority { get { return 0; } }

#endregion

        /// <summary>
        /// 指定した高さに文章が収まる最小の幅を算出します。
        /// </summary>
        /// <param name="constraintHeight"></param>
        /// <returns></returns>
        public float GetPreferredWidth(float constraintHeight)
        {
            var formattedLines = this.FormatText(99999f); // 超巨大ということにしておく
            var firstLine = formattedLines.FirstOrDefault();
            if (firstLine == null) return 0;

            return firstLine.PlacedGlyphs.Max(x => x.X + x.Glyph.AdvanceWidth);
        }

        /// <summary>
        /// 指定した幅に文章が収まる最小の高さを算出します。
        /// </summary>
        /// <param name="constraintWidth"></param>
        /// <returns></returns>

        public float GetPreferredHeight(float constraintWidth)
        {
            if (constraintWidth < 0)
                return 0;

            var formattedLines = this.FormatText(constraintWidth);
            var height = 0f;
            var leadingBase = ((this.Font.lineHeight - (float)this.Font.fontSize) / this.Font.fontSize) / 2;

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
}
