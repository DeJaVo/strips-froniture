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
        private int furnitureId;
        private Furniture furnitureOldData;
        private Furniture furnitureNewData;
        private OperationType operationType;

        public OperationType OperationType
        {
            get
            {
                return operationType;
            }
        }
        public int FurnitureId
        {
            get
            {
                return furnitureId;
            }
        }

        public Furniture FurnitureOldData
        {
            get
            {
                return furnitureOldData;
            }
        }

        public Furniture FurnitureNewData
        {
            get
            {
                return furnitureNewData;
            }
        }
    }
}
