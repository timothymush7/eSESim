using UnityEditor;
using UnityEngine;

public class PropertiesCollectionAsset : ResettableScriptableObject {

    public TextAsset textAsset;
    public Property[] properties;

    public static string AssetLoadPath = "PropertiesCollectionAsset";
    public static string AssetCreationPath = "Assets/Resources/PropertiesCollection.asset";

    public override void Reset()
    {
        if (properties == null)
            return;

        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].BoolValue = properties[i].DefaultBoolValue;
            properties[i].FloatValue = properties[i].DefaultFloatValue;
        }
    }

    public void SortProperties()
    {
        InsertionSortOnPropertiesArray();
    }

    public void SaveChanges()
    {
        AssetDatabase.SaveAssets();
    }

    private void InsertionSortOnPropertiesArray()
    {
        if (properties.Length > 1)
        {
            int initPos = 1;
            while (initPos < properties.Length)
            {
                int listPos = initPos;
                while ((listPos > 0) && (properties[listPos - 1].
                    CompareTo(properties[listPos]) > 0))
                {
                    Property propertyA = properties[listPos];
                    properties[listPos] = properties[listPos - 1];
                    properties[listPos - 1] = propertyA;

                    listPos--;
                }
                initPos++;
            }
        }
    }
}
