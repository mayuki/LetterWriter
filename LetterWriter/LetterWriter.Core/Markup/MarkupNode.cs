using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LetterWriter.Markup
{
    [DebuggerDisplay("Node: {Children,nq}; {Attributes,nq}")]
    public class MarkupNode
    {
        public List<MarkupNode> Children { get; private set; }
        public Dictionary<string, string> Attributes { get; private set; }

        public MarkupNode Parent { get; private set; }

        public MarkupNode()
        {
            this.Children = new List<MarkupNode>();
            this.Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void AppendChild(MarkupNode markupNode)
        {
            this.Children.Add(markupNode);
            markupNode.Parent = this;
        }
    }

    [DebuggerDisplay("TextNode: {TextContent,nq}")]
    public class TextNode : MarkupNode, ITextContentNode
    {
        public string TextContent { get; set; }
        public TextNode(string value)
        {
            this.TextContent = value;
        }
    }

    [DebuggerDisplay("Element: {TagName,nq}")]
    public class Element : MarkupNode, ITextContentNode
    {
        public string TagName { get; set; }
        public Element(string tagName)
        {
            this.TagName = tagName;
        }


        public string TextContent
        {
            get
            {
                return String.Join("", this.Children.OfType<ITextContentNode>().Select(x => x.TextContent).ToArray());
            }
        }
    }

    public interface ITextContentNode
    {
        string TextContent { get; }
    }
}
