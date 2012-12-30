using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using BoardDataModel;

namespace Heuristics
{
    public interface IHeuristic
    {
        IList<StackItem> OrderPredicates(IList<StackItem> predicates);

        Operation ChooseOperation(Board board,Predicate predicateToSatisfy);
    }
}
