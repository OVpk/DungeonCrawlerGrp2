// ComplexCarousel.cs
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

[Serializable]
public class VerticalEntry
{
    public string conditionText;
    public string resultText;
}

[Serializable]
public class ComplexCard
{
    public string mainText;
    public List<VerticalEntry> verticalEntries;
}

public class ComplexCarousel : MonoBehaviour
{
    [Header("Data")]
    public List<ComplexCard> cards;

    [Header("Horizontal Containers")]
    public RectTransform leftCard;
    public RectTransform centerCard;
    public RectTransform rightCard;

    [Header("Vertical Containers (Center Card)")]
    public RectTransform topEntry;
    public RectTransform centerEntry;
    public RectTransform bottomEntry;

    [Header("Text UI (Center Only)")]
    public TextMeshProUGUI centerMainText;
    public TextMeshProUGUI centerConditionText;
    public TextMeshProUGUI centerResultText;

    [Header("Animation Settings")]
    public float horizontalDuration = 0.4f;
    public Ease horizontalEase = Ease.OutQuad;
    public float verticalDuration = 0.3f;
    public Ease verticalEase = Ease.OutQuad;

    private int currentCard = 0;
    private int currentEntry = 0;
    private bool isHorizontalAnimating = false;
    private bool isVerticalAnimating = false;

    private Vector2 leftCardPos, centerCardPos, rightCardPos;
    private Vector2 topEntryPos, centerEntryPos, bottomEntryPos;

    void Start()
    {
        // Cache positions
        leftCardPos   = leftCard.anchoredPosition;
        centerCardPos = centerCard.anchoredPosition;
        rightCardPos  = rightCard.anchoredPosition;

        topEntryPos    = topEntry.anchoredPosition;
        centerEntryPos = centerEntry.anchoredPosition;
        bottomEntryPos = bottomEntry.anchoredPosition;

        UpdateDisplay();
    }

    private void ScrollHorizontal(int dir)
    {
        isHorizontalAnimating = true;
        int newCard = (currentCard + dir).Mod(cards.Count);

        Vector2 leftTarget   = dir > 0 ? centerCardPos : rightCardPos;
        Vector2 centerTarget = dir > 0 ? rightCardPos  : leftCardPos;
        Vector2 rightTarget  = dir > 0 ? leftCardPos   : centerCardPos;
        
        currentCard = newCard;

        var seq = DOTween.Sequence();
        seq.Join(leftCard.DOAnchorPos(leftTarget, horizontalDuration).SetEase(horizontalEase));
        seq.Join(centerCard.DOAnchorPos(centerTarget, horizontalDuration).SetEase(horizontalEase));
        seq.Join(rightCard.DOAnchorPos(rightTarget, horizontalDuration).SetEase(horizontalEase));

        seq.OnComplete(() =>
        {
            currentEntry = 0; // reset vertical on new card
            ResetHorizontalPositions();
            UpdateDisplay();
            isHorizontalAnimating = false;
        });
    }

    private void ScrollVertical(int dir)
    {
        if (cards[currentCard].verticalEntries.Count < 2) return;
        isVerticalAnimating = true;
        int newEntry = (currentEntry + dir).Mod(cards[currentCard].verticalEntries.Count);

        Vector2 topTarget    = dir > 0 ? topEntryPos    : centerEntryPos;
        Vector2 centerTarget = dir > 0 ? centerEntryPos : bottomEntryPos;
        Vector2 bottomTarget = dir > 0 ? bottomEntryPos : topEntryPos;

        var seq = DOTween.Sequence();
        seq.Join(topEntry.DOAnchorPos(topTarget, verticalDuration).SetEase(verticalEase));
        seq.Join(centerEntry.DOAnchorPos(centerTarget, verticalDuration).SetEase(verticalEase));
        seq.Join(bottomEntry.DOAnchorPos(bottomTarget, verticalDuration).SetEase(verticalEase));

        seq.OnComplete(() =>
        {
            currentEntry = newEntry;
            ResetVerticalPositions();
            UpdateDisplay();
            isVerticalAnimating = false;
        });
    }

    private void ResetHorizontalPositions()
    {
        leftCard.anchoredPosition   = leftCardPos;
        centerCard.anchoredPosition = centerCardPos;
        rightCard.anchoredPosition  = rightCardPos;
    }

    private void ResetVerticalPositions()
    {
        topEntry.anchoredPosition    = topEntryPos;
        centerEntry.anchoredPosition = centerEntryPos;
        bottomEntry.anchoredPosition = bottomEntryPos;
    }

    private void UpdateDisplay()
    {
        // Update center card text
        var card = cards[currentCard];
        centerMainText.text = card.mainText;
        // Update center entry text
        var entry = card.verticalEntries[currentEntry];
        centerConditionText.text = entry.conditionText;
        centerResultText.text    = entry.resultText;
    }
}
