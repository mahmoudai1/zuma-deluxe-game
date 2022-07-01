using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DDALineNS;
using PolarCircleNS;
using BezCurveNS;
using TransformNS;
using System.IO;

namespace Graphics_Project_Zuma
{
    public partial class Form1 : Form
    {
        public class MovingBall
        {
            public Bitmap image;
            public float X, Y;
            public int ballIndex;
            public int whichCurve;
            public float curveTime;
            public float curveVirtualTime;

            public MovingBall(Bitmap pImage, float pX, float pY, int pWhichCurve, float pCurveTime, float pCurveVirtualTime, int pBallIndex)
            {
                image = pImage;
                X = pX;
                Y = pY;
                whichCurve = pWhichCurve;
                ballIndex = pBallIndex;
                curveTime = pCurveTime;
                curveVirtualTime = pCurveVirtualTime;
            }
        }

        public class FiredBall
        {
            public Bitmap image;
            public int ballIndex;
            public float X, Y;
            public int speed;

            public FiredBall(Bitmap pImage, float pX, float pY, int pBallIndex, int pSpeed)
            {
                image = pImage;
                X = pX;
                Y = pY;
                ballIndex = pBallIndex;
                speed = pSpeed;
            }
        }

        public class BonusRectangles
        {
            public int X, Y;
            public bool show = false;
            public BonusRectangles(int pX, int pY)
            {
                X = pX;
                Y = pY;
            }
        }

        public class BonusLines
        {
            public int X1, Y1, X2, Y2;
            public bool show = false;
            public BonusLines(int pX1, int pY1, int pX2, int pY2)
            {
                X1 = pX1;
                Y1 = pY1;
                X2 = pX2;
                Y2 = pY2;
            }
        }

        public class SpeedPower
        {
            public int X, Y, W, H;
            public Brush clr;
            public SpeedPower(int pX, int pY, int pW, int pH, Brush pClr)
            {
                X = pX;
                Y = pY;
                W = pW;
                H = pH;
                clr = pClr;
            }
        }

        public class AnimatedObjects
        {
            public float X, Y;
            public int W, H;
            public Brush brush;

            public AnimatedObjects(float pX, float pY, int pW, int pH, Brush pBrush)
            {
                X = pX;
                Y = pY;
                W = pW;
                H = pH;
                brush = pBrush;
            }
        }

        bool debugMode = false;
        bool gameOver = false;
        //bool userWon = false;

        Bitmap off;
        Timer T = new Timer();

        Bitmap bg = new Bitmap("bgMod-2.jpg");
        Bitmap zuma = new Bitmap("zuma.png");

        Bitmap[] selectedBalls = { new Bitmap("CuttedBalls/Blue.png"), new Bitmap("CuttedBalls/Green.png"), new Bitmap("CuttedBalls/Yellow.png"), 
                                  new Bitmap("CuttedBalls/Purple.png"), new Bitmap("CuttedBalls/Red.png"), new Bitmap("CuttedBalls/White.png"),
                                  new Bitmap("CuttedBalls/Stone.png"), new Bitmap("CuttedBalls/Bonus.png")};

        Bitmap[] pendingBalls = { new Bitmap("FullBalls/Blue.png"), new Bitmap("FullBalls/Green.png"), new Bitmap("FullBalls/Yellow.png"),
                                  new Bitmap("FullBalls/Purple.png"), new Bitmap("FullBalls/Red.png"), new Bitmap("FullBalls/White.png"),
                                  new Bitmap("FullBalls/Stone.png"), new Bitmap("FullBalls/Bonus.png")};

        Brush[] animatedBrushes = { Brushes.Blue, Brushes.LimeGreen, Brushes.Yellow, Brushes.Purple, Brushes.Red, Brushes.White, Brushes.Gray, Brushes.Black };
        List<AnimatedObjects> animatedObjects = new List<AnimatedObjects>();
        bool showAnimatedObjects = false;
        int ctAnimationTime = 0;

        int selectedBallIndex = 0;
        int pendingBallIndex = 0;
        int remainingBalls = 100;
        int remainingSwaps = 25;

        List<MovingBall> movingBalls = new List<MovingBall>();
        List<FiredBall> firedBalls = new List<FiredBall>();
        List<DDALine> firedBallsScopeLine = new List<DDALine>();
        float moveSpeed = 0.028f;
        int ballsLimit = 200;
        bool stopGenerating = false;
        PointF toBeFiredBallPoint = new PointF();
        int fixedFiredBallSpeed = 125;
        bool spaceIsPressed = false;
        bool isRearranging = false;
        int savedIndexForLastBallAfterCut = 0;

        SpeedPower speedoctangle = new SpeedPower(10, 120, 0, 30, Brushes.Red);

        List<BonusRectangles> bonusFilledRectangles = new List<BonusRectangles>();
        List<BonusRectangles> bonusDrawedRectangles = new List<BonusRectangles>();
        List<BonusLines> bonusLines = new List<BonusLines>();
        bool isBonusReady = false;
        bool bonusCharging = false;
        bool showBonusAnimation1 = false;
        int animation1RectanglesIndex = 0;
        bool showBonusAnimation2 = false;
        bool waitingToFireTheBonus = false;


        PolarCircle hiddenCircle = new PolarCircle(0, 0, 25);
        bool showHiddenCircle = true;
        List<DDALine> scopeLines = new List<DDALine>();
        int scopeLineDesign = 1;
        int scopeLinesNumber = 40;

