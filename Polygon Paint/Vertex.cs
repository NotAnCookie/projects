using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace proj_1
{
    public class Vertex
    {
        public PointF Position { get; set; }
        private const int Radius = 5; // Promień punktu
        private const int Tolerance = 10; // Tolerancja do najeżdzania
        public VertexConstraint constraint = VertexConstraint.None;

        public Vertex(PointF position, VertexConstraint constraint = VertexConstraint.None)
        {
            Position = position;
            this.constraint = constraint;
        }

        public void SetPosition(float x, float y)
        {
            Position = new PointF(x, y);
        }

        public bool IsMouseHover(PointF mousePosition)
        {
            return Math.Abs(mousePosition.X - Position.X) < Tolerance &&
                   Math.Abs(mousePosition.Y - Position.Y) < Tolerance;
        }

        public void Draw(Graphics g, bool isHovered)
        {
            Brush pen = isHovered ? Brushes.Orange : Brushes.Black;
            g.FillEllipse(pen, Position.X - (Radius - 1), Position.Y - (Radius - 1), (Radius - 1) * 2, (Radius - 1) * 2);
            if (constraint != VertexConstraint.None)
            {
                // Ikona
                Font font = new Font("Arial", 7);
                Brush brush = Brushes.Black;
                PointF point = new PointF(this.Position.X+5, this.Position.Y+5);
                g.DrawString(constraint.ToString(), font, brush, point);
            }
        }
    }
}
