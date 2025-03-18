using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Area/ShopArea")]
public class ShopAreaData : AreaData
{
    [field : Header("Specific Shop Area values")]
    [field:SerializeField] public CandyPackData[] availablePacks { get; private set; }
    
}