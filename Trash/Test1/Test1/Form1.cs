using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDlgImgs.ShowDialog();
            if (openFileDlgImgs.FileNames.Length > 0)
            {
                foreach (string str in openFileDlgImgs.FileNames)
                {
                    if (!File.Exists(str))
                    {
                        MessageBox.Show(str + "²»´æÔÚ", "´íÎó");
                        continue;
                    }
                    listBoxImgs.Items.Add(str);
                }
            }
            if (listBoxImgs.Items.Count > 1)
            {
                trackBarDiff.Maximum = listBoxImgs.Items.Count - 2;
            }
        }

        private void btnClnImgList_Click(object sender, EventArgs e)
        {
            listBoxImgs.Items.Clear();
            picBoxCur.Image = null;
            trackBarDiff.Maximum = 0;
        }

        private bool started = false;

        private void listBoxImgs_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!started)
                {
                    picBoxCur.Image = new Bitmap((string)listBoxImgs.SelectedItem);
                    heightRate = (double)picBoxCur.Image.Height / (double)picBoxCur.Height;
                    widthRate = (double)picBoxCur.Image.Height / (double)picBoxCur.Width;
                }
            }
            catch
            { 
            }
        }

        private void trackBarDiff_ValueChanged(object sender, EventArgs e)
        {
            labelTrack.Text = trackBarDiff.Value.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            started = true;
            trackBarDiff.Enabled = false;
            int totalNum = listBoxImgs.Items.Count;
            decimal distanceDiff = numUpDownDis.Value;
            if (totalNum > 1)
            {
                ys = new int[totalNum - 1];
                xs = new int[totalNum - 1];
                Bitmap prevBmp = null;
                for (int i = 0; i < totalNum; i++)
                {
                    listBoxImgs.SelectedIndex = i;
                    string str = (string)listBoxImgs.Items[i];
                    Bitmap curBmp = new Bitmap(str);

                    if (prevBmp != null)
                    {
                        //Point ty = DiffVertical(prevBmp, curBmp);
                        Bitmap dif = Diff(prevBmp, curBmp);
                        dif.Save("RES\\" + i.ToString() + ".bmp");

                        /**
                        if (!ty.IsEmpty)
                        {
                            if (i > 1)
                            {
                                if (Math.Abs(ty.Y - ys[i - 2]) < distanceDiff)
                                {
                                    ys[i - 1] = ty.Y;
                                    xs[i - 1] = ty.X;
                                }
                                else
                                {
                                    ys[i - 1] = ys[i - 2];
                                    xs[i - 1] = xs[i - 2];
                                }
                            }
                        }
                        else if (i > 1)
                        {
                            ys[i - 1] = ys[i - 2];
                            xs[i - 1] = xs[i - 2];
                        }
                        else
                        {
                            ys[i - 1] = y1;
                            xs[i - 1] = 0;
                        }
                         * */
                    }
                    prevBmp = curBmp;
                }

                /*
                int chartheight = picBoxYChart.Height - 3;
                int imgheight = picBoxCur.Image.Height;
                double rate = (double)chartheight / (double)imgheight;
                Bitmap chartbmp = new Bitmap(totalNum - 1, picBoxYChart.Height);
                for (int i = 0; i < totalNum - 1; i++)
                {
                    chartbmp.SetPixel(i, (int)((imgheight - ys[i]) * rate) + 1, Color.Black);
                }
                picBoxYChart.Image = chartbmp;
                 * */
            }
            trackBarDiff.Enabled = true;
            started = false;
        }

        private int[] ys;
        private int[] xs;

        private unsafe Bitmap Diff(Bitmap pre, Bitmap cur)
        {
            Bitmap tarBMP = new Bitmap(cur.Width, cur.Height, cur.PixelFormat);

            BitmapData predata = pre.LockBits(new Rectangle(0, 0, pre.Width, pre.Height), ImageLockMode.ReadOnly, pre.PixelFormat);
            BitmapData curdata = cur.LockBits(new Rectangle(0, 0, cur.Width, cur.Height), ImageLockMode.ReadOnly, cur.PixelFormat);
            BitmapData tardata = tarBMP.LockBits(new Rectangle(0, 0, tarBMP.Width, tarBMP.Height), ImageLockMode.WriteOnly, tarBMP.PixelFormat);

            int pixLen = (curdata.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int widthLen = curdata.Width * pixLen;
            int n5 = curdata.Stride - widthLen;
            int height = curdata.Height;
            byte* numPtr = (byte*)predata.Scan0.ToPointer();
            byte* numPtr2 = (byte*)curdata.Scan0.ToPointer();
            byte* numTarPtr = (byte*)tardata.Scan0.ToPointer();
            for (int i = 0; i < height; i++)
            {
                int num8 = 0;
                while (num8 < widthLen)
                {
                    int num6 = numPtr[0] - numPtr2[0];
                    numTarPtr[0] = (num6 < 0) ? ((byte)-num6) : ((byte)num6);
                    num8++;
                    numPtr++;
                    numPtr2++;
                    numTarPtr++;
                }
                numPtr += n5;
                numPtr2 += n5;
                numTarPtr += n5;
            }
            pre.UnlockBits(predata);
            cur.UnlockBits(curdata);
            tarBMP.UnlockBits(tardata);

            return tarBMP;
        }

        private unsafe Point DiffVertical(Bitmap pre, Bitmap cur)
        {
            Point p = new Point();
            decimal diff = numUpDownDiff.Value;

            for (int y = y2; y > y1; y -= 2)
            {
                for (int x = x1; x < x2; x += 2)
                {
                    Color prec = pre.GetPixel(x, y);
                    Color curc = cur.GetPixel(x, y);
                    if (Math.Abs(curc.R - prec.R) > diff || Math.Abs(curc.G - prec.G) > diff || Math.Abs(curc.B - prec.B) > diff)
                    {
                        p.X = x;
                        p.Y = y;
                        return p;
                    }
                }
            }
            return p;
        }

        private double heightRate, widthRate;
        private int x1, x2, y1, y2;
        private void picBoxCur_MouseDown(object sender, MouseEventArgs e)
        {
            x1 = (int)(e.X * widthRate);
            y1 = (int)(e.Y * heightRate);
        }

        private void picBoxCur_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.X * widthRate < x1)
            {
                x2 = x1;
                x1 = (int)(e.X * widthRate);
            }
            else
                x2 = (int)(e.X * widthRate);

            if (e.Y * heightRate < y1)
            {
                y2 = y1;
                y1 = (int)(e.Y * heightRate);
            }
            else
                y2 = (int)(e.Y * heightRate);

            labelUL.Text = x1.ToString() + "," + y1.ToString();
            labelUR.Text = x2.ToString() + "," + y1.ToString();
            labelBL.Text = x1.ToString() + "," + y2.ToString();
            labelBR.Text = x2.ToString() + "," + y2.ToString();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            x1 = x2 = y1 = y2 = 0;

            labelUL.Text = x1.ToString() + "," + y1.ToString();
            labelUR.Text = x2.ToString() + "," + y1.ToString();
            labelBL.Text = x1.ToString() + "," + y2.ToString();
            labelBR.Text = x2.ToString() + "," + y2.ToString();
        }

        private void trackBarDiff_Scroll(object sender, EventArgs e)
        {
            int v = trackBarDiff.Value;
            if (ys == null)
                return;
            Bitmap diffbmp = new Bitmap(picBoxDiff.Width, picBoxDiff.Height);
            for (int i = 0; i < picBoxDiff.Width; i++)
            {
                diffbmp.SetPixel(i, (int)(ys[v] / heightRate), Color.Black);
            }
            picBoxDiff.Image = diffbmp;

            int totalNum = listBoxImgs.Items.Count;
            int chartheight = picBoxYChart.Height - 3;
            int imgheight = picBoxCur.Image.Height;
            double rate = (double)chartheight / (double)imgheight;
            Bitmap chartbmp = new Bitmap(totalNum - 1, picBoxYChart.Height);
            for (int i = 0; i < totalNum - 1; i++)
            {
                if (i == trackBarDiff.Value)
                    chartbmp.SetPixel(i, (int)((imgheight - ys[i]) * rate) + 1, Color.Yellow);
                else
                    chartbmp.SetPixel(i, (int)((imgheight - ys[i]) * rate) + 1, Color.Black);
            }
            picBoxYChart.Image = chartbmp;

            Bitmap curbmp = new Bitmap((string)listBoxImgs.Items[v + 1]);
            for (int i = 0; i < picBoxCur.Image.Width; i++)
            {
                if (i == xs[trackBarDiff.Value])
                    curbmp.SetPixel(i, (int)ys[v], Color.White);
                else
                    curbmp.SetPixel(i, (int)ys[v], Color.Black);
            }
            picBoxCur.Image = curbmp;
        }

        private void listBoxImgs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x2E)
            {
                listBoxImgs.Items.Remove(listBoxImgs.SelectedItem);
                if (listBoxImgs.Items.Count > 1)
                {
                    trackBarDiff.Maximum = listBoxImgs.Items.Count - 2;
                }
            }
        }
    }
}