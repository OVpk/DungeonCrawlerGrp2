using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Entity/Character")]
public class CharacterData : EntityData
{
    [field: SerializeField] public CharacterData nextLayer { get; private set; }
    
    [field: SerializeField] public Sprite descriptionVisual { get; private set; }
    
    [field: SerializeField] public string description { get; private set; }
    
    public override EntityDataInstance Instance()
    {
        return new CharacterDataInstance(this);
    }
}

public class CharacterDataInstance : EntityDataInstance
{
    public CharacterData nextLayer;

    public Sprite descriptionVisual;

    public string description;

    public CharacterDataInstance(CharacterData data) : base(data)
    {
        nextLayer = data.nextLayer;
        descriptionVisual = data.descriptionVisual;
        description = data.description;
    }
}