using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfEffectAttribute), true)]
public class ShowIfEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (ShowIfEffectAttribute)attribute;
        var srcProp = property.serializedObject.FindProperty(attr.sourceFieldName);

        // si le champ source est introuvable, on affiche quand même
        if (srcProp == null)
        {
            EditorGUI.PropertyField(position, property, label, true);
            Debug.Log("null");
            return;
        }

        bool shouldShow = false;

        // cas : champ enum unique
        if (srcProp.propertyType == SerializedPropertyType.Enum)
        {
            shouldShow = (srcProp.enumValueIndex == (int)attr.requiredEffect);
        }
        // cas : tableau d'enum
        else if (srcProp.isArray && srcProp.propertyType == SerializedPropertyType.Generic)
        {
            for (int i = 0; i < srcProp.arraySize; i++)
            {
                var elem = srcProp.GetArrayElementAtIndex(i);
                if (elem.propertyType == SerializedPropertyType.Enum &&
                    elem.enumValueIndex == (int)attr.requiredEffect)
                {
                    shouldShow = true;
                    break;
                }
            }
        }
        else
        {
            // autre type : on choisit d'afficher
            shouldShow = true;
        }

        if (shouldShow)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        // else : on n'affiche rien (height sera à 0)
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr    = (ShowIfEffectAttribute)attribute;
        var srcProp = property.serializedObject.FindProperty(attr.sourceFieldName);

        if (srcProp == null)
            return EditorGUI.GetPropertyHeight(property, label, true);

        bool shouldShow = false;

        if (srcProp.propertyType == SerializedPropertyType.Enum)
        {
            shouldShow = (srcProp.enumValueIndex == (int)attr.requiredEffect);
        }
        else if (srcProp.isArray && srcProp.propertyType == SerializedPropertyType.Generic)
        {
            for (int i = 0; i < srcProp.arraySize; i++)
            {
                var elem = srcProp.GetArrayElementAtIndex(i);
                if (elem.propertyType == SerializedPropertyType.Enum &&
                    elem.enumValueIndex == (int)attr.requiredEffect)
                {
                    shouldShow = true;
                    break;
                }
            }
        }
        else
        {
            shouldShow = true;
        }

        return shouldShow
            ? EditorGUI.GetPropertyHeight(property, label, true)
            : 0f;
    }
}
