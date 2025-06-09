using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopedieManager : MonoBehaviour
{
    public static EncyclopedieManager Instance;

    [Header("image slot")]
    public Image tagadaImg;
    public Image carambarImg;
    public Image verImg;
    public Image soucoupeImg;
    public Image teteBruleeImg;
    public Image malabarImg;
    public Image smartiesBoxImg;
    public Image smartieImg;
    
    [Header("discovered sprites")]
    public Sprite tagadadiscoveredSprite;
    public Sprite carambardiscoveredSprite;
    public Sprite verdiscoveredSprite;
    public Sprite soucoupediscoveredSprite;
    public Sprite teteBruleediscoveredSprite;
    public Sprite malabardiscoveredSprite;
    public Sprite smartiesBoxdiscoveredSprite;
    public Sprite smartiediscoveredSprite;
    
    [Header("entities to discover")]
    public EntityData tagada;
    public EntityData carambar;
    public EntityData ver;
    public EntityData soucoupe;
    public EntityData teteBrulee;
    public EntityData malabar;
    public EntityData smartiesBox;
    public EntityData smartie;

    private bool isTagadaDiscovered = false;
    private bool isCarambarDiscovered = false;
    private bool isVerDiscovered = false;
    private bool isSoucoupeDiscovered = false;
    private bool isTeteBruleeDiscovered = false;
    private bool isMalabarDiscovered = false;
    private bool isSmartiesBoxDiscovered = false;
    private bool isSmartieDiscovered = false;
    
    [Header("description sprites")]
    public Sprite tagadaDescription;
    public Sprite carambarDescription;
    public Sprite verDescription;
    public Sprite soucoupeDescription;
    public Sprite teteBruleeDescription;
    public Sprite malabarDescription;
    public Sprite smartiesBoxDescription;
    public Sprite smartieDescription;

    public Image descriptionDisplayer;

    public GameObject ticketContainer;
    
    private float slotHeight;
    private int currentIndex = 0;
    private Sprite[] descriptionSprites;
    
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
        
        slotHeight = tagadaImg.rectTransform.rect.height * tagadaImg.rectTransform.localScale.y;

        descriptionSprites = new Sprite[] {
            tagadaDescription,
            carambarDescription,
            verDescription,
            soucoupeDescription,
            teteBruleeDescription,
            malabarDescription,
            smartiesBoxDescription,
            smartieDescription
        };

        targetPosition = ticketContainer.transform.localPosition;
    }

    public void EntityIsPlaced(EntityData entity)
    {
        if (!isTagadaDiscovered && entity == tagada)
        {
            isTagadaDiscovered = true;
            tagadaImg.sprite = tagadadiscoveredSprite;
            DisplayPopupAlert();
        }
        else if (!isCarambarDiscovered && entity == carambar)
        {
            isCarambarDiscovered = true;
            carambarImg.sprite = carambardiscoveredSprite;
            DisplayPopupAlert();
        }
        else if (!isVerDiscovered && entity == ver)
        {
            isVerDiscovered = true;
            verImg.sprite = verdiscoveredSprite;
            DisplayPopupAlert();
        }
        else if (!isSoucoupeDiscovered && entity == soucoupe)
        {
            isSoucoupeDiscovered = true;
            soucoupeImg.sprite = soucoupediscoveredSprite;
            DisplayPopupAlert();
        }
        else if (!isTeteBruleeDiscovered && entity == teteBrulee)
        {
            isTeteBruleeDiscovered = true;
            teteBruleeImg.sprite = teteBruleediscoveredSprite;
            DisplayPopupAlert();
        }
        else if (!isMalabarDiscovered && entity == malabar)
        {
            isMalabarDiscovered = true;
            malabarImg.sprite = malabardiscoveredSprite;
            DisplayPopupAlert();
        }
        else if (!isSmartiesBoxDiscovered && entity == smartiesBox)
        {
            isSmartiesBoxDiscovered = true;
            smartiesBoxImg.sprite = smartiesBoxdiscoveredSprite;
            DisplayPopupAlert();
        }
        else if (!isSmartieDiscovered && entity == smartie)
        {
            isSmartieDiscovered = true;
            smartieImg.sprite = smartiediscoveredSprite;
            DisplayPopupAlert();
        }
    }

    public Vector3 targetPosition;
    
    
    public void MoveTicketContainer(int verticalMove)
    {
        if (verticalMove == 0) return;
        
        int newIndex = currentIndex + verticalMove;
        newIndex = Mathf.Clamp(newIndex, 0, descriptionSprites.Length - 1);

        if (newIndex == currentIndex) return;

        currentIndex = newIndex;

        targetPosition += new Vector3(0f, slotHeight * verticalMove, 0f);

        ticketContainer.transform.DOLocalMove(targetPosition, 0.5f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(UpdateDescriptionSprite);
    }
    
    public void UpdateDescriptionSprite()
    {
        bool discovered = false;
        switch (currentIndex)
        {
            case 0: discovered = isTagadaDiscovered; break;
            case 1: discovered = isCarambarDiscovered; break;
            case 2: discovered = isVerDiscovered; break;
            case 3: discovered = isSoucoupeDiscovered; break;
            case 4: discovered = isTeteBruleeDiscovered; break;
            case 5: discovered = isMalabarDiscovered; break;
            case 6: discovered = isSmartiesBoxDiscovered; break;
            case 7: discovered = isSmartieDiscovered; break;
        }

        if (discovered)
        {
            descriptionDisplayer.gameObject.SetActive(true);
            descriptionDisplayer.sprite = descriptionSprites[currentIndex];
        }
        else
        {
            descriptionDisplayer.gameObject.SetActive(false);
        }
    }

    public Animation popupAlert;

    public void DisplayPopupAlert()
    {
        popupAlert.Play();
    }

}
