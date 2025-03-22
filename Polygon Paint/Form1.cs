using Microsoft.VisualBasic.Devices;
using proj_1.edge;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace proj_1
{
    public partial class Form1 : Form
    {
        private List<Vertex> vertices = new List<Vertex>(); 
        private List<Edge> edges = new List<Edge>();
        private List<Vertex> bezierVertices = new List<Vertex>();

        private bool isDrawing = true; // Czy mo�na rysowa� 
        private Vertex firstVertex; // Pierwszy punkt (Vertex) potrzebny do zamkni�cia wielok�ta
        private bool isClosed = false; // Czy wielok�t jest zamkni�ty
        private Vertex hoveringVertex = null; // Przechowywanie punktu nad kt�rym jest kursor
        private Vertex selectedVertex = null; // Przechowywanie punktu klikni�tego prawym przyciskiem
        private Edge hoveringEdge = null; // Przechowywanie kraw�dzi nad kt�r� jest kursor
        private Vertex hoveringBezierVertex = null;

        private Edge selectedEdge = null; // Klikni�ta kraw�d�

        private Vertex draggedVertex = null; // Wierzcho�ek, kt�ry przeci�gamy lewym przyciskiem
        private bool isDragging = false; // Czy obecnie przeci�gamy wierzcho�ek
        private Vertex draggingFriendPrev = null;
        private Vertex draggingFriendNext = null;

        private Vertex draggedBezierVertex = null;

        private PointF lastMousePosition;
        private PointF initialVertexPosition;

        private bool UseBresenham = false; // czy rysujemy Bresenhamem
        private bool wasUsingBresenham = false; // czy rysowali�my Bresenhamem (potrzebne przy przeci�ganiu)
        private Bitmap drawingBitmap; // Podczas rysowania Bresenhamem
        private RadioButton bresenhamRadioButton; // opcja Bresenham
        private RadioButton otherOptionRadioButton; // opcja DrawLine

        private PolygonState polygonStateLast; // do cofania w razie b��du
        private bool isPolygonDragging = false; // czy przesuwamy ca�y wielok�t

        

        public Form1()
        {
            InitializeComponent();
            drawingBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            this.DoubleBuffered = true; 
            this.MouseClick += new MouseEventHandler(OnMouseClick); // rysowanie + contextmenu
            this.MouseMove += new MouseEventHandler(OnMouseMove); // pod�wietlanie
            this.Paint += new PaintEventHandler(OnPaint);

            this.MouseDown += Form1_MouseDown; // dla przesuwania
            this.MouseMove += Form1_MouseMove; // dla przesuwania
            this.MouseUp += Form1_MouseUp;

            
            InitializeMenuBar();
            InitializePolygon(); // Pocz�tkowy wielok�t

            // Obs�uga RadioButtons
            bresenhamRadioButton = new RadioButton
            {
                Text = "Use Bresenham",
                Location = new Point(10, 50),
                AutoSize = true,
                ForeColor = Color.Red 
            };
            otherOptionRadioButton = new RadioButton
            {
                Text = "Use DrawLine",
                Location = new Point(10, 80),
                AutoSize = true,
                ForeColor = Color.Green 
            };
            otherOptionRadioButton.Checked = true;
            bresenhamRadioButton.CheckedChanged += RadioButton_CheckedChanged;
            otherOptionRadioButton.CheckedChanged += RadioButton_CheckedChanged;
            this.Controls.Add(bresenhamRadioButton);
            this.Controls.Add(otherOptionRadioButton);

          
        }

        private void OnMouseClick(object sender, MouseEventArgs e) // dla rysowania i contextmenu
        {
            Point clickedPoint = e.Location;

            // Dla contextmenu
            if (e.Button == MouseButtons.Right && isClosed)
            {
                foreach (var vertex in vertices)
                {
                    if (vertex.IsMouseHover(clickedPoint))
                    {
                        selectedVertex = vertex; // Zapami�taj klikni�ty punkt
                        ShowVertexContextMenu(vertex, clickedPoint);
                        return;
                    }
                }
                foreach (var vertex in bezierVertices)
                {
                    if (vertex.IsMouseHover(clickedPoint))
                    {
                        selectedVertex = vertex; // Zapami�taj klikni�ty punkt
                        ShowVertexContextMenu(vertex, clickedPoint);
                        return;
                    }
                }

                foreach (var edge in edges)
                {
                    if (edge.IsMouseHover(clickedPoint))
                    {
                        selectedEdge = edge; // Zapami�taj klikni�t� kraw�d�
                        ShowEdgeContextMenu(selectedEdge, e.Location);
                        this.Invalidate();

                        return;
                    }
                }
            }
            if (isClosed) return; // wyj�cie z mo�liwo�ci rysowania wielok�ta

            // Rysowanie wielok�ta
            if (vertices.Count > 1 && firstVertex.IsMouseHover(clickedPoint))
            {
                isClosed = true; // Zako�czenie rysowania
                AddEdge(vertices[vertices.Count - 1], vertices[0]); // Dodaj ostatni� kraw�d� ��cz�c� pierwszy i ostatni punkt
                polygonStateLast = new PolygonState(vertices, edges);
                isDrawing = false;
                this.Invalidate();
                
                return;
            }

            // Dodaj nowy punkt do listy
            Vertex newVertex = new Vertex(clickedPoint);
            vertices.Add(newVertex);
            if (vertices.Count == 1)
            {
                firstVertex = newVertex;
            }

            // Dodawanie kraw�dzi
            if (vertices.Count > 1)
            {
                AddEdge(vertices[vertices.Count - 2], newVertex);
            }

            this.Invalidate();
        }

        private void AddEdge(Vertex start, Vertex end)
        {
            Edge edge = new Edge(start, end,start,EdgeConstraint.None);
            edges.Add(edge);
        }

        private void OnMouseMove(object sender, MouseEventArgs e) // dla pod�wietlania
        {
            Point mousePosition = e.Location;
            // reset pod�wietlania
            hoveringVertex = null; 
            hoveringEdge = null; 
            hoveringBezierVertex = null;

            // pod�wietlanie
            foreach (var vertex in vertices)
            {
                if (vertex.IsMouseHover(mousePosition))
                {
                    hoveringVertex = vertex;
                    break;
                }
            }
            foreach (var vertex in bezierVertices)
            {
                if (vertex.IsMouseHover(mousePosition))
                {
                    hoveringBezierVertex = vertex;
                    break;
                }
            }

            foreach (var edge in edges)
            {
                if (edge.IsMouseHover(mousePosition))
                {
                    hoveringEdge = edge;
                    break;
                }
            }

            this.Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            if (UseBresenham || !UseBresenham)
            {
                // Je�li u�ywamy algorytmu Bresenhama, kopiujemy bitmap� na ekran

                g.DrawImage(drawingBitmap, Point.Empty);
                UpdateMenuItems();
                drawingBitmap.Dispose();
                drawingBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }


            // Rysowanie ostatniego stanu
            if (polygonStateLast != null && !UseBresenham) 
            {
                polygonStateLast.Draw(g);
            }

            // Rysowanie kraw�dzi
            foreach (var edge in edges)
            {
                bool isHovered = edge == hoveringEdge;
                edge.Draw(g, isHovered,UseBresenham,drawingBitmap); 
            }

            // Rysowanie dynamicznej linii z ostatniego punktu do pozycji kursora, je�li rysowanie nie jest zako�czone
            if (isDrawing && vertices.Count > 0 && !isClosed)
            {
                PointF lastPoint = vertices[vertices.Count - 1].Position;
                PointF mousePosition = this.PointToClient(Cursor.Position);
                g.DrawLine(Pens.Red, lastPoint, mousePosition);
            }

            // Rysowanie punkt�w
            foreach (var vertex in vertices)
            {
                bool isHovered = vertex == hoveringVertex;
                vertex.Draw(g, isHovered);
            }
            foreach (var vertex in bezierVertices)
            {
                bool isHovered = vertex == hoveringBezierVertex; 
                vertex.Draw(g, isHovered);
            }

            UpdateMenuItems();

        }

        private void OnDeleteVertex(object sender, EventArgs e)
        {
            if (selectedVertex != null)
            {
                int index = vertices.IndexOf(selectedVertex);
                Vertex previousVertex = null;
                Vertex nextVertex = null;

                // znajdowanie s�siaduj�cych kraw�dzi
                foreach (var edge in edges)
                {
                    if(edge.Start == selectedVertex)
                    {
                        nextVertex = edge.End;
                        Edge nextedge = edge.GetNextEdge(edges);
                        if (nextedge.Constraint != EdgeConstraint.Bezier)
                        {
                            nextVertex.constraint = VertexConstraint.None;
                        }
                        if (edge.Constraint == EdgeConstraint.Bezier)
                        {
                            bezierVertices.Remove(((BezierEdge)edge).ControlPoint1);
                            bezierVertices.Remove(((BezierEdge)edge).ControlPoint2);
                        }

                    }
                    if (edge.End == selectedVertex)
                    {
                        previousVertex = edge.Start;
                        Edge prevedge = edge.GetPreviousEdge(edges);
                        if (prevedge.Constraint != EdgeConstraint.Bezier)
                        {
                            
                            previousVertex.constraint = VertexConstraint.None;
                        }
                        if(edge.Constraint == EdgeConstraint.Bezier)
                        {
                            bezierVertices.Remove(((BezierEdge)edge).ControlPoint1);
                            bezierVertices.Remove(((BezierEdge)edge).ControlPoint2);
                        }
                        
                    }

                }

                // Usu� kraw�dzie powi�zane z usuwanym punktem
                edges.RemoveAll(edge => edge.Start == selectedVertex || edge.End == selectedVertex);


                // Nowa kraw�d�
                if (previousVertex != null && nextVertex != null)
                {
                    AddEdge(previousVertex, nextVertex);
                }

                // Usu� punkt
                vertices.Remove(selectedVertex);
                selectedVertex = null;
                // Uaktualnienie ewentualnych bezir�w
                foreach (Edge edge in new List<Edge>(edges))
                {
                    if (edge.Constraint == EdgeConstraint.Bezier)
                    {
                        int j = edges.IndexOf(edge);
                        ((BezierEdge)edges[j]).Update(edge.Start, edges);
                        ((BezierEdge)edges[j]).Update(edge.End, edges);
                    }
                }
                this.Invalidate(); 
            }
        }
        private void AddVertexInEdgeMiddle(Edge edge)
        {
            // Obliczenie �rodka kraw�dzi
            PointF middlePoint = new PointF(
                (edge.Start.Position.X + edge.End.Position.X) / 2,
                (edge.Start.Position.Y + edge.End.Position.Y) / 2
            );

            // nowy wierzcho�ek
            Vertex newVertex = new Vertex(middlePoint);
            int startIndex = vertices.IndexOf(edge.Start);
            int endIndex = vertices.IndexOf(edge.End);

            // Wstaw nowy wierzcho�ek w odpowiednie miejsce na li�cie
            vertices.Insert(startIndex + 1, newVertex);

            // Usuwanie kraw�dzi
            if(edge.Constraint == EdgeConstraint.Bezier)
            {
                bezierVertices.RemoveAll(v => v == ((BezierEdge)edge).ControlPoint1 || v == ((BezierEdge)edge).ControlPoint2);
                if(edge.GetNextEdge(edges).Constraint != EdgeConstraint.Bezier)
                {
                    edge.End.constraint = VertexConstraint.None;
                }
                if (edge.GetPreviousEdge(edges).Constraint != EdgeConstraint.Bezier)
                {
                    edge.Start.constraint = VertexConstraint.None;
                }
            }
            edges.Remove(edge);

            // Nowe kraw�dzie
            AddEdge(edge.Start, newVertex);
            AddEdge(newVertex, edge.End);

            this.Invalidate();
        }


        private void ShowEdgeContextMenu(Edge edge, Point mousePosition)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            // Opcja dodania nowego wierzcho�ka
            ToolStripMenuItem addVertexItem = new ToolStripMenuItem("Add Vertex");
            addVertexItem.Click += (s, e) =>
            {
                AddVertexInEdgeMiddle(edge);
                this.Invalidate();
            };

            // Opcje ustawienia ogranicze� na kraw�dzi
            ToolStripMenuItem horizontalConstraintItem = new ToolStripMenuItem("Set Horizontal");
            horizontalConstraintItem.Click += (s, e) =>
            {
                if (edge.CanSetConstraint(EdgeConstraint.Horizontal, edges)) // jak wr�cmy do setconstraint to zadzia�a
                {

                    //edge = ChangeEdgeTypeNONUSE(edge,EdgeConstraint.Horizontal, edge.Start);
                    int i = edges.FindIndex(ed => ed == edge);
                    edges[i] = new HorizontalEdge(edge.Start, edge.End, edge.side);
                    //edge = new HorizontalEdge(edge.Start,edge.End,edge.side);
                    CheckChangeEdge(edges[i]);
                    //edge.MakeHorizontal();
                    this.Invalidate();
                }
            };

            if (!edge.CanSetConstraint(EdgeConstraint.Horizontal, edges))
            {
                horizontalConstraintItem.Enabled = false;
            }
            else
            {
                horizontalConstraintItem.Enabled = true;
            }

            ToolStripMenuItem verticalConstraintItem = new ToolStripMenuItem("Set Vertical");
            verticalConstraintItem.Click += (s, e) =>
            {
                if (edge.CanSetConstraint(EdgeConstraint.Vertical, edges))
                {
                    //edge.ChangeEdgeType(edge, EdgeConstraint.Vertical,edge.Start);
                    int i = edges.FindIndex(ed => ed == edge);
                    edges[i] = new VerticalEdge(edge.Start, edge.End, edge.side);
                    //edge = new VerticalEdge(edge.Start,edge.End,edge.side);
                    //edge.MakeVertical();
                    CheckChangeEdge(edges[i]);
                    this.Invalidate();
                }
                    
            };
            if(!edge.CanSetConstraint(EdgeConstraint.Vertical, edges))
            {
                verticalConstraintItem.Enabled = false;
            }
            else
            {
                verticalConstraintItem.Enabled = true;
            }

            ToolStripMenuItem fixedLengthItem = new ToolStripMenuItem("Set Fixed Length");
            fixedLengthItem.Click += (s, e) =>
            {
                if (edge.CanSetConstraint(EdgeConstraint.FixedLength, edges))
                {
                    using (var lengthDialog = new LengthInputDialog(edge.GetLength()))
                    {
                        if (lengthDialog.ShowDialog() == DialogResult.OK)
                        {
                            float newLength = lengthDialog.NewLength;
                            //edge = new FixedLengthEdge(edge.Start,edge.End,edge.side,newLength);
                            int i = edges.FindIndex(ed => ed == edge);

                            edges[i] = new FixedLengthEdge(edge.Start, edge.End, edge.side, newLength);
                            CheckChangeEdge(edges[i]);

                            this.Invalidate();  // Od�wie� widok
                        }
                    }
                }
                    //this.Invalidate();
            };

            ToolStripMenuItem BezierItem = new ToolStripMenuItem("Set Bezier");
            BezierItem.Click += (s, e) =>
            {
                if (edge.CanSetConstraint(EdgeConstraint.Bezier, edges))
                {
                    int i = edges.FindIndex(ed => ed == edge);
                    //PointF p = new PointF(0, 0);
                    //PointF pp = new PointF(0, 100);
                    //Vertex add1 = new Vertex(p);
                    //Vertex add2 = new Vertex(pp);

                    (Vertex add1, Vertex add2) = BezierSupportCalculatorC1(edge.Start, edge.End, edge);
                    bezierVertices.Add(add1);
                    bezierVertices.Add(add2);
                    edges[i] = new BezierEdge(edge.Start, edge.End,add1,add2, edge.Start); // zrobi� logike ustawiania tych pkt�w
                    //bezierVertices[bezierVertices.FindIndex(a => a == add1)], bezierVertices[bezierVertices.FindIndex(a => a == add2)]


                    //edges[i] = new BezierEdge(edge.Start, edge.End, edge.Start, edges);
                    //bezierVertices.Add(((BezierEdge)edges[i]).ControlPoint1);
                    //bezierVertices.Add(((BezierEdge)edges[i]).ControlPoint2);
                    this.Invalidate();
                }
                //this.Invalidate();
            };

            // Opcja usuni�cia ograniczenia
            ToolStripMenuItem removeConstraintItem = new ToolStripMenuItem("Remove Constraint");
            removeConstraintItem.Click += (s, e) =>
            {
                //edge.RemoveConstraint();
                int i = edges.FindIndex(ed => ed == edge);
                if(edge.Constraint == EdgeConstraint.Bezier)
                {
                    bezierVertices.Remove(((BezierEdge)edge).ControlPoint1);
                    bezierVertices.Remove(((BezierEdge)edge).ControlPoint2);
                }
                if(edge.GetPreviousEdge(edges).Constraint != EdgeConstraint.Bezier)
                {
                    edge.Start.constraint = VertexConstraint.None;
                }
                if (edge.GetNextEdge(edges).Constraint != EdgeConstraint.Bezier)
                {
                    edge.End.constraint = VertexConstraint.None;
                }
                edges[i] = new Edge(edge.Start, edge.End, edge.side, EdgeConstraint.None);
                this.Invalidate();
            };

            ToolStripMenuItem addWU = new ToolStripMenuItem("WU");
            addWU.Click += (s, e) =>
            {
                edge.is_using_WU = !edge.is_using_WU;
                this.Invalidate();
            };

            // Dodanie opcji do menu kontekstowego
            menu.Items.Add(addVertexItem);
            menu.Items.Add(new ToolStripSeparator()); // Separator

            // Dodanie opcji ogranicze�
            menu.Items.Add(horizontalConstraintItem);
            menu.Items.Add(verticalConstraintItem);
            menu.Items.Add(fixedLengthItem);
            menu.Items.Add(BezierItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(removeConstraintItem);
            menu.Items.Add(addWU);

            // Wy�wietlenie menu kontekstowego
            menu.Show(this, mousePosition);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) // dla przesuwania
        {
            if (e.Button == MouseButtons.Left)
            {
                // wy��czenie Bresenhama na czas przesuwania
                wasUsingBresenham = UseBresenham;
                UseBresenham = false;
                // Sprawdzamy, czy klikni�to wierzcho�ek, aby rozpocz�� przeci�ganie
                foreach (var vertex in vertices)
                {
                    if (vertex.IsMouseHover(e.Location))
                    {
                        draggedVertex = vertex;
                        isDragging = true;
                        lastMousePosition = e.Location;
                        initialVertexPosition = draggedVertex.Position;
                        return;
                    }
                }
                foreach (var vertex in bezierVertices)
                {
                    if (vertex.IsMouseHover(e.Location))
                    {
                        draggedBezierVertex = vertex;
                        isDragging = true;
                        lastMousePosition = e.Location;
                        return;
                    }
                }
                if (e.Button == MouseButtons.Left && IsMouseInsidePolygon(e.Location))
                {
                    isPolygonDragging = true;
                    lastMousePosition = e.Location;
                    return;
                }
                UseBresenham = wasUsingBresenham;
                
            }
            polygonStateLast = new PolygonState(vertices, edges);
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e) // dla przesuwania
        {
            if (isDrawing)
            {
                return;
            }

            // Przesuwanie Bezier ControPoint
            if(isDragging && draggedBezierVertex != null)
            {
                PointF currentMousePosition = e.Location;
                initialVertexPosition = draggedBezierVertex.Position;

                HashSet<Vertex> dont_fix = new HashSet<Vertex>();

                PointF mouseDelta = new PointF(currentMousePosition.X - initialVertexPosition.X, currentMousePosition.Y - initialVertexPosition.Y);
                draggedBezierVertex.Position = new PointF(draggedBezierVertex.Position.X + mouseDelta.X, draggedBezierVertex.Position.Y + mouseDelta.Y);


                Vertex[] ghg = new Vertex[4];
                // 0 - workingEdge friend of dragging
                // 1 - workingEdge not friend of dragging
                // 2 - nextEdge next
                // 3 - nextEdge nextEdge next?
                Edge[] ede = new Edge[2];
                Edge workingEdge=edges[0];
                Edge nextEdge= edges[0]; // => potem si� to zmienia (ale musi by� set na czym�
                // Szukanie przyjaci� przesuwanego punktu
                foreach (var edge in edges)
                {
                    if(edge.Constraint == EdgeConstraint.Bezier)
                    {
                        if(((BezierEdge)edge).ControlPoint1 == draggedBezierVertex || ((BezierEdge)edge).ControlPoint2 == draggedBezierVertex)
                        {
                            workingEdge = edge;
                            ghg[0] = ((BezierEdge)edge).ControlPoint1 == draggedBezierVertex ? edge.Start : edge.End;
                            ghg[1] = ((BezierEdge)edge).ControlPoint2 == draggedBezierVertex ? edge.End : edge.Start;
                            nextEdge = ghg[0] == edge.Start?workingEdge.GetPreviousEdge(edges):workingEdge.GetNextEdge(edges);
                            ghg[2] = ghg[1] == nextEdge.Start?nextEdge.End:nextEdge.Start;
                            ede[0] = ghg[2] == nextEdge.Start ? nextEdge.GetPreviousEdge(edges) : nextEdge.GetNextEdge(edges);
                            ghg[3] = ghg[2] == ede[0].Start? ede[0].End: ede[0].Start;
                        }
                    }
                }

                HashSet<Vertex> ver = new HashSet<Vertex>();
                ver.Add(ghg[0]);// aby rekurencja na pewno ju� ie poprawia�a tego wierzcho�ka
                dont_fix.Add(ghg[0]); // nie poprawiamy tego wierzcho�ka przy poprawce
                (nextEdge).MoveForFriendBezier(ghg[0], (BezierEdge)workingEdge, mouseDelta);
                draggingFriendPrev = ghg[0];
                draggingFriendNext = ghg[1];
                UpdateEdgesForVertex(ghg[2], mouseDelta,ver); // poprawienie ca�ej reszty wielok�ta

                // Poprawienie Bezier�w
                foreach (Edge edge in new List<Edge>(edges))
                {
                    if (edge.Constraint == EdgeConstraint.Bezier)
                    {
                        int j = edges.IndexOf(edge);
                        if (!dont_fix.Contains(edge.Start))
                        {
                            ((BezierEdge)edges[j]).Update(edge.Start, edges);
                        }
                        if (!dont_fix.Contains(edge.End))
                        {
                            ((BezierEdge)edges[j]).Update(edge.End, edges);
                        }
                    }
                }
                this.Invalidate();
                return;
            }

            // Przesuwanie 'zwyk�ego' wierzcho�ka
            if (isDragging && draggedVertex != null)
            {
                draggedVertex.SetPosition(e.X, e.Y);
                PointF currentMousePosition = e.Location;
                initialVertexPosition = draggedVertex.Position;

                PointF mouseDelta = new PointF(currentMousePosition.X - initialVertexPosition.X, currentMousePosition.Y - initialVertexPosition.Y);

                Vertex[] ghg = new Vertex[2];
                Edge[] ede = new Edge[2];
                int i = 0;
                foreach (var edge in edges)
                {
                    if(edge.Start == draggedVertex ||  edge.End == draggedVertex)
                    {
                        ghg[i]= (edge.Start == draggedVertex) ? edge.End : edge.Start;
                        ede[i] = edge;
                        i++;
                    }
                    edge.Update(draggedVertex); // ruszamy przystaj�cymi kraw�dziami
                }
                // rekurencja w 1 strone
                draggingFriendPrev = ghg[0];
                draggingFriendNext = ghg[1];
                UpdateEdgesForVertex(draggedVertex, mouseDelta);

                // rekurencja w 2 strone
                draggingFriendPrev = ghg[1];
                draggingFriendNext = ghg[0];
                UpdateEdgesForVertex(draggedVertex, mouseDelta);

                // je�eli po rekurencji nadal jest �le
                if (!all_corect(edges) || IsVertexDivergingFromMouse(draggedVertex, currentMousePosition, 0))
                {
                    PointF mouseDelta2 = new PointF(currentMousePosition.X - draggedVertex.Position.X, currentMousePosition.Y - draggedVertex.Position.Y);
                    MoveEntireShape(mouseDelta2);
                }
                // Poprawienie Bezier�w
                foreach (Edge edge in new List<Edge>(edges))
                {
                    if (edge.Constraint == EdgeConstraint.Bezier)
                    {
                        int j = edges.IndexOf(edge);
                        ((BezierEdge)edges[j]).Update(edge.Start, edges);
                        ((BezierEdge)edges[j]).Update(edge.End, edges);
                    }
                }
                this.Invalidate();
            }
            // Przesuwanie ca�o�ci
            if (isPolygonDragging && !isDrawing)
            {
                float deltaX = e.X - lastMousePosition.X;
                float deltaY = e.Y - lastMousePosition.Y;
                MoveEntireShape(new PointF(deltaX,deltaY));

                lastMousePosition = e.Location;
                this.Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UseBresenham = wasUsingBresenham;
                polygonStateLast = new PolygonState(vertices, edges);
                // Zako�cz przeci�ganie
                isDragging = false;
                draggedVertex = null;
                draggedBezierVertex = null;
                isPolygonDragging = false;
                this.Invalidate();


            }
        }
        private void UpdateEdgesForVertex(Vertex vertex, PointF mouseDelta, HashSet<Vertex> updatedVertex = null)
        {

            if (updatedVertex== null)
            {
                updatedVertex = new HashSet<Vertex>();
            }
            // Przejd� przez wszystkie kraw�dzie zwi�zane z tym wierzcho�kiem
            foreach (var edge in edges)
            {
                // pomijamy zwyk�e i beziery -> nie wymagaj� aktualizacji tu
                if (edge.Constraint == EdgeConstraint.None)
                {
                    continue;
                }
                if (edge.Constraint == EdgeConstraint.Bezier)
                {
                    //((BezierEdge)edge).Update(vertex, edges);
                    continue;

                }
                if (edge.Start == vertex || edge.End == vertex)
                {
                    // Warunek kr�cenia rekurencji tylko w 1 strone na raz
                    if((edge.Start == draggingFriendPrev || edge.End == draggingFriendPrev) && vertex == draggedVertex)
                    {
                        continue;
                    }
                    Vertex otherVertex = (edge.Start == vertex) ? edge.End : edge.Start;

                    // Je�li drugi wierzcho�ek nie by� jeszcze zaktualizowany, aktualizujemy go
                    if (!updatedVertex.Contains(otherVertex))
                    {
                        edge.Update(vertex);
                        updatedVertex.Add(vertex);
                        UpdateEdgesForVertex(otherVertex, mouseDelta, updatedVertex);
                    }
                    // Ostatnia aktualizacja je�eli przejdziemy ju� przez wszystko
                    if(otherVertex == draggedVertex && vertex == draggingFriendPrev )
                    {
                        edge.Update(vertex);   
                    }
                }
            }
        }

        private bool IsVertexDivergingFromMouse(Vertex vertex, PointF mousePosition, float tolerance = 5f)
        {
            float deltaX = Math.Abs(vertex.Position.X - mousePosition.X);
            float deltaY = Math.Abs(vertex.Position.Y - mousePosition.Y);
            return deltaX > tolerance || deltaY > tolerance;
        }

        private void MoveEntireShape(PointF mouseDelta)
        {
            foreach (var vertex in vertices)
            {
                vertex.Position = new PointF(vertex.Position.X + mouseDelta.X, vertex.Position.Y + mouseDelta.Y);
            }
            foreach (var vertex in bezierVertices)
            {
                vertex.Position = new PointF(vertex.Position.X + mouseDelta.X, vertex.Position.Y + mouseDelta.Y);
            }
            
        }

        private bool all_corect(List<Edge> edges) // sprawdza poprawno�� ogranicze� wszystkich kraw�dzi
        {
            foreach (var e in edges)
            {
                if (e.is_ok()==false)
                {
                    return false;
                }
            }
            return true;
        }

        // Pocz�tkowe ustawieie beziara na C1
        private (Vertex support1, Vertex support2) BezierSupportCalculatorC1(Vertex Start, Vertex End, Edge workingEdge) 
        {
            Edge startEdge = workingEdge.GetPreviousEdge(edges); // kraw�d� wychodz�ca ze Start
            Edge endEdge = workingEdge.GetNextEdge(edges); // kraw�d� wychodz�ca z End
            Vertex startVertex = startEdge.Start==Start?startEdge.End:startEdge.Start;
            Vertex endVertex = endEdge.End==End?endEdge.Start:endEdge.End;

            if(startEdge.Constraint==EdgeConstraint.Bezier)
            {
                startVertex = ((BezierEdge)startEdge).ControlPoint2;
            }

            if (endEdge.Constraint == EdgeConstraint.Bezier)
            {
                endVertex = ((BezierEdge)endEdge).ControlPoint1;
            }


            float delta_x = Start.Position.X -startVertex.Position.X;
            float delta_y = Start.Position.Y -startVertex.Position.Y;
            PointF add1 = new PointF(Start.Position.X+(delta_x/3),Start.Position.Y + (delta_y/3));
            if (startEdge.Constraint == EdgeConstraint.Bezier)
            {
                add1 = new PointF(Start.Position.X + (delta_x), Start.Position.Y + (delta_y));
            }


            delta_x = End.Position.X - endVertex.Position.X;
            delta_y = End.Position.Y - endVertex.Position.Y;
            PointF add2 = new PointF(End.Position.X + (delta_x / 3), End.Position.Y + (delta_y / 3));

            if (endEdge.Constraint == EdgeConstraint.Bezier)
            {
                add2 = new PointF(End.Position.X + (delta_x), End.Position.Y + (delta_y));
            }
            Vertex sup1 = new Vertex(add1);
            Vertex sup2 = new Vertex(add2);

            return (sup1, sup2);
        }

        private void ShowVertexContextMenu(Vertex vertex, Point mousePosition)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem del = new ToolStripMenuItem("Delete Vertex", null, OnDeleteVertex);
            del.Enabled = true;
            if (bezierVertices.Contains(vertex))
            {
                del.Enabled = false;
            }
            menu.Items.Add(del);

            // Opcja zmiany ci�g�o�ci
            ToolStripMenuItem continuityMenu = new ToolStripMenuItem("Change Continuity");

            if (vertex.constraint == VertexConstraint.None)
            {
                continuityMenu.Enabled = false; // Blokujemy opcj� zmiany ci�g�o�ci
            }
            else
            {
                continuityMenu.DropDownItems.Add("G0", null, (s, e) => ChangeContinuity(vertex, "G0"));
                continuityMenu.DropDownItems.Add("G1", null, (s, e) => ChangeContinuity(vertex, "G1"));
                continuityMenu.DropDownItems.Add("C1", null, (s, e) => ChangeContinuity(vertex, "C1"));
            }

            menu.Items.Add(new ToolStripSeparator()); // Separator
            menu.Items.Add(continuityMenu);

            menu.Show(this, mousePosition);
        }

        private void ChangeContinuity(Vertex vertex, string continuityType)
        {
            switch (continuityType)
            {
                case "G0":
                    vertex.constraint = VertexConstraint.G0;
                    break;
                case "G1":
                    vertex.constraint = VertexConstraint.G1;
                    break;
                case "C1":
                    vertex.constraint = VertexConstraint.C1;
                    break;
            }

            this.Invalidate(); 
        }

        // menu g�rne
        private void InitializeMenuBar()
        {
            // Tworzenie g��wnego MenuStrip
            MenuStrip menuStrip = new MenuStrip();

            // 1. Menu Help
            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("Help");
            helpMenuItem.Click += (s, e) =>
            {
                Thread helpThread = new Thread(() =>
                {
                    HelpForm helpForm = new HelpForm();
                    Application.Run(helpForm);
                });
                helpThread.Start();
            };

            // 2. Menu Menu (zawiera opcje Delete Polygon i Start Drawing)
            ToolStripMenuItem menuMenuItem = new ToolStripMenuItem("Menu");

            ToolStripMenuItem deletePolygonItem = new ToolStripMenuItem("Delete Polygon");
            deletePolygonItem.Click += (s, e) =>
            {
                DeletePolygon(); 
                UpdateMenuItems(); 
                this.Invalidate(); 
            };

            ToolStripMenuItem startDrawingItem = new ToolStripMenuItem("Start Drawing");
            startDrawingItem.Click += (s, e) =>
            {
                StartDrawingMode(); 
                UpdateMenuItems(); 
            };
            menuMenuItem.DropDownItems.Add(deletePolygonItem);
            menuMenuItem.DropDownItems.Add(startDrawingItem);

            // Chwilowo nie u�ywane
            // 3. Menu Line (zawiera opcje Line1 i Line2) 
            ToolStripMenuItem lineMenuItem = new ToolStripMenuItem("Line");

            ToolStripMenuItem line1Item = new ToolStripMenuItem("DrawLine");
            line1Item.Click += (s, e) =>
            {
                UseBresenham = false;
                wasUsingBresenham = false;
                UpdateMenuItems();
                this.Invalidate();
            };

            ToolStripMenuItem line2Item = new ToolStripMenuItem("DrawBresenham");
            line2Item.Click += (s, e) =>
            {
                UseBresenham = true;
                wasUsingBresenham = true;
                UpdateMenuItems();
                this.Invalidate();
            };

            // Dodanie opcji do rozwijanego menu "Line"
            //lineMenuItem.DropDownItems.Add(line1Item);
            //lineMenuItem.DropDownItems.Add(line2Item);

            // Dodanie wszystkich g��wnych element�w do MenuStrip
            menuStrip.Items.Add(helpMenuItem);
            menuStrip.Items.Add(menuMenuItem);
            //menuStrip.Items.Add(lineMenuItem);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
            UpdateMenuItems();
        }

        private void DeletePolygon()
        {
            vertices.Clear();
            edges.Clear();
            bezierVertices.Clear();
        }

        private void StartDrawingMode()
        {
            isDrawing = true;
            isClosed = false;
        }

        private void UpdateMenuItems()
        {
            // Znajd� odpowiednie elementy menu
            ToolStripMenuItem deletePolygonItem = FindMenuItem("Delete Polygon");
            ToolStripMenuItem startDrawingItem = FindMenuItem("Start Drawing");

            // Sprawd�, czy jest co� narysowane
            bool isPolygonDrawn = vertices.Count > 0;

            deletePolygonItem.Enabled = !isDrawing&& isPolygonDrawn;

            startDrawingItem.Enabled = !isPolygonDrawn && !isDrawing;

            //ToolStripMenuItem drawline1 = FindMenuItem("DrawLine");
            //ToolStripMenuItem drawline2 = FindMenuItem("DrawBresenham");
            //drawline1.Enabled = UseBresenham;
            //drawline2.Enabled = !UseBresenham;

        }

        // Funkcja pomocnicza do znalezienia elementu menu po nazwie
        private ToolStripMenuItem FindMenuItem(string name)
        {
            foreach (ToolStripMenuItem menuItem in this.MainMenuStrip.Items)
            {
                if (menuItem.Text == name)
                {
                    return menuItem;
                }
                foreach (ToolStripMenuItem subItem in menuItem.DropDownItems)
                {
                    if (subItem.Text == name)
                    {
                        return subItem;
                    }
                }
            }
            return null;
        }

        private bool IsMouseInsidePolygon(Point mousePosition)
        {
            // Algorytm ray-casting
            int intersections = 0;

            for (int i = 0; i < vertices.Count; i++)
            {
                // Pobieramy dwa kolejne wierzcho�ki tworz�ce kraw�d�
                PointF v1 = vertices[i].Position;
                PointF v2 = vertices[(i + 1) % vertices.Count].Position;

                // Sprawdzamy, czy promie� od punktu przecina kraw�d� wielok�ta
                if (IsEdgeIntersectingWithRay(mousePosition, v1, v2))
                {
                    intersections++;
                }
            }

            // Punkt jest wewn�trz, je�li liczba przeci�� jest nieparzysta
            return (intersections % 2) != 0;
        }

        // Sprawdza, czy promie� od punktu przecina kraw�d� wielok�ta
        private bool IsEdgeIntersectingWithRay(Point p, PointF v1, PointF v2)
        {
            // Sprawdzamy, czy kraw�d� przechodzi nad/pod poziomem punktu
            if ((v1.Y > p.Y && v2.Y > p.Y) || (v1.Y < p.Y && v2.Y < p.Y))
                return false;

            // Sprawdzamy, czy punkt le�y na prawo od kraw�dzi
            if (p.X > Math.Max(v1.X, v2.X))
                return false;

            // Je�li punkt jest na lewo od kraw�dzi, sprawdzamy po�o�enie wzgl�dem promienia
            if (p.X < Math.Min(v1.X, v2.X))
                return true;

            // Obliczamy punkt przeci�cia na osi X dla wysoko�ci punktu p.Y
            float intersectionX = v1.X + (p.Y - v1.Y) * (v2.X - v1.X) / (v2.Y - v1.Y);

            return p.X < intersectionX;
        }
    
        
        private void CheckChangeEdge(Edge edge) 
        {
            // pr�ba aktualizacji
            UpdateEdgesForVertex(edge.Start, new PointF(0,0));
            UpdateEdgesForVertex(edge.End, new PointF(0, 0));

            if (!all_corect(edges))
            {
                DialogResult result = MessageBox.Show(
                    "Ta zmiana mo�e powodowa� b��dy przy zachowywaniu ogranicze�. \n Czy chcesz wymusi� zmian� pomimo to?",
                    "Error",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error
                );

                // Wymuszenie aktualizacji
                if (result == DialogResult.Yes)
                {
                    return;
                }
                vertices = polygonStateLast.Vertices.Select(v => new Vertex(v.Position)).ToList();
                // Tworzymy kopi� kraw�dzi
                edges = polygonStateLast.Edges.Select(e =>
                    new Edge(
                    vertices[polygonStateLast.Vertices.IndexOf(e.Start)], 
                    vertices[polygonStateLast.Vertices.IndexOf(e.End)], vertices[polygonStateLast.Vertices.IndexOf(e.Start)],
                    e.Constraint,
                    e.FixedLength)
                        ).ToList();
                
                for(int i=0; i<edges.Count; i++)
                {
                    edges[i] = edges[i].ChangeEdgeType(edges[i], edges[i].Constraint, edges[i].Start, edges[i].FixedLength);
                }
            }
        }

        private void InitializePolygon() // Pocz�tkowy wielok�t
        {
            // Tworzenie wierzcho�k�w (przyk�adowe punkty)
            vertices = new List<Vertex>
            {
                new Vertex(new PointF(150, 150)), // Punkt 1
                new Vertex(new PointF(250, 150)), // Punkt 2
                new Vertex(new PointF(300, 250)), // Punkt 3
                new Vertex(new PointF(200, 350)), // Punkt 4
                new Vertex(new PointF(100, 250)),  // Punkt 5
                new Vertex(new PointF(150, 200))   // Punkt 6
            };

            edges = new List<Edge>
            {
                new HorizontalEdge(vertices[0], vertices[1], vertices[0]), // Kraw�d� mi�dzy punktami 1 i 2
                new Edge(vertices[1], vertices[2], vertices[1], EdgeConstraint.None), // Kraw�d� mi�dzy punktami 2 i 3
                new Edge(vertices[2], vertices[3], vertices[2], EdgeConstraint.None), // Kraw�d� mi�dzy punktami 3 i 4
                new Edge(vertices[3], vertices[4], vertices[3], EdgeConstraint.None), // Kraw�d� mi�dzy punktami 4 i 5
                new Edge(vertices[4], vertices[5], vertices[4], EdgeConstraint.None), // Kraw�d� mi�dzy punktami 5 i 6
                new VerticalEdge(vertices[5], vertices[0], vertices[5])  // Kraw�d� mi�dzy punktami 6 i 1 (zamkni�cie)
            };
            edges[4] = new FixedLengthEdge(edges[4].Start, edges[4].End, edges[4].side, (float)Math.Round(edges[4].GetLength(), 2));

            (Vertex add1, Vertex add2) = BezierSupportCalculatorC1(edges[2].Start, edges[2].End, edges[2]);
            bezierVertices.Add(add1);
            bezierVertices.Add(add2);
            edges[2] = new BezierEdge(edges[2].Start, edges[2].End, add1, add2, edges[2].Start);

            isClosed = true;
            isDrawing = false;
            this.Invalidate(); 
        }



        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (bresenhamRadioButton.Checked)
            {
                UseBresenham = true;
                wasUsingBresenham = true;
                bresenhamRadioButton.ForeColor = Color.Green;
                otherOptionRadioButton.ForeColor = Color.Red;
            }
            else if (otherOptionRadioButton.Checked)
            {
                UseBresenham = false;
                wasUsingBresenham = false;
                bresenhamRadioButton.ForeColor = Color.Red;
                otherOptionRadioButton.ForeColor = Color.Green;
            }
            this.Invalidate();
        }
    }
}
