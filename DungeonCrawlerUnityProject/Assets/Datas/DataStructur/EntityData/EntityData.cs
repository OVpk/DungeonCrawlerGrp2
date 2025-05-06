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
        Protector
    }

    [field: SerializeField] public string entityName { get; private set; }
    [field: SerializeField] public int durability { get; private set; }
    
    [field: SerializeField] public EntityTypes type{ get; private set; }
    
    [field: SerializeField] public EntityEffects[] effects{ get; private set; }

    [ShowIfEffect(nameof(effects), EntityEffects.Explosive), SerializeField]
    private int percentOfChangeToGiveExplosive;

    public int PercentOfChangeToGiveExplosive => percentOfChangeToGiveExplosive;
    
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
    
    public int percentOfChangeToGiveExplosive;

    public EntityDataInstance(EntityData data)
    {
        name = data.entityName;
        durability = data.durability;
        type = data.type;
        sprite = data.sprite;
        animator = data.animator;
        attacks = data.attacks;
        isImmuneToExplosions = data.isImmuneToExplosions;
        foreach (var effect in data.effects)
        {
            AddEffect(effect);
        }

        percentOfChangeToGiveExplosive = data.PercentOfChangeToGiveExplosive;
    }

    public int nbTurnBeforeGlueGone;

    public void UpdateEffects()
    {
        if (effects.Contains(EntityData.EntityEffects.Glue)) nbTurnBeforeGlueGone--;
    }

    public void AddEffect(EntityData.EntityEffects effect)
    {
        effects.Add(effect);
        switch (effect)
        {
            case EntityData.EntityEffects.Glue : InitGlueEffect(); break;
        }
    }
    
    private void InitGlueEffect()
    {
        nbTurnBeforeGlueGone = FightManager.Instance.nbTurnBeforeEntityGlueGone;
        effects.Add(EntityData.EntityEffects.Glue);
    }
}