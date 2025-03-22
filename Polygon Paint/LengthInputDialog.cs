using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj_1
{
    public class LengthInputDialog : Form
    {
        private NumericUpDown lengthInput;
        private Button okButton;
        private Button cancelButton;

        public float NewLength { get; private set; }

        public LengthInputDialog(float currentLength)
        {
            Text = "Set Edge Length";
            Width = 250;
            Height = 180;
            MaximumSize = new Size(250, 180);
            MinimumSize = new Size(250, 180);

            Label lengthLabel = new Label
            {
                Text = "New Length:",
                Left = 10,
                Top = 20,
                Width = 80
            };

            lengthInput = new NumericUpDown
            {
                Left = 100,
                Top = 20,
                Width = 100,
                Minimum = 1, // Minimalna długość krawędzi
                Maximum = 1000, // Maksymalna długość krawędzi
                DecimalPlaces = 2, // Ilość miejsc po przecinku
                Increment = 0.01M, // Krok przy zmianie wartości
                Value = Math.Round((decimal)currentLength, 2)
            };

            okButton = new Button
            {
                Text = "OK",
                Left = 20,
                Width = 80,
                Top = 70,
                Height = 50
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Left = 130,
                Width = 80,
                Top = 70,
                Height = 50
            };
            cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; };

            Controls.Add(lengthLabel);
            Controls.Add(lengthInput);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            NewLength = (float)lengthInput.Value; // Pobieramy nową długość
            DialogResult = DialogResult.OK;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // LengthInputDialog
            // 
            ClientSize = new Size(228, 124);
            MaximumSize = new Size(250, 180);
            MinimumSize = new Size(250, 180);
            Name = "LengthInputDialog";
            ResumeLayout(false);
        }
    }
}
