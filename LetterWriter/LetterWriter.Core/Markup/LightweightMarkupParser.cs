using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LetterWriter.Markup
{
    public class LightweightMarkupParser
    {
        protected static readonly Dictionary<string, string> CharacterEntityReferencesTable = new Dictionary<string, string>()
        {
            { "amp", "&" },
            { "apos", "'" },
            { "quot", "\"" },
            { "lt", "<" },
            { "gt", ">" },
        };

        public bool TreatNewLineAsLineBreak { get; set; }

        public LightweightMarkupParser()
        {
            this.TreatNewLineAsLineBreak = false;
        }

        public MarkupNode Parse(string sourceText)
        {
            if (String.IsNullOrEmpty(sourceText))
                return new MarkupNode();

            // あまり生真面目にパースしたりはしないでベタにやる
            var sb = new StringBuilder();
            var pos = 0;
            var state = ParseState.Text;

            var attrValueStart = ' ';

            var rootNode = new MarkupNode();
            var currentNode = rootNode;

            var attrName = "";
            var isCloseTag = false;
            var lineNum = 1;
            var columnNum = 0;
            var charRefAmpStart = -1;

            while (pos < sourceText.Length)
            {
                var c = sourceText[pos++];

                columnNum++;

                if (c == '\n')
                {
                    lineNum++;
                    columnNum = 0;
                }

                // テキスト
                if (state == ParseState.Text)
                {
                    // タグの開始
                    if (c == '<')
                    {
                        if (sb.Length > 0)
                        {
                            currentNode.Children.Add(new TextNode(sb.ToString()));
                        }

                        state = ParseState.TagName;
                        isCloseTag = false;
                        sb.Length = 0;
                        continue;
                    }
                    // & で始まる
                    else if (c == '&')
                    {
                        charRefAmpStart = pos;
                        sb.Append(c);
                        continue;
                    }
                    // &...; 終わる
                    else if (c == ';' && charRefAmpStart != -1)
                    {
                        var len = (pos - charRefAmpStart) - 1;
                        if (len != 0)
                        {
                            var charRef = sourceText.Substring(charRefAmpStart, len);
                            sb.Remove(sb.Length - (len + 1), len + 1);
                            sb.Append(ResolveCharacterReference(charRef));
                            charRefAmpStart = -1;
                        }
                        continue;
                    }
                    // 内容
                    else
                    {
                        sb.Append(c);
                    }
                }
                // 開始・閉じタグの名前
                else if (state == ParseState.TagName)
                {
                    // 閉じタグ?
                    if (sb.Length == 0 && c == '/')
                    {
                        isCloseTag = true;
                        continue;
                    }
                    // 属性探し
                    else if (c == ' ' || c == '=' || c == '/')
                    {
                        state = ParseState.TagAttributes;

                        var newElement = new Element(sb.ToString());
                        currentNode.AppendChild(newElement);
                        currentNode = newElement;
                        isCloseTag = false;
                        sb.Length = 0;

                        continue;
                    }
                    // 属性も内容もなくタグ終了
                    else if (c == '>')
                    {
                        state = ParseState.Text;
                        charRefAmpStart = -1;

                        // 閉じタグだったら戻す
                        if (isCloseTag)
                        {
                            // 戻しすぎてない?
                            if (currentNode.Parent == null)
                            {
                                Debug.WriteLine(String.Format("Unmatched Tag: {0}; Line={1}; Column={2}", sb.ToString(), lineNum, columnNum));
                                continue;
                            }
                            // タグの閉じは対になってる?
                            if (((Element) currentNode).TagName != sb.ToString())
                            {
                                Debug.WriteLine(String.Format("Unmatched Tag: {0}; Line={1}; Column={2}", sb.ToString(), lineNum, columnNum));
                                continue;
                            }
                            currentNode = currentNode.Parent;
                        }
                        else
                        {
                            // 要素開始
                            var newElement = new Element(sb.ToString());
                            currentNode.AppendChild(newElement);
                            currentNode = newElement;
                            isCloseTag = false;
                        }

                        sb.Length = 0;
                        continue;
                    }
                    // タグの名前をぽちぽち
                    else
                    {
                        sb.Append(c);
                    }
                }
                // 属性たち
                else if (state == ParseState.TagAttributes)
                {
                    // 終わり
                    if (c == '>')
                    {
                        state = ParseState.Text;
                        charRefAmpStart = -1;

                        // 閉じタグだったら戻す
                        if (isCloseTag)
                        {
                            currentNode = currentNode.Parent;
                        }
                        continue;
                    }
                    // 空白文字またはコントロール文字
                    else if (Char.IsControl(c) || Char.IsWhiteSpace(c))
                    {
                        continue;
                    }
                    // デフォルト引数的な
                    else if (c == '=')
                    {
                        state = ParseState.TagAttributeValue;
                        sb.Append("value"); // 引数はvalueってことにする
                        attrValueStart = ' ';
                        continue;
                    }
                    // 内容空要素
                    else if (c == '/')
                    {
                        state = ParseState.TagAttributes;
                        isCloseTag = true;
                        continue;
                    }
                    // 属性名開始
                    else
                    {
                        state = ParseState.TagAttributeName;
                        sb.Length = 0;
                        sb.Append(c);
                        continue;
                    }
                }
                // 属性名
                else if (state == ParseState.TagAttributeName)
                {
                    // 属性の値開始
                    if ((Char.IsControl(c) || Char.IsWhiteSpace(c)) || c == '=')
                    {
                        attrName = sb.ToString();
                        sb.Length = 0;
                        state = ParseState.TagAttributeNameCompleted;
                        continue;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                // 属性名から値まで
                else if (state == ParseState.TagAttributeNameCompleted)
                {
                    // 属性
                    if ((Char.IsControl(c) || Char.IsWhiteSpace(c)))
                    {
                        continue;
                    }
                    // クォート
                    else if (c == '"' || c == '\'')
                    {
                        state = ParseState.TagAttributeValue;
                        attrValueStart = c;
                        continue;
                    }
                    // 囲まれていない (いきなり始まる) <tagName=value>...</tagName>
                    else
                    {
                        state = ParseState.TagAttributeValue;
                        attrValueStart = c;
                        pos--;
                        continue;
                    }
                }
                // 属性値
                else if (state == ParseState.TagAttributeValue)
                {
                    // 属性の値の終わり
                    if ((attrValueStart == '"' && c == '"') ||
                        (attrValueStart == '\'' && c == '\'') ||
                        (attrValueStart == ' ' && (Char.IsControl(c) || Char.IsWhiteSpace(c))))
                    {
                        currentNode.Attributes[attrName] = sb.ToString();
                        sb.Length = 0;
                        state = ParseState.TagAttributes;
                        continue;
                    }
                    // & で始まる
                    else if (c == '&')
                    {
                        charRefAmpStart = pos;
                        sb.Append(c);
                        continue;
                    }
                    // &...; 終わる
                    else if (c == ';' && charRefAmpStart != -1)
                    {
                        var len = (pos - charRefAmpStart) - 1;
                        if (len != 0)
                        {
                            var charRef = sourceText.Substring(charRefAmpStart, len);
                            sb.Remove(sb.Length - (len + 1), len + 1);
                            sb.Append(ResolveCharacterReference(charRef));
                            charRefAmpStart = -1;
                        }
                        continue;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            // 残り
            if (sb.Length > 0 && state == ParseState.Text)
            {
                currentNode.AppendChild(new TextNode(sb.ToString()));
            }

            return rootNode;
        }

        private static string ResolveCharacterReference(string value)
        {
            if (value.StartsWith("#x"))
            {
                // &#xNNNN; (数値文字参照)
                var result = 0;
                if (Int32.TryParse(value.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out result))
                {
                    return new string((char) result, 1);
                }
            }
            else if (value.StartsWith("#"))
            {
                // &#NN; (数値文字参照)
                var result = 0;
                if (Int32.TryParse(value.Substring(1), System.Globalization.NumberStyles.Integer, null, out result))
                {
                    return new string((char)result, 1);
                }
            }
            else if (CharacterEntityReferencesTable.ContainsKey(value))
            {
                // amp (文字実態参照)
                return CharacterEntityReferencesTable[value];
            }

            return String.Empty;
        }

        private enum ParseState
        {
            Text,
            TagName,
            TagAttributes,
            TagAttributeName,
            TagAttributeNameCompleted,
            TagAttributeValue,
        }
    }
}