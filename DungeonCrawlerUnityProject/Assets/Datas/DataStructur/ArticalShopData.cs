using UnityEngine;

[CreateAssetMenu(menuName = "Data/Shop/ArticleShop")]
public class ArticalShopData : ScriptableObject
{
    [field: SerializeField] public ItemData item { get; private set; }
    [field: SerializeField] public int price { get; private set; }
    
}

