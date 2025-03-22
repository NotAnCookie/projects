using airportapp.classes;
using airportapp.functions;
using FlightTrackerGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.GUI
{
    internal class GUIUpdate
    {
        public static void UpdateFlightData(List<Flight> flights, List<Airport> airports)
        {
            List<FlightGUI> flightsGUI = new List<FlightGUI>();
            foreach (var flight in flights)
            {
                WorldPosition startPosition = GUIPositions.GetStartPositionForFlight(flight, airports);
                WorldPosition endPosition = GUIPositions.GetEndPositionForFlight(flight, airports);

                // update FlightGUI
                FlightGUI updatedFlightGUI = FlightGUIConverter.ConvertToFlightGUI(flight, DateTime.Now, startPosition, endPosition);
                flightsGUI.Add(updatedFlightGUI);
            }
            FlightsGUIData flightsGUIData = new FlightsGUIData(flightsGUI);
            Runner.UpdateGUI(flightsGUIData);
        }
    }
}
