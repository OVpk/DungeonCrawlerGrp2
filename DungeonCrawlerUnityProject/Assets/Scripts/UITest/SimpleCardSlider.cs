using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleCardSlider : MonoBehaviour
{
    [Header("Liste des CandyPacks")]  
    public List<CandyPack> packs = new List<CandyPack>();

    [Header("UI Images")]
    public Image leftImage;
    public Image centerImage;
    public Image rightImage;

    [Header("Texte de stock du pack central")]
    public TextMeshProUGUI stockText;

    private Vector2 leftPos, centerPos, rightPos;
    private Vector3 imageScale;
    public int currentIndex = 0;
    private bool isAnimating = false;
    public float animDuration = 0.5f;

    void Start()
    {
        // Position initiale
        leftPos = leftImage.rectTransform.anchoredPosition;
        centerPos = centerImage.rectTransform.anchoredPosition;
        rightPos = rightImage.rectTransform.anchoredPosition;
        imageScale = Vector3.one;
    }

    public void UpdateDisplay()
    {
        int leftIdx = Modulo(currentIndex - 1, packs.Count);
        int centerIdx = Modulo(currentIndex, packs.Count);
        int rightIdx = Modulo(currentIndex + 1, packs.Count);

        // Met à jour les sprites
        leftImage.sprite = packs[leftIdx].data.sprite;
        centerImage.sprite = packs[centerIdx].data.sprite;
        rightImage.sprite = packs[rightIdx].data.sprite;
        

        // Texte de stock "current / max"
        var data = packs[centerIdx];
        stockText.text = data.currentStock + " / " + data.maxStock;
    }

    public void ScrollLeft()
    {
        if (isAnimating) return;
        isAnimating = true;
        currentIndex = Modulo(currentIndex + 1, packs.Count);

        var seq = DOTween.Sequence();
        seq.Join(centerImage.rectTransform.DOAnchorPos(leftPos, animDuration))
            .Join(rightImage.rectTransform.DOAnchorPos(centerPos, animDuration))
            .Join(leftImage.rectTransform.DOScale(0, animDuration));

        seq.OnComplete(() =>
        {
            UpdateDisplay();
            ResetTransforms();
            isAnimating = false;
        });
    }

    public void ScrollRight()
    {
        if (isAnimating) return;
        isAnimating = true;
        currentIndex = Modulo(currentIndex - 1, packs.Count);

        var seq = DOTween.Sequence();
        seq.Join(leftImage.rectTransform.DOAnchorPos(centerPos, animDuration))
            .Join(centerImage.rectTransform.DOAnchorPos(rightPos, animDuration))
            .Join(rightImage.rectTransform.DOScale(0, animDuration));

        seq.OnComplete(() =>
        {
            UpdateDisplay();
            ResetTransforms();
            isAnimating = false;
        });
    }

    void ResetTransforms()
    {
        leftImage.rectTransform.anchoredPosition = leftPos;
        centerImage.rectTransform.anchoredPosition = centerPos;
        rightImage.rectTransform.anchoredPosition = rightPos;

        leftImage.rectTransform.localScale = imageScale;
        centerImage.rectTransform.localScale = imageScale;
        rightImage.rectTransform.localScale = imageScale;
    }

    int Modulo(int a, int b) => (a % b + b) % b;
}

public static class IntExtensions
{
    public static int Mod(this int a, int m) => (a % m + m) % m;
}