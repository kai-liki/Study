using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ImageRec
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox1.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                pictureBox1.Load(textBox1.Text);
            }
        }

        int diffR, diffG, diffB, diffX;
        byte r, g, b;

        private void button3_Click(object sender, EventArgs e)
        {
            r = byte.Parse(labelR.Text);
            g = byte.Parse(labelG.Text);
            b = byte.Parse(labelB.Text);
            diffR = (int)numericUpDownR.Value;
            diffG = (int)numericUpDownG.Value;
            diffB = (int)numericUpDownB.Value;
            diffX = (int)numericUpDownX.Value;
            if (pictureBox1.Image != null)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                Bitmap tbmp = new Bitmap(bmp.Width, bmp.Height);
                Bitmap tbmp1 = new Bitmap(bmp.Width, bmp.Height);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color c = bmp.GetPixel(x, y);

                        Point[] testPoints = new Point[4];
                        testPoints[0] = new Point(x + 1, y - 1);
                        testPoints[1] = new Point(x + 1, y);
                        testPoints[2] = new Point(x, y + 1);
                        testPoints[3] = new Point(x + 1, y + 1);

                        foreach (Point p in testPoints)
                        {
                            if (InsideImg(p))
                                if (IsBeyondDiff(bmp.GetPixel(p.X, p.Y), c, diffX))
                                {
                                    tbmp1.SetPixel(x, y, Color.Black);
                                    tbmp.SetPixel(x, y, Color.White);
                                    break;
                                }
                                else
                                {
                                    if (Math.Abs(c.R - r) > diffR && Math.Abs(c.G - g) > diffG && Math.Abs(c.B - b) > diffB)
                                    {
                                        tbmp.SetPixel(x, y, Color.White);
                                    }
                                    else
                                    {
                                        tbmp.SetPixel(x, y, c);
                                    }
                                    break;
                                }
                        }
                    }
                }
                pictureBox2.Image = tbmp;
                pictureBox3.Image = tbmp1;
            }
        }

        private bool InsideImg(Point p)
        {
            int x = p.X; int y = p.Y;
            int sourceWidth = pictureBox1.Width;
            int sourceHeight = pictureBox1.Height;
            return x >= 0 && x < sourceWidth && y >= 0 && y < sourceHeight;
        }

        private bool IsBeyondDiff(Color color1, Color color2, int diff)
        {
            return Math.Abs(color1.ToArgb() - color2.ToArgb()) > Math.Pow(2, diff);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Color c = new Bitmap(pictureBox1.Image).GetPixel(e.X, e.Y);
            labelR.Text = c.R.ToString();
            labelG.Text = c.G.ToString();
            labelB.Text = c.B.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            r = byte.Parse(labelR.Text);
            g = byte.Parse(labelG.Text);
            b = byte.Parse(labelB.Text);
            diffR = (int)numericUpDownR.Value;
            diffG = (int)numericUpDownG.Value;
            diffB = (int)numericUpDownB.Value;
            diffX = (int)numericUpDownX.Value;
            if (pictureBox1.Image != null)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                Bitmap tbmp = new Bitmap(bmp.Width, bmp.Height);
                PixelMatrix matrix = new PixelMatrix(bmp.Width, bmp.Height);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        RNode node = new RNode();
                        node.R = c.R;
                        node.G = c.G;
                        node.B = c.B;
                        node.x = x;
                        node.y = y;
                        matrix.nodes[x, y] = node;
                    }
                }

                RTree.Verify += new VerifyDelegate(VerifyRGB);

                List<RTree> rtrees = new List<RTree>();

                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        RNode node = matrix.nodes[x, y];
                        if (!node.reached)
                        {
                            if (RTree.Verify(node.R, node.G, node.B, r, g, b, diffR, diffG, diffB))
                            {
                                RTree rtree = new RTree(matrix, node, r, g, b, diffR, diffG, diffB, diffX);
                                rtrees.Add(rtree);
                            }
                            
                        }
                    }
                }

                foreach (RTree rt in rtrees)
                {
                    rt.DrawNodes(rt.root, tbmp);
                }

                pictureBox4.Image = tbmp;
                rtrees.Clear();
            }
        }

        private static bool VerifyRGB(byte sr, byte sg, byte sb,byte tr, byte tg, byte tb, int dr, int dg, int db)
        {
            return Math.Abs(sr - tr) < dr && Math.Abs(sg - tg) < dg && Math.Abs(sb - tb) < db;
        }

        private void TraverseNode(RNode r)
        {

        }

    }
}