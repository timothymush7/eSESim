/// <summary>
/// This class represents a field of information associated with an object.
/// </summary>
[System.Serializable]
public class Property
{
    public static string PROPERTY_TYPE_FLOAT = "float";
    public static string PROPERTY_TYPE_BOOL = "bool";

    public string gameObjectName;                   // Idenfitier of game object for property
    public string description;                      // field description associated with property
    public string propertyType;                     // Field indicating the type of the property
    public float floatValue;                       // field value associated with property
    public bool boolValue;                         // bool value associated with property
    public float resetFloatValue = 0f;             // default value used for float value on reset
    public bool resetBoolValue = false;            // default value used for bool value on reset

    public delegate void OnValueChangeDelegate();
    public event OnValueChangeDelegate OnValueChange;

    public float FloatValue
    {
        get
        {
            return floatValue;
        }
        set
        {
            floatValue = value;
            if (OnValueChange != null)
                OnValueChange();
        }
    }

    public bool BoolValue
    {
        get
        {
            return boolValue;
        }
        set
        {
            boolValue = value;
            if (OnValueChange != null)
                OnValueChange();
        }
    }

    public float DefaultFloatValue
    {
        get
        {
            return resetFloatValue;
        }
        set
        {
            resetFloatValue = value;
        }
    }

    public bool DefaultBoolValue
    {
        get
        {
            return resetBoolValue;
        }
        set
        {
            resetBoolValue = value;
        }
    }

    public int CompareTo(Property anotherProperty)
    {
        // Comparison done on parent name, followed by description.
        int parentNameCompare = gameObjectName.CompareTo(anotherProperty.gameObjectName);
        if (parentNameCompare == 0)
        {
            int descriptionCompare = description.CompareTo(anotherProperty.description);
            return descriptionCompare;
        }

        return parentNameCompare;
    }
}
