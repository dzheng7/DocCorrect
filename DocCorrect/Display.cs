using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DocCorrect
{
    public partial class Display : Form
    {
        public Display(int imgWidth, int imgHeight)
        {
            InitializeComponent();
            btm = new Bitmap(imgWidth, imgHeight);
        }
        int width = 0;
        int height = 0;
        Bitmap btm;
        Graphics g;
        public void PaintForm(int[,] colorVals, string path)
        {
            width = colorVals.GetLength(0);
            height = colorVals.GetLength(1);
            for (int h = 0; h < height; h+=1)
            {
                for (int w = 0; w < width; w+=1)
                {
                    Color color = Color.FromArgb(colorVals[w, h],
                        colorVals[w, h], colorVals[w, h]);
                    Brush brush = new SolidBrush(color);
                    Graphics g = this.CreateGraphics();
                    //int X = pts[w, h].X; //(0, 0) problem
                    //int Y = pts[w, h].Y;
                    g.FillRectangle(brush, w, h, 1, 1);
                    btm.SetPixel(w, h, color);
                }
            }
            btm.Save(path.Substring(0, path.LastIndexOf(".")) 
                + dateTimeCon(DateTime.Today) 
                + path.Substring(path.LastIndexOf(".")));
        }
        ImageToPDF iToP = new ImageToPDF();
        public void save(string path, bool PDF, bool oriExt)
        {
            if (PDF)
            {
                string filePath = path.Substring(0, path.LastIndexOf("."))
                    + "_Edited";
                string ext = path.Substring(path.LastIndexOf("."));
                Image i = (Image)btm;
                //MessageBox.Show(filePath + ".pdf");
                if (!File.Exists(filePath + ".pdf"))
                {
                    iToP.savePDF(i, filePath + ".pdf");
                }
                else
                {
                    int n = 1;
                    while (File.Exists(filePath + "(" + n.ToString() + ").pdf"))
                    {
                        n++;
                    }
                    iToP.savePDF(i, filePath + "(" + n.ToString() + ").pdf");
                }
            }
            if (oriExt)
            {
                string filePath = path.Substring(0, path.LastIndexOf("."))
                        + "_Edited";
                string ext = path.Substring(path.LastIndexOf("."));
                if (!File.Exists(filePath + ext))
                {
                    btm.Save(filePath + ext);
                }
                else
                {
                    int i = 1;
                    while (File.Exists(filePath + "(" + i.ToString() + ")" + ext))
                    {
                        i++;
                    }
                    btm.Save(filePath + "(" + i.ToString() + ")" + ext);
                }
            }
        }

        public void setPixelColor(int x, int y, int redVal, int greenVal, int blueVal)
        {
            g = this.CreateGraphics();
            Color color = Color.FromArgb(redVal, greenVal, blueVal);
            Brush brush = new SolidBrush(color);
            g.FillRectangle(brush, x, y, 1, 1);
            btm.SetPixel(x, y, color);
        }

        public string dateTimeCon(DateTime dateTime)
        {
            string date = dateTime.ToString().Substring(0,
                dateTime.ToString().IndexOf(" "));
            string year = date.Substring(date.LastIndexOf(@"/") + 1);
            string month = date.Substring(0, date.IndexOf(@"/"));
            if (month.Length == 1)
                month = "0" + month;
            string day = date.Substring(date.IndexOf(@"/") + 1,
                date.LastIndexOf(@"/") - date.IndexOf(@"/") - 1);
            if (day.Length == 1)
                day = "0" + day;
            return year + month + day;
        }
    }
}
