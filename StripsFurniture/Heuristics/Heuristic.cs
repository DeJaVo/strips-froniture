using System;
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

                // the representative is the last in the list of furnitures
                // (assuming that is sorted)

                if (this.GroupType == Heuristic.GroupType.SameRoom)
                {
                    path = this.FindPathBetweenPoints(startState, destState);
                }
                else
                {
                    path = new List<Rectangle>();
                    if (DoorsPath.Count == 1)
                    {
                        Rectangle roomDoor = this.GetRoomDoor(this.DoorsPath[0]);
                        path.AddRange(this.FindPathBetweenPoints(startState, roomDoor));
                        path.AddRange(this.FindPathBetweenPoints(roomDoor, destState));
                    }
                    else
                    {
                        Rectangle firstRoomDoor = this.GetRoomDoor(this.DoorsPath[0]);
                        Rectangle lastRoomDoor = this.GetRoomDoor(this.DoorsPath[1]);
                        path.AddRange(this.FindPathBetweenPoints(startState,firstRoomDoor));
                        path.AddRange(this.FindPathBetweenDoors(firstRoomDoor, lastRoomDoor));
                        path.AddRange(this.FindPathBetweenPoints(lastRoomDoor, destState));
                    }
                }
            }

            private List<Rectangle> FindPathBetweenDoors(Rectangle start, Rectangle end)
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

            private List<Rectangle> FindPathBetweenPoints(Rectangle start, Rectangle end)
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
                    subPath.Add(new Rectangle(i,end.Y, 1, 1));
                }

                return subPath;
            }

            private Rectangle GetRoomDoor(int roomId)
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

                // if a furniture from g2 in start state is on the path of g1 than g1 is smaller than g2
                if (this.IsOnPath(tested.Path, otherGroupFurnituresInStart))
                {
                    return 1;
                }

                // else if a furniture from g2 in dest state is on the path of g1 than g1 is bigger than g2
                if (this.IsOnPath(tested.Path, otherGroupFurnituresInDest))
                {
                    return -1;
                }

                return 0;
            }

            private bool IsOnPath(List<Rectangle> path,List<Rectangle> furnitures)
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

                this.UpdateRooms(startRect, ((PLocation)currLocation).furniture.ID,
                                ref room1StartStateFurnitures,
                                ref room2StartStateFurnitures,
                                ref room3StartStateFurnitures);

                this.UpdateRooms(destRect, ((PLocation)currLocation).furniture.ID,
                                ref room1DestStateFurnitures,
                                ref room2DestStateFurnitures,
                                ref room3DestStateFurnitures);
            }
        }

        private void UpdateRooms(Rectangle rect,int id,
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
                double dist1 = Math.Pow(ploc1.furniture.Description.X - ploc1.rect.X, 2) +
                               Math.Pow(ploc1.furniture.Description.Y - ploc1.rect.Y, 2);

                double dist2 = Math.Pow(ploc2.furniture.Description.X - ploc2.rect.X, 2) +
                               Math.Pow(ploc2.furniture.Description.Y - ploc2.rect.Y, 2);

                if (dist1 == dist2)
                {
                    return 0;
                }
                return dist1 > dist2 ? 1 : -1;
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

        public Operation ChooseOperation(Board board, Predicate predicateToSatisfy)
        {
            //TODO : need to be implemented 

            //Move move = new Move(((PLocation)predicateToSatisfy).furniture);
            //move.Direction = Direction.Down;
            //move.HowManyStepsInDirection = 1;
            //return move;

            Rotate rotate = new Rotate(((PLocation)predicateToSatisfy).furniture);
            rotate.RotationDirection=RotationDirection.CounterClockWise;
            return rotate;
        }
    }
}
