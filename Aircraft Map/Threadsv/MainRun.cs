using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSourceSimulator;
using airportapp.binary;
using System.Threading;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.ConstrainedExecution;
using airportapp.creationclasses;
using System.Collections;
using System.Runtime.Serialization;
using airportapp.functions;

namespace airportapp.Threadsv
{
    internal class MainRun
    {
        static NetworkSourceSimulator.NetworkSourceSimulator nss;
        static Thread nssThread;

        static List<object> objectsList = new List<object>();
        public static void MainRunner(string ftrFilePath)
        {
            //string ftrFilePath = "example_data.ftr";
            nss = new NetworkSourceSimulator.NetworkSourceSimulator(ftrFilePath, 100, 200);
            nss.OnNewDataReady += Nss_OnNewDataReady;

            // uruchomienie odzielnego wątku
            nssThread = new Thread(new ThreadStart(RunNSS));
            nssThread.Start();

            // Nasłuchiwanie
            CommListening.ListenForCommands(objectsList);
        }

        static private void Nss_OnNewDataReady(object sender, NewDataReadyArgs args)
        {
            var msg = nss.GetMessageAt(args.MessageIndex);
            byte[] newmsg = msg.MessageBytes;
            string type = System.Text.Encoding.ASCII.GetString(newmsg, 0, 3);
            object newObj = BinFactory.CreateObject(type, newmsg);
            objectsList.Add(newObj);
        }

        

        public static void ExitApplication()
        {
            // przerwanie wątku
            nssThread.Interrupt();
            // zakończenie wątku
            nssThread.Join();

            Environment.Exit(0);
        }

        static void RunNSS()
        {
            try
            {
                nss.Run();
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Thread exited");
            }
        }
    }
}
