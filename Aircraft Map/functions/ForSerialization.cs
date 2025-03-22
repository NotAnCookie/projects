using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace airportapp.functions
{
    internal class ForSerialization
    {
        public static void BeforeSerialize(string path)
        {
            string newpath = Path.Combine(Environment.CurrentDirectory, path);
            if (!Directory.Exists(newpath))
            {
                Directory.CreateDirectory(newpath);
            }
            else
            {
                foreach (var file in Directory.GetFiles(newpath))
                {
                    File.Delete(file);
                }
            }
        }
        public static void Serialize(string path, Dictionary<string, List<object>> objects,string name)
        {
            // Current directory + new directory name
            string newpath = Path.Combine(Environment.CurrentDirectory, path);

            

            var allObjects = new List<object>();

            foreach (var kvp in objects)
            {
                var objectsList = kvp.Value;
                allObjects.AddRange(objectsList);
            }

            var serializedData = JsonSerializer.Serialize(allObjects, new JsonSerializerOptions
            {
                IncludeFields = true,
                //WriteIndented = true
            });

            var filePath = Path.Combine(newpath, $"{name}.json");

            // Write all serialized objects to a single JSON file
            File.WriteAllText(filePath, serializedData);
        }



        public static void SerializeSnapshot(List<object> objects) // for snapshots
        {
            string path = "snapshot";
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH_mm_ss");
            string name = $"snapshot_{formattedTime}";
            Dictionary<string, List<object>> dictObjects = new Dictionary<string, List<object>>();
            dictObjects.Add("new", objects);
            ForSerialization.Serialize(path, dictObjects, name);
            Console.WriteLine("Snapshot serialized.");

        }


    }
}
