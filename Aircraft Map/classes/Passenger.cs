using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.classes
{
    internal class Passenger : Person
    {
        public string Class { get; set; }
        public UInt64 Miles { get; set; }
        public Passenger(UInt64 iD, string name, UInt64 age, string phone, string email, string _class, UInt64 miles) : base(iD, name, age, phone, email)
        {
            Class = _class;
            Miles = miles;
        }
    }
}
