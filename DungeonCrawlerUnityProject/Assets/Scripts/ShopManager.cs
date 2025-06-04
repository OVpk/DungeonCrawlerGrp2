using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject shopItemPrefab;
    public Transform gridParent;
    public RectTransform selectorCursor;
    public TMP_Text moneyText;
    public TMP_Text errorText;

    private List<ShopItemUI> itemUIs = new List<ShopItemUI>();
    public List<ArticalShopData> allShopItems = new List<ArticalShopData>();
    private List<ArticalShopData> availableShopItems = new List<ArticalShopData>();
    private int selX = 0;
    private int selY = 0;

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
        FilterAndDisplayItems();
        UpdateMoneyUI();
        PositionCursor();
        errorText.gameObject.SetActive(false);
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

        // Réinitialiser l'affichage
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
        itemUIs.Clear();

        // Créer l'affichage pour les articles disponibles
        foreach (var article in availableShopItems)
        {
            var go = Instantiate(shopItemPrefab, gridParent);
            var ui = go.GetComponent<ShopItemUI>();
            ui.Setup(article.item.visualInShop, article.price, article, true); // Passe l'article
            itemUIs.Add(ui);
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
        // Calcul des limites basées sur les éléments disponibles
        int columns = Mathf.Min(MaxDisplayedItems, itemUIs.Count); // Nombre de colonnes visibles
        int rows = Mathf.CeilToInt((float)itemUIs.Count / MaxDisplayedItems); // Nombre de lignes

        // Met à jour les indices de sélection
        selX = Mathf.Clamp(selX + dx, 0, columns - 1);
        selY = Mathf.Clamp(selY + dy, 0, rows - 1);

        // Positionne le curseur si l'indice est valide
        int idx = selY * MaxDisplayedItems + selX;
        if (idx < itemUIs.Count)
        {
            PositionCursor();
        }
    }


    private void PositionCursor()
    {
        int idx = selY * MaxDisplayedItems + selX;
        if (idx < itemUIs.Count)
        {
            selectorCursor.position = itemUIs[idx].transform.position;
        }
    }

    public void TryPurchase()
    {
        int idx = selY * MaxDisplayedItems + selX;
        if (idx >= availableShopItems.Count) return;

        var article = availableShopItems[idx];

        // Vérifie l'argent
        if (GameManager.Instance.money < article.price)
        {
            ShowError("Pas assez d'argent !");
            return;
        }

        // Réduit l'argent du joueur
        GameManager.Instance.money -= article.price;
        ApplyItemEffect(article.item);
        UpdateMoneyUI();

        // Marquer l'article comme "Sold Out"
        var ui = itemUIs[idx];
        ui.SetSoldOut();
        errorText.gameObject.SetActive(false);
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

    private void ShowError(string msg)
    {
        errorText.text = msg;
        errorText.gameObject.SetActive(true);
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
