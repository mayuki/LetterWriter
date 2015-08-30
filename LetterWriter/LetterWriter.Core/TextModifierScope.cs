namespace LetterWriter
{
    public abstract class TextModifierScope<TTextModifier> : TextModifierScope where TTextModifier:TextModifier, new()
    {
        public new TextModifierScope<TTextModifier> Parent
        {
            set { base.Parent = value; }
            get { return (TextModifierScope<TTextModifier>)base.Parent; }
        }
        public new TTextModifier TextModifier
        {
            set { base.TextModifier = value; }
            get { return (TTextModifier)base.TextModifier; }
        }

        public TextModifierScope(TextModifierScope<TTextModifier> parent, TTextModifier textModifier)
        {
            this.Parent = parent;
            this.TextModifier = new TTextModifier();

            if (textModifier != null)
            {
                this.Apply(textModifier);
            }
        }

        public virtual void Apply(TTextModifier textModifier)
        {
            this.Spacing = textModifier.Spacing;
        }
    }

    public abstract class TextModifierScope
    {
        public TextModifierScope Parent { get; protected set; }
        public TextModifier TextModifier { get; protected set; }

        public float? Spacing
        {
            get { return this.TextModifier.Spacing ?? ((this.Parent != null) ? this.Parent.Spacing : null); }
            set { this.TextModifier.Spacing = value; }
        }
    }
}