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
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }
        bool started = false;
        private void startButton_Click(object sender, EventArgs e)
        {
            //DialogResult dR = MessageBox.Show("Save to PDF?", "", MessageBoxButtons.YesNoCancel);
            Form1 form = new Form1();
            /*if(dR != DialogResult.Cancel)
            {
                if (dR == DialogResult.Yes)
                    form.setPDFTrue();
                if (dR == DialogResult.No)
                    form.setOriTrue();*/
                form.Show();
                if (form.BackgroundImage == null)
                {
                    form.Close();
                }
                else
                {
                    started = true;
                    this.WindowState = FormWindowState.Minimized;
                }

                
            //}
        }
        public bool checkStart()
        {
            return started;
        }
    }
}
