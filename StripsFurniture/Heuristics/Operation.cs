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
        public  OperationType OperationType { get; set; }
        public  int FurnitureId { get; set; }
        public  Rectangle FurnitureOldData { get; set; }
        public  Rectangle FurnitureNewData { get; set; }

        /// <summary>
        ///  Execute 
        ///  </summary>
        public abstract void Execute(Furniture furniture);
    }
}