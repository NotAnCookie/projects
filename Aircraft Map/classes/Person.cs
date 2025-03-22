using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.classes
{
    internal abstract class Person
    {
        public UInt64 ID { get; set; }
        public string Name { get; set; }
        public UInt64 Age { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Person(UInt64 iD, string name, UInt64 age, string phone, string email)
        {
            ID = iD;
            Name = name;
            Age = age;
            Phone = phone;
            Email = email;
        }
    }
}
