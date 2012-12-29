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
        /// Board constructor - Need to be implemented correctly
        /// </summary>
        public Board()
        {
            Rooms = new CellType[10,20];
            //Vertical Seprated wall
            for (int i = 0; i < 10; i++)
            {
                if (((i >= 1) && (i <= 2)) || ((i >= 5) && (i <= 8))) continue;
                Rooms[i, 10] = CellType.Wall;
            }

            //Horizontal Separted wall
            for (int j = 11; j < 20; j++)
            {
                Rooms[4, j] = CellType.Wall;
            }

            //Doors
            for (int i = 0; i < 10; i++)
            {
                if (((i >= 1) && (i <= 2)) || ((i >= 5) && (i <= 8)))
                {
                    Rooms[i, 10] = CellType.Door;
                }
            }
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
            if (instance.InBounds(furStart) && instance.InBounds(furDest) && instance.IsEmpty(furStart) &&
                instance.IsEmpty(furDest))
            {
                //add test: in case star&end positions of the fur' are in defferent rooms check fur' can pass the door!
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
        /// allocates area on board according to the furniture's rect.
        /// </summary>
        /// <param name="furniture"></param>
        public void AllocateOnBoard(Furniture furniture)
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
                    instance.Rooms[i, j] = CellType.Empty;
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
        // stabs for GUI


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
            for (; x < x + height; x++)
            {
                for (; y < y + width; y++)
                {
                    if (Rooms[x, y] != CellType.Empty)
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
            if (rectangle.X <= 0 || rectangle.Y <= 0 || rectangle.X > 11 || rectangle.Y > 20)
            {
                return false;
            }
            return true;
        }
    }
}
