﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
         
        }

        private void Btn_Komme_Click(object sender, EventArgs e)
        {
            Program.SendToMQTT("o");
            this.Close();
        }


        private void Btn_Nein_Click(object sender, EventArgs e)
        {
            Program.SendToMQTT("n");
            this.Close();
        }

        
    }
}