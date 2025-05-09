using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ShowIfEffectAttribute : PropertyAttribute
{
    public readonly string sourceFieldName;
    public readonly EntityData.EntityEffects requiredEffect;

    public ShowIfEffectAttribute(string sourceFieldName, EntityData.EntityEffects requiredEffect)
    {
        this.sourceFieldName  = sourceFieldName;
        this.requiredEffect   = requiredEffect;
    }
}

