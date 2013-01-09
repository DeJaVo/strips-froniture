﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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

        #region PLocation sorting

        private IList<StackItem> OrderLocationPredicate(IList<StackItem> pLocations)
        {
            // split furnitures into groups
            List<Group> groups = this.SplitToGroups(pLocations);

            // sort in groups
            foreach (Group currGroup in groups)
            {
                this.SortGroup(currGroup);
                currGroup.CalcRepresentativePath();
            }

            // sort between groups
            groups.Sort(new GroupsComparer());

            return this.MergeGroups(groups);
        }

        enum GroupType
        {
            PassDoor,
            SameRoom
        }

        class Group
        {
            private GroupType groupType;
            private List<PLocation> furnitures;
            private List<Rectangle> path;

            //public Group()
            //{
            //    this.groupType = groupType;
            //    furnitures = new List<PLocation>();
            //}

            public Group(GroupType groupType)
            {
                this.groupType = groupType;
                furnitures = new List<PLocation>();
            }

            public GroupType GroupType
            {
                get
                {
                    return groupType;
                }
            }
            public List<PLocation> Furnitures
            {
                get
                {
                    return furnitures;
                }
            }
            // relevant only when groupType is PassDoor type
            public bool PassUpperDoor
            {
                get;
                set;
            }

            public List<int> DoorsPath
            {
                get;
                set;
            }

            public void CalcRepresentativePath()
            {
                // take the representative as the last in the sort
                Rectangle startState = this.Furnitures[this.Furnitures.Count - 1].furniture.Description;
                Rectangle destState = this.Furnitures[this.Furnitures.Count - 1].rect;

                //path = Group.CalcRepresentativePath(startState, destState, this.DoorsPath);
                path = Heuristic.CalculatePathByRect(startState, destState);
            }

            //public static List<Rectangle> CalcRepresentativePath(Rectangle startState, Rectangle destState)
            //{
            //    List<int> doorsPath = new List<int>();
            //    int startRoom = Board.Instance.FindRoomPerRect(startState);
            //    int endRoom = Board.Instance.FindRoomPerRect(destState);

            //    if (startRoom != endRoom)
            //    {
            //        if ((startRoom == 1) || (endRoom == 1))
            //        {
            //            if (startRoom != 1)
            //            {
            //                doorsPath.Add(startRoom);
            //            }
            //            else
            //            {
            //                doorsPath.Add(endRoom);
            //            }
            //        }
            //        else
            //        {
            //            doorsPath.Add(startRoom);
            //            doorsPath.Add(endRoom);
            //        }
            //    }
            //    return Group.CalcRepresentativePath(startState, destState, doorsPath);
            //}

            //private static List<Rectangle> CalcRepresentativePath(Rectangle startState, Rectangle destState, List<int> doorsPath)
            //{
            //    List<Rectangle> path = new List<Rectangle>();

            //    // the representative is the last in the list of furnitures
            //    // (assuming that is sorted)

            //    //if (this.GroupType == Heuristic.GroupType.SameRoom)
            //    if (doorsPath.Count == 0)
            //    {
            //        path = FindPathBetweenPoints(startState, destState);
            //    }
            //    else
            //    {
            //        path = new List<Rectangle>();
            //        if (doorsPath.Count == 1)
            //        {
            //            Rectangle roomDoor = GetRoomDoor(doorsPath[0]);                      
            //            path.AddRange(FindPathBetweenPoints(startState, roomDoor));
            //            path.AddRange(FindPathBetweenPoints(roomDoor, destState));
            //        }
            //        else
            //        {
            //            Rectangle firstRoomDoor = GetRoomDoor(doorsPath[0]);
            //            Rectangle lastRoomDoor = GetRoomDoor(doorsPath[1]);
            //            path.AddRange(FindPathBetweenPoints(startState,firstRoomDoor));
            //            path.AddRange(FindPathBetweenDoors(firstRoomDoor, lastRoomDoor));
            //            path.AddRange(FindPathBetweenPoints(lastRoomDoor, destState));
            //        }
            //    }

            //    return path;
            //}

            private static List<Rectangle> FindPathBetweenDoors(Rectangle start, Rectangle end)
            {
                // building for door room 2 to door room 3
                List<Rectangle> subPath = new List<Rectangle>();
                subPath.Add(new Rectangle(10, 3, 1, 1));
                for (int i = 3; i <= 8; i++)
                {
                    subPath.Add(new Rectangle(10, i, 1, 1));
                }
                subPath.Add(new Rectangle(11, 8, 1, 1));

                return subPath;
            }

            public static List<Rectangle> FindPathBetweenPoints(Rectangle start, Rectangle end)
            {
                
                List<Rectangle> subPath = new List<Rectangle>();

                int deltaY = Math.Sign(end.Y - start.Y);
                int deltaX = Math.Sign(end.X - start.X);
                
                for (int i = start.Y; i != end.Y; i += deltaY)
                {
                    subPath.Add(new Rectangle(start.X, i, 1, 1));
                }

                for (int i = start.X; i != end.X; i += deltaX)
                {
                    subPath.Add(new Rectangle(i, end.Y, 1, 1));
                }

                return subPath;
            }

            public static Rectangle GetRoomDoor(int roomId)
            {
                if (roomId == 2)
                {
                    return new Rectangle(11, 3,1,1);
                }
                    // roomId = 3
                else
                {
                    return new Rectangle(11, 8,1,1);
                }
            }

            public List<Rectangle> Path
            {
                get
                {
                    return path;
                }
            }

            public List<Rectangle> GetFurnituresInStart()
            {
                List<Rectangle> furnituresInStart = new List<Rectangle>();
                foreach (PLocation currLoc in this.Furnitures)
                {
                    furnituresInStart.Add(currLoc.furniture.Description);
                }

                return furnituresInStart;
            }

            public List<Rectangle> GetFurnituresInDest()
            {
                List<Rectangle> furnituresInDest = new List<Rectangle>();
                foreach (PLocation currLoc in this.Furnitures)
                {
                    furnituresInDest.Add(currLoc.rect);
                }

                return furnituresInDest;
            }
        }

        #region Sort btween groups
        class GroupsComparer : IComparer<Group>
        {
            public int Compare(Group g1, Group g2)
            {
                int res = this.InternalCompare(g1, g2);
                if (res != 0)
                {
                    return res;
                }

                // perform the same checks when g2 is g1 
                res = this.InternalCompare(g2, g1);
                if (res != 0)
                {
                    return res == 1? -1 : 1;
                }

                // else g1 and g2 are equal
                return 0;
            }

            private int InternalCompare(Group tested, Group other)
            {
                List<Rectangle> otherGroupFurnituresInStart = other.GetFurnituresInStart();
                List<Rectangle> otherGroupFurnituresInDest = other.GetFurnituresInDest();

                List<Rectangle> testedGroupFurnituresInStart = tested.GetFurnituresInStart();
                List<Rectangle> testedGroupFurnituresInDest = tested.GetFurnituresInDest();

                // if a furniture from g2 in start state is on the path of g1 than g1 is smaller than g2
                if (IsOnPath(tested.Path, otherGroupFurnituresInStart))
                {
                    return 1;
                }

                // else if a furniture from g2 in dest state is on the path of g1 than g1 is bigger than g2
                if (IsOnPath(tested.Path, otherGroupFurnituresInDest))
                {
                    return -1;
                }

                // if a furniture from g1 in start state is on the path of g2 than g2 is smaller than g1
                if (IsOnPath(other.Path, testedGroupFurnituresInStart))
                {
                    return -1;
                }

                // else if a furniture from g1 in dest state is on the path of g2 than g2 is bigger than g1
                if (IsOnPath(other.Path, testedGroupFurnituresInDest))
                {
                    return 1;
                }
                return 0;
            }

            public static bool IsOnPath(List<Rectangle> path,List<Rectangle> furnitures)
            {
                foreach (Rectangle currPathPart in path)
                {
                    foreach (Rectangle currFur in furnitures)
                    {
                        if (currFur.IntersectsWith(currPathPart))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
        #endregion

        #region SplitToGroups
        private List<Group> SplitToGroups(IList<StackItem> locations)
        {
            // foreach room build a list of furnitures in start state and list of furnitures in destination state
            List<Furniture> room1StartStateFurnitures, room1DestStateFurnitures;
            List<Furniture> room2StartStateFurnitures, room2DestStateFurnitures;
            List<Furniture> room3StartStateFurnitures, room3DestStateFurnitures;
            this.FindFurnitures(locations, out room1StartStateFurnitures, out room1DestStateFurnitures,
                                out room2StartStateFurnitures, out room2DestStateFurnitures,
                                out room3StartStateFurnitures, out room3DestStateFurnitures);
            
            // foreach room create a SameRoom-Group with all the furnitures in destination state and these furnitures are in start state in the current room
            Group sameRoomGroupInRoom1 = this.FindSameRoomGroups(room1StartStateFurnitures, room1DestStateFurnitures);
            Group sameRoomGroupInRoom2 = this.FindSameRoomGroups(room2StartStateFurnitures, room2DestStateFurnitures);
            Group sameRoomGroupInRoom3 = this.FindSameRoomGroups(room3StartStateFurnitures, room3DestStateFurnitures);

            // Create a PassDoor-Group with all the furnitures in destination state in the same room and these furnitures are in start state in other room together
            List < Group > passDoorGroups = this.FindPassDoorGroups(room1StartStateFurnitures, room1DestStateFurnitures, 
                                                                    room2StartStateFurnitures, room2DestStateFurnitures, 
                                                                    room3StartStateFurnitures, room3DestStateFurnitures);
            List < Group > groups = passDoorGroups;
            if (sameRoomGroupInRoom1 != null)
            {
                groups.Add(sameRoomGroupInRoom1);
            }
            if (sameRoomGroupInRoom2 != null)
            {
                groups.Add(sameRoomGroupInRoom2);
            }
            if (sameRoomGroupInRoom3 != null)
            {
                groups.Add(sameRoomGroupInRoom3);
            }
            return groups;
        }
        private void FindFurnitures(IList<StackItem> locations, 
                                    out List<Furniture> room1StartStateFurnitures, out List<Furniture> room1DestStateFurnitures,
                                    out List<Furniture> room2StartStateFurnitures, out List<Furniture> room2DestStateFurnitures,
                                    out List<Furniture> room3StartStateFurnitures, out List<Furniture> room3DestStateFurnitures)
        {
            room1StartStateFurnitures = new List<Furniture>();
            room1DestStateFurnitures = new List<Furniture>();
            room2StartStateFurnitures = new List<Furniture>();
            room2DestStateFurnitures = new List<Furniture>();
            room3StartStateFurnitures = new List<Furniture>();
            room3DestStateFurnitures = new List<Furniture>();

            foreach (StackItem currLocation in locations)
            {
                Rectangle destRect = ((PLocation)currLocation).rect;
                Rectangle startRect = ((PLocation)currLocation).furniture.Description;

                this.UpdateRoomsStart(((PLocation)currLocation).furniture,
                                ref room1StartStateFurnitures,
                                ref room2StartStateFurnitures,
                                ref room3StartStateFurnitures);

                this.UpdateRoomsDest(destRect, ((PLocation)currLocation).furniture.ID,
                                ref room1DestStateFurnitures,
                                ref room2DestStateFurnitures,
                                ref room3DestStateFurnitures);
            }
        }

        private void UpdateRoomsStart(Furniture fur,
                                    ref List<Furniture> room1Furnitures,
                                    ref List<Furniture> room2Furnitures,
                                    ref List<Furniture> room3Furnitures)
        {
            Rectangle rect = fur.Description;

            // room 1
            if (rect.X <= 11)
            {
                room1Furnitures.Add(fur);
            }
            // room 2
            else if (rect.Y <= 5)
            {
                room2Furnitures.Add(fur);
            }
            // room 3
            else
            {
                room3Furnitures.Add(fur);
            }
        }

        private void UpdateRoomsDest(Rectangle rect, int id,
                                    ref List<Furniture> room1Furnitures,
                                    ref List<Furniture> room2Furnitures,
                                    ref List<Furniture> room3Furnitures)
        {
            // room 1
            if (rect.X <= 11)
            {
                room1Furnitures.Add(new Furniture(rect, id));
            }
                // room 2
            else if (rect.Y <= 5)
            {
                room2Furnitures.Add(new Furniture(rect, id));
            }
            // room 3
            else
            {
                room3Furnitures.Add(new Furniture(rect, id));
            }
        }

        private Group FindSameRoomGroups(List<Furniture> roomStartStateFurnitures, List<Furniture> roomDestStateFurnitures)
        {
            Group sameRoomGroup = new Group(GroupType.SameRoom);
            List<Furniture> roomStartStateFurnituresCpy = new List<Furniture>(roomStartStateFurnitures);
            List<Furniture> roomDestStateFurnituresCpy = new List<Furniture>(roomDestStateFurnitures);

            foreach (Furniture currDestFurniture in roomDestStateFurnituresCpy)
            {
                foreach (Furniture currStartFurniture in roomStartStateFurnituresCpy)
                {
                    if (currDestFurniture.ID == currStartFurniture.ID)
                    {
                        sameRoomGroup.Furnitures.Add(new PLocation(currStartFurniture, currDestFurniture.Description));
                        roomStartStateFurnitures.Remove(currStartFurniture);
                        roomDestStateFurnitures.Remove(currDestFurniture);
                    }
                }
            }

            if (sameRoomGroup.Furnitures.Count == 0)
            {
                return null;
            }
            sameRoomGroup.DoorsPath = new List<int>();
            return sameRoomGroup;
        }

        private List<Group> FindPassDoorGroups(List<Furniture> room1StartStateFurnitures,List<Furniture> room1DestStateFurnitures, 
                                               List<Furniture> room2StartStateFurnitures,List<Furniture> room2DestStateFurnitures,
                                               List<Furniture> room3StartStateFurnitures, List<Furniture> room3DestStateFurnitures)
        {
            List<Group> groupsTemp = new List<Group>();

            groupsTemp.Add(this.FindPassDoorGroups(room1DestStateFurnitures, room2StartStateFurnitures,true,new List<int>{2}));
            groupsTemp.Add(this.FindPassDoorGroups(room1DestStateFurnitures, room3StartStateFurnitures, false, new List<int> { 3 }));
            groupsTemp.Add(this.FindPassDoorGroups(room2DestStateFurnitures, room1StartStateFurnitures, true, new List<int> { 2}));
            groupsTemp.Add(this.FindPassDoorGroups(room2DestStateFurnitures, room3StartStateFurnitures, false, new List<int> { 2,3 }));
            groupsTemp.Add(this.FindPassDoorGroups(room3DestStateFurnitures, room1StartStateFurnitures, false, new List<int> { 3 }));
            groupsTemp.Add(this.FindPassDoorGroups(room3DestStateFurnitures, room2StartStateFurnitures, true, new List<int> { 3,2 }));

            List<Group> groups = new List<Group>();
            foreach (Group currGroup in groupsTemp)
            {
                if (currGroup != null)
                {
                    groups.Add(currGroup);
                }
            }

            return groups;
        }

        private Group FindPassDoorGroups(List<Furniture> destStateFurnitures, List<Furniture> startStateFurnitures,bool upperDoor,List<int> doorsPath)
        {
            List<PLocation> groupsFurnitures = new List<PLocation>();
            foreach (Furniture currFurInStartState in startStateFurnitures)
            {
                foreach (Furniture currFurInDestState in destStateFurnitures)
                {
                    if (currFurInStartState.ID == currFurInDestState.ID)
                    {
                        groupsFurnitures.Add(new PLocation(currFurInStartState, currFurInDestState.Description));
                    }
                }    
            }

            if (groupsFurnitures.Count == 0)
            {
                return null;
            }

            Group group = new Group(GroupType.PassDoor);
            group.Furnitures.AddRange(groupsFurnitures);
            group.PassUpperDoor = upperDoor;
            group.DoorsPath = doorsPath;
            return group;
        }
        #endregion SplitToGroups

        #region Sort group members
        private void SortGroup(Group group)
        {
            if (group.GroupType == GroupType.PassDoor)
            {
                group.Furnitures.Sort(new FurniturePassDoorComparer(group.PassUpperDoor));
            }
            else
            {
                group.Furnitures.Sort(new FurnitureInSameRoomComparer());
            }
        }
        class FurniturePassDoorComparer : IComparer<PLocation>
        {
            Point doorLocation;
            public FurniturePassDoorComparer(bool upperDoor)
            {
                if (upperDoor)
                {
                    doorLocation = new Point(11, 3);
                }
                else
                {
                    doorLocation = new Point(11,8);
                }
            }
            public int Compare(PLocation ploc1, PLocation ploc2)
            {
                double dist1 = Math.Pow(ploc1.rect.X - doorLocation.X, 2) +
                               Math.Pow(ploc1.rect.Y - doorLocation.Y, 2);

                double dist2 = Math.Pow(ploc2.rect.X - doorLocation.X, 2) +
                               Math.Pow(ploc2.rect.Y - doorLocation.Y, 2);

                if (dist1 == dist2)
                {
                    return 0;
                }
                return dist1 < dist2 ? 1 : -1;
            }
        }
        class FurnitureInSameRoomComparer : IComparer<PLocation>
        {
            public int Compare(PLocation ploc1, PLocation ploc2)
            {
                //double dist1 = Math.Pow(ploc1.furniture.Description.X - ploc1.rect.X, 2) +
                //               Math.Pow(ploc1.furniture.Description.Y - ploc1.rect.Y, 2);

                //double dist2 = Math.Pow(ploc2.furniture.Description.X - ploc2.rect.X, 2) +
                //               Math.Pow(ploc2.furniture.Description.Y - ploc2.rect.Y, 2);

                //if (dist1 == dist2)
                //{
                //    return 0;
                //}
                //return dist1 > dist2 ? 1 : -1;

                List<Rectangle> ploc1Path = Heuristic.CalculatePathByRect(ploc1.furniture.Description, ploc1.rect);
                List<Rectangle> ploc2Path = Heuristic.CalculatePathByRect(ploc2.furniture.Description, ploc2.rect);

                // if a furniture from g2 in start state is on the path of g1 than g1 is smaller than g2
                if (GroupsComparer.IsOnPath(ploc1Path, new List<Rectangle> {ploc2.furniture.Description}))
                {
                    return 1;
                }

                // else if a furniture from g2 in dest state is on the path of g1 than g1 is bigger than g2
                if (GroupsComparer.IsOnPath(ploc1Path, new List<Rectangle> { ploc2.rect }))
                {
                    return -1;
                }

                // if a furniture from g1 in start state is on the path of g2 than g2 is smaller than g1
                if (GroupsComparer.IsOnPath(ploc2Path, new List<Rectangle> { ploc1.furniture.Description }))
                {
                    return -1;
                }

                // else if a furniture from g1 in dest state is on the path of g2 than g2 is bigger than g1
                if (GroupsComparer.IsOnPath(ploc2Path, new List<Rectangle> { ploc1.rect }))
                {
                    return 1;
                }

                return 0;

            }
        }
        #endregion

        private IList<StackItem> MergeGroups(List<Group> groups)
        {
            List<StackItem> mergedGroups = new List<StackItem>();
            foreach (Group currGroup in groups)
            {
                mergedGroups.AddRange(currGroup.Furnitures);
            }

            return mergedGroups;
        }
        #endregion
        #endregion

        private List<Direction> SortRemainingDirections(Direction forbidenDir, List<Direction> remainingDirections,
                                                        Rectangle rectToClean,Rectangle rectToMove)
        {
            List<Direction> sortRemainingDirections = new List<Direction>();

            // last is the opposite to the forbiden direction
            Direction oppToForbiden;
            if (forbidenDir == Direction.Down)
            {
                oppToForbiden = Direction.Up;
            }
            else if (forbidenDir == Direction.Up)
            {
                oppToForbiden = Direction.Down;
            }
            else if (forbidenDir == Direction.Right)
            {
                oppToForbiden = Direction.Left;
            }
            else
            {
                oppToForbiden = Direction.Right;
            }

            remainingDirections.Remove(oppToForbiden);
            Dictionary<Direction, int> dictDists = new Dictionary<Direction, int>();
            foreach (Direction currDir in remainingDirections)
            {
                int dirDist;
                if (currDir == Direction.Up)
                {
                    dirDist = rectToMove.Bottom - rectToClean.Y;
                }
                else if (currDir == Direction.Down)
                {
                    dirDist = rectToClean.Bottom - rectToMove.Y;
                }
                else if (currDir == Direction.Right)
                {
                    dirDist = rectToMove.Right - rectToMove.X;
                }
                else
                {
                    dirDist = rectToMove.Right - rectToClean.X;
                }
                dictDists.Add(currDir, dirDist);
            }

            dictDists.OrderBy(i => i.Value);

            return sortRemainingDirections;
        }

        /// <summary>
        /// choose an optimal operation which satisfied the given predicate
        /// </summary>
        /// <param name="board"></param>
        /// <param name="predicateToSatisfy"></param>
        /// <returns></returns>
        public Operation ChooseOperation(Board board, Predicate predicateToSatisfy)
        {
            Dictionary<Operation, List<Furniture>> blockingfurPerOperation = new Dictionary<Operation, List<Furniture>>();
            Furniture furniture;
            Rectangle furDest;
            Rectangle furCurrPos;
            int currRoom;
            int endRoom;
            List<Direction> directions;
            List<Direction> directionsSorted = null;
            List<Direction> directionsToDoor = null;
            List<Direction> directionToDoorSorted = null;

            if (predicateToSatisfy is PClean)
            {
                IList<Furniture> interruptingFurniture = this.InterruptingFurniture(board, predicateToSatisfy);
                furniture = interruptingFurniture.First();
                furDest = Board.Instance.furnitureDestination[furniture];
                furCurrPos = furniture.Description;
                currRoom = board.FindRoomPerRect(furniture.Description);
                endRoom = board.FindRoomPerRect(furDest);   
                Direction forbbiden = new Direction();
                Dictionary<Operation, List<Furniture>> blocking = new Dictionary<Operation, List<Furniture>>();
                if (currRoom == endRoom)
                {
                    directions = FindPossibleDirections(furniture, board);
                    directionsSorted = SortDirectionsByDistance(furniture, directions, board);                  
                    forbbiden =((predicateToSatisfy as PClean).Forbbiden);
                    directionsSorted = directionsSorted.Except(new List<Direction>{forbbiden}).ToList(); 
                }
                else
                {
                    directionsToDoor = FindPossibleDirectionsToDoor(furniture, currRoom, endRoom);
                    directionToDoorSorted = SortDirectionsByDistance(furniture, directionsToDoor, board);                    
                    forbbiden = ((predicateToSatisfy as PClean).Forbbiden);
                    directionToDoorSorted = directionToDoorSorted.Except(new List<Direction> { forbbiden }).ToList();
                }
                if ((directionsSorted==null && directionToDoorSorted.Count==0) || (directionToDoorSorted==null && directionsSorted.Count==0))
                {
                    //create a list directions except for the forbidden
                    //sort list according to: first ortogonal to the forbbiden and internali the direction with min steps in clearing the rect
                    //try moving 
                    var remainingDirections = FindRemainingDirections(forbbiden);

                    var remainingDirectionsSorted = SortRemainingDirections(forbbiden, remainingDirections, (predicateToSatisfy as PClean).CleanRect, furniture.Description);

                    var operation = CheckIfCanMove(remainingDirectionsSorted, furniture,blocking);
                    if (operation == null)
                    {
                        return ReturnOptimalOperation(blocking);
                    }
                }

            }
            //predicate is a location kind
            else
            {                          
                furniture = (predicateToSatisfy as PLocation).furniture;
                furDest = (predicateToSatisfy as PLocation).rect;
                furCurrPos = furniture.Description;
                currRoom = board.FindRoomPerRect(furniture.Description);
                endRoom = board.FindRoomPerRect(furDest);
                 if (currRoom == endRoom)
                {
                    directions = FindPossibleDirections(furniture, board);
                    directionsSorted = SortDirectionsByDistance(furniture, directions, board);                   
                }
                else
                {
                    directionsToDoor = FindPossibleDirectionsToDoor(furniture, currRoom, endRoom);
                    directionToDoorSorted = SortDirectionsByDistance(furniture, directionsToDoor, board);         
                }
            }                           
            // in case sart & end positions are in the same room
            if (currRoom == endRoom)
            {               
                //check if both positions are in the same orientaion.
                if (IsSameOrientaion(furCurrPos, furDest))
                {   
                    //check if can move             
                    var operation = CheckIfCanMove(directionsSorted, furniture, blockingfurPerOperation);
                    if (operation != null)
                        return operation;
                    //can't move                   
                        return ReturnOptimalOperation(blockingfurPerOperation);                                           
                }
                //if not the same orientation- check if can rotate
                {
                    Rotate rotate= new Rotate(furniture);
                    var operation = CheckIfCanRotate(directionsSorted, rotate, blockingfurPerOperation);
                    if (operation != null)
                        return operation;
                    //cant rotate
                    operation = CheckIfCanMove(directionsSorted, furniture, blockingfurPerOperation);
                    if (operation != null)
                        return operation;
                    //cant move or rotate
                    return ReturnOptimalOperation(blockingfurPerOperation);  

                }               
            }
            //else: star& end positions are not in the same room                     
            //if furniture is passing rooms:
            if (directionsToDoor.Count() == 0)
            {
                directionsToDoor = FindPossibleDirections(furniture, board);
                directionToDoorSorted = SortDirectionsByDistance(furniture, directionsToDoor, board);
            }           
            if (CanPassDoor(furniture, currRoom, endRoom))
            {
                var operation = CheckIfCanMove(directionToDoorSorted, furniture, blockingfurPerOperation);
                if (operation != null)
                    return operation;
                //cant move
                return ReturnOptimalOperation(blockingfurPerOperation);           
            }
            //cant pass door
            if (!CanPassDoor(furniture, currRoom, endRoom))
            {
                //check if can rotate
                Rotate rotate = new Rotate(furniture);
                var operation = CheckIfCanRotate(directionToDoorSorted, rotate, blockingfurPerOperation);
                if (operation != null)
                    return operation;
                //cant rotate
                 operation = CheckIfCanMove(directionToDoorSorted, furniture, blockingfurPerOperation);
                if (operation != null)
                    return operation;
                //cant move or rotate
                return ReturnOptimalOperation(blockingfurPerOperation);      
            }
            return null;
        }

        private List<Direction> FindRemainingDirections(Direction forbbiden)
        {
            List<Direction> result= new List<Direction>();
            result.Add(Direction.Down);
            result.Add(Direction.Up);
            result.Add(Direction.Left);
            result.Add(Direction.Right);
            return result.Except(new List<Direction> {forbbiden}).ToList();
        }

        private Operation ReturnOptimalOperation(Dictionary<Operation, List<Furniture>> blockingfurPerOperation)
        {
            var operationSortedByMinNumOfBlockingFur = blockingfurPerOperation.OrderBy(i => i.Value.Count());
            blockingfurPerOperation = operationSortedByMinNumOfBlockingFur.ToDictionary(i => i.Key, i => i.Value);
            return blockingfurPerOperation.Keys.First();
        }

        /// <summary>
        /// returns a sorted list of directions
        /// it calculates the euclidean ditstance after moving in each direction and sorts ascending order
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="directionsToDoor"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        private List<Direction> SortDirectionsByDistance(Furniture furniture, List<Direction> directions, Board board)
        {
            if (directions.Count()==1)
            {
                return directions;
            }

            var distPerDir = new Dictionary<double, Direction>();
           // var sortedDir = new List<Direction>();          

            foreach (var direction in directions)
            {
                Move move = new Move(furniture);
                move.Direction = direction;
                move.HowManyStepsInDirection = 1;
                var diffRect = move.CalculateRectDiff();
                if (!board.InBounds(diffRect) || !board.IsEmpty(diffRect))
                    continue;
                var newRect = move.CalculateNewdestRectangle();
                var temp = CalculatePathByRect(newRect, board.furnitureDestination[furniture]);
                var distance = temp.Count;
                //var distance = board.RectanglesEuclideanDistance(newRect, board.furnitureDestination[furniture]);
                distPerDir[distance] = direction;
            }
            var distPerDirSorted=distPerDir.OrderBy(i => i.Key).ToDictionary(i=>i.Key,i=>i.Value);

            return new List<Direction>(distPerDirSorted.Values);

           
        }

        private List<RotationDirection> SortRotateDirectionsByDistance(Furniture furniture, List<RotationDirection> directions, Board board)
        {
            if (directions.Count() == 1)
            {
                return directions;
            }
            var sortedDir = new List<RotationDirection>();

            Rotate rotate= new Rotate(furniture); 
            rotate.RotationDirection = directions.First();
            var newRect1 = rotate.NewDestRect();
            var temp1 = CalculatePathByRect(newRect1, board.furnitureDestination[furniture]);
            var distance1 = temp1.Count;

            rotate.RotationDirection = directions.Last();
            var newRect2 = rotate.NewDestRect();
            var temp2 = CalculatePathByRect(newRect2, board.furnitureDestination[furniture]);
            var distance2 = temp2.Count;           

            if (distance1 < distance2)
            {
                sortedDir.Add(directions.First());
                sortedDir.Add(directions.Last());
                return sortedDir;
            }
            sortedDir.Add(directions.Last());
            sortedDir.Add(directions.First());
            return sortedDir;
            
        }
        private Operation CheckIfCanMove(List<Direction> directions, Furniture furniture, Dictionary<Operation, List<Furniture>>blockingfurPerOperation)
        {
            Move move = new Move(furniture);
            foreach (var direction in directions)
            {
                move.Direction = direction;
                move.HowManyStepsInDirection = 1;
                Rectangle diffRect = move.CalculateRectDiff();
                if (Board.Instance.InBounds(diffRect))
                {
                    if (Board.Instance.IsNotWall(diffRect))
                    {
                        if(Board.Instance.IsEmpty(diffRect))
                            return (Operation)move;
                        else //is blocked by other fur'
                        {
                            var problematicFur = Board.Instance.FindFurnitureInRect(diffRect);
                            blockingfurPerOperation.Add((Operation)move, problematicFur);
                        }
                    }
                }                       
            }
            return null;
        }

        private Operation CheckIfCanRotate(List<Direction> directions, Rotate rotate, Dictionary<Operation, List<Furniture>> blockingfurPerOperation )
        {
            //TODO: need to decide in what direction to rotate
            List<RotationDirection> RDL =new List<RotationDirection>();
            RDL.Add(RotationDirection.ClockWise);
            RDL.Add(RotationDirection.CounterClockWise);            
            var sortedRD = SortRotateDirectionsByDistance(rotate.Furniture, RDL, Board.Instance);
            foreach (var SRD in sortedRD)
            {             
                rotate.RotationDirection = SRD;
                var diffRect = rotate.CalculateRectToBeCleanByDirection();
                if (Board.Instance.InBounds(diffRect))
                {
                    if (Board.Instance.IsNotWall(diffRect))
                    {
                        if (Board.Instance.IsEmpty(diffRect))
                        {
                            return (Operation) rotate;
                        }
                         var problematicFur = Board.Instance.FindFurnitureInRect(diffRect);
                         blockingfurPerOperation.Add((Operation)rotate, problematicFur);
                    }
                }
            }                               
            return null;
        }

        private bool CanPassDoor(Furniture furniture, int currRoom, int endRoom)
        {
            if (currRoom == 2 || endRoom == 2)
            {
                int doorHieght = 2;
                if (furniture.Description.Height <= doorHieght)
                    return true;
                return false;
            }
            if (currRoom == 3 || endRoom == 3)
            {
                int doorHieght = 4;
                if (furniture.Description.Height <= doorHieght)
                    return true;
                return false;
            }
            return false;
        }

        private List<Direction> FindPossibleDirectionsToDoor(Furniture furniture,int currRoom, int endRoom)
        {
            var result = new List<Direction>();
            Rectangle rect = furniture.Description;
            int rectXL= rect.X;
            int rectYH = rect.Y;
            int rectXH = rect.X + rect.Width-1;
            int rectYL = rect.Y + rect.Height-1;

            // 1->2 or 1->3
            if (currRoom == 1)
            {
                if (endRoom == 2)
                {
                    // the furniture is below the door
                    if (rectYL >= 4)
                    {
                        // the furniture is on the lower door
                        if (rectXH >= 11)
                        {
                            result.Add(Direction.Left);
                            return result;
                        }

                        result.Add(Direction.Right);
                        result.Add(Direction.Up);
                        return result;
                    }
                    // the furniture is above the door
                    else if (rectYH <= 1)
                    {
                        result.Add(Direction.Right);
                        result.Add(Direction.Down);
                        return result;
                    }
                    // the furnite is right in front of the door
                    else
                    {
                        result.Add(Direction.Right);
                        return result;
                    }

                }
                // end room = 3
                else
                {
                    // the furniture is above the door
                    if (rectYH <= 6)
                    {
                        // the furniture is on the upper door
                        if (rectXH >= 11)
                        {
                            result.Add(Direction.Left);
                            return result;
                        }

                        result.Add(Direction.Right);
                        result.Add(Direction.Down);
                        return result;
                    }
                    // the furniture is below the door
                    else if (rectYL >= 11)
                    {
                        result.Add(Direction.Right);
                        result.Add(Direction.Up);
                        return result;
                    }
                    // the furnite is right in front of the door
                    else
                    {
                        result.Add(Direction.Right);
                        return result;
                    }
                }
            }
                // 2->1 or 2->3
            else if ((currRoom == 2) && ((endRoom == 1) || (endRoom == 3)))
            {
                // the furniture is above the door
                if (rectYH <= 1)
                {
                    result.Add(Direction.Down);
                    result.Add(Direction.Left);
                    return result;
                }
                // the furniture is below the door
                else if (rectYL >= 4)
                {
                    result.Add(Direction.Up);
                    result.Add(Direction.Left);
                    return result;
                }
                // the furnite is right in front of the door
                else
                {
                    result.Add(Direction.Left);
                    return result;
                }
            }
            // 3->1 or 3->2
            else
            {
                // the furniture is above the door
                if (rectYH <= 6)
                {
                    result.Add(Direction.Down);
                    result.Add(Direction.Left);
                    return result;
                }
                // the furniture is below the door
                else if (rectYL >= 11)
                {
                    result.Add(Direction.Up);
                    result.Add(Direction.Left);
                    return result;
                }
                // the furnite is right in front of the door
                else
                {
                    result.Add(Direction.Left);
                    return result;
                }
            }
            //if (currRoom == 2 || endRoom==2)
            //{
            //    int doorX = 11;
            //    int doorYL = 3;
            //    int doorYH = 2;
            //    if ((doorYL - rectYL) > 0 )
            //    {
            //        result.Add(Direction.Down);
            //    }
            //    if ((doorYH -rectYH) < 0)
            //    {
            //        result.Add(Direction.Up);
            //    }
            //    if ((rectXL-doorX) >0)
            //    {
            //        result.Add(Direction.Left);
            //    }
            //    if ((doorX - rectXH) > 0)
            //    {
            //        result.Add(Direction.Right);
            //    }
            //}
            //if (currRoom== 3|| endRoom==3)
            //{
            //    int doorX = 11;
            //    int doorYL = 10;
            //    int doorYH = 7;

            //    if ((doorYH - rectYL) >= 0)
            //    {
            //        result.Add(Direction.Down);
            //    }
            //    if ((rectYH - doorYL) >= 0)
            //    {
            //        result.Add(Direction.Up);
            //    }
            //    if ((rectXL-doorX) >=0)
            //    {
            //        result.Add(Direction.Left);
            //    }
            //    if ((doorX - rectXH) >= 0)
            //    {
            //        result.Add(Direction.Right);
            //    }
            //}
            return result;
        }

        private bool IsSameOrientaion(Rectangle furCurrPos, Rectangle furDest)
        {
            if (furCurrPos.Height == furDest.Height && furCurrPos.Width == furDest.Width)
                return true;
            return false;
        }

        /// <summary>
        /// calculates a map that count for each valid move the # of interrupts it creates
        /// </summary>
        /// <param name="board"></param>
        /// <param name="problematicFur"></param>
        /// <returns>return the move with the minimum interrupts</returns>
        /// if there is no move that doesn't create interrupts we can implement a sort that between all operation preffer the move in dir.
        private Operation FindOptimalOperation(Board board, Furniture furniture, Predicate predicateToSatisfy)
        {
            var opWhichSatisfiesPredMinInter =new Dictionary<int, List<Operation>>();
            var optimalOperation = new Dictionary<int, List<Operation>>();

            var listDirections = SortDirectionsByDistance(furniture,
                                                          new List<Direction>
                                                              {
                                                                  Direction.Up,
                                                                  Direction.Down,
                                                                  Direction.Left,
                                                                  Direction.Right
                                                              }, board);
            var listRDirections = SortRotateDirectionsByDistance(furniture,
                                                                 new List<RotationDirection>
                                                                     {
                                                                         RotationDirection.ClockWise,
                                                                         RotationDirection.CounterClockWise
                                                                     }, board);
            //rotating           
            foreach (var rotationDirection in listRDirections)
            {
                FindOptimalRotaion(new Rotate(furniture), rotationDirection, optimalOperation, board, opWhichSatisfiesPredMinInter, predicateToSatisfy);
            }          
            //moving 
            foreach (var dir in listDirections)
            {
                FindOptimalMove(new Move(furniture), dir, optimalOperation, board, opWhichSatisfiesPredMinInter, predicateToSatisfy);
            }                   
            if (opWhichSatisfiesPredMinInter.Count > 0)
            {
                var opWhichSatisfiesPredMinInterSorted = opWhichSatisfiesPredMinInter.OrderBy(i => i.Key);
                return opWhichSatisfiesPredMinInterSorted.First().Value.First();
            }
            var optimalOperationSorted = optimalOperation.OrderBy(i => i.Key);
            return optimalOperationSorted.First().Value.First();
        }

        private void FindOptimalMove(Move move, Direction direction, Dictionary<int, List<Operation>> optimalOperation, Board board, Dictionary<int, List<Operation>> opWhichSatisfiesPredMinInter, Predicate predicateToSatisfy)
        {
            var tempList = new List<Operation>();            
            move.Direction = direction;
            move.HowManyStepsInDirection = 1;
            Rectangle rectToBeClean = move.CalculateRectDiff();
            if (!board.InBounds(move.CalculateNewdestRectangle()) || !board.IsNotWall(rectToBeClean)) return;
            var numberOfFurInRect = board.FindFurnitureInRect(rectToBeClean).Count;            
            var newDestRect = move.CalculateNewdestRectangle();
            Operation op = (Operation)move;
            if (PredIsSatisfied(newDestRect, predicateToSatisfy))
            {
                var tempList1= new List<Operation>();
                if (opWhichSatisfiesPredMinInter.ContainsKey(numberOfFurInRect))
                {
                    opWhichSatisfiesPredMinInter[numberOfFurInRect].Add(op);
                    return;
                }
                tempList1.Add(op);
                opWhichSatisfiesPredMinInter[numberOfFurInRect] = tempList1;                              
                return;
            }
            //else                                   
            if (optimalOperation.ContainsKey(numberOfFurInRect))
            {
                optimalOperation[numberOfFurInRect].Add(op);
                return;
            }
            tempList.Add(op);
            optimalOperation[numberOfFurInRect] = tempList;
        }

        private bool PredIsSatisfied(Rectangle newDestRect, Predicate predicateToSatisfy)
        {
            if (predicateToSatisfy is PLocation)
            {
                var rect = (predicateToSatisfy as PLocation).rect;
                return newDestRect == rect ? true: false;
            }
            var pClean = predicateToSatisfy as PClean;
            if (pClean != null)
            {
                var rect1 = pClean.CleanRect;
                return newDestRect == rect1 ? true: false;
            }
            return false;
        }

        private void FindOptimalRotaion(Rotate rotate, RotationDirection RD, Dictionary<int, List<Operation>> optimalOperation, Board board, Dictionary<int, List<Operation>> opWhichSatisfiesPredMinInter, Predicate predicateToSatisfy)
        {
            var tempList = new List<Operation>();
            rotate.RotationDirection = RD;
            Rectangle rectToBeClean = rotate.CalculateRectToBeCleanByDirection();
            if (!board.InBounds(rectToBeClean) || !board.IsNotWall(rectToBeClean)) return;
            var numberOfFurInRect = board.FindFurnitureInRect(rectToBeClean).Count;         
            Operation op = (Operation)rotate;
            var newDestRect = rotate.NewDestRect();
            if (PredIsSatisfied(newDestRect, predicateToSatisfy))
            {
                var tempList1 = new List<Operation>();
                if (opWhichSatisfiesPredMinInter.ContainsKey(numberOfFurInRect))
                {
                    opWhichSatisfiesPredMinInter[numberOfFurInRect].Add(op);
                    return;
                }
                tempList1.Add(op);
                opWhichSatisfiesPredMinInter[numberOfFurInRect] = tempList1;
                return;
            }
            //else                                   
            if (optimalOperation.ContainsKey(numberOfFurInRect))
            {
                optimalOperation[numberOfFurInRect].Add(op);
                return;
            }
            tempList.Add(op);
            optimalOperation[numberOfFurInRect] = tempList;                                             
        }

        private bool CanMoveInDir(Furniture furniture, Direction direction, Board board)
        {
            Move move = new Move(furniture);
            move.Direction = direction;
            move.HowManyStepsInDirection = 1;
            Rectangle diffRect = move.CalculateRectDiff();
            if (board.InBounds(diffRect))
            {                
                if (board.IsEmpty(diffRect))
                    {   
                        return true;
                    }                
            }
            return false;
        }
      
        private List<Direction> FindPossibleDirections(Furniture furniture, Board board)
        {
            var directions = new List<Direction>();
            Rectangle dest = board.furnitureDestination[furniture];
            Rectangle currPos = furniture.Description;
            int destX = dest.X;
            int destY = dest.Y;
            int currPosX = currPos.X;
            int currPosY = currPos.Y;

            if((destY-currPosY) > 0)
                directions.Add(Direction.Down);
            if ((destY-currPosY) < 0)
                directions.Add(Direction.Up);
            if((destX-currPosX)> 0)
                directions.Add(Direction.Right);
            if((destX-currPosX)< 0)
                directions.Add(Direction.Left);

            return directions;
        }

        public List<Furniture> InterruptingFurniture(Board board, Predicate predicate)
        {
            var result = new List<Furniture>();
            Rectangle problematicRect = (predicate as PClean).CleanRect;
            foreach (var furniture in board.furnitureDestination.Keys)
            {
                if (problematicRect.IntersectsWith(furniture.Description))
                {
                    result.Add(furniture);
                }
            }
            return result;
        }

        public static  List<Rectangle> CalculatePathByRect(Rectangle startRect, Rectangle endRect)
        {
            List<Rectangle> path = new List<Rectangle>();
           var startRoom= Board.Instance.FindRoomPerRect(startRect);
           var endRoom = Board.Instance.FindRoomPerRect(endRect);
           //moving from room 2 to room 3 or vice a versa
            if (startRoom != 1 && endRoom!=1)
           {                             
                   var doorRect=Group.GetRoomDoor(startRoom);
                   path.AddRange(Group.FindPathBetweenPoints(startRect, doorRect));   
                   
                //move rect width time left
               for (int i = 1; i <= startRect.Width; i++)
               {
                   path.Add(new Rectangle(doorRect.X - i,doorRect.Y,1,1));
               }

               Rectangle currLoc = path.Last();
               var secDoorRect = Group.GetRoomDoor(startRoom);
               path.AddRange(Group.FindPathBetweenPoints(currLoc, secDoorRect));
               path.AddRange(Group.FindPathBetweenPoints(secDoorRect,endRect));    
           }
            else
            {
                if (!(((endRoom == 2) && (startRoom == 1) && (startRect.Y >= 2) && (startRect.Y <= 3)) ||
                      ((endRoom == 3) && (startRoom == 1) && (startRect.Y >= 7) && (startRect.Y <= 10))))
                {
                    Rectangle currLoc = new Rectangle(startRect.X,startRect.Y,1,1);
                    while (currLoc.X + startRect.Width >= 11)
                    {
                        path.Add(currLoc);
                        currLoc = new Rectangle(currLoc.X - 1, currLoc.Y,1,1);
                    }
                    startRect = new Rectangle(currLoc.X + 1, currLoc.Y, 1, 1);
                }

                path.AddRange(Group.FindPathBetweenPoints(startRect, endRect)); 

            }
            return path;
        }

    }
}
