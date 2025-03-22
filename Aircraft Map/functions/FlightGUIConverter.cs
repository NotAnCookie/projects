using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;
using FlightTrackerGUI;
using Mapsui.Projections;


namespace airportapp.functions
{
    internal class FlightGUIConverter
    {
        public static FlightGUI ConvertToFlightGUI(Flight flight, DateTime currentTime, WorldPosition startPosition, WorldPosition endPosition)
        {
            
            WorldPosition interpolatedPosition = InterpolatePosition(flight, currentTime, startPosition, endPosition);

            double rotation = CalculateRotation(flight, startPosition, endPosition);
            FlightGUI flightGUI = new FlightGUI
            {
                ID = flight.ID,
                WorldPosition = interpolatedPosition,
                MapCoordRotation = rotation
            };

            return flightGUI;
        }
        
        private static WorldPosition InterpolatePosition(Flight flight, DateTime currentTime, WorldPosition startPosition, WorldPosition endPosition)
        {
            DateTime takeoffTime = DateTime.Parse(flight.TakeoffTime);
            DateTime landingTime = DateTime.Parse(flight.LandingTime);

            // for night flights
            if (landingTime < takeoffTime)
            {
                landingTime = landingTime.AddDays(1);
            }

            // if flight didnt start => returns Airports position
            if (currentTime < takeoffTime)
            {
                return startPosition;
            }
            if (currentTime > landingTime)
            {
                return endPosition;
            }

            // % of time that passed from the start
            double timepassed = (currentTime - takeoffTime).TotalMilliseconds / (landingTime - takeoffTime).TotalMilliseconds;

            // interpolated location based on time
            double interpolatedLongitude = startPosition.Longitude + (endPosition.Longitude - startPosition.Longitude) * timepassed;
            double interpolatedLatitude = startPosition.Latitude + (endPosition.Latitude - startPosition.Latitude) * timepassed;

            return new WorldPosition { Longitude = (float)interpolatedLongitude, Latitude = (float)interpolatedLatitude };
        }








        private static double CalculateRotation(Flight flight, WorldPosition startPosition, WorldPosition endPosition)
        {

            (double xCurrent, double yCurrent) = SphericalMercator.FromLonLat(startPosition.Longitude, startPosition.Latitude);
            (double xPrevious, double yPrevious) = SphericalMercator.FromLonLat(endPosition.Longitude, endPosition.Latitude);

            // Obliczamy różnicę między współrzędnymi X i Y punktów startu i końca
            double deltaX = xPrevious - xCurrent; // Zamiana xPrevious i xCurrent
            double deltaY = yPrevious - yCurrent; // Zamiana yPrevious i yCurrent

            // Tworzymy obiekt PointF na podstawie obliczonych różnic
            PointF deltaPoint = new PointF((float)deltaX, (float)deltaY);

            // Obliczamy kąt obrotu w radianach
            double rotation = Math.Atan2(deltaPoint.X, deltaPoint.Y); // Zamiana argumentów funkcji Atan2

            return rotation;
        }
    }
}
