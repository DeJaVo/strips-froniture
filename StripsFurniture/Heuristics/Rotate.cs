﻿using System;
using System.Drawing;
using BoardDataModel;

namespace Heuristics
{
    /// <summary>
    /// Rotation 90° to the right - ClockWise
    /// Rotation 90° to the left - CounterClockWise
    /// </summary>
    public enum RotationDirection
    {
        ClockWise,
        CounterClockWise
    }

    /// <summary>
    /// Horizontal - (---------)
    ///
    ///             |
    ///             |
    /// vertical   -|
    ///             |
    ///             |
    /// </summary>
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Rotate class
    /// </summary>
    public class Rotate :Operation
    {
        private readonly Board board = Board.Instance;
        private readonly Orientation orientation;
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="currentFurniture"></param>
        public Rotate(Furniture currentFurniture)
        {
            this.Furniture = currentFurniture;
            int width = furniture.Description.Width;
            int height = furniture.Description.Height;
            orientation = width > height ? Orientation.Horizontal : Orientation.Vertical;
        }

        /// <summary>
        /// returns direction
        /// </summary>
        public RotationDirection RotationDirection { get; set; }

        /// <summary>
        /// Angle
        /// </summary>
        public int Angle { get; set; }

        public override string ToString()
        {
            return "Rotate Furniture " + this.Furniture.ID + " " + this.RotationDirection.ToString();
        }

        /// <summary>
        /// Check if Rotation is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValidRotate()
        {

            Rectangle temp1;
            
            CheckRotateByDirection(out temp1);
            if (!(board.InBounds(temp1)))
            {
                return false;
            }
            if(!(board.IsEmpty(temp1)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Check rotation rectangles for bounds and empty.
        /// </summary>
        /// <param name="temp1"></param>
        /// <param name="temp2"></param>
        /// <returns></returns>
        public void CheckRotateByDirection(out Rectangle temp1)
        {
            int width = furniture.Description.Width;
            int height = furniture.Description.Height;
            int x = furniture.Description.X;
            int y = furniture.Description.Y;
            var rec1 = new Rectangle();

            switch (RotationDirection)
            {
                    case RotationDirection.ClockWise:
                    {
                        if (orientation == Orientation.Vertical)
                        {
                            rec1 = new Rectangle(x - height + 1, y, height - width, height);
                        }
                        else if (orientation == Orientation.Horizontal)
                        {
                            rec1 = new Rectangle(x, y + width, width, width - height);
                        }
                        break;
                    }
                    case RotationDirection.CounterClockWise:
                    {
                        if (orientation == Orientation.Vertical)
                        {
                            rec1 = new Rectangle(x + width, y, height - width, height);
                        }
                        else if(orientation==Orientation.Horizontal)
                        {
                            rec1 = new Rectangle(x, y - width + 1, width, width - height);
                        }
                        break;
                    }
            }

            temp1 = rec1;
        }

        /// <summary>
        /// Execute rotation
        /// </summary>
        public override void Execute()
        {
            this.FurnitureOldData =
                new Rectangle(furniture.Description.X,
                              furniture.Description.Y,
                              furniture.Description.Width,
                              furniture.Description.Height);

            int width = furniture.Description.Width;
            int height = furniture.Description.Height;
            int x = furniture.Description.X;
            int y = furniture.Description.Y;
            var rec1 = new Rectangle();
            if (IsValidRotate())
            {
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        {
                            if (RotationDirection == RotationDirection.ClockWise)
                            {
                                rec1 = new Rectangle(x, y, height, width);
                            }
                            else if (RotationDirection == RotationDirection.CounterClockWise)
                            {
                                rec1 = new Rectangle(x, y - width + 1, height, width);
                            }
                            break;
                        }
                    case Orientation.Vertical:
                        {
                            if (RotationDirection == RotationDirection.ClockWise)
                            {
                                rec1 = new Rectangle(x - height + 1, y, height, width);
                            }
                            else if (RotationDirection == RotationDirection.CounterClockWise)
                            {
                                rec1 = new Rectangle(x, y,height, width);
                            }
                            break;
                        }
                        
                }
                board.DeallocateFromBoard(furniture);
                furniture.Description = rec1;
                board.AllocateOnBoard(furniture);

                this.FurnitureNewData = rec1;
            }
        }
    }
}
