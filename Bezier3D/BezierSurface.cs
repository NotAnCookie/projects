using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

namespace Bezier3D
{
    public class BezierSurface
    {
        private ControlPoint[,] controlPoints;
        private List<Triangle> triangles; // zrobić obliczanie tylko przy zmianie rozdzielczości
        private List<Vertex> vertices;

        private int resolution;
        private float our_alpha;
        private float our_beta;

        private bool is_light_on = true;
        private bool is_reflectors_on = false;
        private int mL = 1;
        private Vector3[] tab = new Vector3[3];
        private Vector3[] colors = new Vector3[3];


        private float kd = 0.8f;//0.8f; // Współczynnik rozproszenia
        private float ks = 0.5f;//0.5f; // Współczynnik zwierciadlany
        private int m = 50; // Współczynnik "szklistości"
        private Vector3 IL = new Vector3(1f, 1f, 1f); // Kolor światła (biały)
        private Vector3 IO = new Vector3(1f, 0.5f, 0.3f); // Kolor obiektu
        private static readonly Vector3 LightPosition = new Vector3(1f, 1f, 280f);

        private static readonly Vector3 V = Vector3.Normalize(new Vector3(0, 0, 1)); // Wektor obserwacji


        private Bitmap normalMap;
        public bool is_drawing_texture {get;set;}

        private Bitmap textureMap;
        public bool is_drawing_object_texture { get;set;}


        public BezierSurface(ControlPoint[,] controlPoints, int resolution = 10, int our_beta = 0,int our_alpha = 0)
        {
            this.controlPoints = controlPoints;
            this.resolution = resolution;
            this.vertices = new List<Vertex>();
            this.triangles = new List<Triangle>();
            this.our_beta = our_beta;
            this.our_alpha = our_alpha;


            tab[0] = new Vector3(-400, -400, 300);
            tab[1] = new Vector3(-400, 400, 300);
            tab[2] = new Vector3(400, 400, 300);
            colors[0] = new Vector3(1, 0, 0);
            colors[1] = new Vector3(0, 1, 0);
            colors[2] = new Vector3(0, 0, 1);


            GenerateMesh();

            CalculateNormalsForVertices();
        }

        public void SetNormalMap(Bitmap normalMap)
        {
            this.normalMap = normalMap;
        }

        public void UpdateVariables(float kd, float ks, int m, Vector3 IL, Vector3 IO)
        {
            this.kd = kd;
            this.ks = ks;
            this.m = m;
            this.IL = IL;
            this.IO = IO;

        }

        public void UpdateLightPosition(Vector3 L)
        {
            foreach (var t in triangles)
            {
                t.UpdateLightPosition(L);
            }
            tab[0].Z = L.Z;
            tab[1].Z = L.Z;
            tab[2].Z = L.Z;
        }



        public void UpdateResolution(int newResolution)
        {
            resolution = newResolution;
            CalculateNormalsForVertices();
            //GenerateTriangles();
            GenerateMesh();
            CalculateNormalsForVertices();
        }
        public void UpdateRotation(float beta,float alpha)
        {
            our_beta = beta;
            our_alpha = alpha;
            ApplyRotation(beta, alpha);
        }



