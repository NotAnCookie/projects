using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using airportapp.classes;

namespace airportapp.binary
{
    internal class FromBinCargoPlane
    {
        public static CargoPlane BinCargoPlane(byte[] mess)
        {
            string type = System.Text.Encoding.ASCII.GetString(mess, 0, 3);
            UInt32 messL = BitConverter.ToUInt32(mess, 3);
            UInt64 ID = BitConverter.ToUInt64(mess, 7);
            string serial = System.Text.Encoding.ASCII.GetString(mess, 15, 10).TrimEnd('\0');
            string ISO = System.Text.Encoding.ASCII.GetString(mess, 25, 3);
            UInt16 ML = BitConverter.ToUInt16(mess, 28);
            string model = System.Text.Encoding.ASCII.GetString(mess, 30, ML);
            Single max_load = BitConverter.ToSingle(mess,30+ML);

            return new CargoPlane(ID,serial,ISO,model,max_load);

        }
    }
}
