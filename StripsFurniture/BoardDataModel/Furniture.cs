using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BoardDataModel
{
    public class Furniture
    {
        private Rectangle furnitureDescription;
        public Furniture(Rectangle rec)
        {
            furnitureDescription = rec;
        }

        public Rectangle Description
        {
            get
            {
                return furnitureDescription;
            }
        }
    }
}
