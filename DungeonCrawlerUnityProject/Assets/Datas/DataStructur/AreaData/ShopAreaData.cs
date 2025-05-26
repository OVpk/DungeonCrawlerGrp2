using UnityEngine;

[CreateAssetMenu(menuName = "Data/Area/ShopArea")]
public class ShopAreaData : AreaData
{
    public override AreaTypes areaType => AreaTypes.ShopArea;
    
    [field: SerializeField] public ArticalShopData[] articalShopData { get; private set; }
}