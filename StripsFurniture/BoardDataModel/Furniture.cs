﻿using System.Drawing;

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

        //public override bool Equals(object obj)
        //{
        //    Furniture fur = (Furniture)obj;

        //    return fur.ID == this.ID;// && (fur.furnitureDescription.Equals(this.Description));
        //}

        //public override int GetHashCode()
        //{
        //    return this.ID;
        //} 
    }
}
