using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter;
using UnityEngine;

namespace LetterWriter.Unity
{
    public class UnityGlyph : Glyph
    {
        private UIVertex[] _baseVertices = new[] { UIVertex.simpleVert, UIVertex.simpleVert, UIVertex.simpleVert, UIVertex.simpleVert, };

        public CharacterInfo CharacterInfo { get; private set; }

        public Color? Color { get; private set; }

        public UnityGlyph(char character, CharacterInfo characterInfo, Color? color, int height) : base(character, characterInfo.advance, height)
        {
            this.CharacterInfo = characterInfo;
            this.Color = color;
            this.UpdateBaseVertices();
        }

        public UIVertex[] BaseVertices
        {
            get
            {
                return (UIVertex[])this._baseVertices.Clone();
            }
        }

        private void UpdateBaseVertices()
        {
            // MEMO: なぜか高さはuGUIと微妙に違うサイズのが出てくる…。TextGeneratorが誤差であれになってるのではという気もしなくなくもなくもない。

            // 左下
            this._baseVertices[0].uv0 = this.CharacterInfo.uvBottomLeft;
            this._baseVertices[0].position = new Vector3(this.CharacterInfo.minX, this.CharacterInfo.minY);

            // 右下
            this._baseVertices[1].uv0 = this.CharacterInfo.uvBottomRight;
            this._baseVertices[1].position = new Vector3(this.CharacterInfo.maxX, this.CharacterInfo.minY);

            // 右上
            this._baseVertices[2].uv0 = this.CharacterInfo.uvTopRight;
            this._baseVertices[2].position = new Vector3(this.CharacterInfo.maxX, this.CharacterInfo.maxY);

            // 左上
            this._baseVertices[3].uv0 = this.CharacterInfo.uvTopLeft;
            this._baseVertices[3].position = new Vector3(this.CharacterInfo.minX, this.CharacterInfo.maxY);

            if (this.Color.HasValue)
            {
                this._baseVertices[0].color = this.Color.Value;
                this._baseVertices[1].color = this.Color.Value;
                this._baseVertices[2].color = this.Color.Value;
                this._baseVertices[3].color = this.Color.Value;
            }
        }
    }
}
