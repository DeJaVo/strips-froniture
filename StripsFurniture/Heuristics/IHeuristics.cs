using System.Collections.Generic;
using BoardDataModel;

namespace Heuristics
{
    public interface IHeuristic
    {
        IList<StackItem> OrderPredicates(IList<StackItem> predicates);

        Operation ChooseOperation(Board board, Predicate predicateToSatisfy);
    }
}