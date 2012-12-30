using System.Drawing;

namespace BoardDataModel
{

    public class Furniture
    {
        private Rectangle furnitureDescription;
        public Furniture(Rectangle rec,int id)
        {
            furnitureDescription = rec;
            ID = id;
        }

        public int ID { get; set;}

        public Rectangle Description
        {
            get
            {
                return furnitureDescription;
            }
            set
            {
                furnitureDescription = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Furniture)obj);
        }

        protected bool Equals(Furniture other)
        {
            return furnitureDescription.Equals(other.furnitureDescription);
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return furnitureDescription.GetHashCode();
        }


        /// <summary>
        /// operator==
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Furniture a, Furniture b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Description.X == b.Description .X && a.Description .Y == b.Description .Y && a.ID == b.ID;
        }

        /// <summary>
        /// Operator !=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Furniture a, Furniture b)
        {
            return !(a == b);
        }
    }
}
