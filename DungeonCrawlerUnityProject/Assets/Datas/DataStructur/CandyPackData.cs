using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Item/CandyPack")]
public class CandyPackData : ItemData
{
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public CharacterData candyData { get; private set; }
    [field: SerializeField] public int stock { get; private set; }
}