        private void GenerateMesh()
        {
            vertices.Clear();
            triangles.Clear();

            for (int i = 0; i < resolution; i++)
            {
                float u1 = (float)i / resolution;
                float u2 = (float)(i + 1) / resolution;

                for (int j = 0; j < resolution; j++)
                {
                    float v1 = (float)j / resolution;
                    float v2 = (float)(j + 1) / resolution;

                    // Oblicz punkty dla czterech wierzchołków siatki
                    Vertex p1 = CreateVertex(u1, v1);
                    Vertex p2 = CreateVertex(u2, v1);
                    Vertex p3 = CreateVertex(u1, v2);
                    Vertex p4 = CreateVertex(u2, v2);

                    // Dodaj wierzchołki do listy lub pobierz ich indeks
                    int i1 = AddVertex(p1);
                    int i2 = AddVertex(p2);
                    int i3 = AddVertex(p3);
                    int i4 = AddVertex(p4);

                    //p1.Normal = CalculateNormal(p1.u, p1.v);
                    //p2.Normal = CalculateNormal(p2.u, p2.v);
                    //p3.Normal = CalculateNormal(p3.u, p3.v);
                    //p4.Normal = CalculateNormal(p4.u, p4.v);

                    // Dodanie trójkątów do listy z odwołaniem do indeksów
                    triangles.Add(new Triangle(vertices[i1], vertices[i2], vertices[i3]));
                    triangles.Add(new Triangle(vertices[i2], vertices[i4], vertices[i3]));
                }
            }

            // liczenie wektorów normalnych dla wierzchołków:
            CalculateNormalsForVertices();
            // zapisanie początkowych normalnych
            foreach(var ver in vertices)
            {
                ver.SetNormal(ver.Normal,true);
            }

            
        }

        private Vertex CreateVertex(float u, float v)
        {
            Vector3 position = CalculateBezierPoint(u, v);
            Vertex vertex = new Vertex(position)
            {
                u = u,
                v = v
            };
            return vertex;
        }

        private int AddVertex(Vertex vertex)
        {
            int index = vertices.FindIndex(v => v.Position == vertex.Position);
            if (index == -1)
            {
                vertices.Add(vertex);
                return vertices.Count - 1;
            }
            return index;
        }

        public void ApplyRotation(float alpha, float beta)
        {
            float alphaRadians = alpha * (MathF.PI / 180f);
            float betaRadians = beta * (MathF.PI / 180f);

            Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ(alphaRadians);
            Matrix4x4 rotationX = Matrix4x4.CreateRotationX(betaRadians);
            Matrix4x4 rotationMatrix = rotationZ * rotationX;

            foreach (var vertex in vertices)
            {
                vertex.Position = Vector3.Transform(vertex.OrginalPosition, rotationMatrix);

                vertex.Normal = Vector3.TransformNormal(vertex.OrginalNormal, rotationMatrix);

                vertex.Pu = Vector3.Transform(vertex.OrginalPu, rotationMatrix);
                vertex.Pv = Vector3.Transform(vertex.OrginalPv, rotationMatrix);
            }
        }

        private void CalculateNormalsForVertices()
        {
            foreach (var vertex in vertices)
            {
                vertex.Normal = CalculateNormal(vertex.u,vertex.v, vertex);
            }
        }

        private Vector3 CalculateNormal(float u, float v, Vertex ver)
        {
            Vector3 Pu = CalculatePU(u, v);
            Vector3 Pv = CalculatePV(u, v);
            //ver.Pu = Pu;
            //ver.Pv = Pv;
            ver.SetPuv(Pu, Pv,true);

            return Vector3.Normalize(Vector3.Cross(Pu, Pv));
        }

