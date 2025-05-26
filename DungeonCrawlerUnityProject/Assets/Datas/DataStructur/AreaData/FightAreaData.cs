using UnityEngine;

[CreateAssetMenu(menuName = "Data/Area/FightArea")]
public class FightAreaData : AreaData
{
    public override AreaTypes areaType => AreaTypes.FightArea;
    
    [field: SerializeField] public EnemyGridData enemyGrid { get; private set; }
    
}