﻿using System.Collections.Generic;
using System.Drawing;

namespace BusinessLogic
{
    public enum PredType
    {
        Clean,
        Location
    }

    /// <summary>
    /// predicates are from 2 types: 
    /// clean- meaning that the rect given in the predicate should not be allocated
    /// location- meaning that the given furniture should be placed in the given rect
    /// </summary>
    public class Predicate:StackItem
    {
        public PredType predType;

        public Predicate(PredType pt)
        {
            predType = pt;
        }
    }

    // derived classes
        public class PClean : Predicate
        {
            private IList<Rectangle> rectList;

            public PClean(IList<Rectangle> predRectList)
                : base(PredType.Clean)
            {
                rectList = predRectList;
            }
        }

        public class PLocation : Predicate
        {
            private int furnitureId;
            private Rectangle rect;

            public PLocation(int id, Rectangle predRect)
                : base(PredType.Clean)
            {
                furnitureId = id;
                rect = predRect;
            }

        }
    
}