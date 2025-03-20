using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLevelData : ScriptableObject
{
    [field:SerializeField] public int height { get; protected set; }
    [field:SerializeField] public int width { get; protected set; }
}
