 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BezCurveNS
{
    class BezCurve
    {
        public List<PointF> LCtrPts = new List<PointF>();

        public PointF calcCurvePointAtTime(float t)
        {
            PointF P = new PointF();
            int n = LCtrPts.Count - 1;
            float C;
            for (int i = 0; i < LCtrPts.Count; i++)
            {
                C = factorial(n) / (factorial(i) * factorial(n - i));
                P.X += (float)(Math.Pow(t, i) * Math.Pow(1 - t, n - i) * C * LCtrPts[i].X);
                P.Y += (float)(Math.Pow(t, i) * Math.Pow(1 - t, n - i) * C * LCtrPts[i].Y);
            }
            return P;
        }

        float factorial(int v)
        {
            float f = 1;
            for (int i = 2; i <= v; i++)
            {
                f *= i;
            }
            return f;
        }

        public void drawYourSelf(Graphics g, Brush b)
        {
            for (int i = 0; i < LCtrPts.Count; i++)
            {
                g.FillEllipse(b,
                                LCtrPts[i].X - 5,
                                LCtrPts[i].Y - 5,
                                10, 10);
            }
            for (float t = 0; t <= 1; t += 0.0001f)
            {
                PointF P = calcCurvePointAtTime(t);
                g.FillEllipse(b, P.X - 5, P.Y - 5, 10, 10);
                if(t <= 0.5)
                {
                    //g.DrawLine(Pens.Blue, P.X, P.Y, P.X, P.Y - 100);
                }
                else
                {
                    //g.DrawLine(Pens.Orange, P.X, P.Y, P.X, P.Y + 100);
                }
            }
        }
    }
}
