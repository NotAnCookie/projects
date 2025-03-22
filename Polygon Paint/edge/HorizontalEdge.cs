using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj_1.edge
{
    public class HorizontalEdge : Edge
    {
        public HorizontalEdge(Vertex start, Vertex end, Vertex side) : base(start, end, side, EdgeConstraint.Horizontal)
        {
            Constraint = EdgeConstraint.Horizontal;
            fake_update(side);
        }

        public override bool is_ok()
        {
            if (Start.Position.Y == End.Position.Y)
            {
                return true;
            }
            return false;
        }

        public override void Draw(Graphics g, bool isHovered, bool isBesenham, Bitmap bitmap)
        {
            float desiredWidth = 2f;
            Pen pen = new Pen(isHovered ? Color.Orange : Color.Black, desiredWidth);
            DrawMyLine(g,pen, Start.Position, End.Position, isBesenham, bitmap);

            // Ikona
            Font font = new Font("Arial", 7);
            Brush brush = Brushes.Black;
            float midX = (Start.Position.X + End.Position.X) / 2;
            float midY = (Start.Position.Y + End.Position.Y) / 2;
            g.DrawString("H", font, brush, new PointF(midX, midY));
        }

        private void MakeHorizontal()
        {
            End.Position = new PointF(End.Position.X, Start.Position.Y);
        }

        private void MakeHorizontal_2()
        {
            Start.Position = new PointF(Start.Position.X, End.Position.Y);
        }

        private void fake_update(Vertex side)
        {
            if (side == Start)
            {
                MakeHorizontal();
            }
            else
            {
                MakeHorizontal_2();
            }
        }

        public override bool need_update(Vertex side)
        {
            if (Start.Position.Y == End.Position.Y)
            {
                return false;
            }
            return true;
        }

        public override Edge Update(Vertex movedVertex)
        {
            fake_update(movedVertex);
            return null;
        }

        public override void MoveForFriendBezier(Vertex side, BezierEdge edge, PointF mouseDelta)
        {
            Vertex bezierMovedVertex = side == edge.Start ? edge.ControlPoint1 : edge.ControlPoint2;
            float is_position = (float)Math.Sqrt(Math.Pow(side.Position.X - bezierMovedVertex.Position.X, 2) +
                                    Math.Pow(side.Position.Y - bezierMovedVertex.Position.Y, 2)) * 3 - this.FixedLength;
            float len = (float)Math.Sqrt(Math.Pow(side.Position.X - bezierMovedVertex.Position.X, 2) +
                                    Math.Pow(side.Position.Y - bezierMovedVertex.Position.Y, 2)) * 3;
            Vertex otherSide = side == this.Start ? this.End : this.Start;

            if (side.constraint == VertexConstraint.C1)
            {
                side.Position = new PointF(side.Position.X, side.Position.Y + mouseDelta.Y);

                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;

                float vectorLength = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

                float fixedVectorX = vectorX * 3;
                float fixedVectorY = vectorY * 3;
                otherSide.Position = new PointF(side.Position.X + fixedVectorX, side.Position.Y + fixedVectorY);
            }
            else if (side.constraint == VertexConstraint.G1)
            {
                side.Position = new PointF(side.Position.X , side.Position.Y + mouseDelta.Y);

                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorX_2 = otherSide.Position.X - side.Position.X;

                if (vectorX * vectorX_2 < 0)
                {
                    otherSide.Position = new PointF(side.Position.X - vectorX_2, otherSide.Position.Y + mouseDelta.Y);
                    return;
                }

                otherSide.Position = new PointF(otherSide.Position.X, otherSide.Position.Y + mouseDelta.Y);
            }
        }
    }
}
