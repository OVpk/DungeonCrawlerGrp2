using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField]private LevelGenerator levelGenerator;
    [SerializeField]private LevelDisplayer levelDisplayer;
    
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
        SwitchCurrentAreaTo((0, level.GetLength(1)));
        levelDisplayer.InitDisplay();
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
        if (currentLevelMode == LevelMode.Custom) throw new Exception("Session de test terminée");
        currentStep++;
        if (currentStep >= allSteps.Length)
        {
            //Call GameManager For end game (win)
        }
        else
        {
            level = levelGenerator.NewLevel();
        }
    }
}
