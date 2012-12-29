using System.Collections.Generic;
using BoardDataModel;
using Heuristics;


namespace BusinessLogic
{
    public class Strips
    {
       Heuristic heuristic = new Heuristic();
        
        //maybe hold a list with all final operations- usefull for get next operation. 
        public Strips(Board board)
        {
            IList<Predicate> startState = new List<Predicate>();
            IList<Predicate> goalState = new List<Predicate>();
            startState = heuristic.FindStartStatePredicates(board);
            goalState = heuristic.FindGoalStatePredicates(board);
            //calculate predicates of start positions and to state
            //calculate predicates of final board and add to the work stack

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
