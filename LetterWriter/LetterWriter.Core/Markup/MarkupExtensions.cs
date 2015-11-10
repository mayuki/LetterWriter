using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter.Markup;

namespace LetterWriter.Markup
{
    public static class MarkupExtensions
    {
        /// <summary>
        /// 属性の値を取得します。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetAttribute(this MarkupNode node, string attrName, string defaultValue = "")
        {
            return node.Attributes.ContainsKey(attrName) ? node.Attributes[attrName] : defaultValue;
        }
    }
}
