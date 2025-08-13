using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDataGen.Core.Attributes
{
    public class PromptAttribute : Attribute
    {
        public string Description { get; private set; }

        public string[] Modifiers { get; private set; }

        public PromptAttribute(string description, params string[] modifiers)
        {
            Description = description;
            Modifiers = modifiers;
        }
    }
}
