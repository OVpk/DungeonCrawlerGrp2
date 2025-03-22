using UnityEngine;

[CreateAssetMenu(menuName = "Data/Area/LootArea")]
public class LootAreaData : AreaData
{
    public override AreaTypes areaType => AreaTypes.LootArea;
    
}