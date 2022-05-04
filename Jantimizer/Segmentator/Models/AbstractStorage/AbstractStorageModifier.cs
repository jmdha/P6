using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Segmentator.Models.AbstractStorage
{
    internal static class AbstractStorageModifier
    {
        private static Dictionary<TypeCode, int> _modifiers { get; } = new Dictionary<TypeCode, int>() {
            { TypeCode.SByte, 8 },
            { TypeCode.Byte, 8 },
            { TypeCode.Int16, 16 },
            { TypeCode.UInt16, 16 },
            { TypeCode.Int32, 32 },
            { TypeCode.UInt32, 32 },
            { TypeCode.Int64, 64 },
            { TypeCode.UInt64, 64 }
        };

        public static int GetModifierOrOne(TypeCode code)
        {
            if (_modifiers.ContainsKey(code))
                return _modifiers[code];
            return 1;
        }
    }
}
