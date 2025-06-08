using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [field: SerializeField] public Sprite visualInShop { get; private set; }
    [field: SerializeField] public string descriptionInShop { get; private set; }
}
