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
            Rectangle newdestRectangle = CalculateNewdestRectangle();                       

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
                       //diffRect.Height = destRect.Y - furniture.Description.Y;
                        diffRect.Height = 1;
                        break;
                    }
                case Direction.Down:
                    {
                        diffRect.X = furniture.Description.X;
                        diffRect.Y = furniture.Description.Y + destRect.Height;
                       //diffRect.Height = destRect.Y - diffRect.Y;
                        diffRect.Height = 1;
                        diffRect.Width = furniture.Description.Width;
                        break;
                    }
                case Direction.Left:
                    {
                        diffRect.X = destRect.X;
                        diffRect.Y = destRect.Y;
                        //diffRect.Width = furniture.Description.X - diffRect.X;
                        diffRect.Width = 1;
                        diffRect.Height = furniture.Description.Height;                       
                        break;
                    }
                case Direction.Right:
                    {
                        diffRect.X = furniture.Description.X + furniture.Description.Width;
                        diffRect.Y = destRect.Y;
                        diffRect.Height = furniture.Description.Height;
                        //diffRect.Width = destRect.Width -(diffRect.X - destRect.X);
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
    }
}