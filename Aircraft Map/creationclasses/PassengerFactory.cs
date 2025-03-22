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
    internal class PassengerFactory
    {
        public static Passenger Create_Passenger(object[] parameters)
        {
            UInt64 iD = Ch_UInt64(parameters[0]);
            string name = (string)parameters[1];
            UInt64 age = Ch_UInt64(parameters[2]);
            string phone = (string)parameters[3];
            string email = (string)parameters[4];
            string _class = (string)parameters[5];
            UInt64 miles = Ch_UInt64(parameters[6]);
            return new Passenger(iD,name,age,phone,email,_class,miles);
        }
    }
}
