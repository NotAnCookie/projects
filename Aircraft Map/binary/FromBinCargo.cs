using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;

namespace airportapp.binary
{
    internal class FromBinCargo
    {
        public static Cargo BinCargo(byte[] mess)
        {
            string type = System.Text.Encoding.ASCII.GetString(mess, 0, 3);
            UInt32 messL = BitConverter.ToUInt32(mess, 3);
            UInt64 ID = BitConverter.ToUInt64(mess, 7);
            Single weight = BitConverter.ToSingle(mess, 15);
            string code = System.Text.Encoding.ASCII.GetString(mess, 19, 6);
            UInt16 DL = BitConverter.ToUInt16(mess, 25);
            string desc = System.Text.Encoding.ASCII.GetString(mess, 27, DL);
            return new Cargo(ID, weight, code, desc);

        }
    }
}
