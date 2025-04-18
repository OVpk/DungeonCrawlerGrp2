using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/AttackStageData")]
public class AttackStageData : ScriptableObject
{
    [field: SerializeField] public PatternData pattern { get; private set; }
    [field: SerializeField] public int damage { get; private set; }
    [field: SerializeField] public Condition unlockCondition { get; private set; }
    
    [field: SerializeField] public int selfDamage { get; private set; }

    public bool IsUnlock(CharacterDataInstance[,] playerGrid, EnemyDataInstance[,] enemyGrid)
    {
        return unlockCondition?.IsConditionComplete(playerGrid, enemyGrid) ?? true;
    }
}