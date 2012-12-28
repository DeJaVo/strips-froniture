using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <summary>
        /// returns direction
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// returns how many steps to move in direction
        /// </summary>
        public int HowManyStepsInDirection { get; set; }
    }
}
