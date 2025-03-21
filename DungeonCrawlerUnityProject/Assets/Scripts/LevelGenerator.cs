using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    private int seed;
    
    private int levelHeight;
    private int levelWidth;
    private FightAreaData[] allFightAreas;
    private ShopAreaData[] allShopAreas;
    private GamblingAreaData[] allGamblingAreas;
    private LootAreaData[] allLootAreas;
    
    [Header("Generation common rules")]
    [Header("Percent of chance for each Area type")]
    [SerializeField]private int chanceOfFight;
    [SerializeField]private int chanceOfShop;
    [SerializeField]private int chanceOfGambling;
    [SerializeField]private int chanceOfLoot;

    private int normalizedFightChance;
    private int normalizedShopChance;
    private int normalizedGamblingChance;
    private int normalizedLootChance;

    [Header("Max consecutive same area")] 
    [SerializeField]private int maxConsecutiveFightArea;
    [SerializeField]private int maxConsecutiveShopArea;
    [SerializeField]private int maxConsecutiveGamblingArea;
    [SerializeField]private int maxConsecutiveLootArea;
    
    private AreaData[,] newLevel;

    private enum AreaTypes
    {
        FightArea,
        ShopArea,
        LootArea,
        GamblingArea
    }
    
    private (int x, int y)[] directions =
    {
        (0, 1),  // Droite
        (0, -1), // Gauche
        (1, 0),  // Bas
        (-1, 0)  // Haut
    };
    
    private void Awake()
    {
        InitSeed();
    }
    
    private void InitSeed(int? customSeed = null)
    {
        seed = customSeed ?? Math.Abs(DateTime.Now.Ticks.GetHashCode());
        Random.InitState(seed);
        Debug.Log($"Seed utilisée : {seed}");
    }
    
    public AreaData[,] NewLevel()
    {
        LevelStepData newStep = LevelManager.Instance.allSteps[LevelManager.Instance.currentStep];
        LoadNewStepData(newStep);
        
        newLevel = new AreaData[levelHeight,levelWidth];
        
        GenerateLevel();
        return newLevel;
    }
    
    private void LoadNewStepData(LevelStepData newStep)
    {
        levelHeight = newStep.height;
        levelWidth = newStep.width;
        allFightAreas = newStep.availableFightAreas;
        allShopAreas = newStep.availableShopAreas;
        allGamblingAreas = newStep.availableGamblingAreas;
        allLootAreas = newStep.availableLootAreas;
    }
    
    private void GenerateLevel()
    {
        Queue<(int x, int y)> positionsToGenerate = new Queue<(int x, int y)>(); 
        (int x, int y) firstPosition = (levelHeight/2, levelWidth/2);

        positionsToGenerate.Enqueue(firstPosition);
        
        while (positionsToGenerate.Count > 0)
        {
            var position = positionsToGenerate.Dequeue();

            GenerateAreaAtPosition(position);

            foreach (var direction in directions)
            {
                (int x, int y) newPosition = (position.x + direction.x, position.y + direction.y);

                if (!IsOutsideLimits(newPosition) && newLevel[newPosition.x, newPosition.y] == null)
                {
                    positionsToGenerate.Enqueue(newPosition);
                }
            }
        }
    }
    
    private void GenerateAreaAtPosition((int x, int y) position)
    {
        if (newLevel[position.x, position.y] != null) return;
        
        AreaTypes newAreaType = ChoiceAreaType(position);

        newLevel[position.x, position.y] = ChoiceArea(newAreaType);
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
            if (newLevel[newPosition.x, newPosition.y] == null) continue;

            if (newLevel[newPosition.x, newPosition.y].GetType() == areaType)
            {
                count++;
                count += CountConsecutiveSameArea(newPosition, areaType, direction, visitedAreas);
            }
        }

        return count;
    }
    
    private AreaData ChoiceArea(AreaTypes newAreaType)
    {
        AreaData choiceArea;
        int rnd;
        switch (newAreaType)
        {
            case AreaTypes.FightArea :
                rnd = Random.Range(0, allFightAreas.Length);
                choiceArea = allFightAreas[rnd];
                break;
            case AreaTypes.ShopArea :
                rnd = Random.Range(0, allShopAreas.Length);
                choiceArea = allShopAreas[rnd];
                break;
            case AreaTypes.GamblingArea :
                rnd = Random.Range(0, allGamblingAreas.Length);
                choiceArea = allGamblingAreas[rnd];
                break;
            case AreaTypes.LootArea :
                rnd = Random.Range(0, allLootAreas.Length);
                choiceArea = allLootAreas[rnd];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newAreaType), newAreaType, null);
        }

        return choiceArea;
    }

    private bool IsOutsideLimits((int x, int y) position)
    {
        return position.x < 0 || position.y < 0 || position.x >= levelHeight || position.y >= levelWidth;
    }
}
