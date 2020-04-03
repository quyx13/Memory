using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Memory
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private List<string> motivListe;
        private Dictionary<PictureBox, string> positionen;
        private PictureBox ersteKarte = null;
        private PictureBox zweiteKarte = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            this.Text += version;

            StarteNeuesSpiel();
        }

        private void NeuesSpielToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            StarteNeuesSpiel();
        }

        private void BeendenToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void StarteNeuesSpiel()
        {
            timer1.Stop();
            motivListe = new List<string>();
            positionen = new Dictionary<PictureBox, string>();
            List<string> bilderListe = new List<string>();

            string[] jpgs = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.jpg", SearchOption.AllDirectories);
            foreach (string s in jpgs)
                bilderListe.Add(s);

            string[] pngs = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.png", SearchOption.AllDirectories);
            foreach (string s in pngs)
                bilderListe.Add(s);

            string[] bmps = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bmp", SearchOption.AllDirectories);
            foreach (string s in bmps)
                bilderListe.Add(s);

            if (bilderListe.Count >= 18)
            {
                if (bilderListe.Count == 18)
                {
                    foreach (string s in bilderListe)
                    {
                        motivListe.Add(s);
                        motivListe.Add(s);
                    }
                }
                else
                {
                    for (int i = 0; i < 18; i++)
                    {
                        int rnd = random.Next(bilderListe.Count);
                        motivListe.Add(bilderListe[rnd]);
                        motivListe.Add(bilderListe[rnd]);
                        bilderListe.RemoveAt(rnd);
                    }
                }

                foreach (Control control in tableLayoutPanel1.Controls)
                {
                    PictureBox picBox = control as PictureBox;
                    if (picBox != null)
                    {
                        int rnd = random.Next(motivListe.Count);
                        positionen.Add(picBox, motivListe[rnd]);
                        motivListe.RemoveAt(rnd);
                    }
                }

                foreach (Control control in tableLayoutPanel1.Controls)
                {
                    PictureBox picBox = control as PictureBox;
                    if (picBox != null)
                        picBox.Image = null;
                }
            }
            else
            {
                MessageBox.Show("Es werden mindestens 18 verschiedene Bilder benötigt!");
                Close();
            }
        }       

        private void pb_Click(object sender, EventArgs e)
        {
            string ersteDatei = null, zweiteDatei = null;
            
            if (timer1.Enabled)
                return;

            PictureBox picBox = sender as PictureBox;
            
            if (picBox != null)
            {
                if (picBox.Image != null)
                    return;

                if (ersteKarte == null)
                {
                    ersteKarte = picBox;
                    positionen.TryGetValue(ersteKarte, out ersteDatei);
                    ersteKarte.Image = (Image)new Bitmap(ersteDatei);
                    return;
                }

                zweiteKarte = picBox;
                positionen.TryGetValue(zweiteKarte, out zweiteDatei);
                zweiteKarte.Image = (Image)new Bitmap(zweiteDatei);

                CheckForWinner();

                positionen.TryGetValue(ersteKarte, out ersteDatei);
                positionen.TryGetValue(zweiteKarte, out zweiteDatei);
                if (ersteDatei == zweiteDatei)
                {
                    (new SoundPlayer("richtig.wav")).Play();
                    ersteKarte = null;
                    zweiteKarte = null;
                }
                else
                {
                    (new SoundPlayer("falsch.wav")).Play();
                }

                timer1.Start();
            }
        }

        private void CheckForWinner()
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                PictureBox picBox = control as PictureBox;
                
                if (picBox != null)
                {
                    if (picBox.Image == null)
                        return;
                }
            }
            SoundPlayer sound = new SoundPlayer("gewonnen.wav");
            sound.Play();
            MessageBox.Show("Du hast alle Pärchen gefunden!", "Glückwunsch!");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            if (ersteKarte != null)
                ersteKarte.Image = null;
            if (zweiteKarte != null)
                zweiteKarte.Image = null;

            ersteKarte = null;
            zweiteKarte = null;
        }
    }
}