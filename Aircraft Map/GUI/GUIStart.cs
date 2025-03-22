using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightTrackerGUI;
using airportapp.classes;
using airportapp.functions;
using airportapp.Threadsv;

namespace airportapp.GUI
{
    internal class GUIStart
    {
        public static void GUIStartRun(Dictionary<string, List<object>> AllObjects)
        {
            // getting airport and flight lists
            List<object> airports_1 = ListFromDictionary.GetList(AllObjects, "Airport");
            List<object> flights_1 = ListFromDictionary.GetList(AllObjects, "Flight");

            // converting list to pref type
            List<Airport> airports = airports_1.ConvertAll(obj => (Airport)obj);
            List<Flight> flights = flights_1.ConvertAll(obj => (Flight)obj);

            // Starting in another thread Runner.Run();
            MainGUIRun.MainGUIRunner();
            while (true)
            {
                // updating actual possitions of planes
                GUIUpdate.UpdateFlightData(flights, airports);
                System.Threading.Thread.Sleep(1000);
            }
        }

    }
    
}
