using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
    public class Predicate
    {
        public Predicate(PredType PT)
        {
            predType = PT;
        }

        private PredType predType;

    }

    public class PClean : Predicate
    {
        public PClean(IList<Rectangle> predRectList) : base(PredType.Clean)
        {
            rectList = predRectList;
        }    
        private IList<Rectangle> rectList;
    }

    public class PLocation : Predicate
    {
        public PLocation(int id, Rectangle predRect) : base(PredType.Clean)
        {
            furnitureId = id;
            rect = predRect;
        }

        private int furnitureId;
        private Rectangle rect;
    }
}
