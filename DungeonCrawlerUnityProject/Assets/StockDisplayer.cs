using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockDisplayer : MonoBehaviour
{
    public Image tagadaStockImg;
    public Image carambarStockImg;
    public Image malabarStockImg;
    public TMP_Text tagadaStock;
    public TMP_Text carambarStock;
    public TMP_Text malabarStock;
    public CharacterData tagada;
    public CharacterData carambar;
    public CharacterData malabar;

    public void RefreshDisplay()
    {
        tagadaStockImg.gameObject.SetActive(false);
        carambarStockImg.gameObject.SetActive(false);
        malabarStockImg.gameObject.SetActive(false);
        foreach (var candyPack in GameManager.Instance.candyPacks)
        {
            if (candyPack.data.candyData == tagada)
            {
                tagadaStockImg.gameObject.SetActive(true);
                tagadaStock.text = candyPack.currentStock + "/" + candyPack.maxStock;
            }
            if (candyPack.data.candyData == carambar)
            {
                carambarStockImg.gameObject.SetActive(true);
                carambarStock.text = candyPack.currentStock + "/" + candyPack.maxStock;
            }
            if (candyPack.data.candyData == malabar)
            {
                malabarStockImg.gameObject.SetActive(true);
                malabarStock.text = candyPack.currentStock + "/" + candyPack.maxStock;
            }
        }
    }
}
