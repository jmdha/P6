using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools.Helpers
{
    public static class JsonParsingHelper
    {
        public static T ParseJson<T>(string json)
        {
            var res = JsonSerializer.Deserialize(json, typeof(T));
            if (res is T expList)
                return expList;
            throw new IOException($"Error! Could not parse text `{json}` into model '{nameof(T)}'!");
        }
    }
}
