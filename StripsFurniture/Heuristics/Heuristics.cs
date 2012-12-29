using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using BoardDataModel;


namespace Heuristics
{
    public class Heuristic
    {
        /// <summary>
        /// finds all predicates that describe current position.
        /// </summary>
        /// <param name="board"></param>
        /// <returns>returns a list of predicates </returns>
        public IList<Predicate> FindStartStatePredicates(Board board)
        {
            List<Predicate> predicates = new List<Predicate>();
            foreach (KeyValuePair<Furniture, Rectangle> pair in board.furnitureDestination)
            {
                int id = pair.Key.ID;
                Rectangle rect = pair.Key.Description;
                Predicate newPredicate = new PLocation(id, rect);
                predicates.Add(newPredicate);
            }
            return predicates;
        }

        /// <summary>
        /// Heuristic function that orders a predicates list        
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns> a list of ordered predicates- the first predicate is the first one to fulfill</returns>
        private IList<Predicate> OrderPredicates(IList<Predicate> predicates)
        {
            // split list to sub list according to predicates type
            // if there is more than one location pred - call smart heuristic
            //else- put 'clean' predicates before 'location' predicates

            //split list to sub list- assuming there are 2 kind
            IList<Predicate> result = new List<Predicate>();
            IList<Predicate> pClean = new List<Predicate>();
            IList<Predicate> pLocation = new List<Predicate>();
            IList<IList<Predicate>> kindsOfPredicates = SplitPredicatesPerKind(predicates);
            pClean = kindsOfPredicates.First();
            pLocation = kindsOfPredicates.Last();

            if (pLocation.Count > 1)
            {
                //use smart heuristic for ordering the pLocation predicates;
                IList<Predicate> orderedPLocation = new List<Predicate>();
                orderedPLocation = OrderPLocationPredicate(pLocation);
                //for now we settle for taking all the pClean's as they are ordered, maybe we can add heurisitc that orders them as well.
                result = (IList<Predicate>)result.Concat(pClean);
                result = (IList<Predicate>)result.Concat(orderedPLocation);
                return result;

            }
            else
            {
                //for now we settle for taking all the pClean's as they are ordered, maybe we can add heurisitc that orders them as well.
                result = (IList<Predicate>)result.Concat(pClean);
                result = (IList<Predicate>)result.Concat(pLocation);
                return result;
            }


        }

        private IList<Predicate> OrderPLocationPredicate(IList<Predicate> pLocation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// splits a list of predicates into 2 sub list - first is clean pred, second is location pred;
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        private IList<IList<Predicate>> SplitPredicatesPerKind(IList<Predicate> predicates)
        {
            IList<Predicate> pClean = new List<Predicate>();
            IList<Predicate> pLocation = new List<Predicate>();

            foreach (var predicate in predicates)
            {
                if (predicate.predType == PredType.Clean)
                    pClean.Add(predicate);
                if (predicate.predType == PredType.Location)
                    pLocation.Add(predicate);
            }
            IList<IList<Predicate>> predicatePerKind = new List<IList<Predicate>>();
            predicatePerKind.Add(pClean);
            predicatePerKind.Add(pLocation);

            return predicatePerKind;
        }

        /// <summary>
        /// finds all predicates that describe the goal positions
        /// </summary>
        /// <param name="board"></param>
        /// <returns>return a list of predicates</returns>
        public IList<Predicate> FindGoalStatePredicates(Board board)
        {
            IList<Predicate> predicates = new List<Predicate>();
            foreach (KeyValuePair<Furniture, Rectangle> pair in board.furnitureDestination)
            {
                IList<Rectangle> rectangels = new List<Rectangle>();
                int id = pair.Key.ID;
                Rectangle rect = pair.Value;
                rectangels.Add(rect);
                Predicate newPredicate = new PClean(rectangels);
                predicates.Add(newPredicate);
            }
            return predicates;
        }
    }
}
