using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BoardDataModel;
using BusinessLogic;
using Heuristics;
namespace UI
{
    public partial class Form1 : Form
    {
        private Board board;
        private Label[,] squares;

        private Strips stripsLogic;
        private bool pause;
        public Form1()
        {
            InitializeComponent();

            this.resetButton_Click(null,null);
        }

        private void BuildBoard()
        {
            splitContainer3.Panel1.Controls.Clear();

            int squreWidth = (int)(splitContainer3.Panel1.Width / 22);
            int squreHeight = (int)(splitContainer3.Panel1.Height / 13);
            squares = new Label[13, 22];

            for (int j = 0; j < 22; j++)
            {
                squares[0, j] = new Label();
                squares[0, j].Size = new Size(squreWidth, squreHeight);

                squares[0, j].BackColor = Color.Black;
            }

            for (int i = 0; i < 13; i++)
            {
                squares[i, 0] = new Label();
                squares[i, 0].Size = new Size(squreWidth, squreHeight);

                squares[i, 0].BackColor = Color.Black;
            }

            for (int i = 0; i < 13; i++)
            {
                squares[i, 21] = new Label();
                squares[i, 21].Size = new Size(squreWidth, squreHeight);

                squares[i,21].BackColor = Color.Black;
            }
            
            for (int j = 0; j < 22; j++)
            {
                squares[12, j] = new Label();
                squares[12, j].Size = new Size(squreWidth, squreHeight);

                squares[12, j].BackColor = Color.Black;
            }

            for (int i = 0; i < 13; i++)
            {
                if (((i >= 2) && (i <= 3)) || ((i >= 7) && (i <= 10))) continue;
                squares[i, 11] = new Label();
                squares[i, 11].Size = new Size(squreWidth, squreHeight);

                squares[i, 11].BackColor = Color.Black;
            }

            for (int j = 12; j < 22; j++)
            {
                squares[5, j] = new Label();
                squares[5, j].Size = new Size(squreWidth, squreHeight);

                squares[5, j].BackColor = Color.Black;
            }
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 22; j++)
                {
                    if (squares[i, j] == null)
                    {
                        squares[i, j] = new Label();
                        squares[i, j].Size = new Size(squreWidth, squreHeight);

                        squares[i, j].BorderStyle = BorderStyle.FixedSingle;
                        squares[i, j].BackColor = Color.Gray;
                    }
                    squares[i, j].Location = new Point(j * squreWidth, i * squreHeight);
                    splitContainer3.Panel1.Controls.Add(squares[i, j]);
                }
            }

        }


        private void createFurnitureButton_Click(object sender, EventArgs e)
        {
            // collect the forniture parameters
            int furStartX,furStartY,furStartWidth,furStartHeight;
            int furDestX, furDestY, furDestWidth, furDestHeight;
            bool res = int.TryParse(furStartXCombo.Text, out furStartX);
            res &= int.TryParse(furStartYCombo.Text,out furStartY);
            res &= int.TryParse(furStartWidthCombo.Text,out furStartWidth);
            res &= int.TryParse(furStartHeightCombo.Text,out furStartHeight);
            res &= int.TryParse(furDestXCombo.Text,out furDestX);
            res &= int.TryParse(furDestYCombo.Text,out furDestY);
            res &= int.TryParse(furDestWidthCombo.Text,out furDestWidth);
            res &= int.TryParse(furDestHeightCombo.Text,out furDestHeight);
            
            if (!res)
            {
                MessageBox.Show("Error invalid input");
                return;
            }

            Rectangle furStart = new Rectangle(furStartX,furStartY,furStartWidth,furStartHeight);
            Rectangle furDest = new Rectangle(furDestX, furDestY, furDestWidth, furDestHeight);

            int furId = board.CreateFurniture(furStart,furDest);

            if (furId != -1)
            {
                // draw the furniture start position
                this.DrawFurniture(furDest, furId, true);
                this.DrawFurniture(furStart, furId,false);

                // enable actions
                this.runButton.Enabled = true;
                this.nextStepButton.Enabled = true;

                // clear comboboxes to enter another furniture
                this.ClearCombos();
            }
            else
            {
                MessageBox.Show("Invalid input");
            }
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            this.createFurnitureButton.Enabled = false;
            this.pauseButton.Enabled = true;
            this.PerformOperation(true);
        }

        private void nextStepButton_Click(object sender, EventArgs e)
        {
            this.createFurnitureButton.Enabled = false;
            this.PerformOperation(false);
        }

        private void PerformOperation(bool runAutomatically)
        {
            if (stripsLogic == null)
            {
                if (runAutomatically)
                {
                    this.pauseButton.Enabled = true;
                }
                stripsLogic = new Strips(board);
            }

            while (!board.IsBoardSolved())
            {
                if (pause)
                {
                    pause = false;
                    return;
                }

                Operation currOp = stripsLogic.GetNextOperation();
                this.ExecuteOperation(currOp);
                this.InteractivePause(new TimeSpan(0, 0, 0, 0, 500));

                // perform one step
                if ((!runAutomatically) && (!pause))
                {
                    if (pause)
                    {
                        pause = false;
                    }
                    return;
                }
            }

            MessageBox.Show("Solved!!!");
            this.runButton.Enabled = false;
            this.nextStepButton.Enabled = false;
            this.pauseButton.Enabled = false;
        }

        private void ExecuteOperation(Operation currOp)
        {
            this.DeleteFurniture(currOp.FurnitureOldData,false);
            this.DrawFurniture(currOp.FurnitureNewData,currOp.Furniture.ID,false);
            operationsStack.Items.Add(currOp.ToString());
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            this.pauseButton.Enabled = false;
            this.runButton.Enabled = false;
            this.nextStepButton.Enabled = false;
            this.createFurnitureButton.Enabled = true;
            this.operationsStack.Items.Clear();
            
            this.ClearCombos();

            board = Board.Instance;
            board.Reset();
            pause = false;
            stripsLogic = null;

            this.BuildBoard();
        }

        private void ClearCombos()
        {
            this.furStartXCombo.Text = "";
            this.furStartYCombo.Text = "";
            this.furDestXCombo.Text = "";
            this.furDestYCombo.Text = "";
            this.furStartHeightCombo.Text = "";
            this.furStartWidthCombo.Text = "";
            this.furDestHeightCombo.Text = "";
            this.furDestWidthCombo.Text = "";
        }
        private void pauseButton_Click(object sender, EventArgs e)
        {
            pause = true;
            stripsLogic.Pause();
            this.pauseButton.Enabled = false;
        }

        #region UI utils
        private void DrawFurniture(Rectangle rec,int id,bool isDest)
        {
            for (int i = rec.Top; i < rec.Top + rec.Height; i++)
            {
                for (int j = rec.Left; j < rec.Left + rec.Width; j++)
                {
                    this.DrawCell(i, j, id, isDest);
                }
            }
        }

        private void DrawCell(int i, int j, int id, bool isDest)
        {
            if (isDest && ((!drawFurnituresDestCheckBox.Checked) || (squares[i, j].BorderStyle != BorderStyle.FixedSingle)))
                return;

            using (Font myFont = new Font("Arial", 20))
            {
                int alpha = isDest ? 20 : 255;
                BorderStyle style = isDest ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
                squares[i, j].BorderStyle = style;
                squares[i, j].BackColor = Color.FromArgb(alpha, Color.LightGreen);
                squares[i, j].Text = id.ToString();
                squares[i, j].TextAlign = ContentAlignment.MiddleCenter;
                squares[i, j].Font = myFont;
                if (isDest)
                {
                    squares[i, j].Tag = id;
                }
            }
        }

        private void DeleteFurniture(Rectangle rec,bool isDest)
        {
            for (int i = rec.Top; i < rec.Top + rec.Height; i++)
            {
                for (int j = rec.Left; j < rec.Left + rec.Width; j++)
                {
                    if (isDest && (squares[i, j].BorderStyle != BorderStyle.FixedSingle))
                    {
                        continue;
                    }

                    squares[i, j].BorderStyle = BorderStyle.FixedSingle;
                    squares[i, j].BackColor = Color.Gray;
                    squares[i, j].Text = "";

                    if ((squares[i, j].Tag != null) && (!isDest))
                    {
                        this.DrawCell(i, j, (int)squares[i, j].Tag, true);
                    }
                    else
                    {
                        squares[i, j].Tag = null;
                    }
                }
            }
        }

        private void InteractivePause(TimeSpan length)
        {
            DateTime start = DateTime.Now;
            TimeSpan restTime = new TimeSpan(200000); // 20 milliseconds
            while (true)
            {
                System.Windows.Forms.Application.DoEvents();
                TimeSpan remainingTime = start.Add(length).Subtract(DateTime.Now);
                if (remainingTime > restTime)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("1: {0}", remainingTime));
                    // Wait an insignificant amount of time so that the
                    // CPU usage doesn't hit the roof while we wait.
                    System.Threading.Thread.Sleep(restTime);
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("2: {0}", remainingTime));
                    if (remainingTime.Ticks > 0)
                        System.Threading.Thread.Sleep(remainingTime);
                    break;
                }
            }
        }
        #endregion

        private void furStartWidthCombo_TextUpdate(object sender, EventArgs e)
        {
            if (furStartHeightCombo.Text != "")
            {
                furDestHeightCombo.Text = furStartHeightCombo.Text;
                furDestWidthCombo.Text = furStartWidthCombo.Text;
            }
        }

        private void furStartHeightCombo_TextUpdate(object sender, EventArgs e)
        {
            if (furStartWidthCombo.Text != "")
            {
                furDestHeightCombo.Text = furStartHeightCombo.Text;
                furDestWidthCombo.Text = furStartWidthCombo.Text;
            }
        }

        private void drawFurnituresDestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (drawFurnituresDestCheckBox.Checked)
            {
                foreach (Furniture currFur in this.board.furnitureDestination.Keys)
                {
                    DrawFurniture(board.furnitureDestination[currFur], currFur.ID, true);
                }
            }
            else
            {
                foreach (Rectangle currDest in this.board.furnitureDestination.Values)
                {
                    DeleteFurniture(currDest, true);
                }
            }
        }


    }
}
