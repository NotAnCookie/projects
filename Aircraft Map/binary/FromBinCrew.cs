using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;

namespace airportapp.binary
{
    internal class FromBinCrew
    {
        public static Crew BinCrew(byte[] mess)
        {
            //string ret = "";
            string type = System.Text.Encoding.ASCII.GetString(mess, 0, 3);
            //ret += $"{type},";
            UInt32 messL = BitConverter.ToUInt32(mess, 3);
            UInt64 ID = BitConverter.ToUInt64(mess, 7);
            //ret += $"{ID},";
            UInt16 NL = BitConverter.ToUInt16(mess, 15);
            string name = System.Text.Encoding.ASCII.GetString(mess, 17, NL);
            //ret += $"{name},";
            UInt16 age = BitConverter.ToUInt16(mess, 17 + NL);
            //ret += $"{age},";
            string phone = System.Text.Encoding.ASCII.GetString(mess, 19 + NL, 12);
            //ret += $"{phone},";
            UInt16 EL = BitConverter.ToUInt16(mess, 31 + NL);
            string email = System.Text.Encoding.ASCII.GetString(mess, 33 + NL, EL);
            //ret += $"{email},";
            UInt16 practice = BitConverter.ToUInt16(mess, 33 + NL + EL);
            //ret += $"{practice},";
            string role = System.Text.Encoding.ASCII.GetString(mess, 35 + NL + EL, 1);
            //ret += $"{role}";

            return new Crew(ID,name,age,phone,email,practice,role);

        }
    }
}
