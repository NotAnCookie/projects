using airportapp.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.GUI
{
    internal class GUIPositions
    {
        public static WorldPosition GetStartPositionForFlight(Flight flight, List<Airport> airports)
        {
            // find Airport with coresponding ID
            Airport startAirport = airports.FirstOrDefault(airport => airport.ID == flight.OrginID);

            if (startAirport != null)
            {
                return new WorldPosition { Longitude = startAirport.Longitude, Latitude = startAirport.Latitude };
            }
            else
            {
                // if the Airport doesnt exist return the center
                Console.WriteLine("Couldn't find an Airport");
                return new WorldPosition { Longitude = 0, Latitude = 0 };
            }
        }

        public static WorldPosition GetEndPositionForFlight(Flight flight, List<Airport> airports)
        {
            // find Airport with coresponding ID
            Airport endAirport = airports.FirstOrDefault(airport => airport.ID == flight.TargetID);

            if (endAirport != null)
            {
                return new WorldPosition { Longitude = endAirport.Longitude, Latitude = endAirport.Latitude };
            }
            else
            {
                // if the Airport doesnt exist return the center
                Console.WriteLine("Couldn't find an Airport");
                return new WorldPosition { Longitude = 0, Latitude = 0 };
            }
        }
    }
}
