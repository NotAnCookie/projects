using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace proj_1.edge
{
    public class Edge
    {
        public Vertex Start { get; set; }
        public Vertex End { get; set; }
        public bool IsHovered { get; set; } // Do podświetlania krawędzi
        public EdgeConstraint Constraint { get; set; } = EdgeConstraint.None; 
        public float FixedLength { get; set; } // Długość dla ograniczenia FixedLength
        public Vertex side { get; set; } // Wyznacza jednostronność krawędzi ( równe Start lub End )

        public bool is_using_WU { get; set; } = false;

        public Edge(Vertex start, Vertex end, Vertex side, EdgeConstraint constraint, float length = 0f)
        {
            Start = start;
            End = end;
            this.side = side;
            FixedLength = length;
            if(length == 0f)
            {
                FixedLength = GetLength();
            }
            Constraint = constraint; 
        }

        public virtual bool is_ok()
        {
            return true;
        }


        // Funkcja sprawdzająca, czy można ustawić dane ograniczenie
        public bool CanSetConstraint(EdgeConstraint constraint, List<Edge> edges)
        {
            // Sprawdź sąsiednie krawędzie (przylegające do wierzchołków)
            Edge previousEdge = GetPreviousEdge(edges);
            Edge nextEdge = GetNextEdge(edges);

            if (constraint == EdgeConstraint.Horizontal)
            {
                if (previousEdge?.Constraint == EdgeConstraint.Horizontal ||
                    nextEdge?.Constraint == EdgeConstraint.Horizontal)
                {
                    return false; // Nie można, jeśli sąsiednia krawędź jest pozioma
                }
            }

            if (constraint == EdgeConstraint.Vertical)
            {
                if (previousEdge?.Constraint == EdgeConstraint.Vertical ||
                    nextEdge?.Constraint == EdgeConstraint.Vertical)
                {
                    return false; // Nie można, jeśli sąsiednia krawędź jest pionowa
                }
            }

            return true; // Można ustawić
        }

        public float GetLength()
        {
            return (float)Math.Sqrt(Math.Pow(End.Position.X - Start.Position.X, 2) +
                                    Math.Pow(End.Position.Y - Start.Position.Y, 2));
        }

        // Pobiera poprzednią krawędź (łączącą się z "Start")
        public Edge GetPreviousEdge(List<Edge> edges)
        {
            return edges.Find(e => (e.End == Start || e.Start == Start) && e != this);
        }

        // Pobiera następną krawędź (łączącą się z "End")
        public Edge GetNextEdge(List<Edge> edges)
        {
            return edges.Find(e => (e.Start == End || e.End == End) && e != this);
        }

        public virtual Edge Update(Vertex movedVertex)
        {
            return ChangeEdgeType(this, EdgeConstraint.None, movedVertex, this.FixedLength);
        }


        public virtual void Draw(Graphics g, bool isHovered, bool isBesenham, Bitmap bitmap)
        {
            float desiredWidth = 2f;
            Pen pen = new Pen(isHovered ? Color.Orange : Color.Black, desiredWidth);
            DrawMyLine(g,pen, Start.Position, End.Position, isBesenham,bitmap);
        }

        public bool IsMouseHover(PointF mousePosition)
        {
            const double tolerance = 5.0; // Tolerancja najechania na krawędź
            double distance = DistanceFromPointToLine(mousePosition, Start.Position, End.Position);
            return distance <= tolerance;
        }

        private double DistanceFromPointToLine(PointF p, PointF a, PointF b)
        {
            double A = p.X - a.X;
            double B = p.Y - a.Y;
            double C = b.X - a.X;
            double D = b.Y - a.Y;

            double dot = A * C + B * D;
            double len_sq = C * C + D * D;
            double param = len_sq != 0 ? dot / len_sq : -1;

            double xx, yy;

            if (param < 0)
            {
                xx = a.X;
                yy = a.Y;
            }
            else if (param > 1)
            {
                xx = b.X;
                yy = b.Y;
            }
            else
            {
                xx = a.X + param * C;
                yy = a.Y + param * D;
            }

            double dx = p.X - xx;
            double dy = p.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }


        public virtual bool need_update(Vertex side)
        {
            return false;
        }


        public void DrawMyLine(Graphics g, Pen pen, PointF p1, PointF p2, bool isBesenham, Bitmap bitmap, bool only_point=false)
        {
            if (is_using_WU)
            {
                Color backgroundColor = bitmap.GetPixel(0, 0);
                DrawWU(bitmap, (double)p1.X, (double)p1.Y, (double)p2.X, (double)p2.Y, backgroundColor, g);
                return;
            }
            if (isBesenham)
            {
                if (only_point)
                {
                    bitmap.SetPixel((int)p1.X, (int)p1.Y, pen.Color);
                }
                else
                {
                    DrawLineBresenham(bitmap, (int)Math.Round(p1.X), (int)Math.Round(p1.Y), (int)Math.Round(p2.X), (int)Math.Round(p2.Y), pen.Color, g);
                }
            }
            if (!isBesenham)
            {
                g.DrawLine(pen, p1, p2);
            }
        }



        public Edge ChangeEdgeType(Edge edge, EdgeConstraint newConstraint, Vertex side, float? fixedLength = null)
        {
            Vertex start = edge.Start;
            Vertex end = edge.End;

            return newConstraint switch
            {
                EdgeConstraint.Vertical => new VerticalEdge(start, end, side),
                EdgeConstraint.Horizontal => new HorizontalEdge(start, end, side),
                EdgeConstraint.FixedLength => new FixedLengthEdge(start, end, side, fixedLength.Value),
                _ => new Edge(start, end, side, EdgeConstraint.None),
            };
        }


        public virtual void MoveForFriendBezier(Vertex side, BezierEdge edge, PointF mouseDelta)
        {
            Vertex bezierMovedVertex = side == edge.Start ? edge.ControlPoint1 : edge.ControlPoint2;
            Vertex otherSide = side == this.Start ? this.End : this.Start;

            if (side.constraint == VertexConstraint.C1)
            {
                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;

                float vectorLength = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

                float fixedVectorX = vectorX * 3;
                float fixedVectorY = vectorY * 3;
                otherSide.Position = new PointF(side.Position.X + fixedVectorX, side.Position.Y + fixedVectorY);
            }
            else if (side.constraint == VertexConstraint.G1)
            {
                float vectorX = side.Position.X - bezierMovedVertex.Position.X;
                float vectorY = side.Position.Y - bezierMovedVertex.Position.Y;

                float vectorLength = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

                // Normalizacja wektorów
                float normalizedX = vectorX / vectorLength;
                float normalizedY = vectorY / vectorLength;

                float fixedVectorX = normalizedX * this.GetLength();
                float fixedVectorY = normalizedY * this.GetLength();
                otherSide.Position = new PointF(side.Position.X + fixedVectorX, side.Position.Y + fixedVectorY);
            }
        }

        public void DrawLineBresenham(Bitmap bitmap, int x1, int y1, int x2, int y2, Color color, Graphics g)
        { // https://eduinf.waw.pl/inf/utils/002_roz/2008_06.php
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int kx = (x1 < x2) ? 1 : -1; // Kierunek przesunięcia na osi x
            int ky = (y1 < y2) ? 1 : -1; // Kierunek przesunięcia na osi y

            int e; // Zmienna błędu

            // Ustawienie początkowego punktu
            if (x1 >= 0 && x1 < bitmap.Width && y1 >= 0 && y1 < bitmap.Height)
            {
                bitmap.SetPixel(x1, y1, color);
            }

            if (dx >= dy)
            {
                e = dx / 2;
                for (int i = 0; i < dx; i++)
                {
                    x1 += kx;
                    e -= dy;
                    if (e < 0)
                    {
                        y1 += ky;
                        e += dx;
                    }
                    // Rysowanie pikseli, tylko w granicach bitmapy
                    if (x1 >= 0 && x1 < bitmap.Width && y1 >= 0 && y1 < bitmap.Height)
                    {
                        bitmap.SetPixel(x1, y1, color);
                    }
                }
            }
            else
            {
                e = dy / 2;
                for (int i = 0; i < dy; i++)
                {
                    y1 += ky;
                    e -= dx;
                    if (e < 0)
                    {
                        x1 += kx;
                        e += dy;
                    }
                    // Rysowanie pikseli, tylko w granicach bitmapy
                    if (x1 >= 0 && x1 < bitmap.Width && y1 >= 0 && y1 < bitmap.Height)
                    {
                        bitmap.SetPixel(x1, y1, color);
                    }
                }
            }
        }

        public void DrawWU(Bitmap bitmap, double x1, double y1, double x2, double y2, Color foreColor, Graphics g)
        {
            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);

            // zmiana koordynatów jeżeli nie ma steep
            if (steep)
            {
                (x1, y1) = (y1, x1);
                (x2, y2) = (y2, x2);
            }
            if (x1 > x2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }

            // gradient
            double dx = x2 - x1;
            double dy = y2 - y1;
            double gradient = dy / dx;

            // 1 końca odcinka
            double xEnd = Round(x1);
            double yEnd = y1 + gradient * (xEnd - x1);
            double xGap = One_Frac(x1 + 0.5);
            double xPixel1 = xEnd;
            double yPixel1 = (int)(yEnd);

            double The_y = yEnd;// + gradient 

            // 2 końca odcinka
            xEnd = Round(x2);
            yEnd = y2 + gradient * (xEnd - x2);
            xGap = Frac(x2 + 0.5);
            double xPixel2 = xEnd;
            double yPixel2 = (int)(yEnd);

            // główna linia
            if (steep)
            {
                for (int x = (int)(xPixel1); x <= xPixel2 - 1; x++) // (x=x1,x<x2,x++)
                {
                    DrawPixel(bitmap, (int)(The_y), x, One_Frac(The_y), foreColor);
                    DrawPixel(bitmap, (int)(The_y) + 1, x, Frac(The_y), foreColor);
                    The_y += gradient;
                }
            }
            else
            {
                for (int x = (int)(xPixel1); x <= xPixel2 - 1; x++)
                {
                    DrawPixel(bitmap, x, (int)(The_y), One_Frac(The_y), foreColor);
                    DrawPixel(bitmap, x, (int)(The_y) + 1, Frac(The_y), foreColor);
                    The_y += gradient;
                }
            }
            g.DrawImage(bitmap, 0, 0); // aby się rysowało nad innymi liniami (bo kopiujemy bitmape wcześniej)
        }
        private static void DrawPixel(Bitmap bitmap, double x, double y, double c, Color foreColor)
        {
            int alpha = (int)(c * 255);
            if (alpha > 255) alpha = 255;
            if (alpha < 0) alpha = 0;
            Color color = Color.FromArgb(alpha, foreColor.R, foreColor.G, foreColor.B);
            if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
            {
                bitmap.SetPixel((int)x, (int)y, color);
            }
        }

        private int Round(double x) => (int)(x + 0.5);

        // fractional part of x
        private double Frac(double x) => x - Math.Floor(x);

        // 1 - fractional part of x
        private double One_Frac(double x) => 1 - Frac(x);


    }
}
