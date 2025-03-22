using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.classes
{
    internal abstract class Plane
    {
        public UInt64 ID { get; set; }
        public string Serial { get; set; }
        public string Country { get; set; }
        public string Model { get; set; }
        public Plane(UInt64 iD, string serial, string country, string model)
        {
            ID = iD;
            Serial = serial;
            Country = country;
            Model = model;
        }
    }
}
