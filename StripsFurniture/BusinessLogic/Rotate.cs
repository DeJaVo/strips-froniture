using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BoardDataModel;

namespace BusinessLogic
{
    public enum RotationDirection
    {
        ClockWise,
        CounterClockWise
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }
    public class Rotate :Operation
    {
        private readonly Board board = Board.Instance;
        /// <summary>
        /// returns direction
        /// </summary>
        public RotationDirection RotationDirection { get; set; }

        /// <summary>
        /// Angle
        /// </summary>
        public int Angle { get; set; }

        /// <summary>
        /// Check if Rotation is valid
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool IsValidRotate(Furniture furniture,RotationDirection direction)
        {
            int width = furniture.Description.Width;
            int height = furniture.Description.Height;
            int x = furniture.Description.X;
            int y = furniture.Description.Y;
            Rectangle temp1, temp2;
            Orientation orientation = width > height ? Orientation.Horizontal : Orientation.Vertical;
            CheckRotateByDirection(furniture, direction, orientation, out temp1, out temp2);
            if (!(board.InBounds(temp1) || board.InBounds(temp2)))
            {
                return false;
            }
            if(!(board.IsEmpty(temp1)||board.IsEmpty(temp2)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Check rotation rectangles for bounds and empty.
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="direction"></param>
        /// <param name="orientation"></param>
        /// <param name="temp1"></param>
        /// <param name="temp2"></param>
        /// <returns></returns>
        public void CheckRotateByDirection(Furniture furniture,RotationDirection direction,Orientation orientation,out Rectangle temp1, out Rectangle temp2)
        {
            int width = furniture.Description.Width;
            int height = furniture.Description.Height;
            int x = furniture.Description.X;
            int y = furniture.Description.Y;
            int value = orientation == Orientation.Horizontal ? width : height;
            switch (direction)
            {
                    case RotationDirection.ClockWise:
                    {
                        if (orientation == Orientation.Horizontal)
                        {
                            temp1 = new Rectangle(x - (value) / 2, y + (value) / 2, value / 2,
                                                      (int)Math.Ceiling((double)value / 2));
                            temp2 = new Rectangle(x + 1, y +(int)Math.Ceiling((double)value / 2), value / 2, (int)Math.Ceiling((double)value / 2));
                        }
                        else if (orientation == Orientation.Vertical)
                        {
                            temp1 = new Rectangle(x+1, y - (value) / 2, (int)Math.Ceiling((double)value / 2), (value / 2));
                            temp2 = new Rectangle(x + (int)Math.Ceiling((double)value / 2), y + width,
                                                      (int)Math.Ceiling((double)value / 2), (value) / 2);
                        }
                        break;
                    }
                    case RotationDirection.CounterClockWise:
                    {
                        if (orientation == Orientation.Horizontal)
                        {
                            temp1 = new Rectangle(x - (value)/2, y + (value)/2, value/2,
                                                      (int) Math.Ceiling((double) value/2));
                            temp2 = new Rectangle(x + 1, y, value/2, (int) Math.Ceiling((double) value/2));
                        }
                        else if(orientation==Orientation.Vertical)
                        {
                            temp1 = new Rectangle(x, y - (value)/2, (int) Math.Ceiling((double) value/2), (value/2));
                            temp2 = new Rectangle(x + (int) Math.Ceiling((double) value/2), y + width,
                                                      (int) Math.Ceiling((double) value/2), (value)/2);
                        }
                        break;
                    }
            }

            temp1 = new Rectangle();
            temp2 = new Rectangle();
        }
    }
}
