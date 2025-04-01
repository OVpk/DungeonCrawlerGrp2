using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    public enum EntityTypes
    {
        
    }
    
    [field: Header("Common Entity values"), SerializeField]
    public string entityName { get; private set; }
    [field: SerializeField] public int durability { get; private set; }
    
    [field: SerializeField] public EntityTypes type{ get; private set; }
    
    [field: SerializeField] public Sprite sprite{ get; private set; }
    public abstract EntityDataInstance Instance();

} 

public class EntityDataInstance
{
    public string name;
    public int durability;
    public EntityData.EntityTypes type;
    public Sprite sprite;

    public EntityDataInstance(EntityData data)
    {
        name = data.name;
        durability = data.durability;
        type = data.type;
        sprite = data.sprite;
    }
}