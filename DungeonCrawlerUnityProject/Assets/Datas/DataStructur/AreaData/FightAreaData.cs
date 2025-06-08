using UnityEngine;

[CreateAssetMenu(menuName = "Data/Area/FightArea")]
public class FightAreaData : AreaData
{
    public override AreaTypes areaType => AreaTypes.FightArea;
    
    [field: SerializeField] public EnemyGridData enemyGrid { get; private set; }
    
    [field: SerializeField] public RewardData reward { get; private set; }
    
    [field: SerializeField] public GameObject backgroundInThisFight { get; private set; }
    
    [field: SerializeField] public Sprite explorationBackgroundAfterThisFight { get; private set; }
    
    [field: SerializeField] public Sprite boxVisual { get; private set; }
    
    
    
}