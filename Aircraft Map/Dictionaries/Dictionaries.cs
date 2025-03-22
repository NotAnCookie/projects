using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.Dictionaries
{
    internal class Dictionaries
    {
        // Dictionary to store actual names of classes
        public static Dictionary<string, string> classMap = new Dictionary<string, string>
        {
            { "C", "Crew" },
            { "P", "Passenger" },
            { "CA", "Cargo" },
            { "CP", "CargoPlane" },
            { "PP", "PassengerPlane" },
            { "AI", "Airport" },
            { "FL", "Flight" }
        };
    }
}
