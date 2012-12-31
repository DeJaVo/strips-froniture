using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using BoardDataModel;

namespace Heuristics
{
    public enum OperationType
    {
        Move,
        Rotate
    }

    public abstract class Operation:StackItem
    {
        protected Furniture furniture;
        public  Rectangle FurnitureOldData { get; set; }
        public  Rectangle FurnitureNewData { get; set; }
        public Furniture Furniture { get { return furniture; } set { furniture = value; } }

        /// <summary>
        ///  Execute 
        ///  </summary>
        public abstract void Execute();
    }
}