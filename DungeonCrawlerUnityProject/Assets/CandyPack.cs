using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyPack
{
    public CandyPackData data { get; private set; }
    public int currentStock;
    public int maxStock { get; private set; }
    public HashSet<PackUpgradeData> usedUpgrades { get; private set; } = new HashSet<PackUpgradeData>();
    
    public CandyPack(CandyPackData data)
    {
        this.data = data;
        maxStock = data.stock;
        currentStock = data.stock;
    }

    public void ApplyUpgrade(PackUpgradeData upgrade)
    {
        maxStock = upgrade.newMaxStock;
        usedUpgrades.Add(upgrade);
    }
}

