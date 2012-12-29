using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardDataModel;

namespace BusinessLogic
{
    public enum OperationType
    {
        Move,
        Rotate
    }

    public class Operation
    {
        public  OperationType OperationType { get; set; }
        public  int FurnitureId { get; set; }
        public  Furniture FurnitureOldData { get; set; }
        public  Furniture FurnitureNewData { get; set; }

        /// <summary>
        ///  Execute 
        ///  </summary>
        public virtual void Execute(Furniture furniture)
        {
            throw new NotImplementedException();
        }
    }
}