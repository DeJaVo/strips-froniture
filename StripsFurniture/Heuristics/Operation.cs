﻿using System.Collections.Generic;
using System.Drawing;
using BoardDataModel;

namespace Heuristics
{
    /// <summary>
    /// Operations
    /// </summary>
    public enum OperationType
    {
        Move,
        Rotate
    }

    public abstract class Operation : StackItem
    {
        protected Furniture furniture;
        public Rectangle FurnitureOldData { get; set; }
        public Rectangle FurnitureNewData { get; set; }

        public Furniture Furniture
        {
            get { return furniture; }
            set { furniture = value; }
        }

        /// <summary>
        ///  Execute 
        ///  </summary>
        public abstract void Execute();

        /// <summary>
        /// Retrun the source direction
        /// </summary>
        /// <returns></returns>
        public abstract List<Direction> ForbbidenDirections();
    }
}