using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.classes
{
    internal class Flight
    {
        public UInt64 ID { get; set; }
        public UInt64 OrginID { get; set; }
        public UInt64 TargetID { get; set; }
        public string TakeoffTime { get; set; }
        public string LandingTime { get; set; }
        public Single Longitude { get; set; }
        public Single Latitude { get; set; }
        public Single AMSL { get; set; }
        public UInt64 PlaneID { get; set; }
        public List<UInt64> CrewID { get; set; }
        public List<UInt64> LoadID { get; set; }
        public Flight(UInt64 id, UInt64 orginid, UInt64 targetid, string takeofftime, string landingtime, Single longitude, Single latitude, Single amsl, UInt64 planeid, List<UInt64> crewid, List<UInt64> loadid)
        {
            ID = id;
            OrginID = orginid;
            TargetID = targetid;
            TakeoffTime = takeofftime;
            LandingTime = landingtime;
            Longitude = longitude;
            Latitude = latitude;
            AMSL = amsl;
            PlaneID = planeid;
            CrewID = crewid;
            LoadID = loadid;

        }
    }
}
