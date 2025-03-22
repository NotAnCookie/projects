using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;
using airportapp.functions;
using static airportapp.functions.TypeConverters;

namespace airportapp.creationclasses
{
    internal class CargoPlaneFactory
    {
        public static CargoPlane Create_CargoPlane(object[] parameters)
        {
            UInt64 iD = Ch_UInt64(parameters[0]);
            string serial = (string)parameters[1];
            string country = (string)parameters[2];
            string model = (string)parameters[3];
            Single maxload = Ch_Single(parameters[4]);
            return new CargoPlane(iD, serial, country, model, maxload);
        }
    }
}
