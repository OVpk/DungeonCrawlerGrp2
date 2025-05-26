using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Item/PackUpgrade")]
public class PackUpgradeData : ItemData
{
    [field: SerializeField] public CandyPackData packToUpgrade { get; private set; }
    [field: SerializeField] public int newMaxStock { get; private set; }
}
