using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDistributionService.Helper
{
    class HelperMethods
    {
        public static object DictionaryLookUp(Dictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                return null;
            }
            string val = dictionary[key].ToString().Trim();
            return (object) val;
        }
    }
}
