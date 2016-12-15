using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MovingBox
{
    public partial class Walk : Form
    {
        public Walk()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Help h = new Help();
            h.Show();
            this.Close();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Walk w = new Walk();
            w.Show();
            this.Close();
        }

        private void label7_Click_1(object sender, EventArgs e)
        {
            Explanation ex = new Explanation();
            ex.Show();
            this.Close();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Walk1 w1 = new Walk1();
            w1.Show();
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            Walk1 w1 = new Walk1();
            w1.Show();
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Walk2 w2 = new Walk2();
            w2.Show();
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Walk3 w3 = new Walk3();
            w3.Show();
            this.Close();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
