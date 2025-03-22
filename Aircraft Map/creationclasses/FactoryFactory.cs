using airportapp.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.creationclasses
{
    internal class FactoryFactory
    {
        // Dictionary for constructors
        private static readonly Dictionary<string, Func<object[], object>> constructorMap = new Dictionary<string, Func<object[], object>>
            {
               { "C", args => CrewFactory.Create_Crew(args) },
               { "P", args =>  PassengerFactory.Create_Passenger(args) },
               { "CA", args => CargoFactory.Create_Cargo(args) },
               { "CP", args => CargoPlaneFactory.Create_CargoPlane(args) },
               { "PP", args => PassengerPlaneFactory.Create_PassengerPlane(args) },
               { "AI", args => AirportFactory.Create_Airport(args) },
               { "FL", args => FlightFactory.Create_Flight(args) }
             };

        public static object CreateObject(string objectType, params object[] parameters)
        {
            if (constructorMap.TryGetValue(objectType, out Func<object[], object> constructor))
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
