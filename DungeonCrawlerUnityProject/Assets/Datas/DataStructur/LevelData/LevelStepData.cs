using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level/LevelStep")]
public class LevelStepData : AbstractLevelData
{
    [field:SerializeField] public FightAreaData[] availableFightAreas { get; private set; }
    [field:SerializeField] public ShopAreaData[] availableShopAreas { get; private set; }
    [field:SerializeField] public GamblingAreaData[] availableGamblingAreas { get; private set; }
    [field:SerializeField] public LootAreaData[] availableLootAreas { get; private set; }
}
