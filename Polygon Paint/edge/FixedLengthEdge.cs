using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace proj_1.edge
{
    public class FixedLengthEdge : Edge
    {
        public FixedLengthEdge(Vertex start, Vertex end, Vertex side, float length) : base(start, end, side,EdgeConstraint.FixedLength, length)
        {
            Constraint = EdgeConstraint.FixedLength;
            fakeupdate(side, FixedLength);
        }

        public override bool is_ok()
        {
            if (Math.Abs(this.FixedLength - this.GetLength())<=1)
            {
                return true;
            }
            return false;
        }

        public override void Draw(Graphics g, bool isHovered, bool isBesenham, Bitmap bitmap)
        {
            float desiredWidth = 2f;
            Pen pen = new Pen(isHovered ? Color.Orange : Color.Black, desiredWidth);
            DrawMyLine(g,pen, Start.Position, End.Position,isBesenham,bitmap);

            // "Ikona"
            Font font = new Font("Arial", 7);
            Brush brush = Brushes.Black; 
            float midX = (Start.Position.X + End.Position.X) / 2;
            float midY = (Start.Position.Y + End.Position.Y) / 2;
            g.DrawString((FixedLength).ToString(), font, brush, new PointF(midX, midY));
        }

        public void SetFixedLength(float newLength)
        {
            FixedLength = newLength;
            float currentLength = GetLength();

            if (currentLength != 0)
            {
                float scaleFactor = newLength / currentLength;
                float newX = Start.Position.X + ((End.Position.X - Start.Position.X) * scaleFactor);
                float newY = Start.Position.Y + ((End.Position.Y - Start.Position.Y) * scaleFactor);
                End.SetPosition(newX, newY);
            }
        }

        public void SetFixedLength_2(float newLength)
        {
            FixedLength = newLength;
            float currentLength = GetLength();

            if (currentLength != 0)
            {
                float scaleFactor = newLength / currentLength;
                float newX = End.Position.X + ((Start.Position.X - End.Position.X) * scaleFactor);
                float newY = End.Position.Y + ((Start.Position.Y - End.Position.Y) * scaleFactor);
                Start.SetPosition(newX, newY);
            }
        }

        public void fakeupdate(Vertex side, float newLength)
        {
            if (side == Start)
            {
                SetFixedLength(newLength);
            }
            else
            {
                SetFixedLength_2(newLength);
            }
        }

        public override bool need_update(Vertex side)
        {
            if (FixedLength == GetLength())
            {
                return false;
            }
            return true;
        }

        public override Edge Update(Vertex movedVertex)
        {
            fakeupdate(movedVertex,FixedLength);
            return null;
        }

        public override void MoveForFriendBezier(Vertex side, BezierEdge edge, PointF mouseDelta)
        {
            Vertex bezierMovedVertex = side == edge.Start?edge.ControlPoint1 :edge.ControlPoint2;
            float is_position = (float)Math.Sqrt(Math.Pow(side.Position.X - bezierMovedVertex.Position.X, 2) +
                                    Math.Pow(side.Position.Y - bezierMovedVertex.Position.Y, 2)) * 3 - this.FixedLength;
            Vertex otherSide = side == this.Start ? this.End : this.Start;


            if (Math.Abs(is_position) > 0.01 && side.constraint == VertexConstraint.C1)
            {
                side.Position = new PointF(side.Position.X + mouseDelta.X, side.Position.Y + mouseDelta.Y);

                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;
                otherSide.Position = new PointF(side.Position.X + 3 * vectorX, side.Position.Y + 3 * vectorY);
            }
            else if (side.constraint == VertexConstraint.G1)
            {
                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;

                float vectorLength = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
                // Normalizacja wektorów
                float normalizedX = vectorX / vectorLength;
                float normalizedY = vectorY / vectorLength;

                float fixedVectorX = normalizedX * this.FixedLength;
                float fixedVectorY = normalizedY * this.FixedLength;
                otherSide.Position = new PointF(side.Position.X + fixedVectorX, side.Position.Y + fixedVectorY);
            }
        }
    }
}
