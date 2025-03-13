using System;
using System.Collections.Generic;
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
    public (int x, int y) currentAreaPosition { get; private set; }
    
    private Queue<FightAreaData> lastFightAreas = new Queue<FightAreaData>();
    public int maxNbOfFightAreasSaved;

    public static LevelManager Instance;
    
    private int seed;


    [SerializeField] private CustomLevelData customLevelData;

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
        
        Type newAreaType = ChoiceAreaType(position);

        if (newAreaType == typeof(FightAreaData))
        {
            int rnd = Random.Range(0, allFightArea.Length);
            level[position.x, position.y] = allFightArea[rnd];
        }
        else if (newAreaType == typeof(ShopAreaData))
        {
            int rnd = Random.Range(0, allShopArea.Length);
            level[position.x, position.y] = allShopArea[rnd];
        }
        else if (newAreaType == typeof(GamblingAreaData))
        {
            int rnd = Random.Range(0, allGamblingArea.Length);
            level[position.x, position.y] = allGamblingArea[rnd];
        }
        else if (newAreaType == typeof(LootAreaData))
        {
            int rnd = Random.Range(0, allLootArea.Length);
            level[position.x, position.y] = allLootArea[rnd];
        }
        
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
    
    

    private bool HaveReachConsecutiveSameAreaLimit((int x, int y) position, Type areaType, int maxCount, (int x, int y) lastDirection = default)
    {
        if (maxCount <= 0) return true;
        
        foreach (var direction in directions)
        {
            (int x, int y) newPosition = (position.x + direction.x, position.y + direction.y);
            
            if (lastDirection != default && direction == (-lastDirection.x, -lastDirection.y)) continue; // ne verifie pas la zone deja verifiée précedement
            if (IsOutsideLimits((newPosition.x, newPosition.y))) continue;
            if (level[newPosition.x, newPosition.y] == null) continue;
                
            if (level[newPosition.x, newPosition.y].GetType() == areaType)
            {
                return HaveReachConsecutiveSameAreaLimit((newPosition.x, newPosition.y), areaType, maxCount - 1, direction);
            }
        }
        
        return false;
    }
    
    

    private Type ChoiceAreaType((int x, int y) position)
    {
        List<Type> possibleTypes = DefinePossibleTypes(position);
        return SelectByProbability(possibleTypes);
    }

    private List<Type> DefinePossibleTypes((int x, int y) position)
    {
        List<Type> possibleTypes = new List<Type>();
        
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(FightAreaData), maxConsecutiveFightArea))
        {
            possibleTypes.Add(typeof(FightAreaData));
        }
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(ShopAreaData), maxConsecutiveShopArea))
        {
            possibleTypes.Add(typeof(ShopAreaData));
        }
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(GamblingAreaData), maxConsecutiveGamblingArea))
        {
            possibleTypes.Add(typeof(GamblingAreaData));
        }
        if (!HaveReachConsecutiveSameAreaLimit((position.x, position.y), typeof(LootAreaData), maxConsecutiveLootArea))
        {
            possibleTypes.Add(typeof(LootAreaData));
        }

        return possibleTypes;
    }
    


    private void NormalizeProbabilities(List<Type> possibleAreaTypes)
    {
        int fightChance = chanceOfFight;
        int shopChance = chanceOfShop;
        int gamblingChance = chanceOfFight;
        int lootChance = chanceOfShop;
        
        
        int totalChance = 0;

        bool containsFightArea = possibleAreaTypes.Contains(typeof(FightAreaData));
        bool containsShopArea = possibleAreaTypes.Contains(typeof(ShopAreaData));
        bool containsGamblingArea = possibleAreaTypes.Contains(typeof(GamblingAreaData));
        bool containsLootArea = possibleAreaTypes.Contains(typeof(LootAreaData));

        if (containsFightArea)
        {
            totalChance += fightChance;
        }
        else
        {
            fightChance = 0;
        }

        if (containsShopArea)
        {
            totalChance += shopChance;
        }
        else
        {
            shopChance = 0;
        }
        
        if (containsGamblingArea)
        {
            totalChance += gamblingChance;
        }
        else
        {
            gamblingChance = 0;
        }
        
        if (containsLootArea)
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

    private Type SelectByProbability(List<Type> possibleTypes)
    {
        NormalizeProbabilities(possibleTypes);
        int rndNb = Random.Range(0, 100);

        if (rndNb < normalizedFightChance)
        {
            return typeof(FightAreaData);
        }
        if (rndNb < normalizedFightChance + normalizedShopChance)
        {
            return typeof(ShopAreaData);
        }
        if (rndNb < normalizedFightChance + normalizedShopChance + normalizedGamblingChance)
        {
            return typeof(GamblingAreaData);
        }
        if (rndNb < normalizedFightChance + normalizedShopChance + normalizedGamblingChance + normalizedLootChance)
        {
            return typeof(LootAreaData);
        }

        return possibleTypes[0]; // Retour par défaut
    }




    
}
