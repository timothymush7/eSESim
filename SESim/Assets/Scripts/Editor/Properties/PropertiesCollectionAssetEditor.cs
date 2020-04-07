using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor class for the properties collection asset class. Draws the GUI for the individual 
/// properties in the asset and also enables users to load properties from JSON text files.
/// </summary>
[CustomEditor(typeof(PropertiesCollectionAsset))]
public class PropertiesCollectionAssetEditor : Editor
{
    private PropertiesCollectionAsset PropertiesCollectionAssetInstance;                        // Current instance of the properties collection asset
    private const string AssetCreationPath = "Assets/Resources/PropertiesCollection.asset";     // Creation path of the properties collection asset via menu

    private void OnEnable()
    {
        PropertiesCollectionAssetInstance = (PropertiesCollectionAsset)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawPropertiesGUI();
    }

    /// <summary>
    /// Helper method for drawing the GUI for the properties.
    /// </summary>
    private void DrawPropertiesGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        if (PropertiesCollectionAssetInstance.properties != null)
        {
            if (PropertiesCollectionAssetInstance.properties.Length > 0)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < PropertiesCollectionAssetInstance.properties.Length; i++)
                {
                    Property aProperty = PropertiesCollectionAssetInstance.properties[i];

                    // Draw GUI for property attributes
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    aProperty.gameObjectName = EditorGUILayout.TextField("Parent Name", aProperty.gameObjectName);
                    aProperty.description = EditorGUILayout.TextField("Description", aProperty.description);

                    // Draw appropriate GUI depending on boolean or float type
                    if (aProperty.propertyType.Equals(Property.PROPERTY_TYPE_BOOL))
                    {
                        EditorGUILayout.LabelField("Type", "Bool Property");
                        aProperty.BoolValue = EditorGUILayout.Toggle("Value", aProperty.BoolValue);
                        aProperty.DefaultBoolValue = EditorGUILayout.Toggle("Reset Value", aProperty.DefaultBoolValue);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Type", "Float Property");
                        aProperty.FloatValue = EditorGUILayout.FloatField("Value", aProperty.FloatValue);
                        aProperty.DefaultFloatValue = EditorGUILayout.FloatField("Reset Value", aProperty.DefaultFloatValue);
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }
            else
                EditorGUILayout.LabelField("No property data found.");
        }
        else
            EditorGUILayout.LabelField("Property data not defined.");

        EditorGUILayout.EndVertical();

        // Draw button GUI for loading properties
        if (GUILayout.Button("Load Properties via Text Assets"))
            LoadPropertiesFromTextAssets();
    }

    /// <summary>
    /// Helper method for loading properties from JSON text files.
    /// </summary>
    private void LoadPropertiesFromTextAssets()
    {
        TextAsset textAsset = PropertiesCollectionAssetInstance.textAsset;

        if (textAsset != null)
            PropertiesCollectionAssetInstance.properties = JsonUtility.FromJson<PropertiesFromJSON>(textAsset.text).properties;
        else
            Debug.LogError("No text assets are attached.");
    }

    /// <summary>
    /// Helper method for creating a properties collection asset. This method is accessed
    /// from Unity menus.
    /// </summary>
    [MenuItem("Assets/Create/Properties Collection Asset")]
    private static void CreatePropertiesCollectionAsset()
    {
        // Only create asset if it doesn't exist already in resources folder
        if (Resources.Load<PropertiesCollectionAsset>(PropertiesCollectionAsset.AssetLoadPath) == null)
        {
            PropertiesCollectionAsset newAllPropertiesInstance = CreateInstance<PropertiesCollectionAsset>();
            newAllPropertiesInstance.properties = new Property[0];
            AssetDatabase.CreateAsset(newAllPropertiesInstance, AssetCreationPath);
        }
    }
}
