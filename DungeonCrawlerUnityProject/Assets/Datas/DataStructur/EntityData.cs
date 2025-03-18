using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    public enum EntityType
    {
        
    }
    
    [field: Header("Stats"), SerializeField]
    public string entityName { get; private set; }
    [field: SerializeField] public int durability { get; private set; }
    
    [field: SerializeField] public int speed { get; private set; }
    [field: SerializeField] public EntityType type{ get; private set; }
    
    [field: SerializeField] public Sprite sprite{ get; private set; }
    public abstract EntityDataInstance Instance();

} 

public class EntityDataInstance
{
    public string name;
    public int durability;
    public EntityData.EntityType type;
    public Sprite sprite;
    public int speed;

    public EntityDataInstance(EntityData data)
    {
        name = data.name;
        durability = data.durability;
        type = data.type;
        sprite = data.sprite;
        speed = data.speed;
    }
}