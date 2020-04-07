using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension class which provides several helper methods for list implementations.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Utility method which returns a copy of the current list.
    /// </summary>
    /// <param name="aList">The current list.</param>
    /// <typeparam name="T">Type of the current list.</typeparam>
    /// <returns>A copy of the current list.</returns>
    public static List<T> CopyList<T>(this IList<T> aList)
    {
        List<T> newListOfCopies = new List<T>();
        foreach (T anItem in aList)
            newListOfCopies.Add(anItem);
        return newListOfCopies;
    }

    /// <summary>
    /// Utiltiy method which shuffles the content of the current list.
    /// </summary>
    /// <param name="aList">The current list.</param>
    /// <typeparam name="T">Type of the current list.</typeparam>
    public static void ShuffleList<T>(this IList<T> aList)
    {
        // Fisher-Yates Shuffle

        int nthPosition = aList.Count;
        while (nthPosition > 1)
        {
            nthPosition--;
            int randomPosition = Random.Range(0, nthPosition + 1);
            T tempValue = aList[randomPosition];
            aList[randomPosition] = aList[nthPosition];
            aList[nthPosition] = tempValue;
        }
    }
}
