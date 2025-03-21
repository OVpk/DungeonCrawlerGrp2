using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaData : ScriptableObject
{
    [field:SerializeField] public string areaName { get; private set; }
    
    [field : Header("Common Area values in map")]
    [field:SerializeField] public Sprite iconDistance1 { get; private set; }
    [field:SerializeField] public Sprite iconDistance2 { get; private set; }
    [field:SerializeField] public Sprite iconDistance3 { get; private set; }
    
    [field : Header("Common Area values in scene")]
    [field:SerializeField] public Sprite background { get; private set; }
}