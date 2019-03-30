using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DocCorrect
{
    public partial class Form1 : Form
    {
        #region Variables
        bool clicked = false;
        bool started = false;
        bool paintFlag = false;
        bool clickFlag = false;
        string filePath = "";
        Image img;
        Graphics graphics;
        Graphics gr;
        Display display;
        Size size = new Size(8, 8);
        Pen black = new Pen(Color.Black, 3);
        Pen white = new Pen(Color.White, 3);
        Color color = Color.Black;
        SolveEquations sE = new SolveEquations();
        //Bitmap formPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        Bitmap bit;
        List<Point> pList1 = new List<Point>();
        List<Point> pList2 = new List<Point>();
        List<Point> pList3 = new List<Point>();
        List<Point> pList4 = new List<Point>();
        List<Point> pList5 = new List<Point>();
        List<Point> pList6 = new List<Point>();
        List<Point> pList7 = new List<Point>();
        List<Point> pList8 = new List<Point>();
        Point pnt0 = new Point(-1, -1);
        Point pnt1 = new Point(-1, -1);
        Point pnt2 = new Point(-1, -1);
        Point pnt3 = new Point(-1, -1);
        Point pnt4 = new Point(-1, -1);
        Point pnt5 = new Point(-1, -1);
        Point pnt6 = new Point(-1, -1);
        Point pnt7 = new Point(-1, -1);

        Point[] center = new Point[8];
        Point[] centerF = new Point[8];
        List<Point>[] pList = new List<Point>[8];
        bool[] moving = { false, false, false, false, 
                            false, false, false, false };
        bool pdf = false;
        bool original = false;

        bool pointMoving = false;
        bool[] whichPoint = { false, false, false, false, 
                                false, false, false, false };

        Pen red = new Pen(Color.Red, 3);
        Pen orange = new Pen(Color.Orange, 3);
        Pen yellow = new Pen(Color.Yellow, 3);
        Pen green = new Pen(Color.Green, 3);
        #endregion

        #region ImportVariables
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true,
            ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y,
            int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        #endregion

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(500, 500);
            graphics = this.CreateGraphics();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string ext = "";
            gr = this.CreateGraphics();
            MessageBox.Show("Please choose an image.");
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Rectangle r = Screen.FromControl(this).Bounds;
                ext = openFileDialog1.FileName;
                ext = ext.Substring(ext.LastIndexOf("."));
                
            }
            else
            {
                //this.Close();
                return;
            }
            if (ext == ".pdf")
            {
                MessageBox.Show("PDFs are not supported. Sorry!");
                this.WindowState = FormWindowState.Minimized;
                return;
            }
            else
            {
                img = Image.FromFile(openFileDialog1.FileName);
                //Images past a certain size are rotated automatically for whatever reason
                Rectangle r = Screen.FromControl(this).Bounds;
                //MessageBox.Show(img.Width + ", " + img.Height);
                #region Shrink Image
                if (img.Height > r.Height || img.Width > r.Width)
                {
                    this.BackgroundImageLayout = ImageLayout.Zoom;
                    this.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                    //this.Size = new Size(700, 740);
                    double ratio = img.Width / img.Height;
                    int nWidth = r.Width / 2;
                    int nHeight = r.Height / 2;
                    //MessageBox.Show(img.Width + ", " + img.Height);
                    if (img.Height == 0 || img.Width == 0)
                    {
                        MessageBox.Show("Please choose a valid image.");
                        this.WindowState = FormWindowState.Minimized;
                    }

                    if (img.Height > img.Width)
                    {
                        nWidth = nHeight / (img.Height / img.Width);
                    }
                    if (img.Width > img.Height)
                    {
                        nHeight = nWidth * (img.Height / img.Width);
                    }
                    //MessageBox.Show((nWidth / nHeight) + ", " + (img.Width / img.Height));
                    //MessageBox.Show(nWidth + ", " + nHeight);
                    //this.BackgroundImage = resizeImage(img, new Size(nWidth, nHeight));
                    img = this.BackgroundImage;
                    this.Width = nWidth;
                    this.Height = nHeight;
                    //MessageBox.Show(nWidth + ", " + nHeight + "\n" + img.Width + ", " + img.Height);
                }
                #endregion
                else if (img.Height < 300 || img.Width < 300)
                {
                    this.BackgroundImageLayout = ImageLayout.Zoom;
                }
                else
                {
                    this.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                }
                this.MinimumSize = new Size(img.Width / 2, img.Height / 2);
                img = Image.FromFile(openFileDialog1.FileName);
                filePath = openFileDialog1.FileName;
            }

            this.Height = this.BackgroundImage.Height;
            this.Width = this.BackgroundImage.Width;
            this.TopMost = false;
            started = true;
            //this.Paint -= new PaintEventHandler(Form1_Paint_1);
            //this.Paint += new PaintEventHandler(Form1_Paint);
            try
            {
                bit = new Bitmap(img);
            }
            catch (Exception)
            {
                MessageBox.Show("Bitmap failed");
            }
            /*MessageBox.Show("Please outline the part you wish to undistort using " +
            "the circles and press Enter when you're ready to undistort the image.");
            MessageBox.Show("Click the screen to begin " 
                + "when you're done resizing the screen.");*/
            //MessageBox.Show(label1.Location.ToString() + ", " + label2.Location.ToString()
            //    + "\n" + this.Size.ToString());
        }

        private void label3_Click_2(object sender, EventArgs e)   
        {//private void Form1_KeyDown(object sender, KeyEventArgs e)
            /*bool enter = (e.KeyCode == Keys.Enter);
            bool s = (e.KeyCode == Keys.S);
            if (started && (enter || s))
            {*/
                centerF = center;
                int W = (int)((distPts(centerF[0], centerF[1]) 
                    + distPts(centerF[2], centerF[3])) / 2);
                int H = (int)((distPts(centerF[0], centerF[3])
                    + distPts(centerF[1], centerF[2])) / 2);
                display = new Display(W, H);
                double[] conC = new double[6];
                double[] conR = new double[6];
                double[] Yc = {centerF[0].X, centerF[1].X, 
                               centerF[2].X, centerF[3].X, 
                               centerF[4].X, centerF[5].X, 
                               centerF[6].X, centerF[7].X};
                double[] Yr = {centerF[0].Y, centerF[1].Y, 
                               centerF[2].Y, centerF[3].Y, 
                               centerF[4].Y, centerF[5].Y, 
                               centerF[6].Y, centerF[7].Y};
                double[] rSideC = new double[6];
                double[] rSideR = new double[6];
                double[,] X = new double[8, 6] { 
                    { 1, 0,   0,   0,           0,           0 }, 
                    { 1, 0,   W/2, 0,           0,           (W/2)*(W/2) }, 
                    { 1, 0,   W-1, 0,           0,           (W-1)*(W-1) },
                    { 1, H/2, W-1, (H/2)*(H/2), (H/2)*(W-1), (W-1)*(W-1) },
                    { 1, H-1, W-1, (H-1)*(H-1), (H-1)*(W-1), (W-1)*(W-1) },
                    { 1, H-1, W/1, (H-1)*(H-1), (H/2)*(W/2), (W/2)*(W/2) },
                    { 1, H-1, 0,   (H-1)*(H-1), 0,           0           },
                    { 1, H/2, 0,   (H/2)*(H/2), 0,           0           },
                };
                double[,] X2 = new double[6, 6];
                //START HERE
                /*double Hms = (H - 1) * (H - 1);
                double Wms = (W - 1) * (W - 1);
                double Hh = H / 2;
                double Wh = W / 2;
                double[,] XT = new double[6, 8] { 
                    { 1, 1,     1,   1,        1,           1,        1,   1}, 
                    { 0, 0,     0,   H/2,      H-1,         H-1,      H-1, H/2 }, 
                    { 0, W/2,   W-1, W-1,      W-1,         W/2,      0,   0 },
                    { 0, 0,     0,   (H*H)/4,  Hms,         Hms,      Hms, Hh*Hh },
                    { 0, 0,     0,   Hh*(W-1), (H-1)*(W-1), (H-1)*Wh, 0,   0 },
                    { 0, Wh*Wh, Wms, Wms,       Wms,        Wh*Wh,    0,   0 },
                };*/
                double sum = 0.0;
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        sum = 0.0;
                        for (int k = 0; k < 8; k++)
                        {
                            sum += X[k, i] * X[k, j];
                        }
                        X2[i, j] = sum;
                        /*X2[i, j] = 
                              (XT[i, 0] * X[0, j]) + (XT[i, 1] * X[1, j])
                            + (XT[i, 2] * X[2, j]) + (XT[i, 3] * X[3, j])
                            + (XT[i, 4] * X[4, j]) + (XT[i, 5] * X[5, j])
                            + (XT[i, 6] * X[6, j]) + (XT[i, 7] * X[7, j]);*/
                        //x2a += X2[i, j].ToString() + "      ";
                    }
                    //x2a += "\n";
                }
                string xyc = "";
                string xyr = "";
                for (int i = 0; i < 6; i++)
                {
                    double totC = 0.0;
                    double totR = 0.0;
                    for (int k = 0; k < 8; k++)
                    {
                        totC += X[k, i] * Yc[k];
                        totR += X[k, i] * Yr[k];
                    }
                    rSideC[i] = totC;
                    rSideR[i] = totR;
                    xyc += rSideC[i].ToString() + "  ";
                    xyr += rSideR[i].ToString() + "  ";
                    /*rSideC[i] = 
                          (XT[i, 0] * Yc[0]) + (XT[i, 1] * Yc[1])
                        + (XT[i, 2] * Yc[2]) + (XT[i, 3] * Yc[3])
                        + (XT[i, 4] * Yc[4]) + (XT[i, 5] * Yc[5])
                        + (XT[i, 6] * Yc[6]) + (XT[i, 7] * Yc[7]);
                    xyc += rSideC[i].ToString() + "  ";
                    rSideR[i] = 
                          (XT[i, 0] * Yr[0]) + (XT[i, 1] * Yr[1])
                        + (XT[i, 2] * Yr[2]) + (XT[i, 3] * Yr[3])
                        + (XT[i, 4] * Yr[4]) + (XT[i, 5] * Yr[5])
                        + (XT[i, 6] * Yr[6]) + (XT[i, 7] * Yr[7]);
                    xyr += rSideR[i].ToString() + "  ";*/
                }
                double[,] tempC = new double[6, 7] {
                    { X2[0, 0], X2[0, 1], X2[0, 2], X2[0, 3], X2[0, 4], X2[0, 5], rSideC[0]}, 
                    { X2[1, 0], X2[1, 1], X2[1, 2], X2[1, 3], X2[1, 4], X2[1, 5], rSideC[1]}, 
                    { X2[2, 0], X2[2, 1], X2[2, 2], X2[2, 3], X2[2, 4], X2[2, 5], rSideC[2]},
                    { X2[3, 0], X2[3, 1], X2[3, 2], X2[3, 3], X2[3, 4], X2[3, 5], rSideC[3]},
                    { X2[4, 0], X2[4, 1], X2[4, 2], X2[4, 3], X2[4, 4], X2[4, 5], rSideC[4]},
                    { X2[5, 0], X2[5, 1], X2[5, 2], X2[5, 3], X2[5, 4], X2[5, 5], rSideC[5]}
                };
                /*string temp = "";
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        temp += tempC[i, j] + "    ";
                    }
                    temp += "\n";
                }
                //MessageBox.Show(temp);*/
                double[,] tempR = new double[6, 7] {
                    { X2[0, 0], X2[0, 1], X2[0, 2], X2[0, 3], X2[0, 4], X2[0, 5], rSideR[0]}, 
                    { X2[1, 0], X2[1, 1], X2[1, 2], X2[1, 3], X2[1, 4], X2[1, 5], rSideR[1]}, 
                    { X2[2, 0], X2[2, 1], X2[2, 2], X2[2, 3], X2[2, 4], X2[2, 5], rSideR[2]},
                    { X2[3, 0], X2[3, 1], X2[3, 2], X2[3, 3], X2[3, 4], X2[3, 5], rSideR[3]},
                    { X2[4, 0], X2[4, 1], X2[4, 2], X2[4, 3], X2[4, 4], X2[4, 5], rSideR[4]},
                    { X2[5, 0], X2[5, 1], X2[5, 2], X2[5, 3], X2[5, 4], X2[5, 5], rSideR[5]}
                };
                conC = Array.ConvertAll(sE.SolveLinearEquation(tempC),
                    x => (double)x);
                conR = Array.ConvertAll(sE.SolveLinearEquation(tempR),
                    x => (double)x);

                double c = 0.0;
                double r = 0.0;

                display.Show();
                display.Width = this.Width;
                display.Height = this.Height;
                display.Location = this.Location;
                double[] cs = new double[0];
                double[] rs = new double[0];
                //string filePath2 = @"C:\Dev\MovingBox\MovingBox\pts.txt";
                for (int h = 0; h < H; h++)
                {
                    for (int w = 0; w < W; w++)
                    {
                        c = conC[0] + (conC[1] * h) + (conC[2] * w)
                            + (conC[3] * h * h) + (conC[4] * h * w)
                            + (conC[5] * w*w);
                        r = conR[0] + (conR[1] * h) + (conR[2] * w)
                            + (conR[3] * h * h) + (conR[4] * h * w)
                            + (conR[5] * w * w);
                        int red = averageColor((int)c, (int)r, 0);
                        int green = averageColor((int)c, (int)r, 1);
                        int blue = averageColor((int)c, (int)r, 2);
                        display.setPixelColor(w, h, red, green, blue);
                    }
                }
                /*System.IO.File.WriteAllText(filePath2, String.Empty);
                using (System.IO.StreamWriter file = 
                    new System.IO.StreamWriter(filePath2, true))
                {
                    
                    for(int n = 0; n < cs.Length; n++)
                    {
                        file.WriteLine(cs[n].ToString() + ", " + rs[n].ToString());
                    }
                }*/
                DialogResult dR = MessageBox.Show("Save to PDF?", "", 
                    MessageBoxButtons.YesNo);
                if (dR == DialogResult.Yes)
                    pdf = true;
                if (dR == DialogResult.No)
                    original = true;
                display.save(filePath, pdf, original);
                MessageBox.Show("Done!");
                this.Close();
            //}
        }

        private void label3_Click(object sender, EventArgs e)
        {
                int P = 6; // P is the number of constants
                int N = 8; // N is the number of points

                centerF = center;
                //MessageBox.Show("1: " + centerF[0].ToString() + "2: " + centerF[1].ToString() + "3: " + centerF[2].ToString() + "4: " + centerF[3].ToString());
                int Width = (int)((distPts(centerF[0], centerF[2])
                    + distPts(centerF[4], centerF[6])) / 2);
                int Height = (int)((distPts(centerF[0], centerF[6])
                    + distPts(centerF[2], centerF[4])) / 2);
                display = new Display(Width, Height);
                //MessageBox.Show(Width + ", " + Height);
                double[] conC = new double[6];
                double[] conR = new double[6];
                double[] Yc = {centerF[0].X, centerF[1].X, 
                        centerF[2].X, centerF[3].X, 
                        centerF[4].X, centerF[5].X, 
                        centerF[6].X, centerF[7].X};
                double[] Yr = {centerF[0].Y, centerF[1].Y, 
                        centerF[2].Y, centerF[3].Y,
                        centerF[4].Y, centerF[5].Y, 
                        centerF[6].Y, centerF[7].Y};
                //MessageBox.Show("Yc: " + Yc[0].ToString() + ", " + Yc[1].ToString() 
                //+ ", " + Yc[2].ToString() + ", " + Yc[3].ToString() + ", " +
                //    "\n Yr: " + Yr[0].ToString() + ", " + Yr[1].ToString() + ", " 
                //+ Yr[2].ToString() + ", " + Yr[3].ToString() + ", ");
                //double[] R = new double[8] {0, 0, 0, Height/2.0, Height-1, Height-1, Height-1, Height/2};
                //double[] C = new double[8] {0, Width/2.0, Width-1, Width-1, Width-1, Width/2.0, 0, 0};
                double[] rSideC = new double[6];
                double[] rSideR = new double[6];
                double pt1X = Math.Abs((center[1].X - center[0].X) / (center[2].X - center[0].X)) * Width;
                double pt3Y = Math.Abs((center[3].Y - center[2].Y) / (center[4].Y - center[2].Y)) * Height;
                double pt5X = Math.Abs((center[5].X - center[6].X) / (center[4].X - center[6].X)) * Width;
                double pt7Y = Math.Abs((center[7].Y - center[0].Y) / (center[6].Y - center[0].Y)) * Height;
                /*double[,] X = new double[8, 6] { 
                    { 1, 0, 0, 0, 0, 0 }, 
                    { 1, 0, pt1X, 0, 0, (pt1X)*(pt1X)},
                    { 1, 0, Width-1, 0, 0, (Width-1)*(Width-1)},
                    { 1, pt3Y, Width-1, (pt3Y)*(pt3Y), (pt3Y)*(Width-1), (Width-1)*(Width-1)},
                    { 1, (Height-1), (Width-1), (Height-1)*(Height-1), (Height-1)*(Width-1), (Width-1)*(Width-1)},
                    { 1, (Height-1), (pt5X), (Height-1)*(Height-1), (Height-1)*(pt5X), (pt5X)*(pt5X)},
                    { 1, (Height-1), 0, (Height-1)*(Height-1), 0, 0},
                    { 1, (pt7Y), 0, (pt7Y)*(pt7Y), 0, 0}
                };*/
                double[,] X = new double[8, 6] { 
                    { 1, 0, 0, 0, 0, 0 }, 
                    { 1, 0, Width/2, 0, 0, (Width/2)*(Width/2)},
                    { 1, 0, Width-1, 0, 0, (Width-1)*(Width-1)},
                    { 1, Height/2, Width-1, (Height/2)*(Height/2), (Height/2)*(Width-1), (Width-1)*(Width-1)},
                    { 1, (Height-1), (Width-1), (Height-1)*(Height-1), (Height-1)*(Width-1), (Width-1)*(Width-1)},
                    { 1, (Height-1), (Width/2), (Height-1)*(Height-1), (Height-1)*(Width/2), (Width/2)*(Width/2)},
                    { 1, (Height-1), 0, (Height-1)*(Height-1), 0, 0},
                    { 1, (Height/2), 0, (Height/2)*(Height/2), 0, 0}
                };
                double[,] X2 = new double[6, 6];
                //double[,] XT = new double[3, 4] { 
                //    { 1, 1, 1, 1}, 
                //    { 0, 0, Height - 1, Height - 1 }, 
                //    { 0, Width - 1, Width - 1, 0 } 
                //};
                //string x2a = "";
                //for (int i = 0; i < 3; i++)
                //{
                //    for (int j = 0; j < 3; j++)
                //    {
                //        X2[i, j] = (XT[i, 0] * X[0, j]) + (XT[i, 1] * X[1, j]) +
                //            (XT[i, 2] * X[2, j]) + (XT[i, 3] * X[3, j]);
                //        x2a += X2[i, j].ToString() + "      ";
                //    }
                //    x2a += "\n";
                //}

                double sum = 0.0;
                //string x2a = "";
                for (int i = 0; i < P; i++)
                {
                    for (int j = 0; j < P; j++)
                    {
                        sum = 0.0;
                        for (int k = 0; k < N; k++)
                        {
                            sum += X[k, i] * X[k, j];
                        }
                        X2[i, j] = sum;
                        //x2a += sum.ToString();
                    }
                    //x2a += "\n";
                }

                //MessageBox.Show(x2a.ToString());
                string xyc = "";
                string xyr = "";
                for (int i = 0; i < P; i++)
                {
                    double totC = 0.0;
                    double totR = 0.0;
                    for (int k = 0; k < N; k++)
                    {
                        totC += X[k, i] * Yc[k];
                        totR += X[k, i] * Yr[k];
                    }
                    rSideC[i] = totC;
                    rSideR[i] = totR;
                    xyc += rSideC[i].ToString() + "  ";
                    xyr += rSideR[i].ToString() + "  ";
                    //rSideC[i] = (XT[i, 0] * Yc[0]) + (XT[i, 1] * Yc[1])
                    //    + (XT[i, 2] * Yc[2]) + (XT[i, 3] * Yc[3]);
                    //xyc += rSideC[i].ToString() + "  ";
                    //rSideR[i] = (XT[i, 0] * Yr[0]) + (XT[i, 1] * Yr[1])
                    //    + (XT[i, 2] * Yr[2]) + (XT[i, 3] * Yr[3]);
                    //xyr += rSideR[i].ToString() + "  ";
                }
                //MessageBox.Show("XTYc: " + xyc.ToString() + ", XTYr" + xyr.ToString());
                double[,] tempC = new double[6, 7] {
                    { X2[0, 0], X2[0, 1], X2[0, 2], X2[0, 3], X2[0, 4], X2[0, 5], rSideC[0]}, 
                    { X2[1, 0], X2[1, 1], X2[1, 2], X2[1, 3], X2[1, 4], X2[1, 5], rSideC[1]}, 
                    { X2[2, 0], X2[2, 1], X2[2, 2], X2[2, 3], X2[2, 4], X2[2, 5], rSideC[2]},
                    { X2[3, 0], X2[3, 1], X2[3, 2], X2[3, 3], X2[3, 4], X2[3, 5], rSideC[3]}, 
                    { X2[4, 0], X2[4, 1], X2[4, 2], X2[4, 3], X2[4, 4], X2[4, 5], rSideC[4]}, 
                    { X2[5, 0], X2[5, 1], X2[5, 2], X2[5, 3], X2[5, 4], X2[5, 5], rSideC[5]}
                };
                //string temp = "";
                //for (int i = 0; i < 6; i++)
                //{
                //    for (int j = 0; j < 7; j++)
                //    {
                //        temp += tempC[i, j] + "    ";
                //    }
                //    temp += "\n";
                //}
                //MessageBox.Show(temp);
                double[,] tempR = new double[6, 7] {
                    { X2[0, 0], X2[0, 1], X2[0, 2], X2[0, 3], X2[0, 4], X2[0, 5], rSideR[0]}, 
                    { X2[1, 0], X2[1, 1], X2[1, 2], X2[1, 3], X2[1, 4], X2[1, 5], rSideR[1]}, 
                    { X2[2, 0], X2[2, 1], X2[2, 2], X2[2, 3], X2[2, 4], X2[2, 5], rSideR[2]},
                    { X2[3, 0], X2[3, 1], X2[3, 2], X2[3, 3], X2[3, 4], X2[3, 5], rSideR[3]}, 
                    { X2[4, 0], X2[4, 1], X2[4, 2], X2[4, 3], X2[4, 4], X2[4, 5], rSideR[4]}, 
                    { X2[5, 0], X2[5, 1], X2[5, 2], X2[5, 3], X2[5, 4], X2[5, 5], rSideR[5]}
                };
                conC = Array.ConvertAll(sE.SolveLinearEquation(tempC),
                    x => (double)x);
                conR = Array.ConvertAll(sE.SolveLinearEquation(tempR),
                    x => (double)x);
                for (int i = 0; i < conC.Length; i++)
                {
                    //MessageBox.Show(conC[i].ToString());
                }

                ///start here

                double c = 0.0;
                double r = 0.0;

                display.Show();
                display.Width = this.Width;
                display.Height = this.Height;
                display.Location = this.Location;
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        c = conC[0] + (conC[1] * h) + (conC[2] * w) + (conC[3] * h * h) + (conC[4] * h * w) + (conC[5] * w * w);
                        r = conR[0] + (conR[1] * h) + (conR[2] * w) + (conR[3] * h * h) + (conR[4] * h * w) + (conR[5] * w * w);

                        int red = averageColor((int)c, (int)r, 0);
                        int green = averageColor((int)c, (int)r, 1);
                        int blue = averageColor((int)c, (int)r, 2);
                        display.setPixelColor(w, h, red, green, blue);
                    }
                    //MessageBox.Show(h.ToString());
                }
                DialogResult dR = MessageBox.Show("Save to PDF?", "",
                        MessageBoxButtons.YesNo);
                if (dR == DialogResult.Yes)
                    pdf = true;
                if (dR == DialogResult.No)
                    original = true;
                display.save(filePath, pdf, original);
                MessageBox.Show("Completed!");
                this.Close();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            clicked = true;
            clickFlag = false;
        }
        
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            clickFlag = false;

            if (started)
            {
                Invalidate();
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if ((distPts(center[0], e.Location) <= 7 || moving[0])
                        && (!pointMoving || whichPoint[0]))
                    {
                        whichPoint[0] = true;
                        pointMoving = true;
                        center[0].X = Cursor.Position.X - 4;
                        center[0].Y = Cursor.Position.Y - 4;
                        //center[0].X = Cursor.Position.X;
                        //center[0].Y = Cursor.Position.Y;
                        //adjust point 1 and 7
                        //center[1].X = (center[0].X + center[2].X) / 2;
                        //center[1].Y = (center[0].Y + center[2].Y) / 2;
                        //center[7].X = (center[0].X + center[6].X) / 2;
                        //center[7].Y = (center[0].Y + center[6].Y) / 2;
                        //pList1.Add(e.Location);
                        pnt0 = e.Location;
                        pnt7 = new Point((pnt6.X + pnt0.X)/2, (pnt6.Y + pnt0.Y)/2);
                        pnt1 = new Point((pnt0.X + pnt2.X)/2, (pnt0.Y + pnt2.Y)/2); 
                        //pList2.Add(center[1]);
                        //pList8.Add(center[7]);
                        moving[0] = true;
                    }
                    else if ((distPts(center[2], e.Location) <= 7 || moving[2])
                        && (!pointMoving || whichPoint[2]))
                    {
                        whichPoint[2] = true;
                        pointMoving = true;
                        center[2].X = Cursor.Position.X - 4;
                        center[2].Y = Cursor.Position.Y - 4;
                        //adjust point 1 and 3
                        //center[1].X = (center[0].X + center[2].X) / 2;
                        //center[1].Y = (center[0].Y + center[2].Y) / 2;
                        //center[3].X = (center[2].X + center[4].X) / 2;
                        //center[3].Y = (center[2].Y + center[4].Y) / 2;
                        //pList3.Add(e.Location);
                        pnt2 = e.Location;
                        pnt1 = new Point((pnt0.X + pnt2.X) / 2, (pnt0.Y + pnt2.Y) / 2);
                        pnt3 = new Point((pnt2.X + pnt4.X) / 2, (pnt2.Y + pnt4.Y) / 2);
                        moving[2] = true;
                    }
                    else if ((distPts(center[4], e.Location) <= 7 || moving[4])
                        && (!pointMoving || whichPoint[4]))
                    {
                        whichPoint[4] = true;
                        pointMoving = true;
                        center[4].X = Cursor.Position.X - 4;
                        center[4].Y = Cursor.Position.Y - 4;

                        //adjust point 3 and 5
                        //center[3].X = (center[2].X + center[4].X) / 2;
                        //center[3].Y = (center[2].Y + center[4].Y) / 2;
                        //center[5].X = (center[4].X + center[6].X) / 2;
                        //center[5].Y = (center[4].Y + center[6].Y) / 2;
                        //pList5.Add(e.Location);
                        pnt4 = e.Location;
                        pnt3 = new Point((pnt2.X + pnt4.X) / 2, (pnt2.Y + pnt4.Y) / 2);
                        pnt5 = new Point((pnt4.X + pnt6.X) / 2, (pnt4.Y + pnt6.Y) / 2);
                        moving[4] = true;
                    }
                    else if ((distPts(center[6], e.Location) <= 7 || moving[6])
                        && (!pointMoving || whichPoint[6]))
                    {
                        whichPoint[6] = true;
                        pointMoving = true;
                        center[6].X = Cursor.Position.X - 4;
                        center[6].Y = Cursor.Position.Y - 4;
                        //adjust point 5 and 7
                        //center[5].X = (center[4].X + center[6].X) / 2;
                        //center[5].Y = (center[4].Y + center[6].Y) / 2;
                        //center[7].X = (center[0].X + center[6].X) / 2;
                        //center[7].Y = (center[0].Y + center[6].Y) / 2;
                        //pList7.Add(e.Location);
                        pnt6 = e.Location;
                        pnt5 = new Point((pnt4.X + pnt6.X) / 2, (pnt4.Y + pnt6.Y) / 2);
                        pnt7 = new Point((pnt6.X + pnt0.X) / 2, (pnt6.Y + pnt0.Y) / 2);
                        moving[6] = true;
                    }

                    // MOVE THE MID POINTS
                    else if ((distPts(center[1], e.Location) <= 7 || moving[1])
                        && (!pointMoving || whichPoint[1]))
                    {
                        whichPoint[1] = true;
                        pointMoving = true;
                        // only adjust Y, and X always the mid of 0 to 2
                        center[1].X = Cursor.Position.X - 4;
                        center[1].Y = Cursor.Position.Y - 4;
                        //pList2.Add(e.Location);
                        pnt1 = e.Location;
                        moving[1] = true;
                    }
                    else if ((distPts(center[3], e.Location) <= 7 || moving[3])
                        && (!pointMoving || whichPoint[3]))
                    {
                        whichPoint[3] = true;
                        pointMoving = true;
                        // only adjust X, and Y always the mid of 2 to 4
                        center[3].X = Cursor.Position.X - 4;
                        center[3].Y = Cursor.Position.Y - 4;
                        //pList4.Add(e.Location);
                        pnt3 = e.Location;
                        moving[3] = true;
                    }
                    else if ((distPts(center[5], e.Location) <= 7 || moving[5])
                        && (!pointMoving || whichPoint[5]))
                    {
                        whichPoint[5] = true;
                        pointMoving = true;
                        // only adjust Y, and X always the mid of 4 to 6
                        center[5].X = Cursor.Position.X - 4;
                        center[5].Y = Cursor.Position.Y - 4;
                        //pList6.Add(e.Location);
                        pnt5 = e.Location;
                        moving[5] = true;
                    }
                    else if ((distPts(center[7], e.Location) <= 7 || moving[7])
                        && (!pointMoving || whichPoint[7]))
                    {
                        whichPoint[7] = true;
                        pointMoving = true;
                        // only adjust X, and Y always the mid of 0 to 6
                        center[7].X = Cursor.Position.X - 4;
                        center[7].Y = Cursor.Position.Y - 4;
                        //pList8.Add(e.Location);
                        pnt7 = e.Location;
                        moving[7] = true;
                    }
                }
            }
        }

        private void Form1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (started)
            {
                Invalidate();
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    Point[] pLC = {pList1[pList1.Count-1], pList2[pList2.Count-1],
                                         pList3[pList3.Count-1], pList4[pList4.Count-1],
                                         pList5[pList5.Count-1], pList6[pList6.Count-1],
                                         pList7[pList7.Count-1], pList8[pList8.Count-1]};
                    if ((distPts(center[0], e.Location) <= 7 || moving[0])
                        && (!pointMoving || whichPoint[0]))
                    {
                        whichPoint[0] = true;
                        pointMoving = true;
                        center[0].X = Cursor.Position.X - 4;
                        center[0].Y = Cursor.Position.Y - 4;
                        pList1.Add(e.Location);
                        moving[0] = true;
                        center[7].Y = ((center[3].Y + center[0].Y) / 2) - 4;
                        center[4].X = ((center[0].X + center[1].X) / 2) - 4;
                        pList8.Add(new Point(((pLC[3].X + pLC[0].X) / 2) - 4,
                            ((pLC[3].Y + pLC[0].Y) / 2) - 4));
                        pList5.Add(new Point(((pLC[0].X + pLC[1].X) / 2) - 4,
                            ((pLC[0].Y + pLC[1].Y) / 2) - 4));
                    }
                    if ((distPts(center[1], e.Location) <= 7 || moving[1])
                        && (!pointMoving || whichPoint[1]))
                    {
                        whichPoint[1] = true;
                        pointMoving = true;
                        center[1].X = Cursor.Position.X - 4;
                        center[1].Y = Cursor.Position.Y - 4;
                        pList2.Add(e.Location);
                        moving[1] = true;
                        center[4].X = ((center[0].X + center[1].X) / 2) - 4;
                        center[5].Y = ((center[1].Y + center[2].Y) / 2) - 4;
                        pList5.Add(new Point(((pLC[0].X + pLC[1].X) / 2) - 4,
                            ((pLC[0].Y + pLC[1].Y) / 2) - 4));
                        pList6.Add(new Point(((pLC[1].X + pLC[2].X) / 2) - 4,
                            ((pLC[1].Y + pLC[2].Y) / 2) - 4));
                    }
                    if ((distPts(center[2], e.Location) <= 7 || moving[2])
                        && (!pointMoving || whichPoint[2]))
                    {
                        whichPoint[2] = true;
                        pointMoving = true;
                        center[2].X = Cursor.Position.X - 4;
                        center[2].Y = Cursor.Position.Y - 4;
                        pList3.Add(e.Location);
                        moving[2] = true;
                        center[5].Y = ((center[1].Y + center[2].Y) / 2) - 4;
                        center[6].X = ((center[2].X + center[3].X) / 2) - 4;
                        pList6.Add(new Point(((pLC[1].X + pLC[2].X) / 2) - 4,
                            ((pLC[1].Y + pLC[2].Y) / 2) - 4));
                        pList7.Add(new Point(((pLC[2].X + pLC[3].X) / 2) - 4,
                            ((pLC[2].Y + pLC[3].Y) / 2) - 4));
                    }
                    if ((distPts(center[3], e.Location) <= 7 || moving[3])
                        && (!pointMoving || whichPoint[3]))
                    {
                        whichPoint[3] = true;
                        pointMoving = true;
                        center[3].X = Cursor.Position.X - 4;
                        center[3].Y = Cursor.Position.Y - 4;
                        pList4.Add(e.Location);
                        moving[3] = true;
                        center[6].X = ((center[2].X + center[3].X) / 2) - 4;
                        center[7].Y = ((center[3].Y + center[0].Y) / 2) - 4;
                        pList7.Add(new Point(((pLC[2].X + pLC[3].X) / 2) - 4,
                            ((pLC[2].Y + pLC[3].Y) / 2) - 4));
                        pList8.Add(new Point(((pLC[3].X + pLC[0].X) / 2) - 4,
                            ((pLC[3].Y + pLC[0].Y) / 2) - 4));
                    }

                    if ((distPts(center[4], e.Location) <= 7 || moving[4])
                        && (!pointMoving || whichPoint[4]))
                    {
                        whichPoint[4] = true;
                        pointMoving = true;
                        //MessageBox.Show(center[0].X + ", " + center[1].X);
                        center[4].X = ((center[0].X + center[1].X) / 2) - 4;
                        center[4].Y = Cursor.Position.Y - 4;
                        pList5.Add(new Point(center[4].X, e.Location.Y));
                        //pList5.Add(e.Location);
                        moving[4] = true;
                    }
                    if ((distPts(center[5], e.Location) <= 7 || moving[5])
                        && (!pointMoving || whichPoint[5]))
                    {
                        whichPoint[5] = true;
                        pointMoving = true;
                        center[5].X = Cursor.Position.X - 4;
                        center[5].Y = ((center[1].Y + center[2].Y) / 2) - 4;
                        //pList6.Add(e.Location);
                        pList6.Add(new Point(e.Location.X, center[5].Y));
                        moving[5] = true;
                    }
                    if ((distPts(center[6], e.Location) <= 7 || moving[6])
                        && (!pointMoving || whichPoint[6]))
                    {
                        whichPoint[6] = true;
                        pointMoving = true;
                        center[6].X = ((center[2].X + center[3].X) / 2) - 4;
                        center[6].Y = Cursor.Position.Y - 4;
                        //pList7.Add(e.Location);
                        pList7.Add(new Point(center[6].X, e.Location.Y));
                        moving[6] = true;
                    }
                    if ((distPts(center[7], e.Location) <= 7 || moving[7])
                        && (!pointMoving || whichPoint[7]))
                    {
                        whichPoint[7] = true;
                        pointMoving = true;
                        center[7].X = Cursor.Position.X - 4;
                        center[7].Y = ((center[3].Y + center[0].Y) / 2) - 4;
                        //pList8.Add(e.Location);
                        pList8.Add(new Point(e.Location.X, center[7].Y));
                        moving[7] = true;
                    }

                }

            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {

            pointMoving = false;
            whichPoint[0] = false;
            whichPoint[1] = false;
            whichPoint[2] = false;
            whichPoint[3] = false;
            whichPoint[4] = false;
            whichPoint[5] = false;
            whichPoint[6] = false;
            whichPoint[7] = false;
            if (clickFlag == true)
            {
                moving[0] = false;
                moving[1] = false;
                moving[2] = false;
                moving[3] = false;
                moving[4] = false;
                moving[5] = false;
                moving[6] = false;
                moving[7] = false;
                clicked = false;
                return;
            }

            if (moving[0] == true)
            {
                center[0].X = e.Location.X - 4;
                center[0].Y = e.Location.Y - 4;

                //adjust point 1 and 7
                center[1].X = (center[0].X + center[2].X) / 2;
                center[1].Y = (center[0].Y + center[2].Y) / 2;
                center[7].X = (center[0].X + center[6].X) / 2;
                center[7].Y = (center[0].Y + center[6].Y) / 2;
                //pList2.Add(center[1]);
                //pList8.Add(center[7]);
                pnt1 = center[1];
                pnt7 = center[7];
            }
            else if (moving[2] == true)
            {
                center[2].X = e.Location.X - 4;
                center[2].Y = e.Location.Y - 4;

                //adjust point 1 and 3
                center[1].X = (center[0].X + center[2].X) / 2;
                center[1].Y = (center[0].Y + center[2].Y) / 2;
                center[3].X = (center[2].X + center[4].X) / 2;
                center[3].Y = (center[2].Y + center[4].Y) / 2;
                //pList2.Add(center[1]);
                //pList4.Add(center[3]);
                pnt1 = center[1];
                pnt3 = center[3];
            }
            else if (moving[4] == true)
            {

                center[4].X = e.Location.X - 4;
                center[4].Y = e.Location.Y - 4;

                //adjust point 3 and 5
                center[3].X = (center[2].X + center[4].X) / 2;
                center[3].Y = (center[2].Y + center[4].Y) / 2;
                center[5].X = (center[4].X + center[6].X) / 2;
                center[5].Y = (center[4].Y + center[6].Y) / 2;
                //pList4.Add(center[3]);
                //pList6.Add(center[5]);
                pnt3 = center[3];
                pnt5 = center[5];
            }
            else if (moving[6] == true)
            {
                center[6].X = e.Location.X - 4;
                center[6].Y = e.Location.Y - 4;

                //adjust point 5 and 7
                center[5].X = (center[4].X + center[6].X) / 2;
                center[5].Y = (center[4].Y + center[6].Y) / 2;
                center[7].X = (center[6].X + center[0].X) / 2;
                center[7].Y = (center[6].Y + center[0].Y) / 2;
                //pList6.Add(center[5]);
                //pList8.Add(center[7]);
                pnt5 = center[5];
                pnt7 = center[7];
            }

            else if (moving[1] == true)
            {
                //adjust point 1, adjust only x/c to the mid
                //center[1].X = Cursor.Position.X - 4;
                center[1].Y = e.Location.Y - 4;
                //center[1].X = e.Location.X - 4;
                center[1].X = (center[0].X + center[2].X) / 2;
                //center[1].Y = (center[0].Y + center[2].Y) / 2;          
                //pList2.Add(center[1]);
                pnt1 = center[1];

            }
            else if (moving[3] == true)
            {
                //adjust point 3, adjust only y/r to the mid
                //center[3].X = (center[2].X + center[4].X) / 2;
                center[3].X = e.Location.X - 4;
                //center[3].Y = e.Location.Y - 4;
                center[3].Y = (center[2].Y + center[4].Y) / 2;
                //pList4.Add(center[3]);
                pnt3 = center[3];
            }
            else if (moving[5] == true)
            {
                //adjust point 5, adjust only x/c to the mid
                center[5].Y = e.Location.Y - 4;
                //center[5].X = e.Location.X - 4;
                center[5].X = (center[4].X + center[6].X) / 2;
                //center[5].Y = (center[4].Y + center[6].Y) / 2;
                //pList6.Add(center[5]);
                pnt5 = center[5];
            }
            else if (moving[7] == true)
            {
                //adjust point 7, adjust only y/r to the mid
                //center[7].X = (center[0].X + center[6].X) / 2;
                center[7].X = e.Location.X - 4;
                //center[7].Y = e.Location.Y - 4;
                center[7].Y = (center[0].Y + center[6].Y) / 2;
                //pList8.Add(center[7]);
                pnt7 = center[7];
            }
            moving[0] = false;
            moving[1] = false;
            moving[2] = false;
            moving[3] = false;
            moving[4] = false;
            moving[5] = false;
            moving[6] = false;
            moving[7] = false;
            clicked = false;
            clickFlag = true;
            /*if (Math.Abs(center[4].X - center4.X) >= 100 || Math.Abs(center[4].Y - center4.Y) >= 100)
            {
                MessageBox.Show(pList3[0].ToString()
                    + "\n0: " + center[0].ToString()
                    + "\n1: " + center[1].ToString()
                    + "\n2: " + center[2].ToString()
                    + "\n3: " + center[3].ToString()
                    + "\n4: " + center[4].ToString()
                    + "\n5: " + center[5].ToString()
                    + "\n6: " + center[6].ToString()
                    + "\n7: " + center[7].ToString());
            }*/
        }

        private void Form1_MouseUp_1(object sender, MouseEventArgs e)
        {
            moving[0] = false;
            moving[1] = false;
            moving[2] = false;
            moving[3] = false;
            moving[4] = false;
            moving[5] = false;
            moving[6] = false;
            moving[7] = false;
            clicked = false;
            pointMoving = false;
            whichPoint[0] = false;
            whichPoint[1] = false;
            whichPoint[2] = false;
            whichPoint[3] = false;
            whichPoint[4] = false;
            whichPoint[5] = false;
            whichPoint[6] = false;
            whichPoint[7] = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!clicked)
            {
                graphics = e.Graphics;
                graphics.DrawLine(black, 100 + 4, 100 + 4, 200 + 4, 100 + 4);
                graphics.DrawLine(black, 200 + 4, 100 + 4, 200 + 4, 200 + 4);
                graphics.DrawLine(black, 200 + 4, 200 + 4, 100 + 4, 200 + 4);
                graphics.DrawLine(black, 100 + 4, 200 + 4, 100 + 4, 100 + 4);
                Size size1 = new Size(8, 8);
                center[0] = new Point(100, 100);
                center[1] = new Point(150, 100);
                center[2] = new Point(200, 100);
                center[3] = new Point(200, 150);
                center[4] = new Point(200, 200);
                center[5] = new Point(150, 200);
                center[6] = new Point(100, 200);
                center[7] = new Point(100, 150);
                Point pt1 = new Point(100, 100);
                Point pt2 = new Point(150, 100);
                Point pt3 = new Point(200, 100);
                Point pt4 = new Point(200, 150);
                Point pt5 = new Point(200, 200);
                Point pt6 = new Point(150, 200);
                Point pt7 = new Point(100, 200);
                Point pt8 = new Point(100, 150);
                pList1.Add(center[0]);
                pList2.Add(center[1]);
                pList3.Add(center[2]);
                pList4.Add(center[3]);
                pList5.Add(center[4]);
                pList6.Add(center[5]);
                pList7.Add(center[6]);
                pList8.Add(center[7]);
                Rectangle rt1 = new Rectangle(pt1, size1);
                Rectangle rt2 = new Rectangle(pt2, size1);
                Rectangle rt3 = new Rectangle(pt3, size1);
                Rectangle rt4 = new Rectangle(pt4, size1);
                Rectangle rt5 = new Rectangle(pt5, size1);
                Rectangle rt6 = new Rectangle(pt6, size1);
                Rectangle rt7 = new Rectangle(pt7, size1);
                Rectangle rt8 = new Rectangle(pt8, size1);
                graphics.DrawEllipse(black, rt1);
                graphics.DrawEllipse(black, rt2);
                graphics.DrawEllipse(black, rt3);
                graphics.DrawEllipse(black, rt4);
                graphics.DrawEllipse(black, rt5);
                graphics.DrawEllipse(black, rt6);
                graphics.DrawEllipse(black, rt7);
                graphics.DrawEllipse(black, rt8);
                //MessageBox.Show("hi");
                /*center[0].X = center[0].Y = center[1].Y = center[3].X 
                    = center[4].Y = center[7].X = 100;
                center[1].X = center[2].X = center[2].Y = center[3].Y 
                    = center[5].X = center[6].Y = 200;
                center[4].X = center[5].Y = center[6].X = center[7].Y = 150;*/
                if (!paintFlag)
                {
                    this.Paint -= new PaintEventHandler(Form1_Paint);
                    this.Paint += new PaintEventHandler(Form1_Paint_2);
                    paintFlag = true;
                }
            }
        }

        private void Form1_Paint_2(object sender, PaintEventArgs e)
        {
            //MessageBox.Show("hi");
            Pen pen = new Pen(color, 3);
            //if (color == "white")
            //    pen = white;
            graphics = e.Graphics;
            /*if (pList1.Count > 0)
                center[0] = pList1[pList1.Count - 1];
            if (pList2.Count > 0)
                center[1] = pList2[pList2.Count - 1];
            if (pList3.Count > 0)
                center[2] = pList3[pList3.Count - 1];
            if (pList4.Count > 0)
                center[3] = pList4[pList4.Count - 1];
            if (pList5.Count > 0)
                center[4] = pList5[pList5.Count - 1];
            if (pList6.Count > 0)
                center[5] = pList6[pList6.Count - 1];
            if (pList7.Count > 0)
                center[6] = pList7[pList7.Count - 1];
            if (pList8.Count > 0)
                center[7] = pList8[pList8.Count - 1];
            pList1 = new List<Point>();
            pList2 = new List<Point>();
            pList3 = new List<Point>();
            pList4 = new List<Point>();
            pList5 = new List<Point>();
            pList6 = new List<Point>();
            pList7 = new List<Point>();
            pList8 = new List<Point>();
            pList1.Add(center[0]);
            pList2.Add(center[1]);
            pList3.Add(center[2]);
            pList4.Add(center[3]);
            pList5.Add(center[4]);
            pList6.Add(center[5]);
            pList7.Add(center[6]);
            pList8.Add(center[7]);*/
            if (pnt0.X >= 0)
                center[0] = pnt0;
            if (pnt1.X >= 0)
                center[1] = pnt1;
            if (pnt2.X >= 0)
                center[2] = pnt2;
            if (pnt3.X >= 0)
                center[3] = pnt3;
            if (pnt4.X >= 0)
                center[4] = pnt4;
            if (pnt5.X >= 0)
                center[5] = pnt5;
            if (pnt6.X >= 0)
                center[6] = pnt6;
            if (pnt7.X >= 0)
                center[7] = pnt7;
            pnt0 = center[0];
            pnt1 = center[1];
            pnt2 = center[2];
            pnt3 = center[3];
            pnt4 = center[4];
            pnt5 = center[5];
            pnt6 = center[6];
            pnt7 = center[7];
            Size size2 = new Size(8, 8);
            Rectangle rect1 = new Rectangle(center[0], size2);
            Rectangle rect2 = new Rectangle(center[1], size2);
            Rectangle rect3 = new Rectangle(center[2], size2);
            Rectangle rect4 = new Rectangle(center[3], size2);
            Rectangle rect5 = new Rectangle(center[4], size2);
            Rectangle rect6 = new Rectangle(center[5], size2);
            Rectangle rect7 = new Rectangle(center[6], size2);
            Rectangle rect8 = new Rectangle(center[7], size2);
            graphics.DrawEllipse(pen, rect1);
            graphics.DrawEllipse(pen, rect2);
            graphics.DrawEllipse(pen, rect3);
            graphics.DrawEllipse(pen, rect4);
            graphics.DrawEllipse(pen, rect5);
            graphics.DrawEllipse(pen, rect6);
            graphics.DrawEllipse(pen, rect7);
            graphics.DrawEllipse(pen, rect8);
            center[0].X += 4;
            center[0].Y += 4;
            center[1].X += 4;
            center[1].Y += 4;
            center[2].X += 4;
            center[2].Y += 4;
            center[3].X += 4;
            center[3].Y += 4;
            center[4].X += 4;
            center[4].Y += 4;
            center[5].X += 4;
            center[5].Y += 4;
            center[6].X += 4;
            center[6].Y += 4;
            center[7].X += 4;
            center[7].Y += 4;
            graphics.DrawLine(pen, center[0], center[1]);
            graphics.DrawLine(pen, center[1], center[2]);
            graphics.DrawLine(pen, center[2], center[3]);
            graphics.DrawLine(pen, center[3], center[4]);
            graphics.DrawLine(pen, center[4], center[5]);
            graphics.DrawLine(pen, center[5], center[6]);
            graphics.DrawLine(pen, center[6], center[7]);
            graphics.DrawLine(pen, center[7], center[0]);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            graphics = this.CreateGraphics();
            //label1.Location = new Point(0, this.Height - 75);
            int sizE = (int)(label2.Size.Width*1.5) - 2;
            if (label2.Width >= 50)
                sizE = (int)(label2.Width * 1.25) + 3;
            label2.Location = new Point(this.Width - sizE, label1.Location.Y);
            label3.Location = new Point((this.Width - label3.Width)/2,label1.Location.Y);
        }

        public double distPts(Point pt1, Point pt2)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) 
                + Math.Pow(pt2.Y - pt1.Y, 2)));
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }

        public int[] getColorLevel(int x, int y)
        {
            Color color = Color.Black;
            if (x < this.BackgroundImage.Width && y < this.BackgroundImage.Height && x > 0 && y > 0)
            {
                color = bit.GetPixel(x, y);
            }
            int[] grayScale = { color.R, color.G, color.B };
            //    (int)((color.R * .3) 
            //    + (color.G * .59) + (color.B * .11));
            return grayScale;
        }

        public int averageColor(int c, int r, int color)
        {
            double xMulti1 = 0;
            double xMulti2 = 0;
            double yMulti1 = 0;
            double yMulti2 = 0;
            double r1 = 0;
            double r2 = 0;
            int x1 = (int)c;
            int x2 = (int)(c + 1);
            int y1 = (int)r;
            int y2 = (int)(r + 1);
            int[] c0 = getColorLevel((int)c, (int)r);
            int[] c1 = getColorLevel((int)c + 1, (int)r);
            int[] c2 = getColorLevel((int)c + 1, (int)r + 1);
            int[] c3 = getColorLevel((int)c, (int)r + 1);
            //cPoints[0] = getColorLevel((int)c, (int)r);
            //cPoints[1] = getColorLevel((int)c + 1, (int)r);
            //cPoints[2] = getColorLevel((int)c, (int)r + 1);
            //cPoints[3] = getColorLevel((int)c + 1, (int)r + 1);
            xMulti1 = (x2 - c) / (x2 - x1);
            xMulti2 = (c - x1) / (x2 - x1);
            yMulti1 = (y2 - r) / (y2 - y1);
            yMulti2 = (r - y1) / (y2 - y1);
            r1 = (xMulti1 * c0[color]) + (xMulti2 * c1[color]);
            r2 = (xMulti1 * c3[color]) + (xMulti2 * c2[color]);
            return (int)((yMulti1 * r1) + (yMulti2 * r2)); ;
        }

        public void setPDFTrue()
        {
            pdf = true;
        }

        public void setOriTrue()
        {
            original = true;
        }

        public double[] add(double[] arr, double num)
        {
            double[] temp = new double[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++)
                temp[i] = arr[i];
            temp[temp.Length - 1] = num;
            return temp;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            HelpStart h = new HelpStart();
            h.Show();

        }

        private void label2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                color = colorDialog1.Color;
                label2.Text = colorDialog1.Color.Name;
                int sizE = (int)(label2.Size.Width * 1.5) - 2;
                if(label2.Width >= 50)
                    sizE = (int)(label2.Width * 1.25) + 3;
                label2.Location = new Point(this.Width - sizE, label1.Location.Y);
                this.Paint -= new PaintEventHandler(Form1_Paint_2);
                this.Paint += new PaintEventHandler(Form1_Paint_2);
                label2.ForeColor = color;
                label1.ForeColor = color;
                label3.ForeColor = color;
            }
            //MessageBox.Show(this.Width + ", " + label2.Location.X + " , " + label2.Width);
        }

        private void label3_Click_1(object sender, EventArgs e)
        {//private void Form1_KeyDown(object sender, KeyEventArgs e)
            /*bool enter = (e.KeyCode == Keys.Enter);
            bool s = (e.KeyCode == Keys.S);
            if (started && (enter || s))
            {*/
            centerF = center;
            int Width = (int)((distPts(centerF[0], centerF[1])
                + distPts(centerF[2], centerF[3])) / 2);
            int Height = (int)((distPts(centerF[0], centerF[3])
                + distPts(centerF[1], centerF[2])) / 2);
            display = new Display(Width, Height);
            double[] conC = new double[3];
            double[] conR = new double[3];
            double[] Yc = {centerF[0].X, centerF[1].X, 
                        centerF[2].X, centerF[3].X};
            double[] Yr = {centerF[0].Y, centerF[1].Y, 
                        centerF[2].Y, centerF[3].Y};
            double[] rSideC = new double[3];
            double[] rSideR = new double[3];
            double[,] X = new double[4, 3] { 
                    { 1, 0, 0 }, 
                    { 1, 0, Width - 1 }, 
                    { 1, Height - 1, Width - 1 },
                    { 1, Height - 1, 0 }
                };
            double[,] X2 = new double[3, 3];
            double[,] XT = new double[3, 4] { 
                    { 1, 1, 1, 1}, 
                    { 0, 0, Height - 1, Height - 1 }, 
                    { 0, Width - 1, Width - 1, 0 } 
                };
            string x2a = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    X2[i, j] = (XT[i, 0] * X[0, j]) + (XT[i, 1] * X[1, j]) +
                        (XT[i, 2] * X[2, j]) + (XT[i, 3] * X[3, j]);
                    x2a += X2[i, j].ToString() + "      ";
                }
                x2a += "\n";
            }
            string xyc = "";
            string xyr = "";
            for (int i = 0; i < 3; i++)
            {
                rSideC[i] = (XT[i, 0] * Yc[0]) + (XT[i, 1] * Yc[1])
                    + (XT[i, 2] * Yc[2]) + (XT[i, 3] * Yc[3]);
                xyc += rSideC[i].ToString() + "  ";
                rSideR[i] = (XT[i, 0] * Yr[0]) + (XT[i, 1] * Yr[1])
                    + (XT[i, 2] * Yr[2]) + (XT[i, 3] * Yr[3]);
                xyr += rSideR[i].ToString() + "  ";
            }
            double[,] tempC = new double[3, 4] {
                    { X2[0, 0], X2[0, 1], X2[0, 2], rSideC[0]}, 
                    { X2[1, 0], X2[1, 1], X2[1, 2], rSideC[1]}, 
                    { X2[2, 0], X2[2, 1], X2[2, 2], rSideC[2]}
                };
            string temp = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp += tempC[i, j] + "    ";
                }
                temp += "\n";
            }
            //MessageBox.Show(temp);
            double[,] tempR = new double[3, 4] {
                    { X2[0, 0], X2[0, 1], X2[0, 2], rSideR[0]}, 
                    { X2[1, 0], X2[1, 1], X2[1, 2], rSideR[1]}, 
                    { X2[2, 0], X2[2, 1], X2[2, 2], rSideR[2]}
                };
            conC = Array.ConvertAll(sE.SolveLinearEquation(tempC),
                x => (double)x);
            conR = Array.ConvertAll(sE.SolveLinearEquation(tempR),
                x => (double)x);

            double c = 0.0;
            double r = 0.0;

            display.Show();
            display.Width = this.Width;
            display.Height = this.Height;
            display.Location = this.Location;
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    c = conC[0] + (conC[1] * h) + (conC[2] * w);
                    r = conR[0] + (conR[1] * h) + (conR[2] * w);

                    int red = averageColor((int)c, (int)r, 0);
                    int green = averageColor((int)c, (int)r, 1);
                    int blue = averageColor((int)c, (int)r, 2);
                    display.setPixelColor(w, h, red, green, blue);
                }
            }
            DialogResult dR = MessageBox.Show("Save to PDF?", "", MessageBoxButtons.YesNo);
            if (dR == DialogResult.Yes)
                pdf = true;
            if (dR == DialogResult.No)
                original = true;
            display.save(filePath, pdf, original);
            this.Close();
            //}
        }
    }
}
