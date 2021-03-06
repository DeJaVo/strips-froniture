﻿using System.Collections.Generic;
using System.Drawing;
using BoardDataModel;

namespace Heuristics
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
        /// C'tor
        /// </summary>
        /// <param name="currentFurniture"></param>
        public Move(Furniture currentFurniture)
        {
            this.Furniture = currentFurniture;
            HowManyStepsInDirection = 1;
        }

        /// <summary>
        /// returns direction
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// returns how many steps to move in direction
        /// </summary>
        public int HowManyStepsInDirection { get; set; }

        public override string ToString()
        {
            return "Move Furniture " + this.Furniture.ID + " In Direction : " + this.Direction.ToString();
        }

        /// <summary>
        /// Check if move is valid and if it does, create a new furniture in that location
        /// </summary>
        /// <returns></returns>
        public bool IsValidMove()
        {
            //calculate new rectangle
            Rectangle newdestRectangle = CalculateRectDiff();

            //inbounds
            if (!board.InBounds(newdestRectangle))
            {
                return false;
            }
            //isempty
            if (!board.IsEmpty(newdestRectangle))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calculate rectangles difference
        /// </summary>
        /// <returns>the diff rectangle</returns>
        public Rectangle CalculateRectDiff()
        {
            var destRect = CalculateNewdestRectangle();
            var diffRect = new Rectangle();


            switch (Direction)
            {
                case Direction.Up:
                    {
                        diffRect.X = destRect.X;
                        diffRect.Y = destRect.Y;
                        diffRect.Width = furniture.Description.Width;
                        diffRect.Height = 1;
                        break;
                    }
                case Direction.Down:
                    {
                        diffRect.X = furniture.Description.X;
                        diffRect.Y = furniture.Description.Y + destRect.Height;
                        diffRect.Height = 1;
                        diffRect.Width = furniture.Description.Width;
                        break;
                    }
                case Direction.Left:
                    {
                        diffRect.X = destRect.X;
                        diffRect.Y = destRect.Y;
                        diffRect.Width = 1;
                        diffRect.Height = furniture.Description.Height;
                        break;
                    }
                case Direction.Right:
                    {
                        diffRect.X = furniture.Description.X + furniture.Description.Width;
                        diffRect.Y = destRect.Y;
                        diffRect.Height = furniture.Description.Height;
                        diffRect.Width = 1;
                        break;
                    }
            }
            return diffRect;
        }

        /// <summary>
        /// Calculates new destination rectangle
        /// </summary>
        /// <returns></returns>      
        public Rectangle CalculateNewdestRectangle()
        {
            var newdestRectangle = new Rectangle();
            newdestRectangle.Width = furniture.Description.Width;
            newdestRectangle.Height = furniture.Description.Height;
            newdestRectangle.X = furniture.Description.X;
            newdestRectangle.Y = furniture.Description.Y;
            switch (Direction)
            {
                case Direction.Down:
                    {
                        newdestRectangle.Y += HowManyStepsInDirection;
                        break;
                    }
                case Direction.Up:
                    {
                        newdestRectangle.Y -= HowManyStepsInDirection;
                        break;
                    }
                case Direction.Left:
                    {
                        newdestRectangle.X -= HowManyStepsInDirection;
                        break;
                    }
                case Direction.Right:
                    {
                        newdestRectangle.X += HowManyStepsInDirection;
                        break;
                    }
            }
            return newdestRectangle;
        }

        /// <summary>
        /// Execute move
        /// </summary>
        public override void Execute()
        {
            this.FurnitureOldData =
                new Rectangle(furniture.Description.X,
                              furniture.Description.Y,
                              furniture.Description.Width,
                              furniture.Description.Height);
            if (IsValidMove())
            {
                board.DeallocateFromBoard(furniture);

                Rectangle dest = board.furnitureDestination[furniture];
                board.furnitureDestination.Remove(furniture);

                // update the furniture
                furniture.Description = CalculateNewdestRectangle();
                board.furnitureDestination.Add(furniture, dest);

                board.AllocateOnBoard(furniture);


                this.FurnitureNewData = furniture.Description;
            }
        }

        /// <summary>
        /// Retruns the source direction
        /// </summary>
        /// <returns></returns>
        public override List<Direction> ForbbidenDirections()
        {
            var result = new List<Direction>();
            switch (Direction)
            {
                case Direction.Down:
                    result.Add(Direction.Up);
                    break;
                case Direction.Up:
                    result.Add(Direction.Down);
                    break;
                case Direction.Left:
                    result.Add(Direction.Right);
                    break;
                case Direction.Right:
                    result.Add(Direction.Left);
                    break;
            }

            return result;
        }
    }
}