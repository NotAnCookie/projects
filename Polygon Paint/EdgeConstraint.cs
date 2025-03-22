using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj_1
{
    public enum EdgeConstraint
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        FixedLength = 3,
        Bezier = 4
    }
}
