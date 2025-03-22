using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bezier3D
{
    public class Triangle
    {
        public Vertex v1, v2, v3;
        public Vector3 Normal { get; set; }

        public Vector3 LightPosition = new Vector3(0,0,300f);

        public Triangle(Vertex p1, Vertex p2, Vertex p3)
        {
            var vertices = new[] { p1, p2, p3 };
            Array.Sort(vertices, (a, b) => a.Position.Y.CompareTo(b.Position.Y));

            v1 = vertices[0];
            v2 = vertices[1];
            v3 = vertices[2];
            Normal = CalculateNormal();
        }


        public void UpdateLightPosition(Vector3 L)
        {
            this.LightPosition = L;
        }

        private Vector3 CalculateNormal()
        {
            Vector3 edge1 = v2.Position - v1.Position;
            Vector3 edge2 = v3.Position - v1.Position;
            Vector3 normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

            if (Vector3.Dot(normal, new Vector3(0, 0, 1)) < 0)
            {
                normal = -normal;
            }

            return normal;
        }

        public void Draw(Graphics g, int offsetX, int offsetY)
        {
            PointF[] points = new PointF[]
            {
                new PointF(v1.Position.X + offsetX, v1.Position.Y + offsetY),
                new PointF(v2.Position.X + offsetX, v2.Position.Y + offsetY),
                new PointF(v3.Position.X + offsetX, v3.Position.Y + offsetY)
            };
            g.DrawPolygon(Pens.Black, points);
        }        

        public Vector3 GetBarycentric(Vector2 p)//, Triangle triangle)
        {
            Vector3 a = new Vector3(v1.Position.X, v1.Position.Y, 1);
            Vector3 b = new Vector3(v2.Position.X, v2.Position.Y, 1);
            Vector3 c = new Vector3(v3.Position.X, v3.Position.Y, 1);
            Vector3 pVec = new Vector3(p.X, p.Y, 1);

            float areaABC = Vector3.Cross(b - a, c - a).Z;
            float alpha = Vector3.Cross(b - pVec, c - pVec).Z / areaABC;
            float beta = Vector3.Cross(c - pVec, a - pVec).Z / areaABC;
            float gamma = Vector3.Cross(a - pVec, b - pVec).Z / areaABC;

            return new Vector3(alpha, beta, gamma);
        }

    }
}
