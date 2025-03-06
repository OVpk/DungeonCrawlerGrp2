using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyPackData : ScriptableObject
{
    [field : Header("Candy Pack values")]
    [field:SerializeField] public string packName { get; private set; }
    
}