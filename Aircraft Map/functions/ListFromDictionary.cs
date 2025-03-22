using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using airportapp.Dictionaries;

namespace airportapp.functions
{
    internal class ListFromDictionary
    {
        public static List<object> GetList(Dictionary<string, List<object>> dictionary, string inputClass)
        {
            Dictionary<string, string> classMap = Dictionaries.Dictionaries.classMap;

            List<object> resultList = new List<object>();
            if (classMap.ContainsValue(inputClass))
            {
                string key = classMap.FirstOrDefault(x => x.Value == inputClass).Key;
                if (dictionary.ContainsKey(key))
                {
                    resultList = dictionary[key];
                }
                else
                {
                    Console.WriteLine($"No {inputClass} list found.");
                }
            }
            else
            {
                Console.WriteLine($"{inputClass} is not a possible class.");
            }
            return resultList;
        }

    }
}
