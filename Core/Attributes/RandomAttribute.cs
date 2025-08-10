using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDataGen.Core.Attributes
{
    public class RandomAttribute : Attribute
    {
        public object Min { get; private set; }

        public object Max { get; private set; }

        public object[] AllowedValues { get; private set; }

        public RandomAttribute(object min, object max)
        {
            Min = min;
            Max = max;
        }

        public RandomAttribute(params object[] allowed)
        {
            AllowedValues = allowed;
        }
    }
}
