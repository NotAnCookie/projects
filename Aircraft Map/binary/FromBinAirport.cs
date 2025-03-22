using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;

namespace airportapp.binary
{
    internal class FromBinAirport
    {
        public static Airport BinAirport(byte[] mess)
        {
            string type = System.Text.Encoding.ASCII.GetString(mess, 0, 3);
            UInt32 messL = BitConverter.ToUInt32(mess, 3);
            UInt64 ID = BitConverter.ToUInt64(mess, 7);
            UInt16 NL = BitConverter.ToUInt16(mess, 15);
            string name = System.Text.Encoding.ASCII.GetString(mess, 17, NL);
            string code = System.Text.Encoding.ASCII.GetString(mess, 17+NL, 3);
            Single longtitude = BitConverter.ToSingle(mess, 20 + NL);
            Single latitude = BitConverter.ToSingle(mess, 24 + NL);
            Single amsl = BitConverter.ToSingle(mess, 28 + NL);
            string ISO = System.Text.Encoding.ASCII.GetString(mess, 32+NL, 3);

            return new Airport(ID,name,code,longtitude,latitude,amsl,ISO);

        }
    }
}
