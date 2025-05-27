using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image candyImage;
    public TMP_Text priceText;
    public GameObject soldOutOverlay;

    [HideInInspector] public int index;
    [HideInInspector] public bool isAvailable;
    [HideInInspector] public ArticalShopData associatedArticle; // Nouvelle propriété

    public void Setup(Sprite sprite, int price, ArticalShopData article, bool available = true)
    {
        candyImage.sprite = sprite;
        priceText.text = price.ToString();
        isAvailable = available;
        associatedArticle = article; // Associe l'article
        soldOutOverlay.SetActive(!available);

        var tint = available ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        candyImage.color = tint;
        priceText.color = tint;
    }

    public void SetSoldOut()
    {
        isAvailable = false;
        soldOutOverlay.SetActive(true);
        candyImage.color = new Color(1f, 1f, 1f, 0.5f);
        priceText.color = new Color(1f, 1f, 1f, 0.5f);
    }
}
