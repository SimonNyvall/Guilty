using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MoveCursor
{
    public partial class Form1 : Form
    {
        int posCursor_X, posCursor_Y;
        int mouseSpeed = 50;
        
        // accept cursor control.
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        public Form1()
        {
            // Sets all the global hotkeys.
            InitializeComponent();
            IntPtr thisWindow = this.Handle;
            RegisterHotKey(thisWindow, 1, (uint)fsModifiers.Control, (uint)Keys.Up);
            RegisterHotKey(thisWindow, 2, (uint)fsModifiers.Control, (uint)Keys.Down);
            RegisterHotKey(thisWindow, 3, (uint)fsModifiers.Control, (uint)Keys.Left);
            RegisterHotKey(thisWindow, 4, (uint)fsModifiers.Control, (uint)Keys.Right);

            RegisterHotKey(thisWindow, 5, (uint)fsModifiers.Control, (uint)Keys.NumPad4);
            RegisterHotKey(thisWindow, 6, (uint)fsModifiers.Control, (uint)Keys.NumPad1);

            RegisterHotKey(thisWindow, 7, (uint)fsModifiers.Control, (uint)Keys.NumPad7);

            RegisterHotKey(thisWindow, 8, (uint)fsModifiers.Control, (uint)Keys.NumPad5);
        }

        static void LinearSmoothMove(int pos_X, int pos_Y, int steps, int newPos_X, int newPos_Y)
        {
            int MouseEventDelayMS = 10;
            Point start = new Point(pos_X, pos_Y);
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(newPos_X - start.X, newPos_Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;

            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                Cursor.Position = new Point(Point.Round(iterPoint).X, Point.Round(iterPoint).Y);
                Thread.Sleep(MouseEventDelayMS);
            }

            // Move the mouse to the final destination.
            //Console.SetCursorPosition(newPosition);
            Cursor.Position = new Point(newPos_X, newPos_Y);
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            // Sets so the form is invisable.
            this.BackColor = Color.Wheat;
            this.TransparencyKey = Color.Wheat;
            this.TopMost = true;
            FormBorderStyle = FormBorderStyle.None;

            // Makes so the form can be clicked through.
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
        }
        public enum fsModifiers
        {
            Control = 0x0002
        }
        protected override void  WndProc(ref Message keyPressed)
        {
            Thread.Sleep(40); // Minimal CPU useage.

            posCursor_X = Cursor.Position.X;
            posCursor_Y = Cursor.Position.Y;

            int step = 20;

            // See what hotkey got pressed and do the action.
            if (keyPressed.Msg == 0x0312)
            {
                switch ((short)keyPressed.WParam)
                {
                    case 1:
                        LinearSmoothMove(posCursor_X, posCursor_Y, step, posCursor_X, posCursor_Y - mouseSpeed);
                        break;

                    case 2:
                        LinearSmoothMove(posCursor_X, posCursor_Y, step, posCursor_X, posCursor_Y + mouseSpeed);
                        break;

                    case 3:
                        LinearSmoothMove(posCursor_X, posCursor_Y, step, posCursor_X - mouseSpeed, posCursor_Y);
                        break;

                    case 4:
                        LinearSmoothMove(posCursor_X, posCursor_Y, step, posCursor_X + mouseSpeed, posCursor_Y);
                        break;
                    case 5:
                        mouseSpeed += 10;
                        break;
                    case 6:
                        mouseSpeed -= 10;
                        break;
                    case 7:
                        this.Close();
                        break;
                    case 8:
                        mouseSpeed = 50;
                        break;
                }


            }
            base.WndProc(ref keyPressed);
        }
    }
}
