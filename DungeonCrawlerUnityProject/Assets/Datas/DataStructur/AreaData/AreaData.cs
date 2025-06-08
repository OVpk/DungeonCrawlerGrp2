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
    public Sprite iconDistance1 { get; private set; }
    public Sprite iconDistance2 { get; private set; }
    public Sprite iconDistance3 { get; private set; }
    public string description { get; private set; }
    
}