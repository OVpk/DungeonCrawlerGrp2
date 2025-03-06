using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData : ScriptableObject
{
    [field : Header("Boss values")]
    [field:SerializeField] public string bossName { get; private set; }
    
}