using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    private AreaData[,] level;
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
    private (int x, int y) currentAreaPosition;

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
    }

    private void Start()
    {
        InitSeed();
        
        level = new AreaData[levelHeight,levelWidth];
        currentAreaPosition = (0, levelWidth / 2);
        GenerateAreaAtPosition(currentAreaPosition);
        currentArea = level[currentAreaPosition.x, currentAreaPosition.y];

        GenerateNextAreas();
        Display();
    }


    private void InitSeed(int? customSeed = null)
    {
        seed = customSeed ?? System.DateTime.Now.Ticks.GetHashCode();
        Random.InitState(seed);
        Debug.Log($"Seed utilisée : {seed}");
    }



    [SerializeField] private GameObject areaIconPrefab;
    private SpriteRenderer[,] displayedAreas = new SpriteRenderer[3, 3];

    private void Display()
    {
        InitDisplay();
        DisplayAreas();
    }

    private void InitDisplay()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                displayedAreas[i,j] = 
                    Instantiate(areaIconPrefab, new Vector3(j, 2 - i, 0) * 6, quaternion.identity).GetComponent<SpriteRenderer>();
            }
        }
    }

    private void DisplayAreas()
    {
        displayedAreas[0, 1].sprite = level[currentAreaPosition.x, currentAreaPosition.y].iconDistance1;
        displayedAreas[0, 2].sprite = level[currentAreaPosition.x, currentAreaPosition.y + 1].iconDistance1;
        displayedAreas[0, 0].sprite = level[currentAreaPosition.x, currentAreaPosition.y - 1].iconDistance1;
        displayedAreas[1, 1].sprite = level[currentAreaPosition.x + 1, currentAreaPosition.y].iconDistance1;
    }

    public void GenerateAreaAtPosition((int x, int y) position)
    {
        if (level[position.x, position.y] != null) return;
        
        Type newAreaType = ChoiceAreaType();

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

    private bool IsOutsideLimits((int x, int y) position)
    {
        return position.x < 0 || position.y < 0 || position.x >= levelWidth || position.y >= levelHeight;
    }

    public void GenerateNextAreas()
    {
        (int x, int y)[] directions = 
        {
            (0, 1),  // Droite
            (0, -1), // Gauche
            (1, 0),  // Bas
        };

        foreach (var direction in directions)
        {
            (int x, int y) newPosition = (currentAreaPosition.x + direction.x, currentAreaPosition.y + direction.y);
            
            if (!IsOutsideLimits(newPosition))
            {
                GenerateAreaAtPosition(newPosition);
            }
        }
    }
    
    public bool HaveReachConsecutiveSameAreaLimit((int x, int y) position, Type areaType, int maxCount)
    {
        if (maxCount <= 0) return true;

        (int x, int y)[] directions = 
        {
            (0, 1),  // Droite
            (0, -1), // Gauche
            (1, 0),  // Bas
            (-1, 0)  // Haut
        };
        
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

    private Type ChoiceAreaType()
    {
        List<Type> possibleTypes = DefinePossibleTypes();
        return SelectByProbability(possibleTypes);
    }

    private List<Type> DefinePossibleTypes()
    {
        List<Type> possibleTypes = new List<Type>();
        /*
        ajouter condition pour chaque ajout
        */
        possibleTypes.Add(typeof(FightAreaData));
        possibleTypes.Add(typeof(ShopAreaData));

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
