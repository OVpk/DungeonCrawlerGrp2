using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image candyImage;
    public TMP_Text priceText;

    [HideInInspector] public int index;
    [HideInInspector] public bool isAvailable;
    [HideInInspector] public ArticalShopData associatedArticle; // Nouvelle propriété

    public void Setup(Sprite sprite, int price, ArticalShopData article, bool available = true)
    {
        candyImage.sprite = sprite;
        priceText.text = price.ToString();
        isAvailable = available;
        associatedArticle = article; // Associe l'article

        candyImage.color = available ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        priceText.color = available ? Color.black : new Color(1f, 1f, 1f, 0f);
    }

    public void SetSoldOut()
    {
        isAvailable = false;
        candyImage.color = new Color(1f, 1f, 1f, 0.5f);
        priceText.color = new Color(1f, 1f, 1f, 0.5f);
    }
}
