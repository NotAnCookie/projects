using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proj_1
{
    public partial class HelpForm : Form
    {
        private RichTextBox helpRichTextBox;
        private MenuStrip menuStrip;

        public HelpForm()
        {
            this.Width = 500;
            this.Height = 860;

            helpRichTextBox = new RichTextBox
            {
                Location = new Point(10, 40),
                Width = 470,
                Height = 860,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = SystemColors.Control
            };

            // Dodanie menu
            menuStrip = new MenuStrip();
            var usageMenuItem = new ToolStripMenuItem("Usage");
            var algorithmsMenuItem = new ToolStripMenuItem("Algorithms");

            usageMenuItem.Click += (sender, args) => ShowUsage();
            algorithmsMenuItem.Click += (sender, args) => ShowAlgorithms();

            menuStrip.Items.Add(usageMenuItem);
            menuStrip.Items.Add(algorithmsMenuItem);

            this.Controls.Add(menuStrip);
            this.Controls.Add(helpRichTextBox);

            ShowUsage();
        }

        private void ShowUsage()
        {
            helpRichTextBox.Clear();

            AddText(helpRichTextBox, "Przesuwanie:\n", true);
            AddText(helpRichTextBox, "1. Przesuń cały wielokąt - kliknij lewym przyciskiem wewnątrz wielokąta i przeciągnij.\n");
            AddText(helpRichTextBox, "2. Przesuń wierzchołek - kliknij lewym przyciskiem na wierzchołek i przeciągnij.\n\n");

            AddText(helpRichTextBox, "Edycja:\n", true);
            AddText(helpRichTextBox, "1. Edycja wierzchołka - kliknij prawym przyciskiem na wierzchołek.\n");
            AddText(helpRichTextBox, "2. Edycja krawędzi - kliknij prawym przyciskiem na krawędź.\n");
            AddText(helpRichTextBox, "3. Edycja krawędzi Beziera - kliknij prawym przyciskiem na linię między wierzchołkami.\n\n");

            AddText(helpRichTextBox, "Menu:\n", true);
            AddText(helpRichTextBox, "1. Delete Polygon - usuwa cały wielokąt.\n");
            AddText(helpRichTextBox, "2. Start Drawing - możesz rozpocząć rysowanie nowego wielokąta po usunięciu poprzedniego.\n\n");

            AddText(helpRichTextBox, "Linie:\n", true);
            AddText(helpRichTextBox, "1. Use DrawLine - rysuje linie przy użyciu g.DrawLine z grafiki.\n");
            AddText(helpRichTextBox, "2. Use Bresenham - rysuje linie przy użyciu algorytmu Bresenhama.\n");
            AddText(helpRichTextBox, "3. Podczas przesuwania wszystko jest rysowane domyślnie za pomocą DrawLine.\n\n");

            AddText(helpRichTextBox, "Błędy:\n", true);
            AddText(helpRichTextBox, "1. Podczas zmiany typu krawędzi - kiedy może wystąpić możliwość utraty innych typów krawędzi.\n");
            AddText(helpRichTextBox, "Można zignorować ten błąd i wymusić zmianę.\n\n");


            helpRichTextBox.SelectionStart = 0;
            helpRichTextBox.ScrollToCaret();
        }

        private void ShowAlgorithms()
        {
            helpRichTextBox.Clear();
            AddText(helpRichTextBox, "Algorytm / relacje:\n", true);

            AddText(helpRichTextBox, "Założenia:\n", true);
            AddText(helpRichTextBox, "1. Krawędzie są 'jednostronne' - jednnostronność wyznacza 'nietykalny' wierzchołek krawędzi podczas jej aktualizacji\n");

            AddText(helpRichTextBox, "Przesuwanie wierzchołka:\n", true);
            AddText(helpRichTextBox, "1. Przesunięcie wierzchołka.\n");
            AddText(helpRichTextBox, "2. Rekurencyjne poprawianie krawędzi do przodu aż natrafimy na krawędź beziera lub bez ograniczenia.\n");
            AddText(helpRichTextBox, "3. Ponowne wykonanie rekurencji w drugą stronę.\n");
            AddText(helpRichTextBox, "4. Aktualizacja krawędzi beziera.\n");
            AddText(helpRichTextBox, "5. Sprawdzenie poprawności wszystkich krawędzi.\n");
            AddText(helpRichTextBox, "6. Jeżeli 5 punkt nie zachowany -> przesunięcie całości.\n");

            AddText(helpRichTextBox, "Przesuwanie wierzchołka beziera:\n", true);
            AddText(helpRichTextBox, "1. Przesunięcie wierzchołka.\n");
            AddText(helpRichTextBox, "2. Aktualizacja krawędzi przylegającej do krawędzi beziera.\n");
            AddText(helpRichTextBox, "3. Rekurencyjne poprawianie krawędzi do przodu od punktu krawędzi przylegającej aż natrafimy na krawędź beziera lub bez ograniczenia.\n");
            AddText(helpRichTextBox, "4. Aktualizacja krawędzi beziera.\n");

            AddText(helpRichTextBox, "Ustawianie nowego ograniczenia krawędzi:\n", true);
            AddText(helpRichTextBox, "1. Ustawienie ograniczenia.\n");
            AddText(helpRichTextBox, "2. Puszczenie kilku rekurencji jak dla przesunięcia wierzchołka.\n");
            AddText(helpRichTextBox, "3. Sprawdzenie poprawnośći wszystkich krawędzi.\n");
            AddText(helpRichTextBox, "4. Jeżeli 3 punkt nie zachowany -> cofnięcie do poprzedniego stanu(*).\n");
            AddText(helpRichTextBox, "(*). Możliwość wymuszenia zmiany pomimo braku zachowania standardów.\n");

            helpRichTextBox.SelectionStart = 0;
            helpRichTextBox.ScrollToCaret();
        }

        private void AddText(RichTextBox box, string text, bool bold = false)
        {
            box.SelectionFont = bold ? new Font(box.Font, FontStyle.Bold) : box.Font;
            box.AppendText(text);
            box.SelectionFont = box.Font;
        }

        
    }
}
