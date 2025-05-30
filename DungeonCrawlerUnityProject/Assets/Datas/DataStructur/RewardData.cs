using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Reward")]
public class RewardData : ScriptableObject
{
    public enum RewardType
    {
        Candy,
        Money
    }
    [field: SerializeField] public RewardType rewardType { get; private set; }
    
    [field: SerializeField] public int nbOfCandy { get; private set; }
    [field: SerializeField] public int money { get; private set; }
}
