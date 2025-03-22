using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bezier3D
{
    public partial class MapsPreviewForm : Form
    {
        private PictureBox normalMapPictureBox;
        private PictureBox textureMapPictureBox;

        public MapsPreviewForm(Bitmap normalMap, Bitmap textureMap)
        {
            // Ustawienia okna
            this.Text = "Podgląd map";
            this.ClientSize = new Size(800, 400); // Rozmiar okna na dwie mapy obok siebie
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tworzenie PictureBox dla mapy normalnej
            normalMapPictureBox = new PictureBox
            {
                Dock = DockStyle.Left,
                Image = normalMap,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 400 // Połowa szerokości okna
            };

            // Tworzenie PictureBox dla mapy tekstury
            textureMapPictureBox = new PictureBox
            {
                Dock = DockStyle.Right,
                Image = textureMap,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 400 // Połowa szerokości okna
            };

            // Dodawanie obu PictureBox do formularza
            this.Controls.Add(normalMapPictureBox);
            this.Controls.Add(textureMapPictureBox);
        }

        // Metoda do aktualizacji obrazów, gdy mapy zmienią się
        public void UpdateMaps(Bitmap normalMap, Bitmap textureMap)
        {
            normalMapPictureBox.Image = normalMap;
            textureMapPictureBox.Image = textureMap;
        }
    }
}
