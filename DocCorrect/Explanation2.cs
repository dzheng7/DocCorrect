﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocCorrect
{
    public partial class Explanation2 : Form
    {
        public Explanation2()
        {
            InitializeComponent();
        }

        private void label6_Click_1(object sender, EventArgs e)
        {
            Help h = new Help();
            h.Show();
            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Walk w = new Walk();
            w.Show();
            this.Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Explanation ex = new Explanation();
            ex.Show();
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Explanation ex = new Explanation();
            ex.Show();
            this.Close();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
