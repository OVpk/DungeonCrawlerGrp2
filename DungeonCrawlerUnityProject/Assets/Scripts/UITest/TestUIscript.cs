using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class CardInfo
{
    public Sprite sprite;
    public List<string> texts;
    public string attackName;
}

public class TestUIscript : MonoBehaviour
{
    [Header("Liste des cartes (Sprite + textes + attaque)")]
    public List<CardInfo> cards;

    [Header("Images UI pour afficher les cartes")]
    public Image leftImage;
    public Image centerImage;
    public Image rightImage;

    [Header("TextMeshPro UI et backgrounds pour afficher les textes")]
    public TextMeshProUGUI bottomText;
    public Image            bottomBg;
    public TextMeshProUGUI centerText;
    public Image            centerBg;
    public TextMeshProUGUI topText;
    public Image            topBg;

    [Header("UI pour Nom d'Attaque")]
    public TextMeshProUGUI attackNameText;
    public Image            attackNameBg;

    // position et scale initiaux
    private Vector2 leftPos, centerPos, rightPos;
    private Vector2 bottomTextPos, centerTextPos, topTextPos;
    private Vector2 bottomBgPos, centerBgPos, topBgPos;
    private Vector2 attackNameTextPos, attackNameBgPos;
    private Vector3 bottomTextScale, centerTextScale, topTextScale;
    private Vector3 bottomBgScale, centerBgScale, topBgScale;
    private Vector3 attackNameTextScale, attackNameBgScale;

    // alpha cibler
    private float sideTextAlpha   = 0.5f;
    private float centerTextAlpha = 1f;
    private float leftImgAlpha    = 183f/255f;
    private float centerImgAlpha  = 1f;
    private float rightImgAlpha   = 67f/255f;

    // index courant
    private int firstVisibleIndex     = 0;
    private int firstVisibleTextIndex = 0;
    private bool isAnimating          = false;
    public float animDuration         = 0.5f;

