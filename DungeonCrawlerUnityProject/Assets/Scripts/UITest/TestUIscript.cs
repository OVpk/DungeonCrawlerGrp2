using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TestUIscript : MonoBehaviour
{
    [Header("Liste des cartes (Sprite)")]
    public List<Sprite> cardSprites;

    [Header("Images UI pour afficher les cartes")]
    public Image leftImage;   
    public Image centerImage; 
    public Image rightImage;  
    
    private Vector2 leftPos;
    private Vector2 centerPos;
    private Vector2 rightPos;
    
    private float leftAlpha  = 183f / 255f; 
    private float centerAlpha = 1f;         
    private float rightAlpha = 183f / 255f;    

    // Index de la position de l'image gauche dans la liste
    private int firstVisibleIndex = 0;
    
    private bool isAnimating = false;

    // Durée des animations (en secondes)
    public float animDuration = 0.5f;

    void Start()
    {
        if (cardSprites == null || cardSprites.Count < 3)
        {
            Debug.LogError("EH CONNARD DE GD IL FAUT AU MOIN 3 SPRITE DANS LA LISTE !!!");
            return;
        }
        
        leftPos = leftImage.rectTransform.anchoredPosition;
        centerPos = centerImage.rectTransform.anchoredPosition;
        rightPos = rightImage.rectTransform.anchoredPosition;
        
        UpdateCardImages();
        UpdateCardAlphas();
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            ScrollLeft();
        }
        if (Input.GetKey(KeyCode.A))
        {
            ScrollRight();
        }

        if (Input.GetKey(KeyCode.W))
        {
            Select();
        }
    }
    
    void UpdateCardImages()
    {
        leftImage.sprite = cardSprites[Modulo(firstVisibleIndex, cardSprites.Count)];
        centerImage.sprite = cardSprites[Modulo(firstVisibleIndex + 1, cardSprites.Count)];
        rightImage.sprite = cardSprites[Modulo(firstVisibleIndex + 2, cardSprites.Count)];
    }
    
    void UpdateCardAlphas()
    {
        SetAlpha(leftImage, leftAlpha);
        SetAlpha(centerImage, centerAlpha);
        SetAlpha(rightImage, rightAlpha);
    }

    void SetAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    public void ScrollLeft()
    {
        if (isAnimating) return;
        isAnimating = true;

       
        Sequence seq = DOTween.Sequence();

        // Animation de déplacement et de fondu des images
        seq.Join(centerImage.rectTransform.DOAnchorPos(leftPos, animDuration));
        seq.Join(centerImage.DOFade(leftAlpha, animDuration)); // (Centre vers gauche)

        seq.Join(rightImage.rectTransform.DOAnchorPos(centerPos, animDuration));
        seq.Join(rightImage.DOFade(centerAlpha, animDuration)); // (Droite vers centre)

        // Fade de scale de 0 a 1
        seq.Join(leftImage.rectTransform.DOScale(0f, animDuration));
        seq.Join(leftImage.DOFade(rightAlpha, animDuration));
        
        seq.OnComplete(() =>
        {
            // Redéfinie la valeur de index de la prochaine carte a passer
            firstVisibleIndex = Modulo(firstVisibleIndex + 1, cardSprites.Count);
            UpdateCardImages();

            // Reset de positions, scale et opacité aux valeur de base
            leftImage.rectTransform.anchoredPosition = leftPos;
            centerImage.rectTransform.anchoredPosition = centerPos;
            rightImage.rectTransform.anchoredPosition = rightPos;

            leftImage.rectTransform.localScale = Vector3.one;
            centerImage.rectTransform.localScale = Vector3.one;
            rightImage.rectTransform.localScale = Vector3.one;

            UpdateCardAlphas();

            //Fade de scale 0 a 1
            rightImage.rectTransform.localScale = Vector3.zero;
            rightImage.rectTransform.DOScale(1f, animDuration).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }
    
    public void ScrollRight()
    {
        if (isAnimating) return;
        isAnimating = true;

        Sequence seq = DOTween.Sequence();
        
        seq.Join(leftImage.rectTransform.DOAnchorPos(centerPos, animDuration));
        seq.Join(leftImage.DOFade(centerAlpha, animDuration));

        seq.Join(centerImage.rectTransform.DOAnchorPos(rightPos, animDuration));
        seq.Join(centerImage.DOFade(rightAlpha, animDuration));
        
        seq.Join(rightImage.rectTransform.DOScale(0f, animDuration));
        seq.Join(rightImage.DOFade(leftAlpha, animDuration));

        seq.OnComplete(() =>
        {
            firstVisibleIndex = Modulo(firstVisibleIndex - 1, cardSprites.Count);
            UpdateCardImages();
            
            leftImage.rectTransform.anchoredPosition = leftPos;
            centerImage.rectTransform.anchoredPosition = centerPos;
            rightImage.rectTransform.anchoredPosition = rightPos;

            leftImage.rectTransform.localScale = Vector3.one;
            centerImage.rectTransform.localScale = Vector3.one;
            rightImage.rectTransform.localScale = Vector3.one;

            UpdateCardAlphas();
            
            leftImage.rectTransform.localScale = Vector3.zero;
            leftImage.rectTransform.DOScale(1f, animDuration).OnComplete(() =>
            {
                isAnimating = false;
            });
        });
    }
    
    public void Select()
    {
        int centerIndex = Modulo(firstVisibleIndex + 1, cardSprites.Count);
        Debug.Log(centerIndex);
    }
    
    int Modulo(int a, int b)
    {
        return (a % b + b) % b;
    }
}
