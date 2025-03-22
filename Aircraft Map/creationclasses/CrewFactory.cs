using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;
using airportapp.functions;
using static airportapp.functions.TypeConverters;

namespace airportapp.creationclasses
{
    internal class CrewFactory
    {
        public static Crew Create_Crew(object[] parameters)
        {
            UInt64 iD = Ch_UInt64(parameters[0]);
            string name = (string)parameters[1];
            UInt64 age = Ch_UInt64(parameters[2]);
            string phone = (string)parameters[3];
            string email = (string)parameters[4];
            UInt16 practice = Ch_UInt16(parameters[5]);
            string role = (string)parameters[6];
            return new Crew(iD, name, age, phone, email, practice, role);
        }
    }
}
