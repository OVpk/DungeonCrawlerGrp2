using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLevelData : ScriptableObject
{
    [SerializeField] protected int height { get; set; }
    [SerializeField] protected int width { get; set; }
}
