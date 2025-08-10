using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDataGen.Core
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if a given Type represents a collection.
        /// </summary>
        /// <param name="type">The Type to check.</param>
        /// <returns>True if the type implements IEnumerable or IEnumerable<T>, false otherwise.</returns>
        public static bool IsCollection(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            // Check for generic IEnumerable<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }

            // Check if it implements IEnumerable (for non-generic collections and arrays)
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // Exclude string, as it implements IEnumerable but is not typically considered a collection in this context.
                if (type == typeof(string))
                {
                    return false;
                }
                return true;
            }

            return false;
        }
    }
}
