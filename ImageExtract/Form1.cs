using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageExtract
{
    public partial class Form1 : Form
    {
        string sourceImgPath;
        int sourceWidth, sourceHeight;
        int diff;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            sourceImgPath = openFileDialog1.FileName;
            this.pictureBox1.Image = Image.FromFile(sourceImgPath);
            sourceWidth = this.pictureBox1.Image.Width;
            sourceHeight = this.pictureBox1.Image.Height;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            diff = (int)this.numericUpDown1.Value;
            Bitmap sourceBmp = new Bitmap(this.pictureBox1.Image);
            Bitmap targetBmp = new Bitmap(sourceWidth, sourceHeight);
            for (int x = 0; x < sourceWidth; x++)
            {
                for (int y = 0; y < sourceHeight; y++)
                {
                    Color curColor = sourceBmp.GetPixel(x, y);

                    List<Point> testPoints = new List<Point>(8);
                    //testPoints.Add(new Point(x - 1, y - 1));
                    //testPoints.Add(new Point(x, y - 1));
                    testPoints.Add(new Point(x + 1, y - 1));
                    //testPoints.Add(new Point(x, y));
                    testPoints.Add(new Point(x + 1, y));
                    //testPoints.Add(new Point(x - 1, y + 1));
                    testPoints.Add(new Point(x, y + 1));
                    testPoints.Add(new Point(x + 1, y + 1));

                    foreach (Point p in testPoints)
                    {
                        if (InsideImg(p))
                            if (IsBeyondDiff(sourceBmp.GetPixel(p.X, p.Y), curColor))
                            {
                                targetBmp.SetPixel(x, y, Color.Black);
                                break;
                            }
                    }

                }
            }
            this.pictureBox2.Image = targetBmp;
        }

        private bool InsideImg(Point p)
        {
            int x = p.X; int y = p.Y;
            return x >= 0 && x < sourceWidth && y >= 0 && y < sourceHeight;
        }

        private bool IsBeyondDiff(Color color1, Color color2)
        {
            return Math.Abs(color1.ToArgb() - color2.ToArgb()) > Math.Pow(2, diff);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.pictureBox2.Image = null;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            Bitmap sourceBmp = new Bitmap(this.pictureBox1.Image);
            Bitmap targetBmpHor = new Bitmap(sourceWidth, 100);
            Bitmap targetBmpVir = new Bitmap(sourceWidth, sourceHeight);
            for (int i = 0; i < sourceWidth; i++)
            {
                Color curColor = sourceBmp.GetPixel(i, y);
                int value = (int)((double)Math.Abs(curColor.ToArgb()) * (double)(100.0 / (double)0xffffff));
                targetBmpHor.SetPixel(i, value, Color.Black);
            }
            for (int i = 0; i < sourceHeight; i++)
            {
                Color curColor = sourceBmp.GetPixel(x, i);
                int value = (int)((double)Math.Abs(curColor.ToArgb()) * (double)(100.0 / (double)0xffffff));
                targetBmpVir.SetPixel(i, value, Color.Black);
            }
            this.pictureBox3.Image = targetBmpHor;
            this.pictureBox4.Image = targetBmpVir;
        }

    }
}