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