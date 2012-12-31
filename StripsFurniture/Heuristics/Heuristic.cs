using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BoardDataModel;
namespace Heuristics
{
    public class Heuristic : IHeuristic
    {
        #region OrderPredicates
        /// <summary>
        /// function that orders a predicates list        
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns> a list of ordered predicates- the first predicate is the first one to fulfill</returns>
        public IList<StackItem> OrderPredicates(IList<StackItem> predicates)
        {
            // split list to sub list according to predicates type
            // if there is more than one location pred - call smart heuristic
            //else- put 'clean' predicates before 'location' predicates

            //split list to sub list- assuming there are 2 kind
            IList<StackItem> result = new List<StackItem>();
            IList<StackItem> pClean = new List<StackItem>();
            IList<StackItem> pLocation = new List<StackItem>();
            IList<IList<StackItem>> kindsOfPredicates = SplitPredicatesPerKind(predicates);
            pClean = kindsOfPredicates.First();
            pLocation = kindsOfPredicates.Last();

            if (pLocation.Count > 1)
            {
                //use smart heuristic for ordering the pLocation predicates;
                IList<StackItem> orderedPLocation = new List<StackItem>();
                orderedPLocation = OrderLocationPredicate(pLocation);
                //for now we settle for taking all the pClean's as they are ordered, maybe we can add heurisitc that orders them as well.
                result = (IList<StackItem>)result.Concat(pClean).ToList();
                result = (IList<StackItem>)result.Concat(orderedPLocation).ToList();
                return result;

            }
            else
            {
                //for now we settle for taking all the pClean's as they are ordered, maybe we can add heurisitc that orders them as well.
                result = (IList<StackItem>)result.Concat(pClean).ToList();
                result = (IList<StackItem>)result.Concat(pLocation).ToList();
                return result;
            }


        }

        /// <summary>
        /// splits a list of predicates into 2 sub list - first is clean pred, second is location pred;
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        private IList<IList<StackItem>> SplitPredicatesPerKind(IList<StackItem> predicates)
        {
            IList<StackItem> pClean = new List<StackItem>();
            IList<StackItem> pLocation = new List<StackItem>();

            foreach (var predicate in predicates)
            {
                if (predicate is PClean)
                    pClean.Add(predicate);
                if (predicate is PLocation)
                    pLocation.Add(predicate);
            }
            IList<IList<StackItem>> predicatePerKind = new List<IList<StackItem>>();
            predicatePerKind.Add(pClean);
            predicatePerKind.Add(pLocation);

            return predicatePerKind;
        }
        private IList<StackItem> OrderLocationPredicate(IList<StackItem> pLocation)
        {
            //need to be implemented

            return pLocation;
        }

        #endregion

        public Operation ChooseOperation(Board board, Predicate predicateToSatisfy)
        {
            //TODO : need to be implemented 

            //Move move = new Move(((PLocation)predicateToSatisfy).furniture);
            //move.Direction = Direction.Down;
            //move.HowManyStepsInDirection = 1;
            //return move;

            Rotate rotate = new Rotate(((PLocation)predicateToSatisfy).furniture);
            rotate.RotationDirection=RotationDirection.ClockWise;
            return rotate;
        }
    }
}
