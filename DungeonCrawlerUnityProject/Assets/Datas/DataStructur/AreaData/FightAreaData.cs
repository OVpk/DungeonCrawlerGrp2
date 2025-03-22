using UnityEngine;

[CreateAssetMenu(menuName = "Data/Area/FightArea")]
public class FightAreaData : AreaData
{
    public override AreaTypes areaType => AreaTypes.FightArea;
    
}