using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardDataModel;
using System.Drawing;

using Heuristics;

namespace BusinessLogic
{
    public class Strips
    {
        //public IList<Operation> operationList= new List<Operation>();
        private Stack<IList<StackItem>> stack = new Stack<IList<StackItem>>();
        private Board board;
        private IHeuristic heuristics;

        //maybe hold a list with all final operations- usefull for get next operation. 
        public Strips(Board board)
        {
            this.board = board;

            var goalState = FindGoalStatePredicates(board);
            stack.Push(goalState);

            heuristics = new Heuristic();
            //while (stack.Count>0)
            //{
            //    StripsStep(stack, board);
            //}                     
        }

        /// <summary>
        /// strips step
        /// returns the executed operation.
        /// if there was no Operation in the current step returns null
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="board"></param>
        private Operation StripsStep()
        {          
            var item = stack.Peek();
            bool satisfied = false;
            bool allArePredicateKind = CheckAllArePredicate(item);
            if(allArePredicateKind)
                satisfied = GoalsAreSatisfied(item);
            
            //case 1- goals is satisfied- pop and return
            if (allArePredicateKind && satisfied)
            {
                stack.Pop();
                return null;
            }
            //case 2- goals is unsatisfied and conjunctive- order goals by heuristic and add each one to stack 
            if (allArePredicateKind && item.Count > 1 && !satisfied)
            {
                IList<StackItem> ordered = heuristics.OrderPredicates(item);
                foreach (var pred in ordered)
                {
                    var tempList = new List<StackItem>();
                    tempList.Add(pred);
                    stack.Push(tempList);
                }
                return null;
            }
            // case 3- goal is a single unsatisfied goal- choose an operation push it and all its pre condtiotions
            if (allArePredicateKind && item.Count == 1 && !satisfied)
            {
                Operation operation = heuristics.ChooseOperation(board, (Predicate)item[0]);
                stack.Push(new List<StackItem>{operation});

                // push it's pre condition into stack - need to implement a function that calculates precondition per move
                if (operation is Move)
                {
                    Rectangle rectToBeClean = ((Move)operation).CalculateRectDiff();
                    stack.Push(new List<StackItem>{new PClean(rectToBeClean)});
                }
                    // Rotate
                else
                {
                    Rectangle temp1;
                    ((Rotate)operation).CheckRotateByDirection(out temp1);
                    stack.Push(new List<StackItem> { new PClean(temp1) });
                }
                return null;
            }
            //case 4- item is an operation- pop it, execute it and add to operation list
            if (item.Count==1 && item.First() is Operation)
            {
                var operation = stack.Pop().First();

                ((Operation)operation).Execute();
                return (Operation)operation;
            }

            return null;
        }      

        /// <summary>
        /// returns the next move to execute
        /// </summary>
        /// <returns></returns>
        public Operation GetNextOperation()
        {
            Operation opToPerform = null;
            while ((stack.Count > 0) && (opToPerform == null))
            {
                opToPerform = StripsStep();
            }

            return opToPerform;
        }

        /// <summary>
        /// returns true if all itmes are stackitems from Predicate type
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool CheckAllArePredicate(IList<StackItem> item)
        {
            return item.All(t => t is Predicate);
        }

        /// <summary>
        /// checks if all goals are satisfied - yes return true, else return false
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool GoalsAreSatisfied(IList<StackItem> predicatesToSatisfy)
        {
            foreach (StackItem currItem in predicatesToSatisfy)
            {
                Predicate currPredicateToSatisfy = (Predicate)currItem;
                if (currPredicateToSatisfy is PClean)
                {
                    if (!this.CheckClean(currPredicateToSatisfy as PClean))
                    {
                        return false;
                    }
                }
                    // PLocation
                else
                {
                    if (!this.CheckLocation(currPredicateToSatisfy as PLocation))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckClean(PClean cleanPredicate)
        {
            return this.board.IsEmpty(cleanPredicate.cleanRect);
        }

        private bool CheckLocation(PLocation locationPredicate)
        {
            return this.board.IsFurnitureInRectangle(locationPredicate.furniture.ID, locationPredicate.rect);
        }
        
        /// <summary>
        /// finds all predicates that describe the goal positions
        /// </summary>
        /// <param name="board"></param>
        /// <returns>return a list of predicates</returns>
        public IList<StackItem> FindGoalStatePredicates(Board board)
        {
            IList<StackItem> predicates = new List<StackItem>();
            foreach (KeyValuePair<Furniture, Rectangle> pair in board.furnitureDestination)
            {
                Predicate newPredicate = new PLocation(pair.Key, pair.Value);
                predicates.Add(newPredicate);
            }
            return predicates;
        }
    }




    }

