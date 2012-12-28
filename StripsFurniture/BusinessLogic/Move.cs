using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BoardDataModel;

namespace BusinessLogic
{
    /// <summary>
    /// Moving Direction
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Move:Operation
    {
        private readonly Board board = Board.Instance;
        /// <summary>
        /// returns direction
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// returns how many steps to move in direction
        /// </summary>
        public int HowManyStepsInDirection { get; set; }

        public bool IsValidMove(Furniture furniture)
        {
            //calculate new rectangle
            var newdestRectangle = new Rectangle
                {
                    Width = furniture.Description.Width,
                    Height = furniture.Description.Height
                };
            switch (Direction)
            {
                    case Direction.Down:
                    {
                        newdestRectangle.X = furniture.Description.X + HowManyStepsInDirection;
                        break;
                    }
                case Direction.Up:
                    {
                        newdestRectangle.X = furniture.Description.X - HowManyStepsInDirection;
                        break;
                    }
                case Direction.Left:
                    {
                        newdestRectangle.Y = furniture.Description.Y - HowManyStepsInDirection;
                        break;
                    }
                case Direction.Right:
                    {
                        newdestRectangle.Y = furniture.Description.Y + HowManyStepsInDirection;
                        break;
                    }

            }

            //inbounds
            if (board.InBounds(newdestRectangle))
            {
                return false;
            }
            //isempty
            if (board.IsEmpty(newdestRectangle))
            {
                return false;
            }
            return true;
        }
    }
}
