using airportapp.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.functions;
using static airportapp.functions.TypeConverters;

namespace airportapp.creationclasses
{
    internal class CargoFactory
    {
        public static Cargo Create_Cargo(object[] parameters)
        {
            if (parameters.Length >= 4)
            {
                UInt64 ID = Ch_UInt64(parameters[0]);
                Single Weight = Ch_Single(parameters[1]);
                string Code = (string)parameters[2];
                string Description = (string)parameters[3];
                return new Cargo(ID, Weight, Code, Description);
            }
            else
            {
                throw new ArgumentException("Not enough parameters for Cargo constructor.");
            }

        }
    }
}