        private Vector3 CalculatePU(float u, float v)
        {
            int n = controlPoints.GetLength(0)-1;
            int m = controlPoints.GetLength(1)-1;
            Vector3 Pu = Vector3.Zero;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= m; j++)
                {
                    Vector3 delta = controlPoints[i + 1, j].Position - controlPoints[i, j].Position;
                    float Bu = Bernstein(i, n - 1, u);
                    float Bv = Bernstein(j, m, v);
                    Pu += delta * Bu * Bv;
                }
            }
            return n * Pu;
        }

        private Vector3 CalculatePV(float u, float v)
        {
            int n = controlPoints.GetLength(0) - 1;
            int m = controlPoints.GetLength(1) - 1;
            Vector3 Pv = Vector3.Zero;

            for (int i = 0; i <= n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Vector3 delta = controlPoints[i, j + 1].Position - controlPoints[i, j].Position;
                    float Bu = Bernstein(i, n, u);
                    float Bv = Bernstein(j, m - 1, v);
                    Pv += delta * Bu * Bv;
                }
            }
            return m * Pv;
        }

        private Vector3 CalculateBezierPoint(float u, float v)
        {
            Vector3 point = Vector3.Zero;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float bu = Bernstein(i, 3, u);
                    float bv = Bernstein(j, 3, v);
                    point += controlPoints[i, j].Position * (bu * bv);
                }
            }
            return point;
        }

        private float Bernstein(int i, int n, float t) // Bi,n(t)
        {
            return (float)(Factorial(n) / (Factorial(i) * Factorial(n - i))) * MathF.Pow(t, i) * MathF.Pow(1 - t, n - i);
        }

        private int Factorial(int n)
        {
            return n <= 1 ? 1 : n * Factorial(n - 1);
        }


        public void light_reflectors(bool on_light, bool on_reflectors, int mL)//
        {
            is_light_on = on_light;
            is_reflectors_on = on_reflectors;
            this.mL = mL;
        }


        public void DrawFrame(Graphics g, Pen pen, int width, int height, bool is_frame, DirectBitmap drawingBitmap, bool is_textureN,bool is_texture)
        {
            // Przesunięcie o połowę szerokości i wysokości, aby wyrównać do środka
            int offsetX = width / 2;
            int offsetY = height / 2;

            is_drawing_texture = is_textureN;// tekstura wektorów normalnych
            is_drawing_object_texture = is_texture;

                ApplyRotation(our_alpha, our_beta);
                if (is_frame)
                {
                    //drawingBitmap.Clear(Color.White);
                    foreach (var triangle in triangles)
                    {
                        triangle.Draw(g, offsetX, offsetY);
                    }
                }
                else
                {
                    Vector3 lightPosition = new Vector3(0, 0, 300);//inicjalizacja 
                    drawingBitmap.Clear(Color.White);
                    foreach (var triangle in triangles)
                    {
                        //Vector3 lightDirection = new Vector3(0, 0, 300);

                        lightPosition = triangle.LightPosition;
                        //lightDirection = Vector3.Normalize(lightDirection);
                        Vector3 viewDirection = new Vector3(0, 0, 1);
                        FillTriangle(g, triangle, drawingBitmap, lightPosition, viewDirection);
                    }
                    g.DrawImage(drawingBitmap.Bitmap, 0, 0, width, height);
                }
        }



        public void DrawControlPoints(Graphics g, Brush brush, float alpha, float beta, int width, int height)
        {
            alpha = our_alpha;
            beta = our_beta;

            int offsetX = width / 2;
            int offsetY = height / 2;
            // Przeliczenie kątów na radiany
            float alphaRadians = alpha * (MathF.PI / 180f);
            float betaRadians = beta * (MathF.PI / 180f);

            // Macierze rotacji dla osi Z i X (pełna rotacja 3D)
            Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ(alphaRadians);
            Matrix4x4 rotationX = Matrix4x4.CreateRotationX(betaRadians);

            Matrix4x4 rotationMatrix = rotationZ * rotationX;

            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                for (int j = 0; j < controlPoints.GetLength(1); j++)
                {
                    Vector3 originalPosition = controlPoints[i, j].Position;
                    Vector3 rotatedPosition = Vector3.Transform(originalPosition, rotationMatrix);

                    PointF point2D = new PointF(rotatedPosition.X, rotatedPosition.Y);

                    g.FillEllipse(brush, point2D.X - 3 + offsetX, point2D.Y - 3 + offsetY, 6, 6);
                }
            }
        }


        private void FillTriangle(Graphics g, Triangle triangle, DirectBitmap bitmap, Vector3 lightDirection, Vector3 viewDirection)
        {
            Vertex[] vertex = { triangle.v1, triangle.v2, triangle.v3 };
            Array.Sort(vertex, (v1, v2) => v1.Position.Y.CompareTo(v2.Position.Y));

            Vertex top = vertex[0];
            Vertex middle = vertex[1];
            Vertex bottom = vertex[2];

            float startX = top.Position.X;
            float endX = top.Position.X;

            // przesunięcia
            float delta_y1 = middle.Position.Y - top.Position.Y;
            float delta_y2 = bottom.Position.Y - top.Position.Y;
            float delta_y3 = bottom.Position.Y - middle.Position.Y;

            float dx1 = (delta_y1 == 0) ? 0 : (middle.Position.X - top.Position.X) / delta_y1;
            float dx2 = (delta_y2 == 0) ? 0 : (bottom.Position.X - top.Position.X) / delta_y2;
            float dx3 = (delta_y3 == 0) ? 0 : (bottom.Position.X - middle.Position.X) / delta_y3;

            // góra
            for (float y = top.Position.Y; y <= middle.Position.Y; y++)
            {
                DrawScanLineSorted(bitmap, (int)y, (int)startX, (int)endX, top, middle, bottom, lightDirection, viewDirection);
                startX += dx1;
                endX += dx2;
            }

            // dół
            startX = middle.Position.X;
            for (float y = middle.Position.Y; y <= bottom.Position.Y; y++)
            {
                DrawScanLineSorted(bitmap, (int)y, (int)startX, (int)endX, top, middle, bottom, lightDirection, viewDirection);
                startX += dx3;
                endX += dx2;
            }
            //g.DrawImage(bitmap.Bitmap, 0, 0);
        }

        private void DrawScanLineSorted(DirectBitmap bitmap, int y, int startX, int endX, Vertex v1, Vertex v2, Vertex v3, Vector3 lightDirection, Vector3 viewDirection)
        {

            if (startX > endX)
            {
                int temp = startX;
                startX = endX;
                endX = temp;
            }
            Vector3 lightDirection2 = new Vector3(lightDirection.X, lightDirection.Y, lightDirection.Z); // zapamiętywanie pozycji światła

            for (int x = startX-2; x <= endX+2; x++) // -2 i +2 po to aby nie widać było dziór między trójkątami
            {
                Triangle t = new Triangle(v1, v2, v3);
                Vector3 barycentricCoords = t.GetBarycentric(new Vector2(x, y));

                if (barycentricCoords.X >= 0 && barycentricCoords.Y >= 0 && barycentricCoords.Z >= 0)
                {
                    float interpolatedZ = barycentricCoords.X * v1.Position.Z +
                                      barycentricCoords.Y * v2.Position.Z +
                                      barycentricCoords.Z * v3.Position.Z;
                    Vector3 cord = new Vector3(x,y,interpolatedZ);

                    lightDirection = Vector3.Normalize(lightDirection2 - cord);


                    // texture natural
                    Vector3 Nsurface = Vector3.Normalize(
                        barycentricCoords.X * v1.Normal +
                        barycentricCoords.Y * v2.Normal +
                        barycentricCoords.Z * v3.Normal
                    );

                    Vector3 N;
                    if (is_drawing_texture)
                    {
                        // aby mapa normalna ruszała się z rysunkiem
                        Vector3 originalPoint = ApplyInverseRotation(x, y, our_alpha, our_beta, interpolatedZ);
                        float u = originalPoint.X;
                        float v = originalPoint.Y;
                        (int width, int height) = GetDimControlPoints(controlPoints);
                        Vector3 Ntexture = GetNormalFromMapWithUV(u, v, width, height);

                        // pobranie wektora normalnego dla nie ruszającej się tekstury
                        //Vector3 Ntexture = GetNormalFromMapWithUV(x, y, bitmap.Width, bitmap.Height);

                        Vector3 Pu = Vector3.Normalize(
                            barycentricCoords.X * v1.Pu +
                            barycentricCoords.Y * v2.Pu +
                            barycentricCoords.Z * v3.Pu
                        );

                        Vector3 Pv = Vector3.Normalize(
                            barycentricCoords.X * v1.Pv +
                            barycentricCoords.Y * v2.Pv +
                            barycentricCoords.Z * v3.Pv
                        );

                        Matrix4x4 M = new Matrix4x4(
                            Pu.X, Pv.X, Nsurface.X, 0,
                            Pu.Y, Pv.Y, Nsurface.Y, 0,
                            Pu.Z, Pv.Z, Nsurface.Z, 0,
                            0, 0, 0, 1
                        );


                        N = Vector3.Transform(Ntexture, M);
                        N = Vector3.Normalize(N);
                        N = new Vector3(-N.X, -N.Y, N.Z);

                    }
                    else
                    {
                        N = Nsurface;
                        N = Vector3.Normalize(N);
                    }

                    Vector3 color;
                    if (is_drawing_object_texture) 
                    {
                        // obliczenie współrzędnych na nieprzesuniętej mapie
                        // => obrazek trzyma się naszej powierzchni
                        Vector3 originalPoint = ApplyInverseRotation(x, y, our_alpha, our_beta, interpolatedZ);
                        float u = originalPoint.X;
                        float v = originalPoint.Y;

                        Vector3 pointColor = GetColorFromMapWithUV(u, v);
                        color = CalculateIlluminationColor(lightDirection, viewDirection, barycentricCoords, N,pointColor, cord,lightDirection2.Z);
                    }
                    else
                    {
                        color = CalculateIlluminationColor(lightDirection, viewDirection, barycentricCoords, N,IO, cord,lightDirection2.Z);
                    }


                    Color finalColor = Color.FromArgb((int)color.X, (int)color.Y, (int)color.Z);
                    bitmap.SetPixel(x+bitmap.Width/2, y+bitmap.Height/2, finalColor);

                    // dodatkowe (*)
                    // dodajemy 2 pixel po to aby usunąć białe pola między trójkątami 
                    // jeżeli to nie jest graniczny y to jego kolor zostanie zmieniony na poprawny gdyż poruszamy się w pętli y++
                    bitmap.SetPixel(x + bitmap.Width / 2, y+1 + bitmap.Height / 2, finalColor);
                }
            }
        }

        private static Vector3 ApplyInverseRotation(float x, float y, float alfa, float beta, float z) // rotacja pkt spowrotem do u,v
        {
            float alfaRad = -alfa * (float)(Math.PI / 180);
            float betaRad = -beta * (float)(Math.PI / 180);

            Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ(alfaRad);
            Matrix4x4 rotationX = Matrix4x4.CreateRotationX(betaRad);

            Matrix4x4 inverseRotation = rotationX * rotationZ;

            Vector3 point = new Vector3(x, y, z);
            Vector3 originalPoint = Vector3.Transform(point, inverseRotation);

            return originalPoint;
        }

        private Vector3 CalculateIlluminationColor(Vector3 lightDirection, Vector3 viewDirection, Vector3 barycentricCoords, Vector3 N, Vector3 IO, Vector3 cord, float z)
        {
            if (!is_light_on&&!is_reflectors_on) // wyłączenie światła zupełnie
            {
                Vector3 c = new Vector3(0, 0, 0);
                return c;
            }

            Vector3 diffuse = Vector3.Zero;
            Vector3 specular = Vector3.Zero;
            float cosVR;
            float cosNL;
            Vector3 R;

            if (is_light_on)
            {
                lightDirection = Vector3.Normalize(lightDirection);

                viewDirection = Vector3.Normalize(viewDirection);

                cosNL = Vector3.Dot(N, lightDirection);

                R = 2 * cosNL * N - lightDirection;
                R = Vector3.Normalize(R);
                if (cosNL < 0) cosNL = 0;

                cosVR = Math.Max(Vector3.Dot(viewDirection, R), 0);
                if (cosVR < 0) cosVR = 0;

                //Vector3 diffuse = kd * IL * IO * cosNL;
                diffuse = new Vector3(kd * IL.X * IO.X * cosNL, kd * IL.Y * IO.Y * cosNL, kd * IL.Z * IO.Z * cosNL);
                specular = ks * IL * (float)Math.Pow(cosVR, m);
            }           

            if (is_reflectors_on)
            {

                for(int i=0;i<3; i++)
                {
                    Vector3 newL = tab[i] - cord;
                    newL = Vector3.Normalize(newL);
                    Vector3 LdirR = tab[i] - Vector3.Zero;//świecimy na 0,0 -> nic to nie zmienia
                    LdirR = Vector3.Normalize(LdirR);

                    float cos = Math.Max(Vector3.Dot(newL, LdirR), 0);
                    Vector3 IL_R = colors[i] * (float)Math.Pow(cos, mL);

                    cosNL = Vector3.Dot(N, newL);
                    R = 2 * cosNL * N - newL;
                    R = Vector3.Normalize(R);
                    if (cosNL < 0) cosNL = 0;
                    cosVR = Math.Max(Vector3.Dot(viewDirection, R), 0);
                    if (cosVR < 0) cosVR = 0;
                    Vector3 diffuseR = new Vector3(kd * IL_R.X * IO.X * cosNL, kd * IL_R.Y * IO.Y * cosNL, kd * IL_R.Z * IO.Z * cosNL);
                    Vector3 specularR = ks * IL * (float)Math.Pow(cosVR, m);

                    diffuse += diffuseR;
                    specular += specularR;
                }
            }

            Vector3 color = diffuse + specular;

            color.X = Math.Clamp(color.X * 255, 0, 255);
            color.Y = Math.Clamp(color.Y * 255, 0, 255);
            color.Z = Math.Clamp(color.Z * 255, 0, 255);

            return color;
        }



        // texture

        public void UpdateNormalMap(Bitmap normalMap)
        {
            this.normalMap = normalMap;
        }

        public void UpdateTextureMap(Bitmap textureMap)
        {
            this.textureMap = textureMap;
        }


        private Vector3 GetNormalFromMapWithUV(float u, float v, int width, int height)
        {
            float uNormalized = (u + (width / 2f)) / width;
            float vNormalized = (v + (height / 2f)) / height;

            int x = Math.Clamp((int)(uNormalized * (normalMap.Width - 1)), 0, normalMap.Width - 1);
            int y = Math.Clamp((int)(vNormalized * (normalMap.Height - 1)), 0, normalMap.Height - 1);
            
            Color pixelColor = normalMap.GetPixel(x, y);

            // do przedziału [-1,1]
            float nx = (pixelColor.R * 2) - 255;
            float ny = (pixelColor.G * 2) - 255;
            float nz = (pixelColor.B * 2) - 255;

            if (nz < 0)
            {
                nz = -nz;
            }
            Vector3 normal = new Vector3(nx, ny, nz);
            normal = Vector3.Normalize(normal);

            return normal;
        }

        private Vector3 GetColorFromMapWithUV(float u, float v)//, int width, int height)
        {
            (int width, int height) = GetDimControlPoints(controlPoints);

            //skalowanie
            float uNormalized = (u + (width / 2f)) / width;
            float vNormalized = (v + (height / 2f)) / height;

            int x = Math.Clamp((int)(uNormalized * (textureMap.Width - 1)), 0, textureMap.Width - 1);
            int y = Math.Clamp((int)(vNormalized * (textureMap.Height - 1)), 0, textureMap.Height - 1);

            Color pixelColor = textureMap.GetPixel(x, y);

            float nx = pixelColor.R;
            float ny = pixelColor.G;
            float nz = pixelColor.B;

            Vector3 normal = new Vector3(nx, ny, nz);
            
            // dla koloru białego
            normal = Vector3.Normalize(normal);
            if (nx == 0 && ny == 0 && nz == 0)
            {
                normal = Vector3.Zero;
            }
            return normal;
        }

        private (int width, int height) GetDimControlPoints(ControlPoint[,] controlPoints)// rozmiary rysowanego obrazka
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var point in controlPoints)
            {
                if (point != null)
                {
                    minX = Math.Min(minX, point.Position.X);
                    maxX = Math.Max(maxX, point.Position.X);
                    minY = Math.Min(minY, point.Position.Y);
                    maxY = Math.Max(maxY, point.Position.Y);
                }
            }
            int width = (int)(maxX - minX);
            int height = (int)(maxY - minY);
            return (width, height);
        }

    }
}
