using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfEffectAttribute), true)]
public class ShowIfEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr    = (ShowIfEffectAttribute)attribute;
        var srcProp = property.serializedObject.FindProperty(attr.sourceFieldName);

        // Si on n'a pas trouvé l'enum, on affiche par défaut
        if (srcProp == null || srcProp.propertyType != SerializedPropertyType.Enum)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        // Récupère la valeur courante de l'enum
        var current = (EntityData.EntityEffects)srcProp.enumValueIndex;

        // Recherche manuelle dans requiredEffects
        bool shouldShow = false;
        for (int i = 0; i < attr.requiredEffects.Length; i++)
        {
            if (attr.requiredEffects[i] == current)
            {
                shouldShow = true;
                break;
            }
        }

        if (shouldShow)
            EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var attr    = (ShowIfEffectAttribute)attribute;
        var srcProp = property.serializedObject.FindProperty(attr.sourceFieldName);

        // Par défaut, on retourne la hauteur normale si on ne peut pas vérifier
        if (srcProp == null || srcProp.propertyType != SerializedPropertyType.Enum)
            return EditorGUI.GetPropertyHeight(property, label, true);

        var current = (EntityData.EntityEffects)srcProp.enumValueIndex;

        bool shouldShow = false;
        for (int i = 0; i < attr.requiredEffects.Length; i++)
        {
            if (attr.requiredEffects[i] == current)
            {
                shouldShow = true;
                break;
            }
        }

        return shouldShow
            ? EditorGUI.GetPropertyHeight(property, label, true)
            : 0f;
    }
}
