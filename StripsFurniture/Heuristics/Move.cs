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
        }

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
        /// <returns></returns>
        public bool IsValidMove()
        {
            //calculate new rectangle
            Rectangle newdestRectangle = CalculateNewdestRectangle();                       

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

            // update the furniture
            furniture.Description = newdestRectangle;
            //newFurniture = new Furniture(newdestRectangle, furniture.ID);
            return true;
        }


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
                        diffRect.Height = destRect.X - furniture.Description.X;
                        break;
                    }
                case Direction.Down:
                    {
                        diffRect.X = furniture.Description.X - furniture.Description.Height;
                        diffRect.Y = furniture.Description.Y;
                        diffRect.Height = destRect.Height - (destRect.X - diffRect.X);
                        diffRect.Width = furniture.Description.Width;
                        break;
                    }
                case Direction.Left:
                    {
                        diffRect.X = destRect.X;
                        diffRect.Y = destRect.Y;
                        diffRect.Width = furniture.Description.X - diffRect.X;
                        diffRect.Height = furniture.Description.Height;
                        break;
                    }
                case Direction.Right:
                    {
                        diffRect.X = furniture.Description.X + furniture.Description.Width;
                        diffRect.Y = destRect.Y;
                        diffRect.Height = furniture.Description.Height;
                        diffRect.Width = destRect.Width -(diffRect.X - destRect.X);
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
           var width = furniture.Description.Width;
           var height = furniture.Description.Height;
           var X = furniture.Description.X;
           var Y = furniture.Description.Y;
           switch (Direction)
            {
                case Direction.Down:
                    {
                        newdestRectangle.Y = Y + HowManyStepsInDirection;
                        break;
                    }
                case Direction.Up:
                    {
                        newdestRectangle.Y = Y - HowManyStepsInDirection;
                        break;
                    }
                case Direction.Left:
                    {
                        newdestRectangle.X = X - HowManyStepsInDirection;
                        break;
                    }
                case Direction.Right:
                    {
                        newdestRectangle.X = X + HowManyStepsInDirection;
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
                board.AllocateOnBoard(furniture);

                this.FurnitureNewData = furniture.Description;
                
            }
        }
    }
}