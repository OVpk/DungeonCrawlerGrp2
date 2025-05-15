using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ShowIfEffectAttribute : PropertyAttribute
{
    public readonly string sourceFieldName;
    public readonly EntityData.EntityEffects[] requiredEffects;

    public ShowIfEffectAttribute(string sourceFieldName, params EntityData.EntityEffects[] requiredEffects)
    {
        this.sourceFieldName = sourceFieldName;
        this.requiredEffects = requiredEffects;
    }
}

