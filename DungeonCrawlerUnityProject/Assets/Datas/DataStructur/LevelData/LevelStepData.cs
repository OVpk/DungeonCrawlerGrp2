using UnityEngine;

public class LevelStepData : AbstractLevelData
{
    [SerializeField] public FightAreaData[] availableFightAreas { get; private set; }
    [SerializeField] public ShopAreaData[] availableShopAreas { get; private set; }
    [SerializeField] public GamblingAreaData[] availableGamblingAreas { get; private set; }
    [SerializeField] public LootAreaData[] availableLootAreas { get; private set; }
}
