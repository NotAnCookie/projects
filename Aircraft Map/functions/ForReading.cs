using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static airportapp.creationclasses.FactoryFactory;

namespace airportapp.functions
{
    internal class ForReading
    {
        public static Dictionary<string, List<object>> ReadFromFile(string filePath)
        {
            List<object[]> lines = ReadLines(filePath);

            // Dictionary for storing created objects by type
            // Objects sorted by the shortcut given in first line
            Dictionary<string, List<object>> objectsByType = new Dictionary<string, List<object>>();

            // Serialization into JSON
            foreach (var line in lines)
            {
                // Taking object type
                string objectType = (string)line[0];

                // Create objects -> skipping object type in line
                object obj = CreateObject(objectType, line.Skip(1).ToArray());

                // Adding the object to the dictionary based on type
                if (!objectsByType.ContainsKey(objectType))
                {
                    objectsByType[objectType] = new List<object>();
                }
                objectsByType[objectType].Add(obj);
            }
            return objectsByType;
        }

        public static List<object[]> ReadLines(string filePath)
        {
            List<object[]> lines = new List<object[]>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');


                        lines.Add(values);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading file: {e.Message}");
            }
            return lines;
        }
    }
}
