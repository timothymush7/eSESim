using System.Collections.Generic;
using UnityEngine;

public class PropertiesCollection : Singleton<PropertiesCollection> {

    public PropertiesCollectionAsset PropertiesAsset;
    private Dictionary<string, List<Property>> GameObjectNameToProperties;
    private List<string> GameObjectNames;

    public override void Awake()
    {
        base.Awake();

        if (PropertiesAsset)
            PreparePropertyRelatedDataStructures();
        else
            Debug.LogError("Properties Collection Asset not assigned.");
    }

    private void PreparePropertyRelatedDataStructures()
    {
        ClearPropertiesByParentNameDictionary();
        ClearParentNameSet();

        for (int i = 0; i < PropertiesAsset.properties.Length; i++)
        {
            Property aProperty = PropertiesAsset.properties[i];

            if (!GameObjectNameToProperties.ContainsKey(aProperty.gameObjectName))
            {
                GameObjectNameToProperties.Add(aProperty.gameObjectName, new List<Property>());
                GameObjectNames.Add(aProperty.gameObjectName);
            }

            GameObjectNameToProperties[aProperty.gameObjectName].Add(aProperty);
        }
    }

    private void ClearPropertiesByParentNameDictionary()
    {
        if (GameObjectNameToProperties == null)
            GameObjectNameToProperties = new Dictionary<string, List<Property>>();
        GameObjectNameToProperties.Clear();
    }

    private void ClearParentNameSet()
    {
        if (GameObjectNames == null)
            GameObjectNames = new List<string>();
        GameObjectNames.Clear();
    }

    public string[] GetGameObjectNames()
    {
        if (GameObjectNames != null)
            return GameObjectNames.ToArray();
        return null;
    }

    public Property[] TryGetPropertiesFromAsset()
    {
        if (PropertiesAsset)
            return PropertiesAsset.properties;
        else
        {
            Debug.LogError("PropertyManager - GetPropertiesFromAsset: AllPropertiesAsset not assigned.");
            return new Property[0];
        }
    }

    public List<Property> TryGetProperties(string gameObjectName)
    {
        if (GameObjectNameToProperties.ContainsKey(gameObjectName))
            return GameObjectNameToProperties[gameObjectName];
        return null;
    }

    public int TryGetListOfPropertiesLength()
    {
        if (PropertiesAsset.properties == null)
            return 0;

        return PropertiesAsset.properties.Length;
    }

    public Property TryGetPropertyUsingParentNameAndDescription(string parentName, string description)
    {
        List<Property> propertiesOfParentName = TryGetProperties(parentName);

        if (propertiesOfParentName != null)
        {
            foreach (Property aProperty in propertiesOfParentName)
                if (Animator.StringToHash(aProperty.description) == Animator.StringToHash(description))
                    return aProperty;
        }

        return null;
    }

    public int TryGetPropertyIndexUsingProperty(Property aProperty)
    {
        if (GameObjectNameToProperties.ContainsKey(aProperty.gameObjectName))
        {
            List<Property> propertiesOfParentName = GameObjectNameToProperties[aProperty.gameObjectName];

            for (int x = 0; x < GameObjectNameToProperties.Count; x++)
                if (Animator.StringToHash(propertiesOfParentName[x].description) ==
                    Animator.StringToHash(aProperty.description))
                    return x;
        }

        return -1;
    }

    public string[] TryGetPropertyDescriptionsUsingParentName(string gameObjectName)
    {
        List<Property> gameObjectProperties = TryGetProperties(gameObjectName);

        if (gameObjectProperties != null)
        {
            string[] propertyDescriptions = new string[gameObjectProperties.Count];
            for (int i = 0; i < propertyDescriptions.Length; i++)
                propertyDescriptions[i] = gameObjectProperties[i].description;
            return propertyDescriptions;
        }
        
        return null;
    }

    public Property TryGetProperty(string gameObjectName, int index)
    {
        List<Property> gameObjectProperties = TryGetProperties(gameObjectName);

        if (gameObjectProperties != null)
            if (index < gameObjectProperties.Count)
                return gameObjectProperties[index];

        return null;
    }
}
