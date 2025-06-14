using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExplorationManager : MonoBehaviour
{
    public static ExplorationManager Instance;
    
    public AreaData[,] level { get; private set; }
    
    public (int x, int y) currentAreaPosition { get; private set; }
    public AreaData currentArea { get; private set; }
    private AreaData leftArea;
    private AreaData rightArea;
    
    [SerializeField] private CustomLevelData customLevelData;


    public SpriteRenderer backgroundExplo;

    public bool isLastStageReached = false;

    
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
        
        level = customLevelData.customLevel2d;
        SwitchCurrentAreaTo((0,0));
    }

    private void SwitchCurrentAreaTo((int x, int y) position)
    {
        currentAreaPosition = position;
        currentArea = level[currentAreaPosition.x, currentAreaPosition.y];
        if (level[currentAreaPosition.x + 1, currentAreaPosition.y] == null ||
            level[currentAreaPosition.x, currentAreaPosition.y + 1] == null)
        {
            isLastStageReached = true;
            return;
        }
        leftArea = level[currentAreaPosition.x+1, currentAreaPosition.y];
        rightArea = level[currentAreaPosition.x, currentAreaPosition.y+1];
    }

    public FightAreaController fightController;
    
    public IEnumerator EnterArea((int x, int y) position)
    {
        SwitchCurrentAreaTo(position);

        if (currentArea is FightAreaData data)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.InFightArea);
            FightManager.Instance.LoadFightArea(data);
            yield return null;
            fightController.GoToInitialState();
        }
        else
        {
            throw new ArgumentException("This Area Type is not usable");
        }
    }

    public RewardDisplayer leftAreaReward;
    public RewardDisplayer rightAreaReward;
    public Image leftAreaBox;
    public Image rightAreaBox;

    public void SetDisplay()
    {
        leftAreaReward.DisplayRewards(true,
            ((FightAreaData)leftArea).reward.rewardType == RewardData.RewardType.Money 
            ? ((FightAreaData)leftArea).reward.money 
            : ((FightAreaData)leftArea).reward.nbOfCandy, 
            ((FightAreaData)leftArea).reward.rewardType);
        rightAreaReward.DisplayRewards(true,
            ((FightAreaData)rightArea).reward.rewardType == RewardData.RewardType.Money 
                ? ((FightAreaData)rightArea).reward.money 
                : ((FightAreaData)rightArea).reward.nbOfCandy, 
            ((FightAreaData)rightArea).reward.rewardType);
        leftAreaBox.sprite = ((FightAreaData)leftArea).boxVisual != null ? ((FightAreaData)leftArea).boxVisual : null;
        rightAreaBox.sprite = ((FightAreaData)rightArea).boxVisual != null ? ((FightAreaData)rightArea).boxVisual : null;
        stockDisplayer.RefreshDisplay();
        backgroundExplo.sprite = ((FightAreaData)currentArea).explorationBackgroundAfterThisFight;
    }

    public StockDisplayer stockDisplayer;

    public Transform echelleLeftPosition;
    public Transform echelleRightPosition;
    public GameObject echelle;

    public void MoveAreaSelector((int x, int y) direction)
    {
        switch (direction)
        {
            case (1, 0) :
                echelle.transform.position = echelleLeftPosition.transform.position;
                break;
            case (0, 1) : 
                echelle.transform.position = echelleRightPosition.transform.position;
                break;
            default: throw new Exception("Impossible direction");
        }
    }
    

    public void GoToShop()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.InShop);
    }
}
