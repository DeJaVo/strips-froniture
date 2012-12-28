using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using BoardDataModel;
namespace BusinessLogic
{
    public class Strips
    {
        //maybe hold a list with all final operations- usefull for get next operation. 
        public Strips(Board board)
        {
            IList<Predicate> startState = new List<Predicate>();
            IList<Predicate> goalState = new List<Predicate>();
            startState = FindStartStatePredicates(board);
            goalState = FindGoalStatePredicates(board);
            //calculate predicates of start positions and to state
            //calculate predicates of final board and add to the work stack

        }

        /// <summary>
        /// finds all predicates that describe current position.
        /// </summary>
        /// <param name="board"></param>
        /// <returns>returns a list of predicates </returns>
        private IList<Predicate> FindStartStatePredicates(Board board)
        {
            List<Predicate> predicates = new List<Predicate>();
            foreach (KeyValuePair<Furniture, Rectangle> pair in board.furnitureDestination)
            {                
                int id = pair.Key.ID;
                Rectangle rect = pair.Key.Description;               
                Predicate newPredicate = new PLocation(id,rect);
                predicates.Add(newPredicate);
            }
            return predicates;
        }

        /// <summary>
        /// finds all predicates that describe the goal positions
        /// </summary>
        /// <param name="board"></param>
        /// <returns>return a list of predicates</returns>
        private IList<Predicate> FindGoalStatePredicates(Board board)
        {
            IList<Predicate> predicates = new List<Predicate>();
            foreach ( KeyValuePair<Furniture, Rectangle> pair in board.furnitureDestination )
            {
                IList<Rectangle> rectangels=new List<Rectangle>();
                int id = pair.Key.ID;
                Rectangle rect = pair.Value;
                rectangels.Add(rect);
                Predicate newPredicate = new PClean(rectangels);
                predicates.Add(newPredicate);
            }
            return predicates;
        }

        /// <summary>
        /// returns the next move to execute
        /// </summary>
        /// <returns></returns>
        public Operation GetNextOperation()
        {
            return new Operation();
        }

    }
}
