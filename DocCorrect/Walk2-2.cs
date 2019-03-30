using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocCorrect
{
    public partial class Walk2_2 : Form
    {
        public Walk2_2()
        {
            InitializeComponent();
        }

        private void Walk2_2_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {
            Help h = new Help();
            h.Show();
            this.Close();
        }

        private void label8_Click_1(object sender, EventArgs e)
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

        private void label10_Click(object sender, EventArgs e)
        {
            Walk3 w3 = new Walk3();
            w3.Show();
            this.Close();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            Walk2 w2 = new Walk2();
            w2.Show();
            this.Close();
        }
    }
}
