using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LetterWriter
{
    public interface IRubyTextModifierScope
    {
        TextModifierScope RubyScope { get; }
    }
}
