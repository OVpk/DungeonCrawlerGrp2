using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/CandyPack")]
public class CandyPackData : ScriptableObject
{
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public CharacterData candyData { get; private set; }
    
    [field: SerializeField] public int stock { get; private set; }

    public CandyPackDataInstance Instance()
    {
        return new CandyPackDataInstance(this);
    }
}

public class CandyPackDataInstance
{
    public Sprite sprite;
    public CharacterData candyData;
    public int maxStock { get; private set; }
    public int currentStock;

    public CandyPackDataInstance (CandyPackData data)
    {
        candyData = data.candyData;
        maxStock = data.stock;
        currentStock = data.stock;
        sprite = data.sprite;
    }
}
