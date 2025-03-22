using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;
using airportapp.functions;
using static airportapp.functions.TypeConverters;

namespace airportapp.creationclasses
{
    internal class PassengerPlaneFactory
    {
        public static PassengerPlane Create_PassengerPlane(object[] parameters)
        {
            UInt64 iD = Ch_UInt64(parameters[0]);
            string serial = (string)parameters[1];
            string country = (string)parameters[2];
            string model = (string)parameters[3];
            UInt16 firstClassSize = Ch_UInt16(parameters[4]);
            UInt16 businessClassSize = Ch_UInt16(parameters[5]);
            UInt16 economyClassSize = Ch_UInt16(parameters[0]);
            return new PassengerPlane(iD,serial,country,model,firstClassSize,businessClassSize,economyClassSize);
        }
    }
}
