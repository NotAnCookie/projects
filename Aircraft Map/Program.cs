// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Globalization;
using System.Xml;
using airportapp.functions;
using static airportapp.functions.ForReading;
using static airportapp.functions.ForSerialization;
using airportapp.creationclasses;
using static airportapp.creationclasses.FactoryFactory;
using NetworkSourceSimulator;
using airportapp.binary;
using System.Threading;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.ConstrainedExecution;
using airportapp.Threadsv;
using FlightTrackerGUI;
using airportapp.classes;
using airportapp.GUI;


namespace airportapp
{
    internal class Program
    {

        static void Main(string[] args)
        {

            string ftrFilePath = "Includes/example_data.ftr";
            Dictionary<string, List<object>> AllObjects = ReadFromFile(ftrFilePath);
           
            GUIStart.GUIStartRun(AllObjects);
        }

    }
}




