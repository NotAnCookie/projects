using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.classes
{
    internal class CargoPlane : Plane
    {
        public Single MaxLoad { get; set; }

        public CargoPlane(UInt64 iD, string serial, string country, string model, Single maxload) : base(iD , serial, country, model)
        {
            MaxLoad = maxload;
        }
    }
}
