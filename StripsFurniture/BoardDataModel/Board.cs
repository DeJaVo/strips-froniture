using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace BoardDataModel
{
    public class Board
    {
        // stabs for GUI
        public Board()
        {
        }

        /// <summary>
        /// returns if it is valid to add a furniture at the given start and destination descriptions
        /// </summary>
        /// <param name="furStart"></param>
        /// <param name="furDest"></param>
        /// <returns>id of the furniture if the given params are valid else -1</returns>
        public int CreateFurniture(Rectangle furStart, Rectangle furDest)
        {
            return 1;
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
    }
}
