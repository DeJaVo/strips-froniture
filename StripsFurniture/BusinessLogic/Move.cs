﻿using System.Drawing;
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

    public class Move : Operation
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

        /// <summary>
        /// Check if move is valid and if it does, create a new furniture in that location
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="newFurniture"></param>
        /// <returns></returns>
        public bool IsValidMove(Furniture furniture, out Furniture newFurniture)
        {
            //calculate new rectangle
            var newdestRectangle = new Rectangle
                {
                    Width = furniture.Description.Width,
                    Height = furniture.Description.Height,
                    X = furniture.Description.X,
                    Y = furniture.Description.Y
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
                newFurniture = null;
                return false;
            }
            //isempty
            if (board.IsEmpty(newdestRectangle))
            {
                newFurniture = null;
                return false;
            }

            //Create new furniture
            newFurniture = new Furniture(newdestRectangle, furniture.ID);
            return true;
        }

        /// <summary>
        /// Execute move
        /// </summary>
        public override void Execute(Furniture furniture)
        {
            Furniture newFurniture;
            if (IsValidMove(furniture, out newFurniture))
            {
                board.DeallocateFromBoard(furniture);
                board.AllocateOnBoard(newFurniture);
            }
        }
    }
}