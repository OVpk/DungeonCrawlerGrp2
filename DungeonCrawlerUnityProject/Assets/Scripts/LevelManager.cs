using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public AreaData[,] level { get; private set; }
    
    [Header("Level size")]
    public int levelHeight;
    public int levelWidth;
    

    
    [Header("Percent of chance for each Area type")]
    public int chanceOfFight;
    public int chanceOfShop;
    public int chanceOfGambling;
    public int chanceOfLoot;

    private int normalizedFightChance;
    private int normalizedShopChance;
    private int normalizedGamblingChance;
    private int normalizedLootChance;

    [Header("Max consecutive same area")] 
    public int maxConsecutiveFightArea;
    public int maxConsecutiveShopArea;
    public int maxConsecutiveGamblingArea;
    public int maxConsecutiveLootArea;
    
    [Header("All Areas Data")]
    [SerializeField] private FightAreaData[] allFightArea;
    [SerializeField] private ShopAreaData[] allShopArea;
    [SerializeField] private GamblingAreaData[] allGamblingArea;
    [SerializeField] private LootAreaData[] allLootArea;

    private AreaData currentArea;

    private int currentStep = 1;
    public (int x, int y) currentAreaPosition { get; private set; }
    
    private Queue<FightAreaData> lastFightAreas = new Queue<FightAreaData>();
    public int maxNbOfFightAreasSaved;

    public static LevelManager Instance;
    
    private int seed;


    [SerializeField] private CustomLevelData customLevelData;
    
    public enum AreaTypes
    {
        FightArea,
        ShopArea,
        LootArea,
        GamblingArea
    }

    public enum LevelMode
    {
        Generate,
        Custom
    }

    [SerializeField] private LevelMode currentLevelMode;

    
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

        switch (currentLevelMode)
        {
            case LevelMode.Generate : 
                InitSeed();
                InitLevel();
                break;
            case LevelMode.Custom :
                (int x, int y) firstPosition = (0, 5 / 2);
                Debug.Log(customLevelData.customLevel2d.Length);
                level = customLevelData.customLevel2d;
                SwitchCurrentAreaTo(firstPosition);
                break;
        }
    }

    private void SaveFightArea(FightAreaData fightArea)
    {
        if (lastFightAreas.Count == maxNbOfFightAreasSaved)
        {
            lastFightAreas.Dequeue();
        }
        lastFightAreas.Enqueue(fightArea);
    }

    
    private void InitSeed(int? customSeed = null)
    {
        seed = customSeed ?? Math.Abs(DateTime.Now.Ticks.GetHashCode());
        Random.InitState(seed);
        Debug.Log($"Seed utilisée : {seed}");
    }


    private void GenerateAreaAtPosition((int x, int y) position)
    {
        if (level[position.x, position.y] != null) return;
        
        AreaTypes newAreaType = ChoiceAreaType(position);

        level[position.x, position.y] = ChoiceArea(position, newAreaType);
    }

    private AreaData ChoiceArea((int x, int y) position, AreaTypes newAreaType)
    {
        AreaData choiceArea;
        int rnd;
        switch (newAreaType)
        {
            case AreaTypes.FightArea :
                List<FightAreaData> filteredFightAreas = allFightArea
                    .Where(fightArea => fightArea.availableStep == currentStep)
                    .ToList();
                rnd = Random.Range(0, filteredFightAreas.Count);
                choiceArea = filteredFightAreas[rnd];
                break;
            case AreaTypes.ShopArea :
                List<ShopAreaData> filteredShopAreas = allShopArea
                    .Where(shopArea => shopArea.availableStep == currentStep)
                    .ToList();
                rnd = Random.Range(0, filteredShopAreas.Count);
                choiceArea = filteredShopAreas[rnd];
                break;
            case AreaTypes.GamblingArea :
                List<GamblingAreaData> filteredGamblingAreas = allGamblingArea
                    .Where(gamblingArea => gamblingArea.availableStep == currentStep)
                    .ToList();
                rnd = Random.Range(0, filteredGamblingAreas.Count);
                choiceArea = filteredGamblingAreas[rnd];
                break;
            case AreaTypes.LootArea :
                List<LootAreaData> filteredLootAreas = allLootArea
                    .Where(lootArea => lootArea.availableStep == currentStep)
                    .ToList();
                rnd = Random.Range(0, filteredLootAreas.Count);
                choiceArea = filteredLootAreas[rnd];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newAreaType), newAreaType, null);
        }

        return choiceArea;
    }

    [SerializeField] private int steps;

    private int DefineStepByPosition((int x, int y) position)
    {
        for (int i = 1; i < nbOfStep; i++)
        {
            if (position.x <= levelHeight / nbOfStep *i) return i;
        }

        return nbOfStep;
    }
    
    private (int x, int y)[] directions =
    {
        (0, 1),  // Droite
        (0, -1), // Gauche
        (1, 0),  // Bas
        (-1, 0)  // Haut
    };

    private bool IsOutsideLimits((int x, int y) position)
    {
        return position.x < 0 || position.y < 0 || position.x >= levelWidth || position.y >= levelHeight;
    }

    private void SwitchCurrentAreaTo((int x, int y) position)
    {
        currentAreaPosition = position;
        currentArea = level[currentAreaPosition.x, currentAreaPosition.y];
    }

    private void InitLevel()
    {
        level = new AreaData[levelHeight,levelWidth];
        
        (int x, int y) firstPosition = (0, levelWidth / 2);
        GenerateAreaAtPosition(firstPosition);
        SwitchCurrentAreaTo(firstPosition);
        
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        
        Queue<(int x, int y)> positionsToGenerate = new Queue<(int x, int y)>();

        positionsToGenerate.Enqueue(currentAreaPosition);
        
        while (positionsToGenerate.Count > 0)
        {
            var position = positionsToGenerate.Dequeue();

            GenerateAreaAtPosition(position);

            foreach (var direction in directions)
            {
                (int x, int y) newPosition = (position.x + direction.x, position.y + direction.y);

                if (!IsOutsideLimits(newPosition) && level[newPosition.x, newPosition.y] == null)
                {
                    positionsToGenerate.Enqueue(newPosition);
                }
            }
        }
    }
    
    
    
    private bool HaveReachConsecutiveSameAreaLimit((int x, int y) position, Type areaType, int maxCount)
    {
        return CountConsecutiveSameArea(position, areaType) >= maxCount;
    }
    
    private int CountConsecutiveSameArea((int x, int y) position, Type areaType, (int x, int y) lastDirection = default, HashSet<(int x, int y)> visitedAreas = null)
    {
        visitedAreas ??= new HashSet<(int x, int y)>();
        if (visitedAreas.Contains(position))
        {
            return 0;
        }

        visitedAreas.Add(position);

        int count = 0;
        foreach (var direction in directions)
        {
            (int x, int y) newPosition = (position.x + direction.x, position.y + direction.y);

            if (lastDirection != default && direction == (-lastDirection.x, -lastDirection.y)) continue; // ne verifie pas la zone deja verifiée précedement
            if (IsOutsideLimits(newPosition)) continue;
            if (level[newPosition.x, newPosition.y] == null) continue;

            if (level[newPosition.x, newPosition.y].GetType() == areaType)
            {
                count++;
                count += CountConsecutiveSameArea(newPosition, areaType, direction, visitedAreas);
            }
        }

        return count;
    }


    
    

    private AreaTypes ChoiceAreaType((int x, int y) position)
    {
        List<AreaTypes> possibleTypes = DefinePossibleTypes(position);
        return SelectByProbability(possibleTypes);
    }

    private List<AreaTypes> DefinePossibleTypes((int x, int y) position)
    {
        List<AreaTypes> possibleTypes = new List<AreaTypes>();
        
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(FightAreaData), maxConsecutiveFightArea))
        {
            possibleTypes.Add(AreaTypes.FightArea);
        }
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(ShopAreaData), maxConsecutiveShopArea))
        {
            possibleTypes.Add(AreaTypes.ShopArea);
        }
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(GamblingAreaData), maxConsecutiveGamblingArea))
        {
            possibleTypes.Add(AreaTypes.GamblingArea);
        }
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(LootAreaData), maxConsecutiveLootArea))
        {
            possibleTypes.Add(AreaTypes.LootArea);
        }

        return possibleTypes;
    }
    


    private void NormalizeProbabilities(List<AreaTypes> possibleAreaTypes)
    {
        int fightChance = chanceOfFight;
        int shopChance = chanceOfShop;
        int gamblingChance = chanceOfFight;
        int lootChance = chanceOfShop;
        
        
        int totalChance = 0;

        bool containsFightArea = possibleAreaTypes.Contains(AreaTypes.FightArea);
        bool containsShopArea = possibleAreaTypes.Contains(AreaTypes.ShopArea);
        bool containsGamblingArea = possibleAreaTypes.Contains(AreaTypes.GamblingArea);
        bool containsLootArea = possibleAreaTypes.Contains(AreaTypes.LootArea);

        if (chanceOfFight != 0 && containsFightArea)
        {
            totalChance += fightChance;
        }
        else
        {
            fightChance = 0;
        }

        if (chanceOfShop != 0 && containsShopArea)
        {
            totalChance += shopChance;
        }
        else
        {
            shopChance = 0;
        }
        
        if (chanceOfGambling != 0 && containsGamblingArea)
        {
            totalChance += gamblingChance;
        }
        else
        {
            gamblingChance = 0;
        }
        
        if (chanceOfLoot != 0 && containsLootArea)
        {
            totalChance += lootChance;
        }
        else
        {
            lootChance = 0;
        }

        if (totalChance > 0)
        {
            normalizedFightChance = (fightChance * 100) / totalChance;
            normalizedShopChance = (shopChance * 100) / totalChance;
            normalizedGamblingChance = (gamblingChance * 100) / totalChance;
            normalizedLootChance = (lootChance * 100) / totalChance;
        }
    }

    private AreaTypes SelectByProbability(List<AreaTypes> possibleTypes)
    {
        NormalizeProbabilities(possibleTypes);
        int rndNb = Random.Range(0, 100);

        if (rndNb < normalizedFightChance)
        {
            return AreaTypes.FightArea;
        }
        if (rndNb < normalizedFightChance + normalizedShopChance)
        {
            return AreaTypes.ShopArea;
        }
        if (rndNb < normalizedFightChance + normalizedShopChance + normalizedGamblingChance)
        {
            return AreaTypes.GamblingArea;
        }
        if (rndNb < normalizedFightChance + normalizedShopChance + normalizedGamblingChance + normalizedLootChance)
        {
            return AreaTypes.LootArea;
        }
        
        return possibleTypes[0]; // Retour par défaut
    }




    
}
