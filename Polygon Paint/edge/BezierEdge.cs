using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace proj_1.edge
{
    public class BezierEdge : Edge
    {
        public Vertex ControlPoint1 { get; set; }
        public Vertex ControlPoint2 { get; set; }

        public BezierEdge(Vertex start, Vertex end,Vertex controlPoint1, Vertex controlPoint2, Vertex side) : base(start, end, side, EdgeConstraint.Bezier)
        {
            this.ControlPoint1 = controlPoint1;
            this.ControlPoint2 = controlPoint2;
            if (start.constraint == VertexConstraint.None)
            {
                start.constraint = VertexConstraint.C1;
            }
            if (end.constraint == VertexConstraint.None)
            {
                end.constraint = VertexConstraint.C1;
            }
        }


        public override void Draw(Graphics g, bool isHovered, bool isBresenham, Bitmap bitmap)
        {
            int segments = 100;
            float desiredWidth = 2f;     
            Pen pen = new Pen(isHovered ? Color.Orange : Color.Black, desiredWidth);


            // 2 metody rysowania beziera
            DrawIncrementalBezier(g, pen, Start.Position, ControlPoint1.Position, ControlPoint2.Position, End.Position, segments, isBresenham, bitmap);
            //DrawBezierIterative(g, pen, Start.Position, ControlPoint1.Position, ControlPoint2.Position, End.Position, segments, isBresenham, bitmap);
            
            // Otoczenie beziera
            using (Pen dashedPen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
            {
                g.DrawLine(dashedPen, Start.Position, ControlPoint1.Position);
                g.DrawLine(dashedPen, End.Position, ControlPoint2.Position);
                g.DrawLine(dashedPen, ControlPoint1.Position, ControlPoint2.Position);
                g.DrawLine(dashedPen, Start.Position, End.Position);
            }
        }

        public Edge Update(Vertex movedVertex, List<Edge> edges)
        {
            if(movedVertex.constraint == VertexConstraint.C1)
            {
                if (movedVertex == Start)
                {
                    ControlPoint1.Position = BezierSupportCalculatorC1_start(Start, End, this, edges).Position;
                }
                if (movedVertex == End)
                {
                    ControlPoint2.Position = BezierSupportCalculatorC1_End(Start, End, this, edges).Position;
                }
            }
            if (movedVertex.constraint == VertexConstraint.G1)
            {
                if (movedVertex == Start)
                {
                    ControlPoint1.Position = BezierSupportCalculatorG1_start(Start, End, this, edges).Position;
                }
                if (movedVertex == End)
                {
                    ControlPoint2.Position = BezierSupportCalculatorG1_End(Start, End, this, edges).Position;
                }
            }

            
            return this;
        }

        private PointF CalculateBezierPoint(float t, PointF p0, PointF p1, PointF p2, PointF p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            PointF point = new PointF();
            point.X = uuu * p0.X; // (1 - t)^3 * P0
            point.X += 3 * uu * t * p1.X; // 3 * (1 - t)^2 * t * P1
            point.X += 3 * u * tt * p2.X; // 3 * (1 - t) * t^2 * P2
            point.X += ttt * p3.X; // t^3 * P3

            point.Y = uuu * p0.Y; // (1 - t)^3 * P0
            point.Y += 3 * uu * t * p1.Y; // 3 * (1 - t)^2 * t * P1
            point.Y += 3 * u * tt * p2.Y; // 3 * (1 - t) * t^2 * P2
            point.Y += ttt * p3.Y; // t^3 * P3

            return point;
        }


        

        private Vertex BezierSupportCalculatorC1_start(Vertex Start, Vertex End, Edge workingEdge, List<Edge> edges) 
        {
            Edge startEdge = workingEdge.GetPreviousEdge(edges); // krawędź wychodząca ze Start
            Vertex startVertex = startEdge.Start == Start ? startEdge.End : startEdge.Start;
            if (startEdge.Constraint == EdgeConstraint.Bezier)
            {
                startVertex = ((BezierEdge)startEdge).ControlPoint2;
            }


            float delta_x = Start.Position.X - startVertex.Position.X;
            float delta_y = Start.Position.Y - startVertex.Position.Y;
            PointF add1 = new PointF(Start.Position.X + (delta_x / 3), Start.Position.Y + (delta_y / 3));
            if (startEdge.Constraint == EdgeConstraint.Bezier)
            {
                add1 = new PointF(Start.Position.X + (delta_x), Start.Position.Y + (delta_y));
            }
            Vertex sup1 = new Vertex(add1);

            return sup1;
        }

        private Vertex BezierSupportCalculatorC1_End(Vertex Start, Vertex End, Edge workingEdge, List<Edge> edges)
        {
            Edge endEdge = workingEdge.GetNextEdge(edges); // krawędź wychodząca z End
            Vertex endVertex = endEdge.End == End ? endEdge.Start : endEdge.End;
            if (endEdge.Constraint == EdgeConstraint.Bezier)
            {
                endVertex = ((BezierEdge)endEdge).ControlPoint1;
            }

            float delta_x = End.Position.X - endVertex.Position.X;
            float delta_y = End.Position.Y - endVertex.Position.Y;
            PointF add2 = new PointF(End.Position.X + (delta_x / 3), End.Position.Y + (delta_y / 3));
            if (endEdge.Constraint == EdgeConstraint.Bezier)
            {
                add2 = new PointF(End.Position.X + (delta_x), End.Position.Y + (delta_y));
            }

            Vertex sup2 = new Vertex(add2);

            return sup2;
        }

        

        private Vertex BezierSupportCalculatorG1_start(Vertex Start, Vertex End, Edge workingEdge, List<Edge> edges) 
        {

            Edge startEdge = workingEdge.GetPreviousEdge(edges); // krawędź wychodząca ze Start
            Vertex startVertex = startEdge.Start == Start ? startEdge.End : startEdge.Start;

            if (startEdge.Constraint == EdgeConstraint.Bezier)
            {
                startVertex = ((BezierEdge)startEdge).ControlPoint2;
            }

            float vectorX = Start.Position.X - ControlPoint1.Position.X;
            float vectorY = Start.Position.Y - ControlPoint1.Position.Y;
            //float scale = nextEdge.FixedLength;

            float len = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

            float vectorX_2 = startVertex.Position.X - Start.Position.X;
            float vectorY_2 = startVertex.Position.Y - Start.Position.Y;
            //float scale = nextEdge.FixedLength;

            float vectorLength = (float)Math.Sqrt(vectorX_2 * vectorX_2 + vectorY_2 * vectorY_2);

            // Znormalizuj wektor, aby jego długość wynosiła 1
            float normalizedX = vectorX_2 / vectorLength;
            float normalizedY = vectorY_2 / vectorLength;

            // Przemnóż znormalizowany wektor przez FixedLength
            float fixedVectorX = normalizedX * len;
            float fixedVectorY = normalizedY * len;
            Vertex sup1 = new Vertex(new PointF(0, 0));
            sup1.Position = new PointF(Start.Position.X - fixedVectorX, Start.Position.Y - fixedVectorY);
            if (startEdge.Constraint == EdgeConstraint.Vertical)
            {
                sup1.Position = new PointF(Start.Position.X - fixedVectorX, ControlPoint1.Position.Y);
            }
            if (startEdge.Constraint == EdgeConstraint.Horizontal)
            {
                sup1.Position = new PointF(ControlPoint1.Position.X, Start.Position.Y - fixedVectorY);
            }

            return sup1;

        }

        private Vertex BezierSupportCalculatorG1_End(Vertex Start, Vertex End, Edge workingEdge, List<Edge> edges) 
        {

            Edge endEdge = workingEdge.GetNextEdge(edges); // krawędź wychodząca ze Start
            Vertex startVertex = endEdge.End == End ? endEdge.Start : endEdge.End;

            if (endEdge.Constraint == EdgeConstraint.Bezier)
            {
                startVertex = ((BezierEdge)endEdge).ControlPoint1;
            }

            float vectorX = End.Position.X - ControlPoint2.Position.X;
            float vectorY = End.Position.Y - ControlPoint2.Position.Y;
            //float scale = nextEdge.FixedLength;

            float len = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

            float vectorX_2 = startVertex.Position.X - End.Position.X;
            float vectorY_2 = startVertex.Position.Y - End.Position.Y;
            //float scale = nextEdge.FixedLength;

            float vectorLength = (float)Math.Sqrt(vectorX_2 * vectorX_2 + vectorY_2 * vectorY_2);

            // Znormalizuj wektor, aby jego długość wynosiła 1
            float normalizedX = vectorX_2 / vectorLength;
            float normalizedY = vectorY_2 / vectorLength;
            //if(vectorX < 1)
            //{
            //    normalizedX = 0;
            //}
            //if (vectorY < 1)
            //{
            //    normalizedY = 0;
            //}

            //Przemnóż znormalizowany wektor przez FixedLength
            float fixedVectorX = normalizedX * len;
            float fixedVectorY = normalizedY * len;
            //float fixedVectorX = (vectorX_2 / vectorLength) * len;
            //float fixedVectorY = (vectorY_2 / vectorLength) * len;
            Vertex sup1 = new Vertex(new PointF(0, 0));
            sup1.Position = new PointF(End.Position.X - fixedVectorX, End.Position.Y - fixedVectorY); //ControlPoint2.Position.Y);//End.Position.Y /*- fixedVectorY*/);
            if(endEdge.Constraint == EdgeConstraint.Vertical)
            {
                sup1.Position = new PointF(End.Position.X - fixedVectorX, ControlPoint2.Position.Y);
            }
            if (endEdge.Constraint == EdgeConstraint.Horizontal)
            {
                sup1.Position = new PointF(ControlPoint2.Position.X, End.Position.Y - fixedVectorY);
            }



            return sup1;

        }

        public override void MoveForFriendBezier(Vertex side, BezierEdge edge, PointF mouseDelta)
        {



            Vertex bezierMovedVertex = side == edge.Start ? edge.ControlPoint1 : edge.ControlPoint2;
            float is_position = (float)Math.Sqrt(Math.Pow(side.Position.X - bezierMovedVertex.Position.X, 2) +
                                    Math.Pow(side.Position.Y - bezierMovedVertex.Position.Y, 2)) * 3 - this.FixedLength;
            float len = (float)Math.Sqrt(Math.Pow(side.Position.X - bezierMovedVertex.Position.X, 2) +
                                    Math.Pow(side.Position.Y - bezierMovedVertex.Position.Y, 2)) * 3;
            
            Vertex otherSide = side == this.Start ? ControlPoint1 : ControlPoint2;


            if (Math.Abs(is_position) > 0.01 && side.constraint == VertexConstraint.C1)
            {
                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;
                otherSide.Position = new PointF(side.Position.X + vectorX, side.Position.Y + vectorY);
            }
            else if (side.constraint == VertexConstraint.G1)
            {
                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;

                float vectorLength = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

                // Normalizacja wektora
                float normalizedX = vectorX / vectorLength;
                float normalizedY = vectorY / vectorLength;

                float lenvectorX = otherSide.Position.X - side.Position.X;
                float lenvectorY = otherSide.Position.Y - side.Position.Y;
                vectorLength = (float)Math.Sqrt(lenvectorX * lenvectorX + lenvectorY * lenvectorY);
                float fixedVectorX = normalizedX * vectorLength;
                float fixedVectorY = normalizedY * vectorLength;
                otherSide.Position = new PointF(side.Position.X + fixedVectorX, side.Position.Y + fixedVectorY);
            }
        }



        private void DrawIncrementalBezier(Graphics g, Pen pen, PointF p0, PointF p1, PointF p2, PointF p3, int segments, bool isBresenham, Bitmap bitmap)
        {
            float t = 0;
            float dt = 1f / segments;

            PointF previousPoint = p0; // Punkt początkowy

            for (int i = 1; i <= segments; i++)
            {
                PointF currentPoint = CalculateBezierPoint(t, p0, p1, p2, p3);

                DrawMyLine(g, pen, previousPoint, currentPoint, isBresenham, bitmap);

                previousPoint = currentPoint;
                t += dt;
            }
        }

        public void DrawBezierIterative(Graphics g, Pen pen, PointF p0, PointF p1, PointF p2, PointF p3, int segments, bool isBresenham, Bitmap bitmap)
        {
            float tIncrement = 1f / segments;
            float t = 0;

            PointF previousPoint = p0;

            for (int i = 1; i <= segments; i++)
            {
                t += tIncrement;

                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;
                float uuu = uu * u;
                float ttt = tt * t;

                PointF currentPoint = new PointF();
                currentPoint.X = (uuu * p0.X) + (3 * uu * t * p1.X) + (3 * u * tt * p2.X) + (ttt * p3.X);
                currentPoint.Y = (uuu * p0.Y) + (3 * uu * t * p1.Y) + (3 * u * tt * p2.Y) + (ttt * p3.Y);

                DrawMyLine(g, pen, previousPoint, currentPoint, isBresenham, bitmap);

                previousPoint = currentPoint;
            }

        }


    }
}
