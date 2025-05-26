using System;
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

    
    public List<CandyPackData> purchasedCandyPacks = new List<CandyPackData>();
    private List<ShopItemUI> itemUIs = new List<ShopItemUI>();

    private CandyPack[] instances;
    private int columns = 3;
    private int rows = 2;
    private int selX = 0;
    private int selY = 0;

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

    
    void Start()
    {
        //InitShop();
        UpdateMoneyUI();
        PositionCursor();
        errorText.gameObject.SetActive(false);
    }

    /*
    void InitShop()
    {
        int count = shopAreaData.articalShopData.Length;
        instances = new CandyPackDataInstance[count];

        for (int i = 0; i < count; i++)
        {
            var article = shopAreaData.articalShopData[i];
            instances[i] = article.candyPack.Instance();

            var go = Instantiate(shopItemPrefab, gridParent);
            var ui = go.GetComponent<ShopItemUI>();
            ui.index = i;
            ui.Setup(instances[i].sprite, article.price, instances[i].currentStock > 0);
            itemUIs.Add(ui);
        }
    }
    */

    public void MoveSelector(int dx, int dy)
    {
        selX = Mathf.Clamp(selX + dx, 0, columns - 1);
        selY = Mathf.Clamp(selY + dy, 0, rows - 1);
        PositionCursor();
    }

    void PositionCursor()
    {
        int idx = selY * columns + selX;
        if (idx < itemUIs.Count)
        {
            selectorCursor.position = itemUIs[idx].transform.position;
        }
    }

    public void TryPurchase()
    {
        int idx = selY * columns + selX;
        if (idx >= instances.Length) return;

    //    var article = shopAreaData.articalShopData[idx];
        var inst = instances[idx];
        var ui   = itemUIs[idx];

        if (!ui.isAvailable)
        {
            ShowError("Article épuisé.");
            return;
        }

    //    if (GameManager.Instance.money < article.price)
        {
            ShowError("Pas assez d'argent !");
            return;
        }


    //    GameManager.Instance.money -= article.price;
        inst.currentStock--;
        ui.SetSoldOut();
        UpdateMoneyUI();
        errorText.gameObject.SetActive(false);

    //    purchasedCandyPacks.Add(article.candyPack);
    //    Debug.Log($"Pack acheté : {article.candyPack.name} (total possédés : {purchasedCandyPacks.Count})");
    }

    void UpdateMoneyUI()
    {
        moneyText.text = GameManager.Instance.money.ToString();
    }

    void ShowError(string msg)
    {
        errorText.text = msg;
        errorText.gameObject.SetActive(true);
    }
}
