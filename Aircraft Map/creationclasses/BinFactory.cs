using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.binary;

namespace airportapp.creationclasses
{
    internal class BinFactory
    {
        private static readonly Dictionary<string, Func<byte[], object>> constructorMap = new Dictionary<string, Func<byte[], object>>
            {
               { "NCR", args => FromBinCrew.BinCrew(args) },
               { "NPA", args =>  FromBinPassenger.BinPassenger(args) },
               { "NCA", args => FromBinCargo.BinCargo(args) },
               { "NCP", args => FromBinCargoPlane.BinCargoPlane(args) },
               { "NPP", args => FromBinPassengerPlane.BinPassengerPlane(args) },
               { "NAI", args => FromBinAirport.BinAirport(args) },
               { "NFL", args => FromBinFlight.BinFlight(args) }
             };

        public static object CreateObject(string objectType, params byte[] parameters)
        {
            if (constructorMap.TryGetValue(objectType, out Func<byte[], object> constructor))
            {
                return constructor(parameters);
            }
            else
            {
                throw new ArgumentException("Invalid object type.");
            }
        }
    }
}
