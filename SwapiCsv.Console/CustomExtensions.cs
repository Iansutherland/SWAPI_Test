using System;
using System.Collections.Generic;
using System.Text;

namespace SwapiCsv.ConsoleUI
{
    public static class CustomExtensions
    {
        /// <summary>
        /// Flattens 2d list to 1d list while preserving order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOfLists"></param>
        /// <returns></returns>
        public static List<T> FlattenListOfLists<T>(this List<List<T>> listOfLists)
        {
            List<T> flattenedList = new List<T>();
            while(listOfLists.Count > 0)
            {
                flattenedList.AddRange(listOfLists[0]);
                listOfLists.RemoveAt(0);
            }
            return flattenedList;
        }
    }
}
