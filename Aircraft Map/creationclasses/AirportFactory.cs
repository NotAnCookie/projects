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
    internal class AirportFactory
    {
        public static Airport Create_Airport(object[] parameters)
        {
            UInt64 id = Ch_UInt64(parameters[0]);
            string name = (string)parameters[1];
            string code = (string)parameters[2];
            Single longitude = Ch_Single(parameters[3]);
            Single latitude = Ch_Single(parameters[4]);
            Single amsl = Ch_Single(parameters[5]);
            string country = (string)parameters[5];
            return new Airport(id,name, code, longitude, latitude, amsl, country);
        }
    }
}
