using UnityEngine;
using UnityEditor;

/// <summary>
/// Extension class which provides several helper methods for serialized property objects.
/// </summary>
public static class SerializedPropertyExtensions
{
    /// <summary>
    /// Utility method for adding an element to the serialized property (array).
    /// </summary>
    /// <param name="arrayProperty">Reference to the serialized property (array).</param>
    /// <param name="elementToAdd">Elemented to be added to the serialized property (array).</param>
    /// <typeparam name="T">Type of the element to be added.</typeparam>
    public static void AddToObjectArray<T>(this SerializedProperty arrayProperty, T elementToAdd)
        where T : Object
    {
        if (!arrayProperty.isArray)
            throw new UnityException("SerializedProperty " + arrayProperty.name + " is not an array.");

        arrayProperty.serializedObject.Update();
        arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
        arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = elementToAdd;
        arrayProperty.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Utility method for removing an object within a serialized property (array) at a specific index.
    /// </summary>
    /// <param name="arrayProperty">Reference to the serialized property (array).</param>
    /// <param name="index">Index of element which is removed from the serialized property (array).</param>
    public static void RemoveFromObjectArrayAt(this SerializedProperty arrayProperty, int index)
    {
        if (index < 0)
            throw new UnityException("SerializedProperty " + arrayProperty.name + " cannot have negative elements removed.");

        if (!arrayProperty.isArray)
            throw new UnityException("SerializedProperty " + arrayProperty.name + " is not an array.");

        if (index > arrayProperty.arraySize - 1)
            throw new UnityException("SerializedProperty " + arrayProperty.name + " has only " + arrayProperty.arraySize + " elements, so element " + index + " cannot be removed.");

        arrayProperty.serializedObject.Update();
        if (arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue)
            arrayProperty.DeleteArrayElementAtIndex(index);
        arrayProperty.DeleteArrayElementAtIndex(index);
        arrayProperty.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Utility method for removing the specified object within a serialized property (array).
    /// </summary>
    /// <param name="arrayProperty">Reference to the serialized property (array).</param>
    /// <param name="elementToRemove">Element to be removed from the serialized property (array).</param>
    /// <typeparam name="T"></typeparam>
    public static void RemoveFromObjectArray<T>(this SerializedProperty arrayProperty, T elementToRemove)
        where T : Object
    {
        if (!arrayProperty.isArray)
            throw new UnityException("SerializedProperty " + arrayProperty.name + " is not an array.");

        if (!elementToRemove)
            throw new UnityException("Removing a null element is not supported using this method.");

        arrayProperty.serializedObject.Update();

        for (int i = 0; i < arrayProperty.arraySize; i++)
        {
            SerializedProperty elementProperty = arrayProperty.GetArrayElementAtIndex(i);

            // Seems that the memory points of the conditions are not matching, therefore not removing

            if (elementProperty.objectReferenceValue == elementToRemove)
            {
                arrayProperty.RemoveFromObjectArrayAt(i);
                return;
            }
        }

        throw new UnityException("Element " + elementToRemove.name + " was not found in property " + arrayProperty.name);
    }
}
