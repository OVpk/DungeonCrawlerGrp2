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
        
        slotHeight = tagadaImg.rectTransform.rect.height;

        /*
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

        
        UpdateDescriptionSprite();
        */
    }

    public void EntityIsPlaced(EntityData entity)
    {
        if (!isTagadaDiscovered && entity == tagada)
        {
            isTagadaDiscovered = true;
            tagadaImg.sprite = tagadadiscoveredSprite;
        }
        else if (!isCarambarDiscovered && entity == carambar)
        {
            isCarambarDiscovered = true;
            carambarImg.sprite = carambardiscoveredSprite;
        }
        else if (!isVerDiscovered && entity == ver)
        {
            isVerDiscovered = true;
            verImg.sprite = verdiscoveredSprite;
        }
        else if (!isSoucoupeDiscovered && entity == soucoupe)
        {
            isSoucoupeDiscovered = true;
            soucoupeImg.sprite = soucoupediscoveredSprite;
        }
        else if (!isTeteBruleeDiscovered && entity == teteBrulee)
        {
            isTeteBruleeDiscovered = true;
            teteBruleeImg.sprite = teteBruleediscoveredSprite;
        }
        else if (!isMalabarDiscovered && entity == malabar)
        {
            isMalabarDiscovered = true;
            malabarImg.sprite = malabardiscoveredSprite;
        }
        else if (!isSmartiesBoxDiscovered && entity == smartiesBox)
        {
            isSmartiesBoxDiscovered = true;
            smartiesBoxImg.sprite = smartiesBoxdiscoveredSprite;
        }
        else if (!isSmartieDiscovered && entity == smartie)
        {
            isSmartieDiscovered = true;
            smartieImg.sprite = smartiediscoveredSprite;
        }
    }
    
    public void MoveTicketContainer(int verticalMove)
    {
        if (verticalMove == 0) return;
        
        int newIndex = currentIndex + verticalMove;
        // Clamp to valid range
        newIndex = Mathf.Clamp(newIndex, 0, descriptionSprites.Length - 1);

        // If index hasn't changed, do nothing
        if (newIndex == currentIndex) return;

        // Update currentIndex
        currentIndex = newIndex;

        Vector3 targetPos = ticketContainer.transform.localPosition + new Vector3(0f, slotHeight * verticalMove, 0f);

        // Animate move and update description when done
        ticketContainer.transform.DOLocalMove(targetPos, 0.5f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(UpdateDescriptionSprite);
    }
    
    private void UpdateDescriptionSprite()
    {
        if (descriptionSprites != null && descriptionSprites.Length > currentIndex)
        {
            descriptionDisplayer.sprite = descriptionSprites[currentIndex];
        }
    }
    
}
