using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj_1.edge
{
    public static class EdgeFactory
    {
        public static Edge CreateEdge(Vertex start, Vertex end, EdgeConstraint constraint, float? fixedLength = null)
        {
            switch (constraint)
            {
                case EdgeConstraint.Vertical:
                    return new VerticalEdge(start, end, start);
                case EdgeConstraint.Horizontal:
                    return new HorizontalEdge(start, end, start);
                case EdgeConstraint.FixedLength when fixedLength.HasValue:
                    return new FixedLengthEdge(start, end,start, fixedLength.Value);
                default:
                    return new Edge(start, end, start, EdgeConstraint.None);
            }
        }
    }
}
