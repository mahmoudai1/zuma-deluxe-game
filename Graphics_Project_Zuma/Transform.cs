using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TransformNS
{
    class Transform
    {
        public PointF doRotate(PointF pMe, PointF pRef, float speed)
        {
            PointF me2 = new PointF();
            me2.X = pMe.X - pRef.X;
            me2.Y = pMe.Y - pRef.Y;

            PointF me3 = new PointF();
            me3.X = (float)(me2.X * Math.Cos(speed) - me2.Y * Math.Sin(speed));
            me3.Y = (float)(me2.X * Math.Sin(speed) + me2.Y * Math.Cos(speed));

            pMe.X = me3.X + pRef.X;
            pMe.Y = me3.Y + pRef.Y;

            return pMe;
        }
    }
}
