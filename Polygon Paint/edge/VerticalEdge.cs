using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj_1.edge
{
    public class VerticalEdge : Edge
    {

        public VerticalEdge(Vertex start, Vertex end, Vertex side) : base(start, end, side, EdgeConstraint.Vertical)
        {
            Constraint = EdgeConstraint.Vertical;
            fake_update(side);
        }

        public override bool is_ok()
        {
            if(Start.Position.X == End.Position.X)
            {
                return true;
            }
            return false;
        }

        public override void Draw(Graphics g, bool isHovered, bool isBesenham, Bitmap bitmap)
        {
            float desiredWidth = 2f;
            Pen pen = new Pen(isHovered ? Color.Orange : Color.Black, desiredWidth);
            DrawMyLine(g, pen, Start.Position, End.Position, isBesenham, bitmap);

            Font font = new Font("Arial", 7);
            Brush brush = Brushes.Black;
            float midX = (Start.Position.X + End.Position.X) / 2;
            float midY = (Start.Position.Y + End.Position.Y) / 2;
            g.DrawString("V", font, brush, new PointF(midX, midY));
        }

        private void MakeVertical() // start = side
        {
            if (End.Position.X != Start.Position.X)
            {
                End.Position = new PointF(Start.Position.X, End.Position.Y);
                
            }
        }

        private void MakeVertical_2() // end = side
        {
            if (End.Position.X != Start.Position.X)
            {
                Start.Position = new PointF(End.Position.X, Start.Position.Y);

            }
        }

        public void fake_update(Vertex side)
        {
            if(side == Start)
            {
                MakeVertical();
            }
            else
            {
                MakeVertical_2();
            }
        }

        public override bool need_update(Vertex side)
        {
            if(Start.Position.X == End.Position.X)
            {
                return false;
            }
            return true;
        }

        public override Edge Update(Vertex movedVertex)
        {
            fake_update(movedVertex);
            return ChangeEdgeType(this, this.Constraint, movedVertex, this.FixedLength);
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
                side.Position = new PointF(side.Position.X + mouseDelta.X, side.Position.Y);

                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;

                float vectorLength = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

                float fixedVectorX = vectorX * 3;
                float fixedVectorY = vectorY * 3;
                otherSide.Position = new PointF(side.Position.X + fixedVectorX, side.Position.Y + fixedVectorY);
            }
            else if (side.constraint == VertexConstraint.G1)
            {
                side.Position = new PointF(side.Position.X + mouseDelta.X, side.Position.Y);

                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;
                
                float vectorY_2 = otherSide.Position.Y - side.Position.Y;

                if(vectorY*vectorY_2 < 0)
                {
                    otherSide.Position = new PointF(otherSide.Position.X + mouseDelta.X, side.Position.Y - vectorY_2);
                    return;
                }

                otherSide.Position = new PointF(otherSide.Position.X + mouseDelta.X, otherSide.Position.Y);
            }
        }
    }
}
