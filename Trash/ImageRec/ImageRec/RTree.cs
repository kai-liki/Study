using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ImageRec
{
    public class RNode
    {
        public int x = 0;
        public int y = 0;

        public bool reached = false;

        public byte R = 0;
        public byte G = 0;
        public byte B = 0;

        public RNode ul = null;
        public RNode u = null;
        public RNode ur = null;
        public RNode r = null;
        public RNode br = null;
        public RNode b = null;
        public RNode bl = null;
        public RNode l = null;


    }

    public class RTree
    {
        public RNode root;
        public int num_elem = 0;
        public PixelMatrix pm;

        private byte r;
        private byte g;
        private byte b;
        private int diffR;
        private int diffG;
        private int diffB;
        private int diffX;

        public RTree(PixelMatrix pm, RNode rt, byte tr, byte tg, byte tb, int dr, int dg, int db, int dx)
        {
            if (rt != null && pm != null)
            {
                this.pm = pm;
                this.root = rt;
                r = tr;
                g = tg;
                b = tb;
                diffR = dr;
                diffG = dg;
                diffB = db;
                diffX = dx;

                GetBranches(this.root);
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        private void GetBranches(RNode r)
        {
            r.reached = true;
            num_elem++;
            if (pm.IsInside(r.x, r.y - 1))
            {
                if (!pm.nodes[r.x, r.y - 1].reached)
                    GetOneBrance(r.x, r.y - 1, ref r.u);
            }

            if (pm.IsInside(r.x + 1, r.y - 1))
                if (!pm.nodes[r.x, r.y - 1].reached)
                    GetOneBrance(r.x + 1, r.y - 1, ref r.ur);

            if (pm.IsInside(r.x + 1, r.y))
                if (!pm.nodes[r.x + 1, r.y].reached)
                    GetOneBrance(r.x + 1, r.y, ref r.r);

            if (pm.IsInside(r.x + 1, r.y + 1))
                if (!pm.nodes[r.x + 1, r.y + 1].reached)
                    GetOneBrance(r.x + 1, r.y + 1, ref r.br);

            if (pm.IsInside(r.x, r.y + 1))
                if (!pm.nodes[r.x, r.y + 1].reached)
                    GetOneBrance(r.x, r.y + 1, ref r.b);

            if (pm.IsInside(r.x - 1, r.y + 1))
                if (!pm.nodes[r.x - 1, r.y + 1].reached)
                    GetOneBrance(r.x - 1, r.y + 1, ref r.bl);

            if (pm.IsInside(r.x - 1, r.y))
                if (!pm.nodes[r.x - 1, r.y].reached)
                    GetOneBrance(r.x - 1, r.y, ref r.l);

            if (pm.IsInside(r.x - 1, r.y - 1))
                if (!pm.nodes[r.x - 1, r.y - 1].reached)
                    GetOneBrance(r.x - 1, r.y - 1, ref r.ul);

            return;
        }

        private void GetOneBrance(int x, int y, ref RNode tr)
        {
            if (pm.IsInside(x, y))
            {
                RNode rn = pm.nodes[x, y];
                if (Verify(rn.R, rn.G, rn.B, r, g, b, diffR, diffG, diffB))
                {
                    tr = rn;
                    GetBranches(rn);
                    return;
                }
            }
        }

        public void DrawNodes(RNode r, Bitmap bmp)
        {
            if (r.u != null)
                DrawNodes(r.u, bmp);
            if (r.ur != null)
                DrawNodes(r.ur, bmp);
            if (r.r != null)
                DrawNodes(r.r, bmp);
            if (r.br != null)
                DrawNodes(r.br, bmp);
            if (r.b != null)
                DrawNodes(r.b, bmp);
            if (r.bl != null)
                DrawNodes(r.bl, bmp);
            if (r.l != null)
                DrawNodes(r.l, bmp);
            if (r.ul != null)
                DrawNodes(r.ul, bmp);

            bmp.SetPixel(r.x, r.y, Color.FromArgb(r.R, r.G, r.B));
        }

        static public VerifyDelegate Verify;
    }

    public class PixelMatrix
    {
        public int width, height;
        public RNode[,] nodes;

        public PixelMatrix(int x, int y)
        {
            width = x;
            height = y;
            this.nodes = new RNode[x, y];
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }

    public delegate bool VerifyDelegate(byte sr, byte sg, byte sb, byte tr, byte tg, byte tb, int dr, int dg, int db);
}
