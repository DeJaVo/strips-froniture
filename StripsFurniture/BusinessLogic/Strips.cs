using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardDataModel;
using System.Drawing;


namespace BusinessLogic
{
    public class Strips
    {
        //public IList<Operation> operationList= new List<Operation>();
        private Stack<IList<StackItem>> stack = new Stack<IList<StackItem>>();
        private Board board;

        //maybe hold a list with all final operations- usefull for get next operation. 
        public Strips(Board board)
        {
            this.board = board;

            var goalState = FindGoalStatePredicates(board);
            stack.Push(goalState);

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
        public Operation StripsStep()
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
                IList<StackItem> ordered =OrderPredicates(item);
                foreach (var pred in ordered)
                {
                    var tempList = new List<StackItem>();
                    tempList.Add(pred);
                    stack.Push(tempList);
                }
                return null;
            }
            // case 3- goal is a single unsatisfied goal- choose an operation push it and all is pre condtiotions
            if (allArePredicateKind && item.Count == 1 && !satisfied)
            {
                StackItem operation = ChooseOperation(board, item);
                stack.Push((IList<StackItem>)operation);
                // push it's pre condition into stack- need to implement a function that calculates precondition per move

                return null;
            }
            //case 4- item is an operation- pop it, execute it and add to operation list
            if (item.Count==1 && item.First() is Operation)
            {
                var operation = stack.Pop().First();
                //operation.execute
                //operationList.Add((Operation)operation);
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
        private bool GoalsAreSatisfied(IList<StackItem> item)
        {
           //need to be implemented- for now returns true
            return true;
        }
        private StackItem ChooseOperation(Board board, IList<StackItem> item)
        {
            //need to be implemented 
            
            Move move= new Move();
            move.FurnitureId = 1;
            move.OperationType = OperationType.Move;
            move.Direction=Direction.Right;
            move.HowManyStepsInDirection = 1;
            //move.FurnitureNewData ???
            //move.FurnitureOldData ??

            throw new System.NotImplementedException();
        }


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
                result = (IList<StackItem>)result.Concat(pClean);
                result = (IList<StackItem>)result.Concat(orderedPLocation);
                return result;

            }
            else
            {
                //for now we settle for taking all the pClean's as they are ordered, maybe we can add heurisitc that orders them as well.
                result = (IList<StackItem>)result.Concat(pClean);
                result = (IList<StackItem>)result.Concat(pLocation);
                return result;
            }


        }

       public IList<StackItem> OrderLocationPredicate(IList<StackItem> pLocation)
        {
           //need to be implemented

            return pLocation;
        }

        /// <summary>
        /// splits a list of predicates into 2 sub list - first is clean pred, second is location pred;
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        public IList<IList<StackItem>> SplitPredicatesPerKind(IList<StackItem> predicates)
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

