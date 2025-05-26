using UnityEngine;

[CreateAssetMenu(menuName = "Data/Shop/ArcticalPack")]
public class ArticalShopData : ScriptableObject
{
    [field: SerializeField] public CandyPackData candyPack { get; private set; }
    [field: SerializeField] public int price { get; private set; }
    
}

