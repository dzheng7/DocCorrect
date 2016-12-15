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
    public partial class Explanation : Form
    {
        public Explanation()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Help h = new Help();
            h.Show();
            this.Close();
        }

        private void label5_Click_1(object sender, EventArgs e)
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

        private void label2_Click(object sender, EventArgs e)
        {
            Explanation2 ex2 = new Explanation2();
            ex2.Show();
            this.Close();
        }

    }
}
