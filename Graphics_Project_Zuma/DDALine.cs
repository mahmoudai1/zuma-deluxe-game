using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DDALineNS
{
    class DDALine
    {
        public float xs, ys, xe, ye;
        public bool flagStop = false;
    

        public DDALine(float xst, float yst, float xend, float yend)
        {
            xs = xst;
            ys = yst;
            xe = xend;
            ye = yend;
        }

        public PointF getNextPoint(float cx, float cy, int speed)
        {
            float dx = xe - xs;
            float dy = ye - ys;
            float m = dy / dx;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (xs < xe)
                {
                    cx += speed;
                    cy += m * speed;
                    if (cx >= xe)
                    {
                        flagStop = true;
                    }
                }
                else
                {
                    cx -= speed;
                    cy -= m * speed;
                    if (cx <= xe)
                    {
                        flagStop = true;
                    }
                }

            }
            if (Math.Abs(dx) < Math.Abs(dy))
            {
                if (ys < ye)
                {
                    cy += speed;
                    cx += (1 / m) * speed;
                    if (cy >= ye)
                    {
                        flagStop = true;
                    }
                }
                else
                {
                    cy -= speed;
                    cx -= (1 / m) * speed;
                    if (cy <= ye)
                    {
                        flagStop = true;
                    }
                }
            }
            PointF pnn = new PointF(cx, cy);
            return pnn;
        }
    }
}
