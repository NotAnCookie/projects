using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;

namespace airportapp.binary
{
    internal class FromBinPassenger
    {
        public static Passenger BinPassenger(byte[] mess)
        {
            string type = System.Text.Encoding.ASCII.GetString(mess, 0, 3);
            UInt32 messL = BitConverter.ToUInt32(mess, 3);
            UInt64 ID = BitConverter.ToUInt64(mess, 7);
            UInt16 NL = BitConverter.ToUInt16(mess, 15);
            string name = System.Text.Encoding.ASCII.GetString(mess, 17, NL);
            UInt16 age = BitConverter.ToUInt16(mess, 17 + NL);
            string phone = System.Text.Encoding.ASCII.GetString(mess, 19+NL, 12);
            UInt16 EL = BitConverter.ToUInt16(mess, 31 + NL);
            string email = System.Text.Encoding.ASCII.GetString(mess, 33+NL, EL);
            string _class = System.Text.Encoding.ASCII.GetString(mess, 33 + NL+EL, 1);
            UInt64 miles = BitConverter.ToUInt64(mess, 34 + NL + EL);
            return new Passenger(ID, name, age, phone, email, _class, miles);

        }
    }
}
