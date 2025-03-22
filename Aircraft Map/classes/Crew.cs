using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.classes
{
    internal class Crew : Person
    {
        public UInt16 Practice { get; set; }
        public string Role { get; set; }
        public Crew(UInt64 iD, string name, UInt64 age, string phone, string email, UInt16 practice, string role) : base(iD,name,age,phone,email)
        {
            Practice = practice;
            Role = role;
        }
    }
}
