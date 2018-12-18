using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionalField : PropertyAttribute {

    public string Label;
    public string PropertyToCheck;
    public object CompareValue;
    public ConditionalField(string label, 
            string propertyToCheck, 
            object compareValue = null)
    {
        this.Label = label;
        PropertyToCheck = propertyToCheck;
        CompareValue = compareValue;
    }

    void doStuff()
    {
        if (PropertyToCheck == "")
        {
            
        }
    }
}
