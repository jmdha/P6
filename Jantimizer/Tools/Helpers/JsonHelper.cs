using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Tools.Helpers
{
    public static class JsonHelper
    {
        public static T GetValue<T>(JsonObject jObject, string key)
        {
            T retVal = default(T);
            var obj = jObject.Single(x => x.Key == key);
            if (obj.Value != null)
                obj.Value.AsValue().TryGetValue<T>(out retVal);
            return retVal;
        }
    }
}
