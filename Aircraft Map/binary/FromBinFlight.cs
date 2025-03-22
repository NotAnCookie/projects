using airportapp.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.binary
{
    internal class FromBinFlight
    {
        public static Flight BinFlight(byte[] mess)
        {
            string type = System.Text.Encoding.ASCII.GetString(mess, 0, 3);
            UInt32 messL = BitConverter.ToUInt32(mess, 3);
            UInt64 ID = BitConverter.ToUInt64(mess, 7);
            UInt64 orgID = BitConverter.ToUInt64(mess, 15);
            UInt64 tarID = BitConverter.ToUInt64(mess, 23);
            Int64 takeoff = BitConverter.ToInt64(mess, 31);
            string takeoffn = takeoff.ToString();
            Int64 landing = BitConverter.ToInt64(mess, 39);
            string landingn = landing.ToString();
            UInt64 planeID = BitConverter.ToUInt64(mess, 47);
            UInt16 CC = BitConverter.ToUInt16(mess, 55);
            List<UInt64> crew = new List<UInt64>();
            for(int i  = 0; i < CC; i++)
            {
                crew.Add( BitConverter.ToUInt64(mess, 57+(i-1)*8));
            }
            UInt16 PCC = BitConverter.ToUInt16(mess, 57 + 8 * CC);
            List<UInt64> pass_cargo = new List<UInt64>();
            for (int i = 0; i < CC; i++)
            {
                pass_cargo.Add( BitConverter.ToUInt64(mess, 57 + (i - 1) * 8));
            }
            Single fake = 0;
            return new Flight(ID, orgID, tarID, takeoffn, landingn, fake, fake, fake,planeID, crew, pass_cargo);
        }
    }
}
