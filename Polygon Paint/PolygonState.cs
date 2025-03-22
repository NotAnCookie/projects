using proj_1.edge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace proj_1
{
    internal class PolygonState
    {
        public List<Vertex> Vertices { get; set; }
        public List<Edge> Edges { get; set; }

        public PolygonState(List<Vertex> vertices, List<Edge> edges)
        {

            Vertices = vertices.Select(v => new Vertex(v.Position)).ToList();
            // Tworzymy kopię krawędzi
            Edges = edges.Select(e => new Edge(
                Vertices[vertices.IndexOf(e.Start)], 
                Vertices[vertices.IndexOf(e.End)], Vertices[vertices.IndexOf(e.Start)],
                e.Constraint,
                e.FixedLength)).ToList();
            foreach (var e in Edges)
            {
                e.ChangeEdgeType(e, e.Constraint, e.Start, e.FixedLength);
            }
        }

        public void Draw(Graphics g)
        {
            // Rysowanie tylko krawędzi przerywaną linią
            if (Edges == null)
            {
                return;
            }
            foreach (var edge in Edges)
            {
                using (Pen pen = new Pen(Color.Pink, 5) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    g.DrawLine(pen, edge.Start.Position, edge.End.Position);
                }
            }
        }

        public void Update(List<Vertex> vertices, List<Edge> edges)
        {
            // Robimy głęboką kopię
            Vertices = null;
            Edges = null;
            foreach (var v in vertices)
            {
                Vertex new_v = new Vertex(v.Position);
                if (Vertices == null)
                {
                    Vertices = new List<Vertex>();
                }

                Vertices.Add(new_v);
            }
            foreach (var e in edges)
            {
                Edge new_e = new Edge(e.Start, e.End, e.side,e.Constraint, e.FixedLength);
                new_e.ChangeEdgeType(new_e, e.Constraint, e.side, e.FixedLength);
                if (Edges == null)
                {
                    Edges = new List<Edge>();
                }
                Edges.Add(new_e);
            }
        }
    }
}
