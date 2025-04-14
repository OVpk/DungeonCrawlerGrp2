using UnityEngine;

[CreateAssetMenu(menuName = "Fight/Attack")]
public class AttackData : ScriptableObject
{
    [field: SerializeField] public AttackStageData[] attackStages { get; private set; }
}
