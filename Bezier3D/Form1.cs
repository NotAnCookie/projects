using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Windows.Forms;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Bezier3D
{
    public partial class Form1 : Form
    {
        private MapsPreviewForm mapsPreviewForm;



        private ControlPoint[,] controlPoints = new ControlPoint[4, 4];
        Panel drawPanel; // zrobiæ wiêkszy panel + poszerzyæ siatkê

        //private Bitmap drawingBitmap;
        private DirectBitmap drawingBitmap;
        private Bitmap normalMap; // mapa wektorów normalnych
        private Vector3 normalMapVector;

        private Bitmap textureMap;

        private BezierSurface bezierSurface;
        private float alpha = 0f;
        private float beta = 0f;

        private bool is_drawing_frame = true;
        private bool is_drawing_fill = false;
        private bool is_drawing_textureNat = false; // textura wektorów normalnych
        private bool is_drawing_texture = false; // textura obiektu

        private RadioButton frameRadio;
        private RadioButton fillRadio;
        private CheckBox textureNatRadio;
        private CheckBox textureRadio;

        private CheckBox off_on_light;
        private CheckBox off_on_reflectors;
        private int mL=1;
        private bool is_light_on = true;
        private bool is_reflectors_on = false;

        private MenuStrip MenuItem = new MenuStrip();

        private bool is_moving = false;

        private float kd = 0.8f;//0.8f; // Wspó³czynnik rozproszenia
        private float ks = 0.5f;//0.5f; // Wspó³czynnik zwierciadlany
        private int m = 50; // Wspó³czynnik "szklistoœci"
        private Vector3 IL = new Vector3(1f, 1f, 1f); // Kolor œwiat³a (bia³y)
        private Vector3 IO = new Vector3(1f, 0.5f, 0.3f); // Kolor obiektu
        private int z = 300;


        private Vector3 lightPosition; // Aktualna pozycja œwiat³a
        private float lightRadius = 200f; // Promieñ okrêgu, po którym œwiat³o bêdzie siê poruszaæ
        private float lightHeight = 300f; // Sta³a wysokoœæ (Z) dla œwiat³a
        private float angle = 0f; // Aktualny k¹t pozycji œwiat³a na okrêgu
        private Thread lightMovementThread;
        private System.Windows.Forms.Timer lightMovementTimer;
        private bool isRunning = false; // Flaga do zatrzymywania w¹tku



        public Form1()
        {
            InitializeMenu();
            InitializeComponent();
            InitializeUI();
            UpdateRadioButtonStates();
            InitializeVariables();

            
            //InitializeMenu();
            this.DoubleBuffered = true;
            drawPanel.EnableDoubleBuffering();

            this.Size = new System.Drawing.Size(1200, 950);
            this.MinimumSize = new System.Drawing.Size(1200, 950);

            drawingBitmap = new DirectBitmap(drawPanel.Width, drawPanel.Height);

            LoadControlPointsFromFile("dane/scaledtimes50.txt");

            string normalMapPath = "dane/brick_normalmap.png";
            string textureMapPath = "dane/images.jpg";
            normalMap = new Bitmap(normalMapPath);
            bezierSurface.UpdateNormalMap(normalMap);

            textureMap = new Bitmap(textureMapPath);
            bezierSurface.UpdateTextureMap(textureMap);
            off_on_light.Checked = true;

        }

        private void InitializeUI()
        {
            // Panel do rysowania
            drawPanel = new Panel
            {
                Location = new System.Drawing.Point(10, 60),
                Size = new System.Drawing.Size(800, 800),
                BorderStyle = BorderStyle.FixedSingle
            };
            drawPanel.Paint += OnPaint;
            this.Controls.Add(drawPanel);

            // Etykieta i suwak do k¹ta alfa
            Label alphaLabel = new Label { Text = "K¹t alfa", Location = new System.Drawing.Point(820, 50), AutoSize = true };
            this.Controls.Add(alphaLabel);

            TrackBar alphaTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(920, 50),
                Minimum = -45,
                Maximum = 45,
                Width = 150
            };
            alphaTrackBar.ValueChanged += (s, e) => { alpha = alphaTrackBar.Value; bezierSurface.UpdateRotation(alpha, beta); drawPanel.Invalidate(); };
            this.Controls.Add(alphaTrackBar);

            // Etykieta i suwak do k¹ta beta
            Label betaLabel = new Label { Text = "K¹t beta", Location = new System.Drawing.Point(820, 100), AutoSize = true };
            this.Controls.Add(betaLabel);

            TrackBar betaTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(920, 100),
                Minimum = 0,
                Maximum = 10,
                Width = 150
            };
            betaTrackBar.ValueChanged += (s, e) => { beta = betaTrackBar.Value; bezierSurface.UpdateRotation(alpha, beta); drawPanel.Invalidate(); };
            this.Controls.Add(betaTrackBar);

            // Etykieta i suwak do dok³adnoœci triangulacji
            Label resolutionLabel = new Label { Text = "Dok³adnoœæ", Location = new System.Drawing.Point(820, 150), AutoSize = true };
            this.Controls.Add(resolutionLabel);

            TrackBar resolutionTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(920, 150),
                Minimum = 1,
                Maximum = 20,
                Width = 150
            };
            resolutionTrackBar.ValueChanged += (s, e) => { bezierSurface.UpdateResolution(resolutionTrackBar.Value); bezierSurface.ApplyRotation(alpha, beta); drawPanel.Invalidate(); };
            this.Controls.Add(resolutionTrackBar);

            // Radiobuttons dla zmiany wyœwietlania
            frameRadio = new RadioButton
            {
                Text = "Siatka",
                Location = new System.Drawing.Point(820, 450),
                AutoSize = true
            };
            frameRadio.CheckedChanged += (sender, args) =>
            {
                if (frameRadio.Checked)
                {
                    is_drawing_frame = true;
                    is_drawing_fill = false;
                    UpdateRadioButtonStates();
                    drawPanel.Invalidate();
                }
            };
            this.Controls.Add(frameRadio);

            fillRadio = new RadioButton
            {
                Text = "Wype³nienie",
                Location = new System.Drawing.Point(820, 500),
                AutoSize = true
            };
            fillRadio.CheckedChanged += (sender, args) =>
            {
                if (fillRadio.Checked)
                {
                    is_drawing_frame = false;
                    is_drawing_fill = true;
                    UpdateRadioButtonStates();
                    drawPanel.Invalidate();
                }
            };
            this.Controls.Add(fillRadio);

            // CheckBox dla tekstury
            textureNatRadio= new CheckBox
            {
                Text = "Tekstura Wektorów",
                Location = new System.Drawing.Point(820, 550),
                AutoSize = true
            };
            textureNatRadio.CheckedChanged += (sender, args) =>
            {
                is_drawing_textureNat = textureNatRadio.Checked;
                UpdateRadioButtonStates();
                drawPanel.Invalidate();
            };
            this.Controls.Add(textureNatRadio);

            textureRadio = new CheckBox
            {
                Text = "Tekstura Obiektu",
                Location = new System.Drawing.Point(820, 600),
                AutoSize = true
            };
            textureRadio.CheckedChanged += (sender, args) =>
            {
                is_drawing_texture = textureRadio.Checked;
                UpdateRadioButtonStates();
                drawPanel.Invalidate();
            };
            this.Controls.Add(textureRadio);
        }

        private void UpdateRadioButtonStates()
        {
            frameRadio.Checked = is_drawing_frame;
            fillRadio.Checked = is_drawing_fill;
            textureNatRadio.Checked = is_drawing_textureNat;
            textureRadio.Checked = is_drawing_texture;
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = Path.Combine(projectFolder, "dane");
                openFileDialog.InitialDirectory = imagesFolder;
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadControlPointsFromFile(openFileDialog.FileName);
                    bezierSurface.UpdateNormalMap(normalMap);
                    bezierSurface.UpdateTextureMap(textureMap);
                    drawPanel.Invalidate();
                }
            }
        }

        private void LoadControlPointsFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length == 16)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            string[] parts = lines[i * 4 + j].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (parts.Length == 3 &&
                                parts[0].StartsWith("X") &&
                                parts[1].StartsWith("Y") &&
                                parts[2].StartsWith("Z"))
                            {
                                // Usuwanie prefiksów "X", "Y", "Z" i parsowanie wartoœci
                                float x = float.Parse(parts[0].Substring(1), CultureInfo.InvariantCulture);
                                float y = float.Parse(parts[1].Substring(1), CultureInfo.InvariantCulture);
                                float z = float.Parse(parts[2].Substring(1), CultureInfo.InvariantCulture);

                                controlPoints[i, j] = new ControlPoint(x, y, z);
                            }
                            else
                            {
                                MessageBox.Show("Wymagany model pliku: X00 Y00 Z00");
                                return;
                            }
                        }
                    }
                    bezierSurface = new BezierSurface(controlPoints, 10);
                    //MessageBox.Show("Punkty kontrolne wczytane pomyœlnie!");
                }
                else
                {
                    MessageBox.Show("Plik nie ma 16 linii.");
                }
            }
            else
            {
                MessageBox.Show("Plik nie istnieje: " + filePath);
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (bezierSurface != null)
            {
                g.DrawImage(drawingBitmap.Bitmap, 0, 0, drawPanel.Width, drawPanel.Height);
                drawingBitmap.Clear(Color.White);
                bezierSurface.UpdateVariables(kd, ks, m, IL, IO);

                bezierSurface.DrawFrame(g,Pens.Black,drawPanel.Width,drawPanel.Height,is_drawing_frame,drawingBitmap,is_drawing_textureNat,is_drawing_texture) ;

                bezierSurface.DrawControlPoints(g, new SolidBrush(Color.Green), alpha, beta, drawPanel.Width, drawPanel.Height);
                //g.DrawImage(drawingBitmap, 0, 0, drawPanel.Width, drawPanel.Height);
            }
            

        }
    
    
        private void InitializeMenu()
        {
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem optionsMenuItem = new ToolStripMenuItem("Kolory");

            // Opcja wyboru koloru obiektu
            ToolStripMenuItem objectColorMenuItem = new ToolStripMenuItem("Kolor obiektu");
            objectColorMenuItem.Click += (s, e) =>
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        Vector3 objectColor = new Vector3(colorDialog.Color.R / 255f, colorDialog.Color.G / 255f, colorDialog.Color.B / 255f);
                        objectColor = Vector3.Normalize(objectColor);
                        IO = objectColor;
                        drawPanel.Invalidate();
                    }
                }
            };
            optionsMenuItem.DropDownItems.Add(objectColorMenuItem);

            // Opcja wyboru koloru œwiat³a
            ToolStripMenuItem lightColorMenuItem = new ToolStripMenuItem("Kolor œwiat³a");
            lightColorMenuItem.Click += (s, e) =>
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        Vector3 lightColor = new Vector3(colorDialog.Color.R / 255f, colorDialog.Color.G / 255f, colorDialog.Color.B / 255f);
                        lightColor = Vector3.Normalize(lightColor);
                        IL = lightColor;
                        drawPanel.Invalidate();
                    }
                }
            };
            optionsMenuItem.DropDownItems.Add(lightColorMenuItem);

            menuStrip.Items.Add(optionsMenuItem);

            ToolStripMenuItem loadPointsMenuItem = new ToolStripMenuItem("Wczytaj punkty");
            loadPointsMenuItem.Click += LoadButton_Click; 
            menuStrip.Items.Add(loadPointsMenuItem);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);


            ToolStripMenuItem loadNormalMapMenuItem = new ToolStripMenuItem("Wczytaj mapê normalnych");
            loadNormalMapMenuItem.Click += (s, e) =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Obrazy (*.png;*.jpg)|*.png;*.jpg|Wszystkie pliki (*.*)|*.*";
                    string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
                    string imagesFolder = Path.Combine(projectFolder, "dane");
                    openFileDialog.InitialDirectory = imagesFolder;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        normalMap = new Bitmap(openFileDialog.FileName);
                        bezierSurface.UpdateNormalMap(normalMap);
                        drawPanel.Invalidate();
                    }
                }
            };
            menuStrip.Items.Add(loadNormalMapMenuItem);


            ToolStripMenuItem loadTextureMapMenuItem = new ToolStripMenuItem("Wczytaj mapê tekstury");
            loadTextureMapMenuItem.Click += (s, e) =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Obrazy (*.png;*.jpg)|*.png;*.jpg|Wszystkie pliki (*.*)|*.*";
                    string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
                    string imagesFolder = Path.Combine(projectFolder, "dane");
                    openFileDialog.InitialDirectory = imagesFolder;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textureMap = new Bitmap(openFileDialog.FileName);
                        bezierSurface.UpdateTextureMap(textureMap);
                        drawPanel.Invalidate();
                    }
                }
            };
            menuStrip.Items.Add(loadTextureMapMenuItem);





            ToolStripMenuItem showMapsMenuItem = new ToolStripMenuItem("Poka¿ wczytane mapy");
            showMapsMenuItem.Click += (s, e) =>
            {
                if (mapsPreviewForm == null || mapsPreviewForm.IsDisposed)
                {
                    mapsPreviewForm = new MapsPreviewForm(normalMap, textureMap);
                    mapsPreviewForm.Show();
                }
                else
                {
                    mapsPreviewForm.UpdateMaps(normalMap, textureMap);
                }
            };
            menuStrip.Items.Add(showMapsMenuItem);
        }




        private void InitializeVariables()
        {
            // Suwak dla wspó³czynnika kd (0 - 1)
            Label kdLabel = new Label { Text = "Wspó³czynnik kd", Location = new System.Drawing.Point(820, 250), AutoSize = true };
            this.Controls.Add(kdLabel);

            TrackBar kdTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(1020, 250),
                Minimum = 0,
                Maximum = 100,
                Width = 150
            };
            kdTrackBar.ValueChanged += (s, e) => { kd = kdTrackBar.Value / 100.0f; drawPanel.Invalidate(); };
            this.Controls.Add(kdTrackBar);

            // Suwak dla wspó³czynnika ks (0 - 1)
            Label ksLabel = new Label { Text = "Wspó³czynnik ks", Location = new System.Drawing.Point(820, 300), AutoSize = true };
            this.Controls.Add(ksLabel);

            TrackBar ksTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(1020, 300),
                Minimum = 0,
                Maximum = 100,
                Width = 150
            };
            ksTrackBar.ValueChanged += (s, e) => { ks = ksTrackBar.Value / 100.0f; drawPanel.Invalidate(); };
            this.Controls.Add(ksTrackBar);

            // Suwak dla wspó³czynnika m (1 - 100)
            Label mLabel = new Label { Text = "Wspó³czynnik m", Location = new System.Drawing.Point(820, 350), AutoSize = true };
            this.Controls.Add(mLabel);

            TrackBar mTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(1020, 350),
                Minimum = 1,
                Maximum = 100,
                Width = 150
            };
            mTrackBar.ValueChanged += (s, e) => { m = mTrackBar.Value; drawPanel.Invalidate(); };
            this.Controls.Add(mTrackBar);



            Label zLabel = new Label { Text = "Light z", Location = new System.Drawing.Point(820, 400), AutoSize = true };
            this.Controls.Add(zLabel);

            TrackBar zTrackBar = new TrackBar
            {
                Location = new System.Drawing.Point(1020, 400),
                Minimum = -500,
                Maximum = 500,
                Width = 150
            };
            zTrackBar.ValueChanged += (s, e) => { lightHeight = zTrackBar.Value; lightPosition.Z = zTrackBar.Value; bezierSurface.UpdateLightPosition(lightPosition); drawPanel.Invalidate(); };
            this.Controls.Add(zTrackBar);


            // Przycisk "Zatrzymaj L / Wznów L"
            Button toggleMovementButton = new Button
            {
                Text = is_moving ? "Zatrzymaj L" : "Wznów L",
                Location = new System.Drawing.Point(820, 650),
                Size = new System.Drawing.Size(150, 40)
            };
            toggleMovementButton.Click += (s, e) =>
            {
                is_moving = !is_moving;
                toggleMovementButton.Text = is_moving ? "Zatrzymaj L" : "Wznów L";
                if (!isRunning)
                {
                    isRunning = true;
                    StartLightMovement();
                }
                // Akcja zwi¹zana z ruchem, jeœli potrzeba
                drawPanel.Invalidate();
            };
            this.Controls.Add(toggleMovementButton);


            off_on_light = new CheckBox
            {
                Text = "On/Off L",
                Location = new System.Drawing.Point(820, 700),
                AutoSize = true
            };
            off_on_light.CheckedChanged += (s, e) =>
            {
                is_light_on = off_on_light.Checked;
                bezierSurface.light_reflectors(is_light_on, is_reflectors_on, mL);
                drawPanel.Invalidate();
            };
            this.Controls.Add(off_on_light);


            off_on_reflectors = new CheckBox
            {
                Text = "On/Off Reflectors",
                Location = new System.Drawing.Point(820, 730),
                AutoSize = true
            };
            off_on_reflectors.CheckedChanged += (s, e) =>
            {
                is_reflectors_on = off_on_reflectors.Checked;
                bezierSurface.light_reflectors(is_light_on, is_reflectors_on, mL);
                drawPanel.Invalidate();
            };
            this.Controls.Add(off_on_reflectors);




            Label focusLabel = new Label
            {
                Text = "Skupienie (mL):",
                Location = new System.Drawing.Point(820, 760),
                AutoSize = true
            };

            TextBox focusTextBox = new TextBox
            {
                Location = new System.Drawing.Point(820, 780),
                Size = new System.Drawing.Size(150, 20)
            };

            Button setFocusButton = new Button
            {
                Text = "Ustaw mL",
                Location = new System.Drawing.Point(820, 810),
                Size = new System.Drawing.Size(150, 40)
            };

            // Dodaj zdarzenie do przycisku
            setFocusButton.Click += (s, e) =>
            {
                if (int.TryParse(focusTextBox.Text, out int value))
                {
                    mL = value;
                    bezierSurface.light_reflectors(is_light_on, is_reflectors_on, mL);
                    drawPanel.Invalidate();
                    //MessageBox.Show($"Zmienna mL zosta³a ustawiona na: {mL}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            this.Controls.Add(focusLabel);
            this.Controls.Add(focusTextBox);
            this.Controls.Add(setFocusButton);
        }





        private void StartLightMovement()
        {
            lightPosition = new Vector3(lightRadius, 0, lightHeight);

            lightMovementTimer = new System.Windows.Forms.Timer();
            lightMovementTimer.Interval = 30; 
            lightMovementTimer.Tick += LightMovementTimer_Tick; 
            lightMovementTimer.Start();
        }
        private void LightMovementTimer_Tick(object sender, EventArgs e)
        {
            if (is_moving)
            {
                angle += 0.2f; // zmiana k¹ta
                if (angle >= 2 * Math.PI) angle -= 2 * (float)Math.PI;

                lightPosition.X = lightRadius * (float)Math.Cos(angle);
                lightPosition.Y = lightRadius * (float)Math.Sin(angle);
                lightPosition.Z = lightHeight; 

                bezierSurface.UpdateLightPosition(lightPosition);
                drawPanel.Invalidate();
            }
        }
        private void StopLightMovement()
        {
            // Zatrzymanie timera
            if (lightMovementTimer != null)
            {
                lightMovementTimer.Stop();
                lightMovementTimer.Dispose();
                lightMovementTimer = null;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // zatrzymanie w¹tku œwiat³a
            if (isRunning)
            {
                StopLightMovement();
            }

            base.OnFormClosed(e);
        }

    }
}
