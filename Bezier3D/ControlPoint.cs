using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bezier3D
{
    public class ControlPoint
    {
        public Vector3 Position { get; set; }
        public ControlPoint(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
    }
}
