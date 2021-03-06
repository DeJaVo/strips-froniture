﻿using System.Collections.Generic;
using System.Linq;
using BoardDataModel;
using System.Drawing;
using Heuristics;

namespace BusinessLogic
{
    public class Strips
    {
        //public IList<Operation> operationList= new List<Operation>();
        private readonly Stack<IList<StackItem>> stack = new Stack<IList<StackItem>>();
        private readonly Board board;
        private readonly IHeuristic heuristics;

        //maybe hold a list with all final operations- usefull for get next operation. 
        public Strips(Board board)
        {
            this.board = board;

            var goalState = FindGoalStatePredicates();
            stack.Push(goalState);

            heuristics = new Heuristic();
        }

        /// <summary>
        /// strips step
        /// returns the executed operation.
        /// if there was no Operation in the current step returns null
        /// </summary>
        private Operation StripsStep()
        {
            var item = stack.Peek();
            bool satisfied = false;
            bool allArePredicateKind = CheckAllArePredicate(item);
            if (allArePredicateKind)
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
                // the list is ordered in the order of the execution
                IList<StackItem> ordered = heuristics.OrderPredicates(item);

                // push the items in the list in reverse order so we will get in the top of the stack
                // the first item to execute
                for (int i = ordered.Count - 1; i >= 0; i--)
                {
                    var tempList = new List<StackItem>();
                    tempList.Add(ordered[i]);
                    stack.Push(tempList);
                }
                return null;
            }
            // case 3- goal is a single unsatisfied goal- choose an operation push it and all its pre condtiotions
            if (allArePredicateKind && item.Count == 1 && !satisfied)
            {
                Operation operation = heuristics.ChooseOperation(board, (Predicate) item[0]);
                stack.Push(new List<StackItem> {operation});

                // push it's pre condition into stack - need to implement a function that calculates precondition per move
                if (operation is Move)
                {
                    Rectangle rectToBeClean = ((Move) operation).CalculateRectDiff();
                    var clean = new PClean(rectToBeClean);
                    clean.Forbbiden = operation.ForbbidenDirections();
                    stack.Push(new List<StackItem> {clean});
                }
                    // Rotate
                else
                {
                    Rectangle rect = ((Rotate) operation).CalculateRectToBeCleanByDirection();
                    var clean = new PClean(rect);
                    clean.Forbbiden = operation.ForbbidenDirections();
                    stack.Push(new List<StackItem> {clean});
                }
                return null;
            }
            //case 4- item is an operation- pop it, execute it and add to operation list
            if (item.Count == 1 && item.First() is Operation)
            {
                var operation = stack.Pop().First();

                ((Operation) operation).Execute();
                return (Operation) operation;
            }

            return null;
        }

        /// <summary>
        /// returns the next move to execute
        /// </summary>
        /// <returns></returns>
        public Operation GetNextOperation()
        {
            pause = false;
            Operation opToPerform = null;
            while ((stack.Count > 0) && (opToPerform == null) && !pause)
            {
                opToPerform = StripsStep();
            }

            return opToPerform;
        }

        private bool pause = false;

        public void Pause()
        {
            pause = true;
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
        /// <returns></returns>
        private bool GoalsAreSatisfied(IList<StackItem> predicatesToSatisfy)
        {
            foreach (StackItem currItem in predicatesToSatisfy)
            {
                Predicate currPredicateToSatisfy = (Predicate) currItem;
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

        /// <summary>
        /// Checks if predicate rectangle is clean
        /// </summary>
        /// <param name="cleanPredicate"></param>
        /// <returns></returns>
        private bool CheckClean(PClean cleanPredicate)
        {
            return this.board.IsEmpty(cleanPredicate.CleanRect);
        }

        /// <summary>
        /// Checks if predicate rectangle is in location rectangle
        /// </summary>
        /// <param name="locationPredicate"></param>
        /// <returns></returns>
        private bool CheckLocation(PLocation locationPredicate)
        {
            return this.board.IsFurnitureInRectangle(locationPredicate.furniture.ID, locationPredicate.rect);
        }

        /// <summary>
        /// finds all predicates that describe the goal positions
        /// </summary>
        /// <returns>return a list of predicates</returns>
        public IList<StackItem> FindGoalStatePredicates()
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