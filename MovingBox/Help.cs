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
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
