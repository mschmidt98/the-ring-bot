using System;
using System.Drawing;
using System.Windows.Forms;

namespace SelectionPopup
{
    public partial class Form1 : Form
    {
        string ImgPath = "";
        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string imgPath)
        {
            ImgPath = imgPath;
            InitializeComponent();
            pictureBox1.Image = Image.FromFile(imgPath);
        }

        private void Btn_Komme_Click(object sender, EventArgs e)
        {
            Program.SendToMQTT("o");
            Close();
        }


        private void Btn_Nein_Click(object sender, EventArgs e)
        {
            Program.SendToMQTT("n");
            Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Activate();
        }
    }
}
