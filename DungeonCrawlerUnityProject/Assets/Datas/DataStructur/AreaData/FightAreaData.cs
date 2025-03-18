using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Area/FightArea")]
public class FightAreaData : AreaData
{
    [field : Header("Specific Fight Area values")]
    [field:SerializeField] public BossData boss { get; private set; }
    
}