    void Start()
    {
        //validation
        if (cards == null || cards.Count < 3)
        {
            Debug.LogError("3 images sont requises."); return;
        }
        foreach (var c in cards)
        {
            if (c.texts == null || c.texts.Count < 3)
            {
                Debug.LogError("chaque images dois contenir au moins 3 texte."); return;
            }
        }

        // sauvegarde position et scale
        leftPos           = leftImage.rectTransform.anchoredPosition;
        centerPos         = centerImage.rectTransform.anchoredPosition;
        rightPos          = rightImage.rectTransform.anchoredPosition;
        bottomTextPos     = bottomText.rectTransform.anchoredPosition;
        centerTextPos     = centerText.rectTransform.anchoredPosition;
        topTextPos        = topText.rectTransform.anchoredPosition;
        bottomBgPos       = bottomBg.rectTransform.anchoredPosition;
        centerBgPos       = centerBg.rectTransform.anchoredPosition;
        topBgPos          = topBg.rectTransform.anchoredPosition;
        attackNameTextPos = attackNameText.rectTransform.anchoredPosition;
        attackNameBgPos   = attackNameBg.rectTransform.anchoredPosition;

        bottomTextScale      = bottomText.rectTransform.localScale;
        centerTextScale      = centerText.rectTransform.localScale;
        topTextScale         = topText.rectTransform.localScale;
        bottomBgScale        = bottomBg.rectTransform.localScale;
        centerBgScale        = centerBg.rectTransform.localScale;
        topBgScale           = topBg.rectTransform.localScale;
        attackNameTextScale  = attackNameText.rectTransform.localScale;
        attackNameBgScale    = attackNameBg.rectTransform.localScale;

        // fond des texte
        Color grey = new Color(0.5f,0.5f,0.5f,1f);
        bottomBg.color    = grey;
        centerBg.color    = grey;
        topBg.color       = grey;
        attackNameBg.color= grey;

        UpdateDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) ScrollLeft();
        if (Input.GetKeyDown(KeyCode.A)) ScrollRight();
        if (Input.GetKeyDown(KeyCode.W)) ScrollTextUp();
        if (Input.GetKeyDown(KeyCode.S)) ScrollTextDown();
    }

    void UpdateDisplay()
    {
        // sprites
        leftImage.sprite   = cards[Modulo(firstVisibleIndex, cards.Count)].sprite;
        centerImage.sprite = cards[Modulo(firstVisibleIndex+1, cards.Count)].sprite;
        rightImage.sprite  = cards[Modulo(firstVisibleIndex+2, cards.Count)].sprite;
        SetFade(leftImage, leftImgAlpha);
        SetFade(centerImage, centerImgAlpha);
        SetFade(rightImage, rightImgAlpha);

        // textes
        var texts = cards[Modulo(firstVisibleIndex+1, cards.Count)].texts;
        bottomText.text = texts[Modulo(firstVisibleTextIndex, texts.Count)];
        centerText.text = texts[Modulo(firstVisibleTextIndex+1, texts.Count)];
        topText.text    = texts[Modulo(firstVisibleTextIndex+2, texts.Count)];
        SetFade(bottomText, sideTextAlpha);
        SetFade(centerText, centerTextAlpha);
        SetFade(topText,    sideTextAlpha);

        // backgrounds 
        SetFade(bottomBg, 1f);
        SetFade(centerBg, 1f);
        SetFade(topBg,    1f);

        // nom d'attaque
        attackNameText.text = cards[Modulo(firstVisibleIndex+1, cards.Count)].attackName;
        SetFade(attackNameText, centerTextAlpha);
        SetFade(attackNameBg,   1f);
    }

    void SetFade(Graphic g, float a)
    {
        var c = g.color; c.a = a; g.color = c;
    }

    public void ScrollLeft()
    {
        if (isAnimating) return; isAnimating = true;
        var seq = DOTween.Sequence();
        // fade des texte
        seq.Join(bottomText.DOFade(0,animDuration))
           .Join(centerText.DOFade(0,animDuration))
           .Join(topText.DOFade(0,animDuration))
           .Join(bottomBg.DOFade(0,animDuration))
           .Join(centerBg.DOFade(0,animDuration))
           .Join(topBg.DOFade(0,animDuration))
           .Join(attackNameText.DOFade(0,animDuration))
           .Join(attackNameBg.DOFade(0,animDuration));
        // mouvement des sprites
        seq.Join(centerImage.rectTransform.DOAnchorPos(leftPos,animDuration))
           .Join(rightImage.rectTransform.DOAnchorPos(centerPos,animDuration))
           .Join(leftImage.rectTransform.DOScale(0,animDuration));
        seq.OnComplete(()=>{
            firstVisibleIndex = Modulo(firstVisibleIndex+1, cards.Count);
            firstVisibleTextIndex=0;
            UpdateDisplay();
            ResetTransforms();
            // Fade-in texts & bgs
            DOTween.Sequence()
                .Join(bottomText.DOFade(sideTextAlpha,animDuration))
                .Join(centerText.DOFade(centerTextAlpha,animDuration))
                .Join(topText.DOFade(sideTextAlpha,animDuration))
                .Join(bottomBg.DOFade(1,animDuration))
                .Join(centerBg.DOFade(1,animDuration))
                .Join(topBg.DOFade(1,animDuration))
                .Join(attackNameText.DOFade(centerTextAlpha,animDuration))
                .Join(attackNameBg.DOFade(1,animDuration))
                .OnComplete(()=> isAnimating=false);
        });
    }

    public void ScrollRight()
    {
        if (isAnimating) return; isAnimating=true;
        var seq=DOTween.Sequence();
        seq.Join(bottomText.DOFade(0,animDuration))
           .Join(centerText.DOFade(0,animDuration))
           .Join(topText.DOFade(0,animDuration))
           .Join(bottomBg.DOFade(0,animDuration))
           .Join(centerBg.DOFade(0,animDuration))
           .Join(topBg.DOFade(0,animDuration))
           .Join(attackNameText.DOFade(0,animDuration))
           .Join(attackNameBg.DOFade(0,animDuration));
        seq.Join(leftImage.rectTransform.DOAnchorPos(centerPos,animDuration))
           .Join(centerImage.rectTransform.DOAnchorPos(rightPos,animDuration))
           .Join(rightImage.rectTransform.DOScale(0,animDuration));
        seq.OnComplete(()=>{
            firstVisibleIndex=Modulo(firstVisibleIndex-1,cards.Count);
            firstVisibleTextIndex=0;
            UpdateDisplay(); ResetTransforms();
            DOTween.Sequence()
                .Join(bottomText.DOFade(sideTextAlpha,animDuration))
                .Join(centerText.DOFade(centerTextAlpha,animDuration))
                .Join(topText.DOFade(sideTextAlpha,animDuration))
                .Join(bottomBg.DOFade(1,animDuration))
                .Join(centerBg.DOFade(1,animDuration))
                .Join(topBg.DOFade(1,animDuration))
                .Join(attackNameText.DOFade(centerTextAlpha,animDuration))
                .Join(attackNameBg.DOFade(1,animDuration))
                .OnComplete(()=> isAnimating=false);
        });
    }

    public void ScrollTextUp()
    {
        if (isAnimating) return; isAnimating=true;
        var seq=DOTween.Sequence();
        seq.Join(centerText.DOFade(sideTextAlpha,animDuration/2))
           .Append(centerText.DOFade(centerTextAlpha,animDuration/2));
        // mouvement ici
        seq.Join(centerText.rectTransform.DOAnchorPos(bottomTextPos,animDuration))
           .Join(centerBg.rectTransform.DOAnchorPos(bottomBgPos,animDuration))
           .Join(topText.rectTransform.DOAnchorPos(centerTextPos,animDuration))
           .Join(topBg.rectTransform.DOAnchorPos(centerBgPos,animDuration))
           .Join(bottomText.rectTransform.DOScale(0,animDuration))
           .Join(bottomBg.rectTransform.DOScale(0,animDuration));
        seq.OnComplete(()=>{
            firstVisibleTextIndex++;
            var cnt=cards[Modulo(firstVisibleIndex+1,cards.Count)].texts.Count;
            firstVisibleTextIndex=Modulo(firstVisibleTextIndex,cnt);
            UpdateDisplay(); ResetTransforms();
            isAnimating=false;
        });
    }

    public void ScrollTextDown()
    {
        if (isAnimating) return; isAnimating=true;
        var seq=DOTween.Sequence();
        seq.Join(centerText.DOFade(sideTextAlpha,animDuration/2))
           .Append(centerText.DOFade(centerTextAlpha,animDuration/2));
        seq.Join(centerText.rectTransform.DOAnchorPos(topTextPos,animDuration))
           .Join(centerBg.rectTransform.DOAnchorPos(topBgPos,animDuration))
           .Join(bottomText.rectTransform.DOAnchorPos(centerTextPos,animDuration))
           .Join(bottomBg.rectTransform.DOAnchorPos(centerBgPos,animDuration))
           .Join(topText.rectTransform.DOScale(0,animDuration))
           .Join(topBg.rectTransform.DOScale(0,animDuration));
        seq.OnComplete(()=>{
            firstVisibleTextIndex--;
            var cnt=cards[Modulo(firstVisibleIndex+1,cards.Count)].texts.Count;
            firstVisibleTextIndex=Modulo(firstVisibleTextIndex,cnt);
            UpdateDisplay(); ResetTransforms();
            isAnimating=false;
        });
    }

    void ResetTransforms()
    {
        leftImage.rectTransform.anchoredPosition   = leftPos;
        centerImage.rectTransform.anchoredPosition = centerPos;
        rightImage.rectTransform.anchoredPosition  = rightPos;
        leftImage.rectTransform.localScale         = Vector3.one;
        centerImage.rectTransform.localScale       = Vector3.one;
        rightImage.rectTransform.localScale        = Vector3.one;

        bottomText.rectTransform.anchoredPosition  = bottomTextPos;
        centerText.rectTransform.anchoredPosition  = centerTextPos;
        topText.rectTransform.anchoredPosition     = topTextPos;
        bottomBg.rectTransform.anchoredPosition    = bottomBgPos;
        centerBg.rectTransform.anchoredPosition    = centerBgPos;
        topBg.rectTransform.anchoredPosition       = topBgPos;
        attackNameText.rectTransform.anchoredPosition = attackNameTextPos;
        attackNameBg.rectTransform.anchoredPosition   = attackNameBgPos;

        bottomText.rectTransform.localScale        = bottomTextScale;
        centerText.rectTransform.localScale        = centerTextScale;
        topText.rectTransform.localScale           = topTextScale;
        bottomBg.rectTransform.localScale          = bottomBgScale;
        centerBg.rectTransform.localScale          = centerBgScale;
        topBg.rectTransform.localScale             = topBgScale;
        attackNameText.rectTransform.localScale    = attackNameTextScale;
        attackNameBg.rectTransform.localScale      = attackNameBgScale;
    }
    
    public int GetCenterSpriteIndex()
    {
        return Modulo(firstVisibleIndex + 1, cards.Count);
    }

    int Modulo(int a,int b)=> (a%b + b)%b;
}
