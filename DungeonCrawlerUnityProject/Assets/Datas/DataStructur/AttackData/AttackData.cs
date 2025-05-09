using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Attack")]
public class AttackData : ScriptableObject
{
    [field: SerializeField] public string attackName { get; private set; }
    [field: SerializeField] public AttackStageData[] attackStages { get; private set; }

    [field: Header("Grid side where conditions are check"), SerializeField]
    public FightManager.TurnState gridToCheck { get; private set; }
    
    [field: Header("Grid side where attack must be apply"), SerializeField] 
    public FightManager.TurnState gridToApply { get; private set; }
    
    [field: SerializeField] public bool isPositionLocked { get; private set; }
    
}
