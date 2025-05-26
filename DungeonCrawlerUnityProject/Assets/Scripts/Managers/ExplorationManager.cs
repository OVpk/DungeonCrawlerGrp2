using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    public static ExplorationManager Instance;
    
    public AreaData[,] level { get; private set; }
    
    public (int x, int y) currentAreaPosition { get; private set; }
    private AreaData currentArea;
    private AreaData leftArea;
    private AreaData rightArea;
    
    [SerializeField] private CustomLevelData customLevelData;
    
    public TMP_Text moneyText;



    
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

    private void Start()
    {
        level = customLevelData.customLevel2d;
        SwitchCurrentAreaTo((0,0));
        SetDisplay();
    }

    private void SwitchCurrentAreaTo((int x, int y) position)
    {
        currentAreaPosition = position;
        currentArea = level[currentAreaPosition.x, currentAreaPosition.y];
        leftArea = level[currentAreaPosition.x+1, currentAreaPosition.y];
        rightArea = level[currentAreaPosition.x, currentAreaPosition.y+1];
    }
    
    public void EnterArea((int x, int y) position)
    {
        SwitchCurrentAreaTo(position);

        if (currentArea is FightAreaData data)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.InFightArea);
            FightManager.Instance.LoadFightArea(data);
        }
        else
        {
            throw new ArgumentException("This Area Type is not usable");
        }
    }

    public AreaInWorldDisplayController leftDisplayedArea;
    public AreaInWorldDisplayController rightDisplayedArea;

    public void SetDisplay()
    {
        leftDisplayedArea.Init((FightAreaData)leftArea);
        rightDisplayedArea.Init((FightAreaData)rightArea);
        UpdateMoneyUI();
    }

    public void MoveAreaSelector((int x, int y) direction)
    {
        switch (direction)
        {
            case (1, 0) : 
                leftDisplayedArea.SetHighlight(Color.magenta);
                rightDisplayedArea.ClearHighlight();
                break;
            case (0, 1) : 
                rightDisplayedArea.SetHighlight(Color.magenta); 
                leftDisplayedArea.ClearHighlight();
                break;
            default: throw new Exception("Impossible direction");
        }
    }
    
    private void UpdateMoneyUI()
    {
        moneyText.text = GameManager.Instance.money.ToString();
    }
}
