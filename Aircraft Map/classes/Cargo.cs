using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.classes
{
    internal class Cargo
    {
        public UInt64 ID { get; set; }
        public Single Weight { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public Cargo(UInt64 id, Single weight, string code, string desc)
        {
            ID = id;
            Weight = weight;
            Code = code;
            Description = desc;
        }
    }
}
