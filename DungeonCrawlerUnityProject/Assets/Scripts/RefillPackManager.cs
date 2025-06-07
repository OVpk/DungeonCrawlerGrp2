using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillPackManager : MonoBehaviour
{
    public static RefillPackManager Instance;

    public int nbOfCandy { get; private set; }

    public List<Sprite> numbers;

    public SpriteRenderer nbMax;
    public SpriteRenderer currentNb;

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
    }

    public void Init(int nbOfCandyToRefill)
    {
        InitPack();
        nbOfCandy = nbOfCandyToRefill;
        nbMax.sprite = numbers[nbOfCandy];
        currentNb.sprite = numbers[nbOfCandy];
    }

    [SerializeField] public SimpleCardSlider packDisplayer;

    private void InitPack()
    {
        packDisplayer.packs = GameManager.Instance.candyPacks;
        packDisplayer.UpdateDisplay();
    }

    public void RefillPack(CandyPack packToRefill)
    {
        if (nbOfCandy <= 0) return;
        if (packToRefill.currentStock < packToRefill.maxStock)
        {
            packToRefill.currentStock++;
            nbOfCandy--;
            currentNb.sprite = numbers[nbOfCandy];
            packDisplayer.UpdateDisplay();
        }
    }

    public void ReturnToExploration()
    {
        ShopManager.Instance.InitShop();
        GameManager.Instance.ChangeGameState(GameManager.GameState.InOverWorld);
        ExplorationManager.Instance.SetDisplay();
    }
}