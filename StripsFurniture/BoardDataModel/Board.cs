using System.Collections.Generic;
using System.Drawing;

namespace BoardDataModel
{
    public class Board
    {
        private static Board instance;
        public enum CellType 
        {
            Empty,
            Allocated,
            Door,
            Wall
        }

        /// <summary>
        /// Board
        /// </summary>
        public CellType[,] Rooms { get; set; }

        /// <summary>
        /// List of furnitures
        /// </summary>
        public IDictionary<Furniture, Rectangle> furnitureDestination;
        
        /// <summary>
        /// Board constructor
        /// </summary>
        private Board()
        {
            Rooms = new CellType[13,22];
            //Rooms = new CellType[10, 20];

            for (int i = 0; i < 13; i++)
            {
                Rooms[i, 0] = CellType.Wall;
                Rooms[i, 21] = CellType.Wall;
            }

            for (int j = 0; j < 22; j++)
            {
                Rooms[0,j] = CellType.Wall;
                Rooms[12,j] = CellType.Wall;
            }

            //Vertical Seprated wall
            for (int i = 0; i < 13; i++)
            {
                if (((i >= 2) && (i <= 3)) || ((i >= 7) && (i <= 10))) continue;
                Rooms[i, 10] = CellType.Wall;
            }

            //Horizontal Separted wall
            for (int j = 11; j < 22; j++)
            {
                Rooms[5, j] = CellType.Wall;
            }

            //Doors
            for (int i = 0; i < 13; i++)
            {
                if (((i >= 2) && (i <= 3)) || ((i >= 7) && (i <= 10)))
                {
                    Rooms[i, 10] = CellType.Door;
                }
            }

            furnitureDestination = new Dictionary<Furniture, Rectangle>();
        }

        /// <summary>
        /// Board singelton
        /// </summary>
        public static Board Instance
        {
            get { return instance ?? (instance = new Board()); }
        }

        //Talk with Sheira about this implemenation.
        //My point of view - only create a new furniture than the calling env. will call the allocateOnBoard function.
        /// <summary>
        /// Adds a furniture at the given start and destination descriptions create 
        /// </summary>
        /// <param name="furStart"></param>
        /// <param name="furDest"></param>
        /// <returns>if given params are valid ->creates new furnitre at start position and returns id of the furniture, else -> -1</returns>
        public int CreateFurniture(Rectangle furStart, Rectangle furDest)
        {
            if (Instance.InBounds(furStart) && Instance.InBounds(furDest) && Instance.IsEmpty(furStart) &&
                Instance.IsEmpty(furDest) && Instance.IsDestFurValid(furStart, furDest) && Instance.WillPassDoor(furStart, furDest))
            {
                //create new furniture
                var newFurniture = new Furniture(furStart, furnitureDestination.Count+1);
                //add furniture and its end position to the boards' map
                furnitureDestination.Add(newFurniture,furDest);
                // change the relevant cell type on the board to allocated
                AllocateOnBoard(newFurniture);
                return newFurniture.ID;
            }
            return -1;
        }

        /// <summary>
        /// Checks if the dimensions of the destination furniture are the same as in the start (consider rotatation)
        /// </summary>
        /// <param name="furStart"></param>
        /// <param name="furDest"></param>
        /// <returns></returns>
        private bool IsDestFurValid(Rectangle furStart, Rectangle furDest)
        {
            return (((furStart.Width == furDest.Width) && (furStart.Height == furDest.Height)) ||
                    ((furStart.Width == furDest.Height) && (furStart.Height == furDest.Width)));
        }

        /// <summary>
        /// Checks if the furniture needs to pass a door
        /// if true checks if it wiil be able to pass it
        /// </summary>
        /// <param name="furStart"></param>
        /// <param name="furDest"></param>
        private bool WillPassDoor(Rectangle furStart, Rectangle furDest)
        {
            bool needToPassUpperDoor = false;
            bool needToPassLowerDoor = false;

            //int furStartRight = furDest.X + furDest.Width;
            //int furStartBottom = furDest.Y + furDest.Height;

            //int furDestRight = furDest.X + furDest.Width;
            //int furDestBottom = furDest.Y + furDest.Height;
            
            // pass room1 to room 2 or vise versa
            if (((furStart.X <= 11) && (furDest.X > 11) && (furDest.Y < 5)) ||
                ((furDest.X <= 11) && (furStart.X > 11) && (furStart.Y < 5)))
            {
                needToPassUpperDoor = true;
            }
                // pass room 1 to room 3 or vise versa
            else if (((furStart.X <= 11) && (furDest.X > 11) && (furDest.Y > 5)) ||
                     ((furDest.X <= 11) && (furStart.X > 11) && (furStart.Y > 5)))
            {
                needToPassLowerDoor = true;
            }
            // pass room 2 to room 3 or vise versa
            else if ((furStart.X > 11) && (furDest.X > 11) &&
                     (((furStart.Y < 5) && (furDest.Y > 5)) ||
                      ((furDest.Y < 5) && (furStart.Y > 5))))
            {
                needToPassUpperDoor = true;
                needToPassLowerDoor = true;
            }

            if ((needToPassUpperDoor) &&
                ((furStart.Width > 2) || (furStart.Height > 2)))
            {
                return false;
            }

            if ((needToPassLowerDoor) &&
                ((furStart.Width > 4) || (furStart.Height > 4)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// allocates area on board according to the furniture's rect.
        /// </summary>
        /// <param name="furniture"></param>
        public void AllocateOnBoard(Furniture furniture)
        {            
            var rect = furniture.Description;
            int xl = rect.X;
            int yh = rect.Y;
            int xh = xl + rect.Width;
            int yl = yh + rect.Height;

            for (int i = yl; i < yh; i++)
            {
                for (int j = xl; j < xh; j++)
                {
                    instance.Rooms[i,j] = CellType.Allocated;
                }
            }   
        }

        /// <summary>
        /// deallocates area from boaard according to furniture's rect
        /// </summary>
        /// <param name="furniture"></param>
        public void DeallocateFromBoard(Furniture furniture)
        {
            var rect = furniture.Description;
            int xl = rect.X;
            int yh = rect.Y;
            int xh = xl + rect.Width;
            int yl = yh - rect.Height;

            for (int i = yl; i <= yh; i++)
            {
                for (int j = xl; j <= xh; j++)
                {
                    Instance.Rooms[i, j] = CellType.Empty;
                }
            }
        }

        /// <summary>
        /// returns if all the furnitures are in their destionation location and orientation
        /// </summary>
        /// <returns></returns>
        public bool IsBoardSolved()
        {
            return false;
        }

        /// <summary>
        /// Checks if given rectangle is an empty slot on Rooms board
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public bool IsEmpty(Rectangle rectangle)
        {
            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width;
            int height = rectangle.Height;
            for (; x < rectangle.X + width; x++)
            {
                for (; y < rectangle.Y + height; y++)
                {
                    if (Rooms[ y,x] != CellType.Empty)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check if furniture is in board bounds
        /// </summary>
        /// <returns></returns>
        public bool InBounds(Rectangle rectangle)
        {
            //Out of board bounds
            if (rectangle.X <= 0 || rectangle.Y <= 0 || rectangle.X > 21 || rectangle.Y > 12)
            {
                return false;
            }
            return true;
        }
    }
}
