using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirCannon.Framework.Utilities
{
    public static class LinqEx
    {
        /// <summary>
        /// Cobmines two items into a tuple.
        /// </summary>
        public static Tuple<T1, T2> ToTuple<T1, T2>(T1 left, T2 right)
        {
            return new Tuple<T1, T2>(left, right);
        }
    }
}
