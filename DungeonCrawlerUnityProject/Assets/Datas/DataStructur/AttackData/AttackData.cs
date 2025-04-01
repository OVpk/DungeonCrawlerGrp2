using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Attack")]
public class AttackData : ScriptableObject
{
    [field: SerializeField] public PatternData attackPattern { get; private set; }
    
    
}
