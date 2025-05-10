using System.Collections.Generic;
using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    public enum EntityTypes
    {
        Mou,
        Dur
    }
    
    public enum EntityEffects
    {
        Empty,
        Glue,
        ProtectedHorizontaly,
        ProtectedVerticaly,
        Explosive,
        Protector,
        Spawner
    }

    [field: SerializeField] public string entityName { get; private set; }
    [field: SerializeField] public int durability { get; private set; }
    
    [field: SerializeField] public EntityTypes type{ get; private set; }
    
    [SerializeField] 
    private EntityEffects[] effects;
    public EntityEffects[] Effects => effects;

    [ShowIfEffect(nameof(effects), EntityEffects.Explosive), SerializeField]
    private int percentOfChanceToGiveExplosive;
    public int PercentOfChanceToGiveExplosive => percentOfChanceToGiveExplosive;
    
    [ShowIfEffect(nameof(effects), EntityEffects.Explosive), SerializeField]
    private int explosionDamages;
    public int ExplosionDamages => explosionDamages;
    
    [field: SerializeField] public bool isImmuneToExplosions{ get; private set; }
    
    [field: SerializeField] public Sprite sprite{ get; private set; }
    [field: SerializeField] public AnimatorOverrideController animator{ get; private set; }
    
    [field: SerializeField] public AttackData[] attacks { get; private set; }
    
    public abstract EntityDataInstance Instance();

} 

public class EntityDataInstance
{        
    public string name;
    public int durability;
    public EntityData.EntityTypes type;
    public Sprite sprite;
    public HashSet<EntityData.EntityEffects> effects = new HashSet<EntityData.EntityEffects>();
    public AnimatorOverrideController animator;
    public AttackData[] attacks;
    public bool isImmuneToExplosions;
    
    public int percentOfChanceToGiveExplosive;
    public int explosionDamages;

    public EntityDataInstance(EntityData data)
    {
        name = data.entityName;
        durability = data.durability;
        type = data.type;
        sprite = data.sprite;
        animator = data.animator;
        attacks = data.attacks;
        isImmuneToExplosions = data.isImmuneToExplosions;
        
        if (data.Effects != null)
            foreach (var effect in data.Effects)
                AddEffect(effect);
        

        percentOfChanceToGiveExplosive = data.PercentOfChanceToGiveExplosive;
        explosionDamages = data.ExplosionDamages;
    }

    public int nbTurnBeforeGlueGone;

    public void AddEffect(EntityData.EntityEffects effect)
    {
        effects.Add(effect);
    }
    

}