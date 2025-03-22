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
    internal class FlightFactory
    {
        public static Flight Create_Flight(object[] parameters)
        {
            UInt64 id = Ch_UInt64(parameters[0]);
            UInt64 orginid = Ch_UInt64(parameters[1]);
            UInt64 targetid = Ch_UInt64(parameters[2]);
            string takeofftime = (string)parameters[3];
            string landingtime = (string)parameters[4];
            Single longitude = Ch_Single(parameters[5]);
            Single latitude = Ch_Single(parameters[6]);
            Single amsl = Ch_Single(parameters[7]);
            UInt64 planeid = Ch_UInt64(parameters[8]);
            List< UInt64 > crewid = Ch_UInt64_list(parameters[9]);
            List<UInt64> loadid = Ch_UInt64_list(parameters[10]);
            return new Flight(id, orginid, targetid, takeofftime, landingtime, longitude, latitude, amsl, planeid,crewid, loadid);
        }
    }
}
