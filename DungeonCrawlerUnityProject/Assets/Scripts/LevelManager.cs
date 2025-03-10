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

    private int normalizedFightChance;
    private int normalizedShopChance;

    [Header("Max consecutive same area")] 
    public int maxConsecutiveFightArea;
    public int maxConsecutiveShopArea;
    
    [Header("All Areas Data")]
    [SerializeField] private FightAreaData[] allFightArea;
    [SerializeField] private ShopAreaData[] allShopArea;

    private AreaData currentArea;
    public (int x, int y) currentAreaPosition { get; private set; }
    
    private Queue<FightAreaData> lastFightAreas = new Queue<FightAreaData>();
    public int maxNbOfFightAreasSaved;

    public static LevelManager Instance;
    
    private int seed;
    


    
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
        
        InitSeed();
        InitLevel();
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







    public void GenerateAreaAtPosition((int x, int y) position)
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
    
    public void GenerateLevel()
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
    
    public bool HaveReachConsecutiveSameAreaLimit((int x, int y) position, Type areaType, int maxCount)
    {
        if (maxCount <= 0) return true;
        
        foreach (var direction in directions)
        {
            (int x, int y) newPosition = (position.x + direction.x, position.y + direction.y);

            if (IsOutsideLimits((newPosition.x, newPosition.y))) continue;
            if (level[newPosition.x, newPosition.y] == null) continue;
                
            if (level[newPosition.x, newPosition.y].GetType() == areaType)
            {
                return HaveReachConsecutiveSameAreaLimit((newPosition.x, newPosition.y), areaType, maxCount - 1);
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

        return possibleTypes;
    }
    


    private void NormalizeProbabilities(List<Type> possibleAreaTypes)
    {
        int fightChance = chanceOfFight;
        int shopChance = chanceOfShop;
        int totalChance = 0;

        bool containsFightArea = possibleAreaTypes.Contains(typeof(FightAreaData));
        bool containsShopArea = possibleAreaTypes.Contains(typeof(ShopAreaData));

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

        if (totalChance > 0)
        {
            normalizedFightChance = (fightChance * 100) / totalChance;
            normalizedShopChance = (shopChance * 100) / totalChance;
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

        return possibleTypes[0]; // Retour par défaut
    }




    
}
