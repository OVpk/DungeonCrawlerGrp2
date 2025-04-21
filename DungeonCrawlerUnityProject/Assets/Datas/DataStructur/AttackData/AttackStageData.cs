using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/AttackStageData")]
public class AttackStageData : ScriptableObject
{
    [field: SerializeField] public PatternData pattern { get; private set; }
    [field: SerializeField] public int damage { get; private set; }
    [field: SerializeField] public Condition unlockCondition { get; private set; }
    
    [field: SerializeField] public int selfDamage { get; private set; }

    public bool IsUnlock(EntityDataInstance[,] gridToCheck)
    {
        return unlockCondition?.IsConditionComplete(gridToCheck) ?? true;
    }
}