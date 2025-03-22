using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bezier3D
{
    public class Vertex
    {
        public Vector3 Position { get; set; } // aktualna pozycja
        public Vector3 OrginalPosition { get; private set; } // pozycja przy 1 liczeniu
        public Vector3 Normal { get; set; } // aktualny wektor normalny
        public Vector3 OrginalNormal { get; private set; } // wektor normalny przy 1 liczeniu


        public Vector3 OrginalPu {  get; private set; }
        public Vector3 OrginalPv { get; private set; }
        public Vector3 Pu { get; set; }
        public Vector3 Pv { get; set; }


        public float u  { get; set; } // pozycja na liczonej siatce beziera
        public float v { get; set; } // pozycja na liczonej siatce beziera

        public Vertex(Vector3 position)
        {
            Position = position;
            OrginalPosition = position;
            Normal = Vector3.Zero;
            OrginalNormal = Vector3.Zero;
        }

        public void SetNormal(Vector3 normal, bool is_orginal = false)
        {
            Normal = normal;
            if(is_orginal)
            {
                OrginalNormal = normal;
            }
        }

        public void SetPuv(Vector3 pu,Vector3 pv,bool is_orginal = false)
        {
            Pu = pu;
            Pv = pv;
            if (is_orginal)
            {
                OrginalPu = pu;
                OrginalPv = pv;
            }
        }
    }
}