        List<BezCurve> curves = new List<BezCurve>();
        int whichPointSelected = -1;
        int whichCurveSelected = -1;
        int whichCurve = -1;
        Brush[] brushes = { Brushes.Red, Brushes.Yellow, Brushes.Green, Brushes.Blue, Brushes.Purple };

        bool iDrag = false;
        bool iDrag2 = false;
        int ctTimer = 0;
        int prevX, prevY;
        int zumaX = 0, zumaY = 0;
        int zumaAngle = 0;
        int zumaRotationSpeed = 5;

        int zumaLookAngle = 270;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Paint += Form1_Paint;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            T.Tick += T_Tick;
            T.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.Width, this.Height);
            zumaX = (this.Width / 2) - 350;
            zumaY = (this.Height / 2) - 120;
            hiddenCircle.xc = (this.Width / 2) - 350 + (zuma.Width / 2);
            hiddenCircle.yc = (this.Height / 2) - 120 + (zuma.Height / 2);

            setScopeLines();

            toBeFiredBallPoint.X = hiddenCircle.getNextPoint(zumaLookAngle).X - 30;
            toBeFiredBallPoint.Y = hiddenCircle.getNextPoint(zumaLookAngle).Y - 30;

            Random random = new Random();
            selectedBallIndex = 0;
            pendingBallIndex = random.Next(0, pendingBalls.Length - 1);

            placeTheCurvePoints();

            this.Text = " Mahmoud's Project";
            label2.Text = "Remaining Ball(s): " + remainingBalls;
            label4.Text = "Remaining Swaps: " + remainingSwaps;

            createBonusRectangles();

