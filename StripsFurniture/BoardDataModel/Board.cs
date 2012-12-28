using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private IList<Furniture> furnitureList; 
        
        private Board()
        {
            //Vertical Seprated wall
            for (int i = 0; i < 11; i++)
            {
                if (((i >= 2) && (i <= 3)) || ((i >= 7) && (i <= 10))) continue;
                Rooms[i, 11] = CellType.Wall;
            }

            //Horizontal Separted wall
            for (int j = 12; j < 20; j++)
            {
                Rooms[5, j] = CellType.Wall;
            }

            //Doors
            for (int i = 0; i < 11; i++)
            {
                if (((i >= 2) && (i <= 3)) || ((i >= 7) && (i <= 10)))
                {
                    Rooms[i, 11] = CellType.Door;
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
                //create new furniture, add to furniture list, update board that these cells are allocated
                var newFurniture = new Furniture(furStart, furnitureList.Count+1);
                furnitureList.Add(newFurniture);
                AllocateOnBoard(newFurniture);
                return newFurniture.ID;
            }
            return -1;
        }

        /// <summary>
        /// allocates area on board according to the furniture's rect.
        /// </summary>
        /// <param name="furniture"></param>
        private void AllocateOnBoard(Furniture furniture)
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
        private void DeallocateFromBoard(Furniture furniture)
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
        /// Swap width and height
        /// </summary>
        public void SwapWidthHeight(Furniture furniture)
        {
            Rectangle rectangle = furniture.Description;
            var tempWidth = rectangle.Width;
            rectangle.Width = rectangle.Height;
            rectangle.Height = tempWidth;
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
