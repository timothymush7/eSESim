using UnityEngine;
using UnityEditor;

/// <summary>
/// Abstract class which describes an editor with sub-editors inside of it.
/// </summary>
/// <typeparam name="TEditor">The custom editor class for the target class/object.</typeparam>
/// <typeparam name="TTarget">The target class/object which has a custom editor.</typeparam>
public abstract class EditorWithSubEditors<TEditor, TTarget> : Editor
    where TEditor : Editor
    where TTarget : Object
{
    /*
        This class enables editor classes to contain sub-editors and
        reuse editor classes.

        For example, the reaction collection editor uses multiple
        reaction editors.
    */

    protected TEditor[] subEditors;         // sub-editors within the editor class

    /// <summary>
    /// Helper method for recreating subeditors if necessary.
    /// </summary>
    /// <param name="subEditorTargets">The target objects which each require a subeditor.</param>
    /// <returns>True if subeditors were recreated. False if otherwise.</returns>
    protected bool RecreateSubEditors(TTarget[] subEditorTargets)
    {
        // Are subeditors not defined? Or if number of subeditors does not match number of targets?
        if ((subEditors == null) || (subEditors.Length != subEditorTargets.Length))
        {
            // Clean out old subeditors
            DestroySubEditors();

            // Redefine + setup new subeditors
            subEditors = new TEditor[subEditorTargets.Length];
            for (int i = 0; i < subEditors.Length; i++)
            {
                subEditors[i] = CreateEditor(subEditorTargets[i]) as TEditor;
                SubEditorSetup(subEditors[i]);
            }

            // Changes made - return true
            return true;
        }

        // No changes made - return false
        return false;
    }

    protected void DestroySubEditors()
    {
        if (subEditors == null) return;
        for (int i = 0; i < subEditors.Length; i++)
            DestroyImmediate(subEditors[i]);
        subEditors = null;
    }

    protected abstract void SubEditorSetup(TEditor editor);
}
