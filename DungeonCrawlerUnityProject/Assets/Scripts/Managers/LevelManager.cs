using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField]private LevelGenerator levelGenerator;
    [field:SerializeField]public LevelDisplayer levelDisplayer { get; private set; }
    
    public AreaData[,] level { get; private set; }
    private AreaData currentArea; 
    public (int x, int y) currentAreaPosition { get; private set; }
    
    private Queue<FightAreaData> lastFightAreas = new Queue<FightAreaData>();
    public int maxNbOfFightAreasSaved;

    private enum LevelMode
    {
        Generate,
        Custom
    }
    [SerializeField] private LevelMode currentLevelMode;
    
    [field:Header("All level's steps (for generation)"),SerializeField]
    public LevelStepData[] allSteps { get; private set; }
    public int currentStep { get; private set; } = 0;
    
    [SerializeField] private CustomLevelData customLevelData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        level = currentLevelMode switch
        {
            LevelMode.Generate => levelGenerator.NewLevel(),
            LevelMode.Custom => customLevelData.customLevel2d,
            _ => throw new Exception("Mode non valide")
        };
        SwitchCurrentAreaTo((0, 0));
        levelDisplayer.DisplayLevel(true);
    }
    
    public void EnterArea((int x, int y) position)
    {
        if (position != (currentAreaPosition.x+1,currentAreaPosition.y) &&
            position != (currentAreaPosition.x,currentAreaPosition.y+1)) return;

        if (IsOutsideLimits(position))
        {
            GoToNextStep();
        }
        else
        {
            SwitchCurrentAreaTo(position);
        }
        
        levelDisplayer.DisplayLevel(false);
        levelDisplayer.DisplayArea(true);
        GameManager.Instance.ChangeController(GameManager.ControllerTypes.InArea);
    }

    public void ExitArea()
    {
        levelDisplayer.DisplayLevel(true);
        levelDisplayer.DisplayArea(false);
        GameManager.Instance.ChangeController(GameManager.ControllerTypes.InOverWorld);
    }

    private void SwitchCurrentAreaTo((int x, int y) position)
    {
        currentAreaPosition = position;
        currentArea = level[currentAreaPosition.x, currentAreaPosition.y];
    }
    
    private void SaveFightArea(FightAreaData fightArea)
    {
        if (lastFightAreas.Count == maxNbOfFightAreasSaved)
        {
            lastFightAreas.Dequeue();
        }
        lastFightAreas.Enqueue(fightArea);
    }

    private void GoToNextStep()
    {
        if (currentLevelMode == LevelMode.Custom)
        {
            Debug.Log("Session de test terminée");
            return;
        }
        
        currentStep++;
        if (currentStep >= allSteps.Length)
        {
            //Call GameManager For end game (win)
            Debug.Log("No more step");
        }
        else
        {
            level = levelGenerator.NewLevel();
            SwitchCurrentAreaTo((0,0));
        }
    }
    
    public bool IsOutsideLimits((int x, int y) position)
    {
        return position.x < 0 || position.y < 0 || position.x >= level.GetLength(0) || position.y >= level.GetLength(1);
    }
}
