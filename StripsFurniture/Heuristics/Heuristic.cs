﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                result = (IList<StackItem>) result.Concat(pClean).ToList();
                result = (IList<StackItem>) result.Concat(orderedPLocation).ToList();
                return result;
            }
            else
            {
                //for now we settle for taking all the pClean's as they are ordered, maybe we can add heurisitc that orders them as well.
                result = (IList<StackItem>) result.Concat(pClean).ToList();
                result = (IList<StackItem>) result.Concat(pLocation).ToList();
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

        /// <summary>
        /// Order location predicates
        /// </summary>
        /// <param name="pLocations"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Grouptype - PassDoor and SameRoom
        /// </summary>
        private enum GroupType
        {
            PassDoor,
            SameRoom
        }

        private class Group
        {
            private GroupType groupType;
            private List<PLocation> furnitures;
            private List<Rectangle> path;

            public Group(GroupType groupType)
            {
                this.groupType = groupType;
                furnitures = new List<PLocation>();
            }

            public GroupType GroupType
            {
                get { return groupType; }
            }

            public List<PLocation> Furnitures
            {
                get { return furnitures; }
            }

            // relevant only when groupType is PassDoor type
            public bool PassUpperDoor { get; set; }

            public List<int> DoorsPath { get; set; }

            /// <summary>
            /// Calculates representative path
            /// </summary>
            public void CalcRepresentativePath()
            {
                // take the representative as the last in the sort
                Rectangle startState = this.Furnitures[this.Furnitures.Count - 1].furniture.Description;
                Rectangle destState = this.Furnitures[this.Furnitures.Count - 1].rect;

                //path = Group.CalcRepresentativePath(startState, destState, this.DoorsPath);
                path = Heuristic.CalculatePathByRect(startState, destState);
            }

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

            /// <summary>
            /// find path between points
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public static List<Rectangle> FindPathBetweenPoints(Rectangle start, Rectangle end, int width, int height)
            {
                List<Rectangle> subPath = new List<Rectangle>();

                int deltaY = Math.Sign(end.Y - start.Y);
                int deltaX = Math.Sign(end.X - start.X);

                for (int i = start.Y; i != end.Y; i += deltaY)
                {
                    subPath.Add(new Rectangle(start.X, i, width, height));
                }

                for (int i = start.X + deltaX; i != end.X; i += deltaX)
                {
                    subPath.Add(new Rectangle(i, end.Y, width, height));
                }
                subPath.Add(new Rectangle(end.X, end.Y, width, height));

                return subPath;
            }

            /// <summary>
            /// return door location per room
            /// </summary>
            /// <param name="roomId"></param>
            /// <returns></returns>
            public static Rectangle GetRoomDoor(int roomId)
            {
                if (roomId == 2)
                {
                    return new Rectangle(11, 3, 1, 1);
                }
                    // roomId = 3
                else
                {
                    return new Rectangle(11, 8, 1, 1);
                }
            }

            public List<Rectangle> Path
            {
                get { return path; }
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

        private class GroupsComparer : IComparer<Group>
        {
            public int Compare(Group g1, Group g2)
            {
                List<Rectangle> otherGroupFurnituresInStart = g2.GetFurnituresInStart();
                List<Rectangle> otherGroupFurnituresInDest = g2.GetFurnituresInDest();

                List<Rectangle> testedGroupFurnituresInStart = g1.GetFurnituresInStart();
                List<Rectangle> testedGroupFurnituresInDest = g1.GetFurnituresInDest();
                int res = GroupsComparer.InternalCompare(g1.Path, g2.Path,
                                                         testedGroupFurnituresInStart, testedGroupFurnituresInDest,
                                                         otherGroupFurnituresInStart, otherGroupFurnituresInDest);

                return res;
            }

            private int InternalCompareBasedOnRooms(Group tested, Group other)
            {
                if ((tested.DoorsPath.Count > 0) &&
                    (other.DoorsPath.Count > 0))
                {
                    Rectangle testedStartPos = tested.Path.First();
                    Rectangle testedEndPos = tested.Path.Last();

                    Rectangle otherStartPos = other.Path.First();
                    Rectangle otherEndPos = other.Path.Last();

                    int testedStartRoom = Board.Instance.FindRoomPerRect(testedStartPos);
                    int testedEndRoom = Board.Instance.FindRoomPerRect(testedEndPos);
                    int otherStartRoom = Board.Instance.FindRoomPerRect(otherStartPos);
                    int otherEndRoom = Board.Instance.FindRoomPerRect(otherEndPos);

                    if ((testedStartRoom == otherEndRoom) && ((testedStartRoom == 2) || (testedStartRoom == 3)))
                    {
                        return 1;
                    }
                }

                return 0;
            }

            public static int InternalCompare(List<Rectangle> testedPath, List<Rectangle> otherPath,
                                              List<Rectangle> testedGroupFurnituresInStart,
                                              List<Rectangle> testedGroupFurnituresInDest,
                                              List<Rectangle> otherGroupFurnituresInStart,
                                              List<Rectangle> otherGroupFurnituresInDest)
            {
                bool otherStartOnTestedPath = false;
                bool otherDestOnTestedPath = false;
                bool testedStartOnOtherPath = false;
                bool testedDestOnOtherPath = false;

                // if a furniture from g2 in start state is on the path of g1 than g1 is smaller than g2
                if (IsOnPath(testedPath, otherGroupFurnituresInStart))
                {
                    otherStartOnTestedPath = true;
                    //return 1;
                }

                // else if a furniture from g2 in dest state is on the path of g1 than g1 is bigger than g2
                if (IsOnPath(testedPath, otherGroupFurnituresInDest))
                {
                    otherDestOnTestedPath = true;
                    //return -1;
                }

                // if a furniture from g1 in start state is on the path of g2 than g2 is smaller than g1
                if (IsOnPath(otherPath, testedGroupFurnituresInStart))
                {
                    testedStartOnOtherPath = true;
                    //return -1;
                }

                // else if a furniture from g1 in dest state is on the path of g2 than g2 is bigger than g1
                if (IsOnPath(otherPath, testedGroupFurnituresInDest))
                {
                    testedDestOnOtherPath = true;
                    //return 1;
                }

                if ((otherStartOnTestedPath || testedDestOnOtherPath) &&
                    (!testedStartOnOtherPath) && (!otherDestOnTestedPath))
                {
                    return 1;
                }
                else if ((testedStartOnOtherPath || otherDestOnTestedPath) &&
                         (!otherStartOnTestedPath) && (!testedDestOnOtherPath))
                {
                    return -1;
                }

                // select the group that is in the small room and the destination of the other group is in the same room of the group
                Rectangle testedStart = testedPath.First();
                Rectangle testedDest = testedPath.Last();
                Rectangle otherStart = otherPath.First();
                Rectangle otherDest = otherPath.Last();
                if (IsDestAndSourceInSameSmallRoom(testedDest, otherStart))
                {
                    return 1;
                }
                else if (IsDestAndSourceInSameSmallRoom(otherDest, testedStart))
                {
                    return -1;
                }

                return 0;
            }

            private static bool IsDestAndSourceInSameSmallRoom(Rectangle testedDest, Rectangle otherStart)
            {
                int testedDestRoom = Board.Instance.FindRoomPerRect(testedDest);
                int otherStartRoom = Board.Instance.FindRoomPerRect(otherStart);

                if ((testedDestRoom == otherStartRoom) &&
                    (testedDestRoom == 2 || testedDestRoom == 3))
                {
                    return true;
                }

                return false;
            }

            private static bool IsOnPath(List<Rectangle> path, List<Rectangle> furnitures)
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
            List<Group> passDoorGroups = this.FindPassDoorGroups(room1StartStateFurnitures, room1DestStateFurnitures,
                                                                 room2StartStateFurnitures, room2DestStateFurnitures,
                                                                 room3StartStateFurnitures, room3DestStateFurnitures);
            List<Group> groups = passDoorGroups;
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
                                    out List<Furniture> room1StartStateFurnitures,
                                    out List<Furniture> room1DestStateFurnitures,
                                    out List<Furniture> room2StartStateFurnitures,
                                    out List<Furniture> room2DestStateFurnitures,
                                    out List<Furniture> room3StartStateFurnitures,
                                    out List<Furniture> room3DestStateFurnitures)
        {
            room1StartStateFurnitures = new List<Furniture>();
            room1DestStateFurnitures = new List<Furniture>();
            room2StartStateFurnitures = new List<Furniture>();
            room2DestStateFurnitures = new List<Furniture>();
            room3StartStateFurnitures = new List<Furniture>();
            room3DestStateFurnitures = new List<Furniture>();

            foreach (StackItem currLocation in locations)
            {
                Rectangle destRect = ((PLocation) currLocation).rect;
                Rectangle startRect = ((PLocation) currLocation).furniture.Description;

                this.UpdateRoomsStart(((PLocation) currLocation).furniture,
                                      ref room1StartStateFurnitures,
                                      ref room2StartStateFurnitures,
                                      ref room3StartStateFurnitures);

                this.UpdateRoomsDest(destRect, ((PLocation) currLocation).furniture.ID,
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

        private Group FindSameRoomGroups(List<Furniture> roomStartStateFurnitures,
                                         List<Furniture> roomDestStateFurnitures)
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

        private List<Group> FindPassDoorGroups(List<Furniture> room1StartStateFurnitures,
                                               List<Furniture> room1DestStateFurnitures,
                                               List<Furniture> room2StartStateFurnitures,
                                               List<Furniture> room2DestStateFurnitures,
                                               List<Furniture> room3StartStateFurnitures,
                                               List<Furniture> room3DestStateFurnitures)
        {
            List<Group> groupsTemp = new List<Group>();

            groupsTemp.Add(this.FindPassDoorGroups(room1DestStateFurnitures, room2StartStateFurnitures, true,
                                                   new List<int> {2}));
            groupsTemp.Add(this.FindPassDoorGroups(room1DestStateFurnitures, room3StartStateFurnitures, false,
                                                   new List<int> {3}));
            groupsTemp.Add(this.FindPassDoorGroups(room2DestStateFurnitures, room1StartStateFurnitures, true,
                                                   new List<int> {2}));
            groupsTemp.Add(this.FindPassDoorGroups(room2DestStateFurnitures, room3StartStateFurnitures, false,
                                                   new List<int> {2, 3}));
            groupsTemp.Add(this.FindPassDoorGroups(room3DestStateFurnitures, room1StartStateFurnitures, false,
                                                   new List<int> {3}));
            groupsTemp.Add(this.FindPassDoorGroups(room3DestStateFurnitures, room2StartStateFurnitures, true,
                                                   new List<int> {3, 2}));

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

        private Group FindPassDoorGroups(List<Furniture> destStateFurnitures, List<Furniture> startStateFurnitures,
                                         bool upperDoor, List<int> doorsPath)
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

        private class FurniturePassDoorComparer : IComparer<PLocation>
        {
            private Point doorLocation;

            public FurniturePassDoorComparer(bool upperDoor)
            {
                if (upperDoor)
                {
                    doorLocation = new Point(11, 3);
                }
                else
                {
                    doorLocation = new Point(11, 8);
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

        private class FurnitureInSameRoomComparer : IComparer<PLocation>
        {
            public int Compare(PLocation ploc1, PLocation ploc2)
            {
                List<Rectangle> ploc1Path = Heuristic.CalculatePathByRect(ploc1.furniture.Description, ploc1.rect);
                List<Rectangle> ploc2Path = Heuristic.CalculatePathByRect(ploc2.furniture.Description, ploc2.rect);

                return GroupsComparer.InternalCompare(ploc1Path, ploc2Path,
                                                      new List<Rectangle> {ploc1.furniture.Description},
                                                      new List<Rectangle> {ploc1.rect},
                                                      new List<Rectangle> {ploc2.furniture.Description},
                                                      new List<Rectangle> {ploc2.rect});
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

        private List<Direction> SortRemainingDirections(List<Direction> forbidenDir, List<Direction> remainingDirections,
                                                        Rectangle rectToClean, Rectangle rectToMove, Furniture furniture,
                                                        Direction originalForbbiden)
        {
            List<Direction> sortRemainingDirections = new List<Direction>();

            // last is the opposite to the forbiden direction
            List<Direction> oppToForbiden = new List<Direction>();
            if (forbidenDir.Contains(Direction.Down))
            {
                oppToForbiden.Add(Direction.Up);
            }
            if (forbidenDir.Contains(Direction.Up))
            {
                oppToForbiden.Add(Direction.Down);
            }
            if (forbidenDir.Contains(Direction.Right))
            {
                oppToForbiden.Add(Direction.Left);
            }
            if (forbidenDir.Contains(Direction.Left))
            {
                oppToForbiden.Add(Direction.Right);
            }

            var remainingDirectionsSorted = remainingDirections.Except(oppToForbiden).ToList();
            Dictionary<Direction, int> dictDists = new Dictionary<Direction, int>();
            if (remainingDirectionsSorted.Count == 0)
                remainingDirectionsSorted = oppToForbiden;
            foreach (Direction currDir in remainingDirectionsSorted)
            {
                int dirDist;
                if (currDir == originalForbbiden)
                    continue;
                if (currDir == Direction.Up)
                {
                    dirDist = rectToMove.Bottom - rectToClean.Y;
                    Move move = new Move(furniture);
                    move.Direction = currDir;
                    move.HowManyStepsInDirection = dirDist;
                    var dest = move.CalculateRectDiff();
                    if (!Board.Instance.InBounds(dest))
                        continue;
                    if (!Board.Instance.IsEmpty(dest))
                        dirDist = int.MaxValue - 1;
                    if (currDir == originalForbbiden)
                        dirDist = int.MaxValue;
                }
                else if (currDir == Direction.Down)
                {
                    dirDist = rectToClean.Bottom - rectToMove.Y;
                    Move move = new Move(furniture);
                    move.Direction = currDir;
                    move.HowManyStepsInDirection = dirDist;
                    var dest = move.CalculateRectDiff();
                    if (!Board.Instance.InBounds(dest))
                        continue;
                    if (!Board.Instance.IsEmpty(dest))
                        dirDist = int.MaxValue - 1;
                    if (currDir == originalForbbiden)
                        dirDist = int.MaxValue;
                }
                else if (currDir == Direction.Right)
                {
                    dirDist = rectToClean.Right - rectToMove.X;
                    Move move = new Move(furniture);
                    move.Direction = currDir;
                    move.HowManyStepsInDirection = dirDist;
                    var dest = move.CalculateRectDiff();
                    if (!Board.Instance.InBounds(dest))
                        continue;
                    if (!Board.Instance.IsEmpty(dest))
                        dirDist = int.MaxValue - 1;
                    if (currDir == originalForbbiden)
                        dirDist = int.MaxValue;
                }
                else
                {
                    dirDist = rectToMove.Right - rectToClean.X;
                    Move move = new Move(furniture);
                    move.Direction = currDir;
                    move.HowManyStepsInDirection = dirDist;
                    var dest = move.CalculateRectDiff();
                    if (!Board.Instance.InBounds(dest))
                        continue;
                    if (!Board.Instance.IsEmpty(dest))
                        dirDist = int.MaxValue - 1;
                    if (currDir == originalForbbiden)
                        dirDist = int.MaxValue;
                }
                dictDists.Add(currDir, dirDist);
            }

            var dictDistsSorted = dictDists.OrderBy(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
            sortRemainingDirections.AddRange(dictDistsSorted.Keys.ToList());
            sortRemainingDirections.AddRange(oppToForbiden);
            sortRemainingDirections = sortRemainingDirections.Distinct().ToList();


            return sortRemainingDirections;
        }

        private List<Direction> FilterUnVaildDirection(List<Direction> directions, Furniture furniture)
        {
            var result = new List<Direction>(directions);
            foreach (var direction in directions)
            {
                Move move = new Move(furniture);
                move.Direction = direction;
                move.HowManyStepsInDirection = 1;
                var diffRect = move.CalculateRectDiff();
                if (!Board.Instance.InBounds(diffRect) || !Board.Instance.IsNotWall(diffRect))
                    result.Remove(direction);
            }
            return result;
        }

        /// <summary>
        /// choose an optimal operation which satisfied the given predicate
        /// </summary>
        /// <param name="board"></param>
        /// <param name="predicateToSatisfy"></param>
        /// <returns></returns>
        public Operation ChooseOperation(Board board, Predicate predicateToSatisfy)
        {
            Dictionary<Operation, List<Furniture>> blockingfurPerOperation =
                new Dictionary<Operation, List<Furniture>>();
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
                List<Direction> forbbiden = (predicateToSatisfy as PClean).Forbbiden;
                var forbbidenSaved = forbbiden.First();
                forbbiden.AddRange(FindFurbbidenDirections(furniture, (predicateToSatisfy as PClean).CleanRect));
                forbbiden = forbbiden.Distinct().ToList();
                Operation operation;
                Dictionary<Operation, List<Furniture>> blocking = new Dictionary<Operation, List<Furniture>>();
                if (currRoom == endRoom)
                {
                    var RemainigDirections = FindRemainingDirections(new List<Direction> {forbbidenSaved});
                    var SortedRemainingDirections = SortRemainingDirections(new List<Direction> {forbbidenSaved},
                                                                            RemainigDirections,
                                                                            (predicateToSatisfy as PClean).CleanRect,
                                                                            furniture.Description, furniture,
                                                                            forbbidenSaved);

                    directionsSorted = SortedRemainingDirections;
                    directionsSorted = FilterUnVaildDirection(directionsSorted, furniture);
                    directions = FindPossibleDirections(furniture, board);



                }
                else
                {
                    var RemainigDirections = FindRemainingDirections(new List<Direction> {forbbidenSaved});
                    var SortedRemainingDirections = SortRemainingDirections(new List<Direction> {forbbidenSaved},
                                                                            RemainigDirections,
                                                                            (predicateToSatisfy as PClean).CleanRect,
                                                                            furniture.Description, furniture,
                                                                            forbbidenSaved);

                    directionToDoorSorted = SortedRemainingDirections;
                    directionsToDoor = FindPossibleDirectionsToDoor(furniture, currRoom, endRoom);
                }
                if ((directionsSorted == null && directionToDoorSorted.Count == 0) ||
                    (directionToDoorSorted == null && directionsSorted.Count == 0))
                {
                    //create a list directions except for the forbidden
                    //sort list according to: first ortogonal to the forbbiden and internali the direction with min steps in clearing the rect
                    //try moving 
                    var remainingDirections = FindRemainingDirections(forbbiden);
                    if (remainingDirections.Count == 0)
                        remainingDirections = allDirection;

                    var remainingDirectionsSorted = SortRemainingDirections(forbbiden, remainingDirections,
                                                                            (predicateToSatisfy as PClean).CleanRect,
                                                                            furniture.Description, furniture,
                                                                            forbbidenSaved);
                    //filter out un valid directions                   
                    remainingDirectionsSorted = FilterUnVaildDirection(remainingDirectionsSorted, furniture);
                    if (remainingDirectionsSorted.Count != 0)
                    {
                        if (IsOpposite(remainingDirectionsSorted.First(), forbbidenSaved) && currRoom != endRoom &&
                            !CanPassDoor(furniture, currRoom, endRoom))
                        {
                            operation = CheckIfCanRotate(new List<Direction>(), new Rotate(furniture), blocking);
                            if (operation != null)
                                return operation;
                        }
                    }
                    if (remainingDirectionsSorted.Count == 0)
                        remainingDirectionsSorted = allDirection;
                    operation = CheckIfCanMove(remainingDirectionsSorted, furniture, blocking);
                    if (operation != null)
                    {
                        return operation;
                    }
                    else
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
                    Rotate rotate = new Rotate(furniture);
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

        /// <summary>
        /// Check if direction is oppose to
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="forbbiden"></param>
        /// <returns></returns>
        private bool IsOpposite(Direction direction, Direction forbbiden)
        {
            switch (forbbiden)
            {
                case Direction.Up:
                    {
                        if (direction == Direction.Down)
                            return true;
                        break;
                    }
                case Direction.Down:
                    {
                        if (direction == Direction.Up)
                            return true;
                        break;
                    }
                case Direction.Left:
                    {
                        if (direction == Direction.Right)
                            return true;
                        break;
                    }
                case Direction.Right:
                    {
                        if (direction == Direction.Left)
                            return true;
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// Find furbbiden direction to move to
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="cleanRect"></param>
        /// <returns></returns>
        private List<Direction> FindFurbbidenDirections(Furniture furniture, Rectangle cleanRect)
        {
            List<Direction> result = new List<Direction>();
            foreach (Direction dir in this.allDirection)
            {
                var move = new Move(furniture);
                move.Direction = dir;
                move.HowManyStepsInDirection = 1;
                var newDest = move.CalculateNewdestRectangle();
                if (cleanRect.IntersectsWith(newDest))
                    result.Add(dir);
            }
            return result;
        }

        /// <summary>
        /// Find remaining direction to move
        /// </summary>
        /// <param name="forbbiden"></param>
        /// <returns></returns>
        private List<Direction> FindRemainingDirections(List<Direction> forbbiden)
        {
            List<Direction> result = new List<Direction>();
            result.Add(Direction.Down);
            result.Add(Direction.Up);
            result.Add(Direction.Left);
            result.Add(Direction.Right);
            return result.Except(forbbiden).ToList();
        }

        /// <summary>
        /// All move directions
        /// </summary>
        public List<Direction> allDirection = new List<Direction>
                                                  {
                                                      Direction.Up,
                                                      Direction.Down,
                                                      Direction.Left,
                                                      Direction.Right
                                                  };

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
        /// <param name="directions"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        private List<Direction> SortDirectionsByDistance(Furniture furniture, List<Direction> directions, Board board)
        {
            if (directions.Count() == 1)
            {
                return directions;
            }

            var distPerDir = new Dictionary<Direction, double>();
            // var sortedDir = new List<Direction>();          

            foreach (var direction in directions)
            {
                Move move = new Move(furniture);
                move.Direction = direction;
                move.HowManyStepsInDirection = 1;
                var diffRect = move.CalculateRectDiff();

                Rectangle furnitureAfterMove = move.CalculateNewdestRectangle();
                Rectangle furnitureDest = Board.Instance.furnitureDestination[furniture];
                int destRoom = Board.Instance.FindRoomPerRect(furnitureDest);
                bool isOnDoor = (Board.Instance.FindRoomPerRect(furnitureAfterMove) == 1) &&
                                ((destRoom == 1) || (destRoom == 3)) &&
                                (furnitureAfterMove.X + furnitureAfterMove.Width - 1 >= 11 &&
                                 direction == Direction.Right);

                if (!board.InBounds(diffRect) || !board.IsNotWall(diffRect) || isOnDoor)
                {
                    continue;
                }
                var newRect = move.CalculateNewdestRectangle();
                var temp = CalculatePathByRect(newRect, board.furnitureDestination[furniture]);
                var distance = temp.Count;
                //var distance = board.RectanglesEuclideanDistance(newRect, board.furnitureDestination[furniture]);
                distPerDir[direction] = distance;
            }
            var distPerDirSorted = distPerDir.OrderBy(i => i.Value).ToDictionary(i => i.Key, i => i.Value);

            return new List<Direction>(distPerDirSorted.Keys);
        }

        /// <summary>
        /// Sort roataion direction by distance 
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="directions"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        private List<RotationDirection> SortRotateDirectionsByDistance(Furniture furniture,
                                                                       List<RotationDirection> directions, Board board)
        {
            if (directions.Count() == 1)
            {
                return directions;
            }
            var sortedDir = new List<RotationDirection>();

            Rotate rotate = new Rotate(furniture);
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

        /// <summary>
        /// Checks if furniture can move
        /// </summary>
        /// <param name="directions"></param>
        /// <param name="furniture"></param>
        /// <param name="blockingfurPerOperation"></param>
        /// <returns></returns>
        private Operation CheckIfCanMove(List<Direction> directions, Furniture furniture,
                                         Dictionary<Operation, List<Furniture>> blockingfurPerOperation)
        {
            foreach (var direction in directions)
            {
                Move move = new Move(furniture);
                move.Direction = direction;
                move.HowManyStepsInDirection = 1;
                Rectangle diffRect = move.CalculateRectDiff();
                if (Board.Instance.InBounds(diffRect))
                {
                    if (Board.Instance.IsNotWall(diffRect))
                    {
                        if (Board.Instance.IsEmpty(diffRect))
                            return (Operation) move;
                        else //is blocked by other fur'
                        {
                            var problematicFur = Board.Instance.FindFurnitureInRect(diffRect);
                            blockingfurPerOperation.Add((Operation) move, problematicFur);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Check if furntiure can be rotated
        /// </summary>
        /// <param name="directions"></param>
        /// <param name="rotate"></param>
        /// <param name="blockingfurPerOperation"></param>
        /// <returns></returns>
        private Operation CheckIfCanRotate(List<Direction> directions, Rotate rotate,
                                           Dictionary<Operation, List<Furniture>> blockingfurPerOperation)
        {
            //TODO: need to decide in what direction to rotate
            List<RotationDirection> RDL = new List<RotationDirection>();
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
                            return rotate;
                        }
                        var problematicFur = Board.Instance.FindFurnitureInRect(diffRect);
                        var newRotate = new Rotate(rotate.Furniture) {RotationDirection = SRD};
                        blockingfurPerOperation.Add(newRotate, problematicFur);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if furniture can pass door
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="currRoom"></param>
        /// <param name="endRoom"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Finds possible directions to door
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="currRoom"></param>
        /// <param name="endRoom"></param>
        /// <returns></returns>
        private List<Direction> FindPossibleDirectionsToDoor(Furniture furniture, int currRoom, int endRoom)
        {
            var result = new List<Direction>();
            Rectangle rect = furniture.Description;
            int rectXL = rect.X;
            int rectYH = rect.Y;
            int rectXH = rect.X + rect.Width - 1;
            int rectYL = rect.Y + rect.Height - 1;

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

        /// <summary>
        /// Checks if both rectangles are in the same orientation
        /// </summary>
        /// <param name="furCurrPos"></param>
        /// <param name="furDest"></param>
        /// <returns></returns>
        private bool IsSameOrientaion(Rectangle furCurrPos, Rectangle furDest)
        {
            if (furCurrPos.Height == furDest.Height && furCurrPos.Width == furDest.Width)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// calculates a map that count for each valid move the # of interrupts it creates
        /// </summary>
        /// <param name="board"></param>
        /// <param name="furniture"></param>
        /// <param name="predicateToSatisfy"></param>
        /// <returns>return the move with the minimum interrupts</returns>
        /// if there is no move that doesn't create interrupts we can implement a sort that between all operation preffer the move in dir.
        private Operation FindOptimalOperation(Board board, Furniture furniture, Predicate predicateToSatisfy)
        {
            var opWhichSatisfiesPredMinInter = new Dictionary<int, List<Operation>>();
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
                FindOptimalRotaion(new Rotate(furniture), rotationDirection, optimalOperation, board,
                                   opWhichSatisfiesPredMinInter, predicateToSatisfy);
            }
            //moving 
            foreach (var dir in listDirections)
            {
                FindOptimalMove(new Move(furniture), dir, optimalOperation, board, opWhichSatisfiesPredMinInter,
                                predicateToSatisfy);
            }
            if (opWhichSatisfiesPredMinInter.Count > 0)
            {
                var opWhichSatisfiesPredMinInterSorted = opWhichSatisfiesPredMinInter.OrderBy(i => i.Key);
                return opWhichSatisfiesPredMinInterSorted.First().Value.First();
            }
            var optimalOperationSorted = optimalOperation.OrderBy(i => i.Key);
            return optimalOperationSorted.First().Value.First();
        }

        /// <summary>
        /// Finds the optimal move to perform
        /// </summary>
        /// <param name="move"></param>
        /// <param name="direction"></param>
        /// <param name="optimalOperation"></param>
        /// <param name="board"></param>
        /// <param name="opWhichSatisfiesPredMinInter"></param>
        /// <param name="predicateToSatisfy"></param>
        private void FindOptimalMove(Move move, Direction direction, Dictionary<int, List<Operation>> optimalOperation,
                                     Board board, Dictionary<int, List<Operation>> opWhichSatisfiesPredMinInter,
                                     Predicate predicateToSatisfy)
        {
            var tempList = new List<Operation>();
            move.Direction = direction;
            move.HowManyStepsInDirection = 1;
            Rectangle rectToBeClean = move.CalculateRectDiff();
            if (!board.InBounds(move.CalculateNewdestRectangle()) || !board.IsNotWall(rectToBeClean)) return;
            var numberOfFurInRect = board.FindFurnitureInRect(rectToBeClean).Count;
            var newDestRect = move.CalculateNewdestRectangle();
            Operation op = (Operation) move;
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

        /// <summary>
        /// Check if perdicate is satisfied
        /// </summary>
        /// <param name="newDestRect"></param>
        /// <param name="predicateToSatisfy"></param>
        /// <returns></returns>
        private bool PredIsSatisfied(Rectangle newDestRect, Predicate predicateToSatisfy)
        {
            if (predicateToSatisfy is PLocation)
            {
                var rect = (predicateToSatisfy as PLocation).rect;
                return newDestRect == rect ? true : false;
            }
            var pClean = predicateToSatisfy as PClean;
            if (pClean != null)
            {
                var rect1 = pClean.CleanRect;
                return newDestRect == rect1 ? true : false;
            }
            return false;
        }

        /// <summary>
        /// Finds which rotation is the optimal to perform in current situation
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="RD"></param>
        /// <param name="optimalOperation"></param>
        /// <param name="board"></param>
        /// <param name="opWhichSatisfiesPredMinInter"></param>
        /// <param name="predicateToSatisfy"></param>
        private void FindOptimalRotaion(Rotate rotate, RotationDirection RD,
                                        Dictionary<int, List<Operation>> optimalOperation, Board board,
                                        Dictionary<int, List<Operation>> opWhichSatisfiesPredMinInter,
                                        Predicate predicateToSatisfy)
        {
            var tempList = new List<Operation>();
            rotate.RotationDirection = RD;
            Rectangle rectToBeClean = rotate.CalculateRectToBeCleanByDirection();
            if (!board.InBounds(rectToBeClean) || !board.IsNotWall(rectToBeClean)) return;
            var numberOfFurInRect = board.FindFurnitureInRect(rectToBeClean).Count;
            Operation op = (Operation) rotate;
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

        /// <summary>
        /// Finds possible direction to move in
        /// </summary>
        /// <param name="furniture"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        private List<Direction> FindPossibleDirections(Furniture furniture, Board board)
        {
            var directions = new List<Direction>();
            Rectangle dest = board.furnitureDestination[furniture];
            Rectangle currPos = furniture.Description;
            int destX = dest.X;
            int destY = dest.Y;
            int currPosX = currPos.X;
            int currPosY = currPos.Y;

            bool isOnDoor = (Board.Instance.FindRoomPerRect(furniture.Description) == 1 &&
                             Board.Instance.FindRoomPerRect(dest) == 1) &&
                            (furniture.Description.X + furniture.Description.Width - 1 >= 11);

            if ((destY - currPosY) > 0)
                directions.Add(Direction.Down);
            if ((destY - currPosY) < 0)
                directions.Add(Direction.Up);
            if ((destX - currPosX) > 0)
                directions.Add(Direction.Right);
            if ((destX - currPosX) < 0)
                directions.Add(Direction.Left);

            //support cases in which curr room & end room are 1, and block is in the passage
            if (isOnDoor)
            {
                directions.Add(Direction.Left);
            }
            return directions;
        }

        /// <summary>
        /// Finds which furniture intefere others
        /// </summary>
        /// <param name="board"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// calculate the path by rectangle
        /// </summary>
        /// <param name="startRect"></param>
        /// <param name="endRect"></param>
        /// <returns></returns>
        public static List<Rectangle> CalculatePathByRect(Rectangle startRect, Rectangle endRect)
        {
            List<Rectangle> path = new List<Rectangle>();
            var startRoom = Board.Instance.FindRoomPerRect(startRect);
            var endRoom = Board.Instance.FindRoomPerRect(endRect);
            //moving from room 2 to room 3 or vice a versa
            if (startRoom != 1 && endRoom != 1)
            {
                // moving to the door
                var doorRect = Group.GetRoomDoor(startRoom);
                int width = startRect.Width;
                int height = startRect.Height;
                if (startRect.Height > doorRect.Height)
                {
                    width = startRect.Height;
                    height = startRect.Width;
                }
                path.AddRange(Group.FindPathBetweenPoints(startRect, doorRect, width, height));

                //move rect width time left
                for (int i = 1; i <= startRect.Width; i++)
                {
                    path.Add(new Rectangle(doorRect.X - i, doorRect.Y, width, height));
                }

                Rectangle currLoc = path.Last();
                var secDoorRect = Group.GetRoomDoor(startRoom);
                if (height > secDoorRect.Height)
                {
                    int temp = width;
                    width = height;
                    height = temp;
                }
                path.AddRange(Group.FindPathBetweenPoints(currLoc, secDoorRect, width, height));
                path.AddRange(Group.FindPathBetweenPoints(secDoorRect, endRect, endRect.Width, endRect.Height));
            }
            else
            {
                // furniture on the middle of the door
                if (!(((endRoom == 2) && (startRoom == 1) && (startRect.Y >= 2) && (startRect.Y <= 3)) ||
                      ((endRoom == 3) && (startRoom == 1) && (startRect.Y >= 7) && (startRect.Y <= 10))))
                {
                    int width = Math.Max(startRect.Width, startRect.Height);
                    int height = Math.Min(startRect.Width, startRect.Height);
                    Rectangle currLoc = new Rectangle(startRect.X, startRect.Y, width, height);
                    while (currLoc.X + startRect.Width >= 11)
                    {
                        path.Add(currLoc);
                        currLoc = new Rectangle(currLoc.X - 1, currLoc.Y, width, height);
                    }
                    startRect = new Rectangle(currLoc.X + 1, currLoc.Y, width, height);
                }

                path.AddRange(Group.FindPathBetweenPoints(startRect, endRect, endRect.Width, endRect.Height));
            }
            return path;
        }
    }
}