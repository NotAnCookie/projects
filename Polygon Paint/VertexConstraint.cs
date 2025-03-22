using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj_1
{
    public enum VertexConstraint
    {
        None = 0, // nie jest wierzchołkiem beziera
        G0 = 1,
        G1 = 2,
        C1 = 3,
    }
}
