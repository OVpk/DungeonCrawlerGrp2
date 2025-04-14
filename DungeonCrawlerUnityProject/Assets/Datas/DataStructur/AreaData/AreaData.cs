using UnityEngine;

public abstract class AreaData : ScriptableObject
{
    public enum AreaTypes
    {
        FightArea,
        ShopArea,
        LootArea,
        GamblingArea
    }
    public abstract AreaTypes areaType { get;}
    
    [field:SerializeField] public string areaName { get; private set; }
    
    [field : Header("Common Area values in map")]
    [field:SerializeField] public Sprite iconDistance1 { get; private set; }
    [field:SerializeField] public Sprite iconDistance2 { get; private set; }
    [field:SerializeField] public Sprite iconDistance3 { get; private set; }
    [field:SerializeField] public string description { get; private set; }
    
    [field : Header("Common Area values in scene")]
    [field:SerializeField] public Sprite background { get; private set; }
}