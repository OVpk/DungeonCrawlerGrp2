using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/AttackStageData")]
public class AttackStageData : ScriptableObject
{
    [field: SerializeField] public string effectDescription { get; private set; }
    [field: SerializeField] public PatternData pattern { get; private set; }
    [field: SerializeField] public int damage { get; private set; }
    
    [SerializeField]
    private EntityData.EntityEffects effect;
    public EntityData.EntityEffects Effect => effect;

    [ShowIfEffect(nameof(effect), EntityData.EntityEffects.Spawner), SerializeField]
    private EntityData entityToSpawn;
    public EntityData EntityToSpawn => entityToSpawn;
    
    [ShowIfEffect(nameof(effect), EntityData.EntityEffects.Glue), SerializeField]
    private int percentOfChanceOfGlue;
    public int PercentOfChanceOfGlue => percentOfChanceOfGlue;
    
    [ShowIfEffect(nameof(effect), EntityData.EntityEffects.Glue), SerializeField]
    private int glueDurability;
    public int GlueDurability => glueDurability;
    
    [ShowIfEffect(nameof(effect), EntityData.EntityEffects.ProtectedHorizontaly, EntityData.EntityEffects.ProtectedVerticaly), SerializeField]
    private int bubbleDurability;
    public int BubbleDurability => bubbleDurability;
    
    [field: SerializeField] public Condition unlockCondition { get; private set; }
    
    [field: SerializeField] public int selfDamage { get; private set; }

    public bool IsUnlock(EntityDataInstance[,] gridToCheck)
    {
        return unlockCondition?.IsConditionComplete(gridToCheck) ?? true;
    }
}