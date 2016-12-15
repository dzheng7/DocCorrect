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
    public partial class HelpStart : Form
    {
        public HelpStart()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Help h = new Help();
            h.Show();
            this.Close();
        }
    }
}
