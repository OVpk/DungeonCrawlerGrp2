// ComplexCarousel.cs
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ComplexCarousel : MonoBehaviour
{
    [Header("Data")]
    public List<AttackData> attacks =new List<AttackData>();

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

    public int currentAttack = 0;
    public int currentAttackStage = 0;
    private int bestAttackStageIndex = 0;
    private bool isHorizontalAnimating = false;
    private bool isVerticalAnimating = false;

    private Vector2 leftCardPos, centerCardPos, rightCardPos;
    private Vector2 topEntryPos, centerEntryPos, bottomEntryPos;


    private Color actualStageColor = new Color(0.6549f, 1f, 0.4863f);
    private Color obsoleteStageColor = new Color(0.6039f, 0.6039f, 0.6039f);
    private Color nextStageColor = new Color(1f, 1f, 1f);

    public void LoadData(AttackData[] attacksData)
    {
        currentAttack = 0;
        attacks.Clear();
        foreach (var attackData in attacksData)
        {
            attacks.Add(attackData);
        }
        SetBestStageIndex();
        currentAttackStage = bestAttackStageIndex >= 0 ? bestAttackStageIndex : 0;
        UpdateDisplay();
    }

    public void FixStageToBest()
    {
        SetBestStageIndex();
        currentAttackStage = bestAttackStageIndex >= 0 ? bestAttackStageIndex : 0;
        UpdateDisplay();
    }

    private void SetBestStageIndex()
    {
        AttackStageData bestAttackStage = FightManager.Instance.FindBestUnlockedStage(attacks[currentAttack]);
        if (bestAttackStage == null)
        {
            bestAttackStageIndex = -1;
            return;
        }
        for (int i = 0; i < attacks[currentAttack].attackStages.Length; i++)
        {
            if (attacks[currentAttack].attackStages[i] == bestAttackStage)
            {
                bestAttackStageIndex = i;
                return;
            }
        }
    }

    void Start()
    {
        // Cache positions
        leftCardPos   = leftCard.anchoredPosition;
        centerCardPos = centerCard.anchoredPosition;
        rightCardPos  = rightCard.anchoredPosition;

        topEntryPos    = topEntry.anchoredPosition;
        centerEntryPos = centerEntry.anchoredPosition;
        bottomEntryPos = bottomEntry.anchoredPosition;
    }

    public void ScrollHorizontal(int dir)
    {
        isHorizontalAnimating = true;
        int newCard = (currentAttack + dir).Mod(attacks.Count);

        Vector2 leftTarget   = dir > 0 ? centerCardPos : rightCardPos;
        Vector2 centerTarget = dir > 0 ? rightCardPos  : leftCardPos;
        Vector2 rightTarget  = dir > 0 ? leftCardPos   : centerCardPos;
        
        currentAttack = newCard;
        
        SetBestStageIndex();
        currentAttackStage = bestAttackStageIndex >= 0 ? bestAttackStageIndex : 0;

        var seq = DOTween.Sequence();
        seq.Join(leftCard.DOAnchorPos(leftTarget, horizontalDuration).SetEase(horizontalEase));
        seq.Join(centerCard.DOAnchorPos(centerTarget, horizontalDuration).SetEase(horizontalEase));
        seq.Join(rightCard.DOAnchorPos(rightTarget, horizontalDuration).SetEase(horizontalEase));

        seq.OnComplete(() =>
        {
            ResetHorizontalPositions();
            UpdateDisplay();
            isHorizontalAnimating = false;
        });
    }

    public void ScrollVertical(int dir)
    {
        if (attacks[currentAttack].attackStages.Length < 2) return;
        isVerticalAnimating = true;
        int newEntry = currentAttackStage + dir;
        
        if (newEntry < 0 || newEntry >= attacks[currentAttack].attackStages.Length) return;
        
        currentAttackStage = newEntry;

        Vector2 topTarget    = dir > 0 ? topEntryPos    : centerEntryPos;
        Vector2 centerTarget = dir > 0 ? centerEntryPos : bottomEntryPos;
        Vector2 bottomTarget = dir > 0 ? bottomEntryPos : topEntryPos;

        var seq = DOTween.Sequence();
        seq.Join(topEntry.DOAnchorPos(topTarget, verticalDuration).SetEase(verticalEase));
        seq.Join(centerEntry.DOAnchorPos(centerTarget, verticalDuration).SetEase(verticalEase));
        seq.Join(bottomEntry.DOAnchorPos(bottomTarget, verticalDuration).SetEase(verticalEase));

        seq.OnComplete(() =>
        {
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
        var attack = attacks[currentAttack];
        centerMainText.text = attack.attackName;
        // Update center entry text
        var attackStage = attack.attackStages[currentAttackStage];

        if (currentAttackStage == bestAttackStageIndex)
        {
            centerEntry.gameObject.GetComponent<Image>().color = actualStageColor;
        }
        else if (currentAttackStage < bestAttackStageIndex)
        {
            centerEntry.gameObject.GetComponent<Image>().color = obsoleteStageColor;
        }
        else if (currentAttackStage > bestAttackStageIndex)
        {
            centerEntry.gameObject.GetComponent<Image>().color = nextStageColor;
        }

        if (currentAttackStage == 0)
        {
            bottomEntry.gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            bottomEntry.gameObject.GetComponent<Image>().enabled = true;
        }

        if (currentAttackStage == attack.attackStages.Length-1)
        {
            topEntry.gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            topEntry.gameObject.GetComponent<Image>().enabled = true;
        }
        
        if (attackStage.unlockCondition == null)
        {
            centerConditionText.text = "X";
        }
        else
        {
            centerConditionText.text = attackStage.unlockCondition.description;
        }
        centerResultText.text    = attackStage.effectDescription;
    }
}
