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
    private int nbOfTurnBeforeGlueGone;
    public int NbOfTurnBeforeGlueGone => nbOfTurnBeforeGlueGone;
    
    [ShowIfEffect(nameof(effect), EntityData.EntityEffects.Fog), SerializeField]
    private int percentOfChanceOfAvoidingAttackThanksToFog;
    public int PercentOfChanceOfAvoidingAttackThanksToFog => percentOfChanceOfAvoidingAttackThanksToFog;
    
    [ShowIfEffect(nameof(effect), EntityData.EntityEffects.Fog), SerializeField]
    private int nbOfTurnBeforeFogGone;
    public int NbOfTurnBeforeFogGone => nbOfTurnBeforeFogGone;
    
    [field: SerializeField] public Condition unlockCondition { get; private set; }
    
    [field: SerializeField] public int selfDamage { get; private set; }

    public bool IsUnlock(EntityDataInstance[,] gridToCheck)
    {
        return unlockCondition?.IsConditionComplete(gridToCheck) ?? true;
    }
}