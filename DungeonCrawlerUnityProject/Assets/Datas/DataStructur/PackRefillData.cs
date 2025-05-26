using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackRefillData : ItemData
{
    [field: SerializeField] public CandyPackData packToRefill { get; private set; }
    [field: SerializeField] public int nbToRefill { get; private set; }
}
