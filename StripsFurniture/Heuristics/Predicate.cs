using System.Collections.Generic;
using System.Drawing;

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
            public Rectangle cleanRect;

            public PClean(Rectangle predRect)
            {
                cleanRect = predRect;
            }
        }

        public class PLocation : Predicate
        {
            public int furnitureId;
            public Rectangle rect;

            public PLocation(int id, Rectangle predRect)
            {
                furnitureId = id;
                rect = predRect;
            }

        }
    
}
