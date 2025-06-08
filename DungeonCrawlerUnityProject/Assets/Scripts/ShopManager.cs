using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TMP_Text moneyText;

    public List<ShopItemUI> itemUIs = new List<ShopItemUI>();
    public List<ArticalShopData> allShopItems = new List<ArticalShopData>();
    private List<ArticalShopData> availableShopItems = new List<ArticalShopData>();
    private int selX = 0;

    public TMP_Text descriptionShop;

    public StockDisplayer stockDisplayer;

    private const int MaxDisplayedItems = 4;

    public static ShopManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void InitShop()
    {
        stockDisplayer.RefreshDisplay();
        FilterAndDisplayItems();
        UpdateMoneyUI();
        PositionCursor();
    }

    private int currentRefillArticleNb = 0;
    public int maxRefillArticleNb = 2;

    private void FilterAndDisplayItems()
    {
        // Filtrer les items disponibles
        availableShopItems = new List<ArticalShopData>();
        currentRefillArticleNb = 0;
        allShopItems = ShuffleList(allShopItems);

        foreach (var article in allShopItems)
        {
            if (availableShopItems.Count >= MaxDisplayedItems)
                break;

            if (IsItemAvailable(article))
            {
                if (article.item is PackRefillData)
                {
                    if (currentRefillArticleNb >= maxRefillArticleNb) continue;
                    currentRefillArticleNb++;
                    availableShopItems.Add(article);
                    continue;
                }
                availableShopItems.Add(article);
            }
        }

        for (var i = 0; i < itemUIs.Count; i++)
        {
            itemUIs[i].Setup(availableShopItems[i].item.visualInShop, availableShopItems[i].price, availableShopItems[i], true);
        }
    }

    private bool IsItemAvailable(ArticalShopData article)
    {
        if (article.item is CandyPackData candy)
        {
            // Vérifie si le joueur ne possède pas déjà ce CandyPack
            var pack = GameManager.Instance.candyPacks.Find(p => p.data == candy);
            if (pack != null)
                return false;

            // Vérifie si le CandyPack requis est possédé
            if (candy.necessaryPackToSeeThisOneInShop != null)
            {
                var requiredPack = GameManager.Instance.candyPacks.Find(p => p.data == candy.necessaryPackToSeeThisOneInShop);
                if (requiredPack == null)
                    return false;
            }

            return true;
        }

        if (article.item is PackUpgradeData upgrade)
        {
            // Disponible si le joueur possède le CandyPack à upgrader, et si l'upgrade n'a pas encore été appliquée
            var pack = GameManager.Instance.candyPacks.Find(p => p.data == upgrade.packToUpgrade);
            if (pack == null) return false;
            return !pack.usedUpgrades.Contains(upgrade) 
                   && !availableShopItems.Exists(a => a.item is PackUpgradeData upg && upg.packToUpgrade == upgrade.packToUpgrade)
                   && (upgrade.necessaryUpgradeToSeeThisOneInShop == null || pack.usedUpgrades.Contains(upgrade.necessaryUpgradeToSeeThisOneInShop));
        }

        if (article.item is PackRefillData refill)
        {
            // Disponible si le joueur possède le CandyPack à recharger
            var pack = GameManager.Instance.candyPacks.Find(p => p.data == refill.packToRefill);
            return pack != null;
        }

        return false;
    }


    public void MoveSelector(int dx, int dy)
    {
        int movementY = selX + (dy * 2) >= 0 && selX + (dy * 2) < itemUIs.Count ? (dy * 2) : 0;
        selX = Mathf.Clamp(selX + dx + movementY, 0, itemUIs.Count - 1);

        if (selX < itemUIs.Count)
        {
            PositionCursor();
        }
    }

    public Image displayedCursor;

    private void PositionCursor()
    {
        displayedCursor.transform.position = itemUIs[selX].transform.position;
        descriptionShop.text = availableShopItems[selX].item.descriptionInShop;
    }

    public void TryPurchase()
    {
        if (selX >= availableShopItems.Count) return;

        var article = availableShopItems[selX];

        // Vérifie l'argent
        if (GameManager.Instance.money < article.price)
        {
            return;
        }

        // Réduit l'argent du joueur
        GameManager.Instance.money -= article.price;
        ApplyItemEffect(article.item);
        UpdateMoneyUI();
        stockDisplayer.RefreshDisplay();

        // Marquer l'article comme "Sold Out"
        var ui = itemUIs[selX];
        ui.SetSoldOut();
    }


    private void ApplyItemEffect(ItemData item)
    {
        if (item is CandyPackData candy)
        {
            GameManager.Instance.candyPacks.Add(new CandyPack(candy));
        }
        else if (item is PackUpgradeData upgrade)
        {
            var pack = GameManager.Instance.candyPacks.Find(p => p.data == upgrade.packToUpgrade);
            if (pack != null)
            {
                pack.ApplyUpgrade(upgrade);
            }
        }
        else if (item is PackRefillData refill)
        {
            var pack = GameManager.Instance.candyPacks.Find(p => p.data == refill.packToRefill);
            if (pack != null)
            {
                pack.currentStock += refill.nbToRefill;
                if (pack.currentStock > pack.maxStock)
                    pack.currentStock = pack.maxStock;
            }
        }
    }

    private void UpdateMoneyUI()
    {
        moneyText.text = GameManager.Instance.money.ToString();
    }

    public void ExitShop()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.InOverWorld);
        ExplorationManager.Instance.SetDisplay();
    }
    
    public static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> newList = new List<T>();
        while (list.Count > 0)
        {
            var rnd = list[Random.Range(0, list.Count)];
            newList.Add(rnd);
            list.Remove(rnd);
        }
        return newList;
    }
}