            /*
            //Directory d = new Directory(@"C:\Users\mahmoud\Desktop\Zumas1");
            string folder = @"C:\Users\mahmoud\Desktop\Zumas1";
            var files = Directory.GetFiles(folder, "*.png").OrderBy(f => f);
            int i = 99;
            foreach (var file in files)
            {
                //System.IO.File.Move(f.Name, i + ".png");
                //f.MoveTo(@"C:\Users\mahmoud\Desktop\Zumas2\" + i + ".png");
                string fileName = Path.GetFileName(file);
                Console.WriteLine(fileName);
                i++;
            }
            */
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawDouble(e.Graphics);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if(debugMode)
                    curves[whichCurve].LCtrPts.Add(new PointF(e.X, e.Y));
                else
                {
                    iDrag = true;
                    prevX = e.X;
                    prevY = e.Y;
                }
            }

            if(e.Button == MouseButtons.Right && remainingSwaps > 0)
            {
                if (debugMode)
                {
                    for (int k = 0; k < curves.Count; k++)
                    {
                        for (int i = 0; i < curves[k].LCtrPts.Count; i++)
                        {
                            if (e.X > curves[k].LCtrPts[i].X - 10 && e.X < curves[k].LCtrPts[i].X + 10
                                 && e.Y > curves[k].LCtrPts[i].Y - 10 && e.Y < curves[k].LCtrPts[i].Y + 10)
                            {
                                iDrag2 = true;
                                prevX = e.X;
                                prevY = e.Y;
                                whichCurveSelected = k;
                                whichPointSelected = i;
                                whichCurve = k;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    swapColors();
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(debugMode)
            {
                label3.Visible = true;
                label3.Text = e.X + "  " + e.Y;
            }
            else
            {
                label3.Visible = false;
            }

            if (iDrag)
            {
                int dx = e.X - prevX;
                int dy = e.Y - prevY;

                if(e.X > prevX)
                {
                    zumaAngle += dx;
                    zumaLookAngle += dx;
                    prevX = e.X;
                }
                if (e.X < prevX)
                {
                    zumaAngle += dx;
                    zumaLookAngle += dx;
                    prevX = e.X;
                }
                if(e.Y > prevY)
                {
                    zumaAngle += dy;
                    zumaLookAngle += dy;
                    prevY = e.Y;
                }
                if (e.Y < prevY)
                {
                    zumaAngle += dy;
                    zumaLookAngle += dy;
                    prevY = e.Y;
                }
                afterChangingAngleActions();
            }
            if(iDrag2 && debugMode)
            {
                curves[whichCurveSelected].LCtrPts[whichPointSelected] = new PointF(e.X, e.Y);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            iDrag = false;

            if(debugMode)
                iDrag2 = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && remainingSwaps > 0)
            {
                swapColors();
            }
            if(e.KeyCode == Keys.Right)
            {
                zumaAngle += zumaRotationSpeed;
                zumaLookAngle += zumaRotationSpeed;
                afterChangingAngleActions();
            }
            if(e.KeyCode == Keys.Left)
            {
                zumaAngle -= zumaRotationSpeed;
                zumaLookAngle -= zumaRotationSpeed;
                afterChangingAngleActions();
            }
            if(e.KeyCode == Keys.S)
            {
                //scopeLines.Clear();
                //scopeLineDesign *= -1;
                //setScopeLines();
            }
            if(e.KeyCode == Keys.N && debugMode)
            {
                curves.Add(new BezCurve());
                whichCurve = curves.Count - 1;
            }
            if(e.KeyCode == Keys.P && debugMode)
            {
                if (whichCurve >= 0)
                {
                    Console.WriteLine("Curve " + whichCurve + ":");
                    for (int i = 0; i < curves[whichCurve].LCtrPts.Count; i++)
                    {
                        Console.WriteLine("curves[whichCurve].LCtrPts.Add(new PointF(" + curves[whichCurve].LCtrPts[i].X + ", " + curves[whichCurve].LCtrPts[i].Y + "));");
                    }
                }
            }
            if(e.KeyCode == Keys.D)
            {
                debugMode = debugMode ? false : true;
                label1.Visible = debugMode ? true : false;
            }
            if(e.KeyCode == Keys.Space && remainingBalls >= 1 && !bonusCharging && !gameOver)
            {
                spaceIsPressed = true;
                fixedFiredBallSpeed += 10;
                for (int i = 0; i < 6; i++)
                {
                    speedoctangle.W++;
                    if (speedoctangle.W >= 0) speedoctangle.clr = Brushes.Red;
                    if (speedoctangle.W >= 50) speedoctangle.clr = Brushes.Yellow;
                    if (speedoctangle.W >= 100) speedoctangle.clr = Brushes.LimeGreen;
                    if (speedoctangle.W >= 150)
                    {
                        speedoctangle.W = 0;
                        fixedFiredBallSpeed = 125;
                    }
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (spaceIsPressed)
            {
                float thRadian1 = (float)((zumaLookAngle) * Math.PI / 180);

                float xs = (float)(60 * Math.Cos(thRadian1)) + hiddenCircle.xc;  // the radius for the line that comes out from zuma's mouth
                float ys = (float)(60 * Math.Sin(thRadian1)) + hiddenCircle.yc;
                float xe = (float)(1000 * Math.Cos(thRadian1)) + hiddenCircle.xc; // the radius for the line in the outer environment
                float ye = (float)(1000 * Math.Sin(thRadian1)) + hiddenCircle.yc;

                firedBallsScopeLine.Add(new DDALine(xs, ys, xe, ye));
                firedBalls.Add(new FiredBall(pendingBalls[selectedBallIndex], toBeFiredBallPoint.X, toBeFiredBallPoint.Y, selectedBallIndex, fixedFiredBallSpeed));

                Random random = new Random();
                selectedBallIndex = pendingBallIndex;
                pendingBallIndex = random.Next(0, pendingBalls.Length);
                if (selectedBallIndex == 7)
                {
                    isBonusReady = true;
                    bonusCharging = true;
                    showBonusAnimation1 = true;
                }
                else
                {
                    isBonusReady = false;
                    bonusCharging = false;
                    showBonusAnimation1 = false;
                    showBonusAnimation2 = false;
                    waitingToFireTheBonus = false;
                    animation1RectanglesIndex = 0;
                    for (int i = 0; i < bonusFilledRectangles.Count; i++)
                    {
                        bonusFilledRectangles[i].show = false;
                        bonusLines[i].show = false;
                    }
                }
                remainingBalls--;
                label2.Text = "Remaining Ball(s): " + remainingBalls;
                label2.ForeColor = Color.White;


                spaceIsPressed = false;
                speedoctangle.W = 0;
                speedoctangle.clr = Brushes.Red;
                fixedFiredBallSpeed = 125;
            }
            
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if(movingBalls.Count > ballsLimit)
            {
                stopGenerating = true;
            }
            if (!gameOver && !stopGenerating && !isRearranging)
            {
                if (ctTimer >= 22)
                {
                    moveSpeed = 0.0022f;
                    if (ctTimer % 12 == 0)
                        generateBalls();
                }
                else
                {
                    generateBalls();
                }
            }

            for(int i = 0; i < firedBalls.Count; i++)
            {
                int f = 0;
                for (int k = 0; k < firedBalls[i].speed; k++)
                {
                    firedBalls[i].X = firedBallsScopeLine[i].getNextPoint(firedBalls[i].X, firedBalls[i].Y, 1).X;
                    firedBalls[i].Y = firedBallsScopeLine[i].getNextPoint(firedBalls[i].X, firedBalls[i].Y, 1).Y;
                    if (checkIfNear(firedBalls[i], i))
                    {
                        f = 1;
                        break;
                    }
                }
                if (f == 0)
                {
                    if (firedBalls[i].X > this.Width || firedBalls[i].X + pendingBalls[0].Width < 0
                        || firedBalls[i].Y > this.Height || firedBalls[i].Y + pendingBalls[0].Height < 0)
                    {
                        firedBalls.Remove(firedBalls[i]);
                        firedBallsScopeLine.Remove(firedBallsScopeLine[i]);
                    }
                }
            }

            if(isBonusReady && bonusCharging)
            {
                if(ctTimer % 2 == 0)
                {
                    bonusFilledRectangles[animation1RectanglesIndex].show = true;
                    bonusLines[animation1RectanglesIndex].show = true;
                    animation1RectanglesIndex++;
                    if(animation1RectanglesIndex > bonusFilledRectangles.Count - 1)
                    {
                        bonusCharging = false;
                        waitingToFireTheBonus = true;
                        showBonusAnimation2 = true;
                        animation1RectanglesIndex = 0;
                    }
                }
            }

            if(waitingToFireTheBonus)
            {
                if(ctTimer % 4 == 0)
                {
                    showBonusAnimation2 = showBonusAnimation2 ? false : true;
                }
            }

            if (movingBalls.Count > 0)
                moveTheBalls();

            if (isRearranging)
            {
                if (savedIndexForLastBallAfterCut - 1 >= 0)
                {
                    //checkIfWentBackEnough();
                }
                else
                {
                    isRearranging = false;
                }
            }

            if(showAnimatedObjects)
            {
                for(int i = 0; i < animatedObjects.Count; i++)
                {
                    Random r = new Random();
                    int rValue = r.Next(1, 9);
                    animatedObjects[i].W += rValue;
                    animatedObjects[i].H += rValue;
                }
                ctAnimationTime++;
                if(ctAnimationTime > 3)
                {
                    ctAnimationTime = 0;
                    showAnimatedObjects = false;
                }
            }

            if(movingBalls.Count > 0 && !isRearranging)
            {
                fixGaps();
            }

            if(!isRearranging)
                ctTimer++;

            drawDouble(this.CreateGraphics());
        }

        public void swapColors()
        {
            Random random = new Random();
            selectedBallIndex = pendingBallIndex;
            pendingBallIndex = random.Next(0, pendingBalls.Length);
            if (selectedBallIndex == 7)
            {
                isBonusReady = true;
                bonusCharging = true;
                showBonusAnimation1 = true;
            }
            else
            {
                isBonusReady = false;
                bonusCharging = false;
                showBonusAnimation1 = false;
                showBonusAnimation2 = false;
                waitingToFireTheBonus = false;
                animation1RectanglesIndex = 0;
                for (int i = 0; i < bonusFilledRectangles.Count; i++)
                {
                    bonusFilledRectangles[i].show = false;
                    bonusLines[i].show = false;
                }
            }
            remainingBalls--;
            remainingSwaps--;
            label2.Text = "Remaining Ball(s): " + remainingBalls;
            label4.Text = "Remaining Swaps: " + remainingSwaps;
            label2.ForeColor = Color.White;
            
        }

        public void setScopeLines()
        {
            if (scopeLineDesign == 1)
            {
                int vx = 20;
                for (int i = 0; i < scopeLinesNumber; i++)
                {
                    float thRadian1 = (float)((zumaLookAngle - vx) * Math.PI / 180);
                    float thRadian2 = (float)((zumaLookAngle) * Math.PI / 180);     // Depending on the angle rathar depending on variable x, y thats bugs while rotating

                    float x1 = (int)(80 * Math.Cos(thRadian1)) + hiddenCircle.xc;  // the radius for the line that comes out from zuma's mouth
                    float y1 = (int)(80 * Math.Sin(thRadian1)) + hiddenCircle.yc;

                    float x2 = (int)(200 * Math.Cos(thRadian2)) + hiddenCircle.xc; // the radius for the line in the outer environment
                    float y2 = (int)(200 * Math.Sin(thRadian2)) + hiddenCircle.yc;

                    scopeLines.Add(new DDALine(x1, y1, x2, y2));
                    vx -= 1;
                }
            }
        }

        public void moveScopeLines()
        {
            if (scopeLineDesign == 1)
            {
                int vx = 20;
                for (int i = 0; i < scopeLinesNumber; i++)
                {
                    float thRadian1 = (float)((zumaLookAngle - vx) * Math.PI / 180);
                    float thRadian2 = (float)((zumaLookAngle) * Math.PI / 180);     // Depending on the angle rathar depending on variable x, y thats bugs while rotating

                    float x1 = (int)(80 * Math.Cos(thRadian1)) + hiddenCircle.xc;  // the radius for the line that comes out from zuma's mouth
                    float y1 = (int)(80 * Math.Sin(thRadian1)) + hiddenCircle.yc;

                    float x2 = (int)(200 * Math.Cos(thRadian2)) + hiddenCircle.xc; // the radius for the line in the outer environment
                    float y2 = (int)(200 * Math.Sin(thRadian2)) + hiddenCircle.yc;

                    scopeLines[i].xs = x1;
                    scopeLines[i].ys = y1;
                    scopeLines[i].xe = x2;
                    scopeLines[i].ye = y2;
                    vx--;
                }
            }
        }

        public void placeTheCurvePoints()
        {
            curves.Add(new BezCurve());
            whichCurve++;
            //Curve 1:
            curves[whichCurve].LCtrPts.Add(new PointF(885, -1));
            curves[whichCurve].LCtrPts.Add(new PointF(1089, 83));
            curves[whichCurve].LCtrPts.Add(new PointF(1231, 151));
            curves[whichCurve].LCtrPts.Add(new PointF(1465, 292));
            curves[whichCurve].LCtrPts.Add(new PointF(1634, 437));
            curves[whichCurve].LCtrPts.Add(new PointF(1677, 508));
            curves[whichCurve].LCtrPts.Add(new PointF(1680, 806));
            curves[whichCurve].LCtrPts.Add(new PointF(1552, 842));
            curves[whichCurve].LCtrPts.Add(new PointF(1399, 910));
            curves[whichCurve].LCtrPts.Add(new PointF(1223, 948));

            curves.Add(new BezCurve());
            whichCurve++;
            // Curve 2:
            curves[whichCurve].LCtrPts.Add(new PointF(1224, 947));
            curves[whichCurve].LCtrPts.Add(new PointF(1012, 988));
            curves[whichCurve].LCtrPts.Add(new PointF(740, 1011));
            curves[whichCurve].LCtrPts.Add(new PointF(554, 977));
            curves[whichCurve].LCtrPts.Add(new PointF(346, 888));
            curves[whichCurve].LCtrPts.Add(new PointF(68, 840));
            curves[whichCurve].LCtrPts.Add(new PointF(0, 690));
            curves[whichCurve].LCtrPts.Add(new PointF(35, 429));
            curves[whichCurve].LCtrPts.Add(new PointF(165, 276));
            curves[whichCurve].LCtrPts.Add(new PointF(408, 182));

            curves.Add(new BezCurve());
            whichCurve++;
            // Curve 3:
            curves[whichCurve].LCtrPts.Add(new PointF(408, 182));
            curves[whichCurve].LCtrPts.Add(new PointF(629, 141));
            curves[whichCurve].LCtrPts.Add(new PointF(817, 188));
            curves[whichCurve].LCtrPts.Add(new PointF(1025, 233));
            curves[whichCurve].LCtrPts.Add(new PointF(1239, 317));
            curves[whichCurve].LCtrPts.Add(new PointF(1534, 536));
            curves[whichCurve].LCtrPts.Add(new PointF(1641, 585));
            curves[whichCurve].LCtrPts.Add(new PointF(1545, 890));
            curves[whichCurve].LCtrPts.Add(new PointF(1125, 851));
            curves[whichCurve].LCtrPts.Add(new PointF(1104, 874));
            curves[whichCurve].LCtrPts.Add(new PointF(902, 872));

            curves.Add(new BezCurve());
            whichCurve++;
            // Curve 4:
            curves[whichCurve].LCtrPts.Add(new PointF(904, 873));
            curves[whichCurve].LCtrPts.Add(new PointF(754, 873));
            curves[whichCurve].LCtrPts.Add(new PointF(663, 843));
            curves[whichCurve].LCtrPts.Add(new PointF(335, 810));
            curves[whichCurve].LCtrPts.Add(new PointF(21, 757));
            curves[whichCurve].LCtrPts.Add(new PointF(14, 375));
            curves[whichCurve].LCtrPts.Add(new PointF(184, 307));
            curves[whichCurve].LCtrPts.Add(new PointF(541, 151));
            curves[whichCurve].LCtrPts.Add(new PointF(688, 252));
            curves[whichCurve].LCtrPts.Add(new PointF(866, 340));
            curves[whichCurve].LCtrPts.Add(new PointF(1247, 407));
            curves[whichCurve].LCtrPts.Add(new PointF(1200, 639));
        }

        public void afterChangingAngleActions()
        {
            if (zumaLookAngle > 360) zumaLookAngle = zumaLookAngle - 360;
            if (zumaLookAngle < 0) zumaLookAngle = 360 + zumaLookAngle;

            if (zumaAngle > 360) zumaAngle = zumaAngle - 360;
            if (zumaAngle < 0) zumaAngle = 360 + zumaAngle;

            moveScopeLines();

            toBeFiredBallPoint.X = hiddenCircle.getNextPoint(zumaLookAngle).X - 30;
            toBeFiredBallPoint.Y = hiddenCircle.getNextPoint(zumaLookAngle).Y - 30;

            label1.Text = "Zuma Angle = " + zumaAngle + "  " + "Zuma Look Angle = " + zumaLookAngle;
        }

        public void generateBalls()
        {
            if (curves.Count > 0)
            {
                Random random = new Random();
                int ballIndex = random.Next(0, pendingBalls.Length - 2);
                movingBalls.Add(new MovingBall(pendingBalls[ballIndex], 
                                    curves[0].calcCurvePointAtTime(0).X, 
                                    curves[0].calcCurvePointAtTime(0).Y, 
                                    0, 
                                    0,
                                    0,
                                    ballIndex));
            }
        }

        public void moveTheBalls()
        {
            for(int i = 0; i < curves.Count; i++)
            {
                for(int j = 0; j < movingBalls.Count; j++)
                {
                    if (movingBalls[j].whichCurve == i)
                    {
                        if (isRearranging)
                        {
                            if (j < savedIndexForLastBallAfterCut)
                            {
                                movingBalls[j].X = curves[i].calcCurvePointAtTime(movingBalls[j].curveTime).X - 35;
                                movingBalls[j].Y = curves[i].calcCurvePointAtTime(movingBalls[j].curveTime).Y - 35;

                                movingBalls[j].curveTime -= 0.01f;
                                movingBalls[j].curveVirtualTime -= 0.01f;

                                //checkIfExceededTheCurrentCurve(movingBalls[j]);
                                checkIfRecededTheCurrentCurve(movingBalls[j]);

                                if (checkIfWentBackEnough())
                                {
                                    break;
                                }
                            
                            }
                        }
                        else
                        {
                            movingBalls[j].X = curves[i].calcCurvePointAtTime(movingBalls[j].curveTime).X - 35;
                            movingBalls[j].Y = curves[i].calcCurvePointAtTime(movingBalls[j].curveTime).Y - 35;

                            movingBalls[j].curveTime += moveSpeed;
                            movingBalls[j].curveVirtualTime += moveSpeed;

                            //checkIfRecededTheCurrentCurve(movingBalls[j]);
                            checkIfExceededTheCurrentCurve(movingBalls[j]);
                        }
                    }
                }
            }
        }

        public bool checkIfNear(FiredBall firedBall, int firedBallindex)
        {
            for(int i = 0; i < movingBalls.Count; i++)
            {
                if(firedBall.X > movingBalls[i].X && firedBall.X < movingBalls[i].X + 60
                    && firedBall.Y > movingBalls[i].Y && firedBall.Y < movingBalls[i].Y + 60)
                {
                    for (int j = i; j >= 0; j--)
                    {
                        for (float f = 0; f < 14; f++)
                        {
                            movingBalls[j].curveTime += 0.0024f;
                            movingBalls[j].curveVirtualTime += 0.0024f;
                            checkIfExceededTheCurrentCurve(movingBalls[j]);
                        }
                    }

                    movingBalls.Insert(i + 1, new MovingBall(firedBall.image, movingBalls[i].X, movingBalls[i].Y, movingBalls[i].whichCurve, movingBalls[i].curveTime - 0.03f, movingBalls[i].curveVirtualTime - 0.03f, firedBall.ballIndex));
                    // Never put (i) instead of (i + 1) above!!!! too many bugs will occur :D

                    int k = i + 1;
                    int ctSimilar = 0;
                    int flag = 0;

                    if (true)
                    {
                        while (movingBalls[k].ballIndex == firedBall.ballIndex)
                        {
                            if (k + 1 < movingBalls.Count)
                            {
                                k++;
                                flag = 1;
                            }
                            else
                            {
                                flag = 0;
                                break;
                            }
                        }
                        if (flag == 1) { k--; }

                        int savedK = k;
                        while (movingBalls[k].ballIndex == firedBall.ballIndex)
                        {
                            if (k - 1 >= 0)
                            {
                                k--;
                                ctSimilar++;
                            }
                            else
                            {
                                ctSimilar++;
                                break;
                            }
                        }
                        
                        if (ctSimilar > 2 && firedBall.ballIndex != 6 && firedBall.ballIndex != 7)
                        {
                            savedIndexForLastBallAfterCut = savedK - ctSimilar + 1;
                            isRearranging = true;
                            animateOnRemovingSimilarBalls(firedBall.X, firedBall.Y, animatedBrushes[firedBall.ballIndex], firedBall.ballIndex);
                            for (int j = 0; j < ctSimilar; j++)
                            {
                                movingBalls.Remove(movingBalls[savedK]);
                                savedK--;
                            }
                            remainingBalls += (5 * ctSimilar);
                            label2.Text = "Remaining Ball(s): " + remainingBalls;
                            label2.ForeColor = Color.Yellow;
                        }

                        if (firedBall.ballIndex == 7)
                        {
                            savedIndexForLastBallAfterCut = i - 3;
                            isRearranging = true;
                            animateOnRemovingSimilarBalls(firedBall.X, firedBall.Y, animatedBrushes[firedBall.ballIndex], firedBall.ballIndex);
                            for (int j = i + 3; j >= i - 3; j--)
                            {
                                if (j >= 0 && j < movingBalls.Count)
                                    movingBalls.Remove(movingBalls[j]);
                            }
                            remainingBalls += (5 * 7);
                            label2.Text = "Remaining Ball(s): " + remainingBalls;
                            label2.ForeColor = Color.Yellow;
                        }
                    }

                    firedBalls.Remove(firedBall);
                    firedBallsScopeLine.Remove(firedBallsScopeLine[firedBallindex]);
                    return true;
                }
            }
            return false;
        }

        public void checkIfRecededTheCurrentCurve(MovingBall movingBall)
        {
            if (movingBall.curveTime < 0)
            {
                movingBall.whichCurve--;
                movingBall.curveTime = 0.999f;
                //movingBall.curveVirtualTime = whichCurve + 1; //
            }
        }

        public void checkIfExceededTheCurrentCurve(MovingBall movingBall)
        {
            if (movingBall.curveTime > 1)
            {
                movingBall.whichCurve++;
                movingBall.curveTime = 0;
                //movingBall.curveVirtualTime = whichCurve; //
                if (movingBall.whichCurve > curves.Count - 1)
                {
                    gameOver = true;
                    movingBalls.Remove(movingBall);
                    moveSpeed = 0.05f;
                }
            }
        }

        public bool checkIfWentBackEnough()
        {
            if (savedIndexForLastBallAfterCut < movingBalls.Count)
            {
                if (movingBalls[savedIndexForLastBallAfterCut - 1].curveVirtualTime - movingBalls[savedIndexForLastBallAfterCut].curveVirtualTime <= 0.07f)
                {
                    isRearranging = false;
                    for (int i = 0; i < savedIndexForLastBallAfterCut; i++)
                    {
                        movingBalls[i].curveTime -= 0.038f;
                        movingBalls[i].curveVirtualTime -= 0.038f;
                        checkIfRecededTheCurrentCurve(movingBalls[i]);
                    }
                    return true;
                }
            }
            return false;
        }

        public void createBonusRectangles()
        {
            int vX = 441, vY = 428;

            bonusFilledRectangles.Add(new BonusRectangles(vX, vY));
            bonusFilledRectangles.Add(new BonusRectangles(vX - 63, vY + 53));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[0].X + 2, bonusFilledRectangles[0].Y + 25, bonusFilledRectangles[1].X + 25, bonusFilledRectangles[1].Y + 2));

            bonusFilledRectangles.Add(new BonusRectangles(vX - (63 * 2) + 2, vY + (53 * 2) + 5));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[1].X + 2, bonusFilledRectangles[1].Y + 25, bonusFilledRectangles[2].X + 25, bonusFilledRectangles[2].Y + 2));

            bonusFilledRectangles.Add(new BonusRectangles(vX - (63) + 3, vY + (53 * 3) + 13));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[2].X + 25, bonusFilledRectangles[2].Y + 25, bonusFilledRectangles[3].X + 2, bonusFilledRectangles[3].Y + 2));

            bonusFilledRectangles.Add(new BonusRectangles(vX + 11, vY + (53 * 4) + 13));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[3].X + 25, bonusFilledRectangles[3].Y + 25, bonusFilledRectangles[4].X + 2, bonusFilledRectangles[4].Y + 2));


            vX = 812;
            vY = 430;

            bonusFilledRectangles.Add(new BonusRectangles(vX - 11, vY + (53 * 4) + 13));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[4].X + 25, bonusFilledRectangles[4].Y + 24, bonusFilledRectangles[5].X, bonusFilledRectangles[5].Y + 25));

            bonusFilledRectangles.Add(new BonusRectangles(vX + (63) - 2, vY + (53 * 3) + 13));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[5].X + 25, bonusFilledRectangles[5].Y, bonusFilledRectangles[6].X, bonusFilledRectangles[6].Y + 25));

            bonusFilledRectangles.Add(new BonusRectangles(vX + (63 * 2) - 1, vY + (53 * 2) + 5));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[6].X + 25, bonusFilledRectangles[6].Y, bonusFilledRectangles[7].X, bonusFilledRectangles[7].Y + 25));

            bonusFilledRectangles.Add(new BonusRectangles(vX + 63, vY + 53));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[7].X, bonusFilledRectangles[7].Y, bonusFilledRectangles[8].X + 25, bonusFilledRectangles[8].Y + 25));

            bonusFilledRectangles.Add(new BonusRectangles(vX, vY));
            bonusLines.Add(new BonusLines(bonusFilledRectangles[8].X, bonusFilledRectangles[8].Y, bonusFilledRectangles[9].X + 25, bonusFilledRectangles[9].Y + 25));

            bonusLines.Add(new BonusLines(bonusFilledRectangles[9].X + 25, bonusFilledRectangles[9].Y, bonusFilledRectangles[0].X, bonusFilledRectangles[0].Y));

            vX = 420;
            vY = 410;

            bonusDrawedRectangles.Add(new BonusRectangles(vX, vY));
            bonusDrawedRectangles.Add(new BonusRectangles(vX - 63, vY + 53));
            bonusDrawedRectangles.Add(new BonusRectangles(vX - (63 * 2) + 2, vY + (53 * 2) + 8));
            bonusDrawedRectangles.Add(new BonusRectangles(vX - (63) + 3, vY + (53 * 3) + 13));
            bonusDrawedRectangles.Add(new BonusRectangles(vX + 11, vY + (53 * 4) + 13));

            vX = 791;
            vY = 412;

            bonusDrawedRectangles.Add(new BonusRectangles(vX, vY));
            bonusDrawedRectangles.Add(new BonusRectangles(vX + 63, vY + 53));
            bonusDrawedRectangles.Add(new BonusRectangles(vX + (63 * 2) - 1, vY + (53 * 2) + 8));
            bonusDrawedRectangles.Add(new BonusRectangles(vX + (63) - 2, vY + (53 * 3) + 13));
            bonusDrawedRectangles.Add(new BonusRectangles(vX - 11, vY + (53 * 4) + 13));

        }

        public void animateOnRemovingSimilarBalls(float animateX, float animateY, Brush brush, int ballColorIndex)
        {
            animateX -= 5;
            animateY -= 5;
            if (animatedObjects.Count == 0)
            {
                animatedObjects.Add(new AnimatedObjects(animateX, animateY, 0, 0, brush));
                animatedObjects.Add(new AnimatedObjects(animateX + 50, animateY, 0, 0, brush));
                animatedObjects.Add(new AnimatedObjects(animateX + 30, animateY + 45, 0, 0, brush));
                animatedObjects.Add(new AnimatedObjects(animateX + 100, animateY + 25, 0, 0, brush));
                animatedObjects.Add(new AnimatedObjects(animateX + 70, animateY - 30, 0, 0, brush));
                animatedObjects.Add(new AnimatedObjects(animateX + 25, animateY + 60, 0, 0, brush));
                animatedObjects.Add(new AnimatedObjects(animateX + 45, animateY - 60, 0, 0, brush));

            }
            else
            {
                animatedObjects[0].X = animateX; animatedObjects[0].Y = animateY;
                animatedObjects[1].X = animateX + 50; animatedObjects[1].Y = animateY;
                animatedObjects[2].X = animateX + 30; animatedObjects[2].Y = animateY + 45;
                animatedObjects[3].X = animateX + 100; animatedObjects[3].Y = animateY + 25;
                animatedObjects[4].X = animateX + 70; animatedObjects[4].Y = animateY - 30;
                animatedObjects[5].X = animateX + 25; animatedObjects[5].Y = animateY + 60;
                animatedObjects[6].X = animateX + 45; animatedObjects[6].Y = animateY - 60;

                for (int i = 0; i < animatedObjects.Count; i++)
                {
                    animatedObjects[i].W = 0;
                    animatedObjects[i].H = 0;
                    animatedObjects[i].brush = brush;
                }
            
            }

            showAnimatedObjects = true;
        }

        // -- //
        public void fixGaps()
        {
            for (int i = 0; i < movingBalls.Count - 1; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (movingBalls[i].curveVirtualTime - movingBalls[i + 1].curveVirtualTime > 0.035f)
                    {
                        movingBalls[i].curveTime -= 0.01f;
                        movingBalls[i].curveVirtualTime -= 0.01f;
                        checkIfRecededTheCurrentCurve(movingBalls[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < movingBalls.Count - 1; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (movingBalls[i].curveVirtualTime - movingBalls[i + 1].curveVirtualTime < 0.020f)
                    {
                    
                        movingBalls[i].curveTime += 0.01f;
                        movingBalls[i].curveVirtualTime += 0.01f;
                        checkIfExceededTheCurrentCurve(movingBalls[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void drawScene(Graphics g)
        {
            g.DrawImage(bg, 0, 0, this.Width, this.Height);

            if (debugMode)
            {
                for (int i = 0; i < curves.Count; i++)
                    curves[i].drawYourSelf(g, brushes[i]);

                if (showHiddenCircle)
                    hiddenCircle.drawCircle(g, 1, 0);

                float thRadian = (float)((zumaLookAngle - 20) * Math.PI / 180);
                float thRadian2 = (float)((zumaLookAngle + 20) * Math.PI / 180);  // Depending on the angle rathar depending on variable x, y thats bugs while rotating

                float x1 = (int)(80 * Math.Cos(thRadian)) + hiddenCircle.xc - 7;  // the radius for the line that comes out from zuma's mouth
                float y1 = (int)(80 * Math.Sin(thRadian)) + hiddenCircle.yc;
                float x2 = (int)(80 * Math.Cos(thRadian2)) + hiddenCircle.xc;  // the radius for the line that comes out from zuma's mouth
                float y2 = (int)(80 * Math.Sin(thRadian2)) + hiddenCircle.yc;

                g.FillEllipse(Brushes.Red, x1, y1, 7, 7);
                g.FillEllipse(Brushes.Red, x2, y2, 7, 7);


                g.FillEllipse(Brushes.Green, toBeFiredBallPoint.X, toBeFiredBallPoint.Y, 60, 60);

                for (int i = 0; i < firedBalls.Count; i++)
                {
                    g.DrawLine(Pens.Red, firedBallsScopeLine[i].xs, firedBallsScopeLine[i].ys, firedBallsScopeLine[i].xe, firedBallsScopeLine[i].ye);
                }
            }

            for (int i = 0; i < movingBalls.Count; i++)
            {
                g.DrawImage(movingBalls[i].image, movingBalls[i].X, movingBalls[i].Y);
                if (debugMode)
                {
                    g.DrawString(i + " ", new Font("Calibri", 18f, FontStyle.Bold), Brushes.Black, movingBalls[i].X + 20, movingBalls[i].Y + 5);
                    g.DrawString(movingBalls[i].ballIndex + " ", new Font("Calibri", 18f, FontStyle.Bold), Brushes.Black, movingBalls[i].X + 20, movingBalls[i].Y + 35);
                }
            }

            for (int i = 0; i < scopeLines.Count; i++)
            {
                Pen p;
                if (i % 2 == 0)
                    p = new Pen(Color.GreenYellow, 2);
                else
                    p = new Pen(Color.LightGreen, 2);

                g.DrawLine(p, scopeLines[i].xs, scopeLines[i].ys, scopeLines[i].xe, scopeLines[i].ye);
            }

            for (int i = 0; i < firedBalls.Count; i++)
            {
                g.DrawImage(firedBalls[i].image, firedBalls[i].X, firedBalls[i].Y);
            }

            if (isBonusReady)
            {
                if (showBonusAnimation1)
                {
                    Pen p = new Pen(Color.Yellow, 4);
                    for (int i = 0; i < bonusFilledRectangles.Count; i++)
                    {
                        if(bonusFilledRectangles[i].show)
                            g.FillRectangle(Brushes.Yellow, bonusFilledRectangles[i].X, bonusFilledRectangles[i].Y, 27, 27);

                        if (bonusLines[i].show)
                            g.DrawLine(p, bonusLines[i].X1, bonusLines[i].Y1, bonusLines[i].X2, bonusLines[i].Y2);
                    }
                }

                if (showBonusAnimation2)
                {
                    Pen p = new Pen(Color.Yellow, 5);
                    for (int i = 0; i < bonusDrawedRectangles.Count; i++)
                    {
                        g.DrawRectangle(p, bonusDrawedRectangles[i].X, bonusDrawedRectangles[i].Y, 70, 60);
                    }
                }
            }

            if(showAnimatedObjects)
            {
                for(int i = 0; i < animatedObjects.Count; i++)
                {
                    g.FillEllipse(animatedObjects[i].brush, animatedObjects[i].X, animatedObjects[i].Y, animatedObjects[i].W, animatedObjects[i].H);
                }
            }

            g.FillRectangle(Brushes.Black, 10, 120, 150, 30);
            g.FillRectangle(speedoctangle.clr, speedoctangle.X, speedoctangle.Y, speedoctangle.W, speedoctangle.H);

            g.DrawString((ballsLimit - movingBalls.Count - 1) + " ", new Font("Calibri", 18f, FontStyle.Bold), Brushes.Black, 1105, 15);

            // --
                g.TranslateTransform(zumaX + (zuma.Width / 2), zumaY + (zuma.Height / 2));
                g.RotateTransform(zumaAngle);
                g.TranslateTransform(-zumaX + (-zuma.Width / 2), -zumaY + (-zuma.Height / 2));
            // --

            if (!debugMode)
            {
                g.DrawImage(zuma, zumaX, zumaY);

                if (remainingBalls >= 1)
                    g.DrawImage(selectedBalls[selectedBallIndex], zumaX + 117, zumaY + 46);

                if (remainingBalls >= 2)
                    g.DrawImage(pendingBalls[pendingBallIndex], zumaX + 131, zumaY + 190, 30, 30);
            }
        }

        public void drawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            drawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
