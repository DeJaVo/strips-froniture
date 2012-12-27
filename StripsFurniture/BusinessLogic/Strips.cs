using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BoardDataModel;
namespace BusinessLogic
{
    public class Strips
    {
        public Strips(Board board)
        {
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
