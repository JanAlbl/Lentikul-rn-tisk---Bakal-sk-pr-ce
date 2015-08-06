using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interlacer
{
    public partial class ClipForm : Form
    {
        private Point previousTopLeft = Point.Empty;
        private Point previousBottomRight = Point.Empty;

        private Boolean keepWidth = true;
        private Point mouseDownPoint = Point.Empty;
        private Point mousePoint = Point.Empty;

        private Point topLeft = Point.Empty;
        private Point bottomRight = Point.Empty;

        private Rectangle rubberBand;
        private Pen drawingPen;

        public ClipForm()
        {
            InitializeComponent();
            init(); 
        }

        private void init()
        {
            rubberBand = new Rectangle();  
            drawingPen = new Pen(Color.Black);

            previousTopLeft.X = (int)topLeftXnumeric.Value;
            previousTopLeft.Y = (int)topLeftYnumeric.Value;

            topLeftXnumeric.Tag = topLeftXnumeric.Value;
            topLeftYnumeric.Tag = topLeftYnumeric.Value;
            bottomRightXnumeric.Tag = bottomRightXnumeric.Value;
            bottomRightYnumeric.Tag = bottomRightYnumeric.Value;

            previousBottomRight.X = (int)bottomRightXnumeric.Value;
            previousBottomRight.Y = (int)bottomRightYnumeric.Value;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(drawingPen, rubberBand);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = mouseDownPoint = e.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mousePoint = e.Location;

                topLeft.X = Math.Max(0, Math.Min(mouseDownPoint.X, mousePoint.X));
                topLeft.Y = Math.Max(0, Math.Min(mouseDownPoint.Y, mousePoint.Y));
                bottomRight.X = topLeft.X + Math.Abs(mouseDownPoint.X - Math.Min(clipPictureBox.Width - 1, Math.Max(0, mousePoint.X)));
                bottomRight.Y = topLeft.Y + Math.Abs(mouseDownPoint.Y - Math.Min(clipPictureBox.Height - 1, Math.Max(0, mousePoint.Y)));
                
                // rozebrat a uděalt jednotlivý body atd
                rubberBand = new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

                updateRubberBandInfo();
                clipPictureBox.Invalidate();
            }
        }

        private void updateRubberBandInfo()
        {
            // Levy horni roh
            topLeftXnumeric.Value = topLeft.X;
            topLeftYnumeric.Value = topLeft.Y;

            // Dolni pravy roh
            bottomRightXnumeric.Value = bottomRight.X;
            bottomRightYnumeric.Value = bottomRight.Y;

            // Rozmery
            rubberWidthNumeric.Value = rubberBand.Width;
            rubberHeightNumeric.Value = rubberBand.Height;
        }

        private void topLeftXnumeric_ValueChanged(object sender, EventArgs e)
        {
            moveTopLeftX((NumericUpDown)sender);
        }

        private void moveTopLeftX(NumericUpDown sender)
        {
            previousTopLeft.X = (int)((decimal)(sender).Tag);

            rubberBand.X = topLeft.X = Math.Max(0, (int)topLeftXnumeric.Value);
            bottomRight.X = topLeft.X + rubberBand.Width;

            if (rubberBand.X + rubberBand.Width > clipPictureBox.Width - 1)
                sender.Value = previousTopLeft.X;

            topLeftXnumeric.Tag = topLeftXnumeric.Value;
            updateRubberBandInfo();
            clipPictureBox.Invalidate();
        }

        private void topLeftYnumeric_ValueChanged(object sender, EventArgs e)
        {
            moveTopLeftY((NumericUpDown)sender);
        }

        private void moveTopLeftY(NumericUpDown sender)
        {
            previousTopLeft.Y = (int)((decimal)sender.Tag);

            rubberBand.Y = topLeft.Y = Math.Max(0, (int)topLeftYnumeric.Value);
            bottomRight.Y = topLeft.Y + rubberBand.Height;

            if (rubberBand.Y + rubberBand.Height > clipPictureBox.Height - 1)
            {
                ((NumericUpDown)sender).Value = previousTopLeft.Y;
                rubberBand.Y = topLeft.Y = previousTopLeft.Y;
            }

            topLeftYnumeric.Tag = topLeftYnumeric.Value;
            updateRubberBandInfo();
            clipPictureBox.Invalidate();
        }

        private void bottomRightXnumeric_ValueChanged(object sender, EventArgs e)
        {
            moveBottomRightX((NumericUpDown)sender);
        }

        private void moveBottomRightX(NumericUpDown sender)
        {
            previousBottomRight.X = (int)((decimal)((NumericUpDown)sender).Tag);

            bottomRight.X = Math.Max(0, (int)bottomRightXnumeric.Value);
            rubberBand.X = topLeft.X = bottomRight.X - rubberBand.Width;
            
            if((bottomRight.X > clipPictureBox.Width - 1) || rubberBand.X < 0) 
                sender.Value = previousBottomRight.X;

            bottomRightXnumeric.Tag = bottomRightXnumeric.Value;
            updateRubberBandInfo();
            clipPictureBox.Invalidate();
        }

        private void bottomRightYnumeric_ValueChanged(object sender, EventArgs e)
        {
            moveBottomRightY((NumericUpDown)sender);
        }

        private void moveBottomRightY(NumericUpDown sender)
        {
            previousBottomRight.Y = (int)((decimal)(sender).Tag);

            bottomRight.Y = Math.Max(0, (int)bottomRightYnumeric.Value);
            rubberBand.Y = topLeft.Y = bottomRight.Y - rubberBand.Height;

            if ((bottomRight.Y > clipPictureBox.Height - 1) || rubberBand.Y < 0)
                sender.Value = previousBottomRight.Y;

            bottomRightYnumeric.Tag = bottomRightYnumeric.Value;
            updateRubberBandInfo();
            clipPictureBox.Invalidate();
        }

        private void clipImagesButton_Click(object sender, EventArgs e)
        {

        }

        private void rubberWidthNumeric_ValueChanged(object sender, EventArgs e)
        {

        }

        private void rubberHeightNumeric_ValueChanged(object sender, EventArgs e)
        {

        }
        // když se klikne na ořezat tak to pojede jeden po druhým a když to najde
    }
}
