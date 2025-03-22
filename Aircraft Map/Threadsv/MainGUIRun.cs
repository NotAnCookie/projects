using airportapp.creationclasses;
using FlightTrackerGUI;
using NetworkSourceSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.Threadsv
{
    // Place for the visual application
    internal class MainGUIRun
    {
        static NetworkSourceSimulator.NetworkSourceSimulator nss;
        static Thread nssThread;

        static List<object> objectsList = new List<object>();
        public static void MainGUIRunner()
        {
            nssThread = new Thread(new ThreadStart(RunGUI));
            nssThread.Start();

        }



        public static void ExitApplication()
        {
            nssThread.Interrupt();
            nssThread.Join();

            Environment.Exit(0);
        }

        static void RunGUI()
        {
            try
            {
                Runner.Run();
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Thread exited");
            }
        }
    }
}
