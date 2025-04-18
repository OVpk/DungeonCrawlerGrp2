using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Entity/Character")]
public class CharacterData : EntityData
{
    [field: SerializeField] public CharacterData nextLayer { get; private set; }
    
    public override EntityDataInstance Instance()
    {
        return new CharacterDataInstance(this);
    }
}

public class CharacterDataInstance : EntityDataInstance
{
    public CharacterData nextLayer;

    public CharacterDataInstance(CharacterData data) : base(data)
    {
        nextLayer = data.nextLayer;
    }
}