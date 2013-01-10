using System.Collections.Generic;
using System.Drawing;

using BoardDataModel;
namespace Heuristics
{

    /// <summary>
    /// predicates are from 2 types: 
    /// clean- meaning that the rect given in the predicate should not be allocated
    /// location- meaning that the given furniture should be placed in the given rect
    /// </summary>
    public class Predicate:StackItem
    {
        public Predicate()
        {
        }
    }

    // derived classes
        public class PClean : Predicate
        {
            public Rectangle CleanRect { get; set; }

           // public List<Direction> Forbbiden { get; set; }

            public PClean(Rectangle predRect)
            {
                CleanRect = predRect;
            }
        }

        public class PLocation : Predicate
        {
            public Furniture furniture;
            public Rectangle rect;

            public PLocation(Furniture fur, Rectangle predRect)
            {
                furniture = fur;
                rect = predRect;
            }

        }
    
}
