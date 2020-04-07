using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class includes several helper methods for generating variations of interaction data sequences.
/// /// </summary>
public static class InteractionDataMethods
{
    /// <summary>
    /// Helper method for generating variations of interaction sequences.
    /// </summary>
    /// <param name="interactionDataArray">Array of interaction data for generating the variations.</param>
    /// <param name="generateVariationAttempts">Number of attempts for generating variations of the specified interaction data array.</param>
    /// <param name="maxConflictThreshold">Maximum threshold of encountering dependency conflicts before stopping.</param>
    /// <param name="ignoreDependencies">Boolean which indicates whether dependencies between interactions should be taken into account.</param>
    /// <param name="generateSequenceVariations">Boolean which indicates whether variations should be generated.</param>
    /// <returns>List of variations of interaction sequences.</returns>
    public static List<InteractionData[]> GetInteractionDataSequenceVariations(InteractionData[] interactionDataArray,
       int generateVariationAttempts, int maxConflictThreshold, bool generateSequenceVariations)
    {
        // List used instead of array because unsure of how many variations exist
        List<InteractionData[]> sequenceVariationsCollection = new List<InteractionData[]>();

        if (generateSequenceVariations)
        {
            for (int i = 0; i < generateVariationAttempts; i++)
            {
                // Generate a sequence
                InteractionData[] interactionSequenceVariation = GenerateInteractionSequenceVariation(ConvertArrayToList(interactionDataArray), maxConflictThreshold);

                // Only consider sequences that have the full length
                if (interactionSequenceVariation.Length == interactionDataArray.Length)
                {
                    // Does queue exist in the list? - add if not
                    if (!DoesVariationExistInCollection(sequenceVariationsCollection, interactionSequenceVariation))
                        sequenceVariationsCollection.Add(interactionSequenceVariation);
                }
            }
        }
        else
        {
            // If no variations - Just add the raw array itself
            sequenceVariationsCollection.Add(interactionDataArray);
        }

        return sequenceVariationsCollection;
    }

    /// <summary>
    /// Helper method for generating a variation of an interaction sequence.
    /// </summary>
    /// <param name="interactionData">An interaction sequence (in list format).</param>
    /// <param name="maxConflictThreshold">Maximum threshold of finding conflicting dependencies before stopping.</param>
    /// <returns>A newly generated variation of an interaction sequence.</returns>
    private static InteractionData[] GenerateInteractionSequenceVariation(List<InteractionData> interactionData, int maxConflictThreshold)
    {
        List<InteractionData> generatedSequence = new List<InteractionData>();
        int conflictCount = 0;

        List<InteractionData> disposibleList = interactionData.CopyList();

        while (disposibleList.Count != 0)
        {
            /*
                NOTE:
                Need to link this somehow to the RNG game object to ensure
                consistent generation of random numbers according to a specific
                seed.

                The RNG game object, however, is only defined at runtime.
            */

            int randomPosition = Random.Range(0, disposibleList.Count);
            InteractionData randomInteraction = disposibleList[randomPosition];

            // Add interaction only if dependencies are satisfied
            if (AreDependenciesSatisfiedInSequenceForInteraction(generatedSequence, randomInteraction))
            {
                generatedSequence.Add(randomInteraction);
                disposibleList.RemoveAt(randomPosition);
            }
            else
            {
                // If max conflict amount reached - stop, and return whatever sequence that has been generated
                conflictCount++;
                if (conflictCount == maxConflictThreshold)
                    break;
            }
        }

        return generatedSequence.ToArray();
    }

    /// <summary>
    /// Helper method for converting an array to a list.
    /// </summary>
    /// <param name="interactionDataArray">Array of interaction data to be converted to list.</param>
    /// <returns>A list of interaction data.</returns>
    private static List<InteractionData> ConvertArrayToList(InteractionData[] interactionDataArray)
    {
        List<InteractionData> interactionDataList = new List<InteractionData>();
        for (int i = 0; i < interactionDataArray.Length; i++)
            interactionDataList.Add(interactionDataArray[i]);
        return interactionDataList;
    }

    /// <summary>
    /// Helper method for evaluating if dependencies of an interaction are met in the interaction sequence.
    /// </summary>
    /// <param name="sequence">Interaction sequence which is evaluated upon.</param>
    /// <param name="anInteraction">An interaction whose dependencies are evaluated.</param>
    /// <returns>True if the dependencies of specified interaction are satisfied. False if otherwise.</returns>
    private static bool AreDependenciesSatisfiedInSequenceForInteraction(List<InteractionData> sequence, InteractionData anInteraction)
    {
        int identifiedDependenciesCount = 0;
        foreach (string anInteractionDescription in anInteraction.DependencyDescriptions)
        {
            foreach (InteractionData aDestination in sequence)
            {
                // Check if dependency object already in upcoming destinations
                if (aDestination.Description.Equals(anInteractionDescription))
                {
                    identifiedDependenciesCount++;
                    break;
                }
            }
        }

        // Return whether dependencies are satisfied
        return anInteraction.DependencyDescriptions.Length == identifiedDependenciesCount;
    }

    /// <summary>
    /// Helper method for evaluating if a variation already exists in a specified collection of variations of an interaction sequence.
    /// </summary>
    /// <param name="interactionSequenceVariations">Collection of variations of an interaction sequence.</param>
    /// <param name="interactionSequenceVariation">A newly created variation of an interaction sequence.</param>
    /// <returns></returns>
    private static bool DoesVariationExistInCollection(List<InteractionData[]> interactionSequenceVariations, InteractionData[] interactionSequenceVariation)
    {
        if (interactionSequenceVariations.Count == 0)
            return false;

        foreach (InteractionData[] anInteractionDataArray in interactionSequenceVariations)
            if (AreInteractionSequencesEqual(anInteractionDataArray, interactionSequenceVariation))
                return true;
        return false;
    }

    /// <summary>
    /// Helper method for evaluating if two interaction sequences are equal.
    /// </summary>
    /// <param name="sequenceA">First interaction sequence to be evaluated.</param>
    /// <param name="sequenceB">Second interaction sequence to be evaluated.</param>
    /// <returns>True if the specified interaction sequences are equal. False if otherwise.</returns>
    private static bool AreInteractionSequencesEqual(InteractionData[] sequenceA, InteractionData[] sequenceB)
    {
        if ((sequenceA == null) && (sequenceB == null))
            return true;

        if ((sequenceA == null) || (sequenceB == null))
            return false;

        // Can only be equal if length is the equal
        if (sequenceA.Length == sequenceB.Length)
        {
            // If any element not equal in same position - false
            for (int i = 0; i < sequenceA.Length; i++)
                if (!sequenceA[i].Equals(sequenceB[i]))
                    return false;

            // Otherwise, arrays equal
            return true;
        }

        // If not same length = false
        return false;
    }
}
