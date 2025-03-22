using airportapp.creationclasses;
using NetworkSourceSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkSourceSimulator;
using airportapp.functions;
using static airportapp.Threadsv.MainRun;

namespace airportapp.Threadsv
{
    internal class CommListening
    {
        public static void ListenForCommands(List<object> objectsList)
        {
            string path = "snapshot";
            path = Path.Combine(Environment.CurrentDirectory, path);
            ForSerialization.BeforeSerialize(path);
            string command;
            do
            {
                Console.WriteLine("Enter a command (print/exit):");
                command = Console.ReadLine();
                switch (command)
                {
                    case "print":
                        // serializacja do pliku JSON
                        ForSerialization.SerializeSnapshot(objectsList);
                        break;
                    case "exit":
                        // wyjście z aplikacji
                        ExitApplication();
                        break;
                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
            } while (command != "exit");
        }
    }
}
