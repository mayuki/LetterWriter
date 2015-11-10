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
