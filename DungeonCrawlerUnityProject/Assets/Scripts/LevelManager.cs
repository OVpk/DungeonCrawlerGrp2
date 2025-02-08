using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public Tile currentTile;
    private GameObject currentTileObject;

    public Tile.TileType nextLeftTile { get; private set;}
    public Tile.TileType nextRightTile { get; private set;}
    public Tile.TileType nextDownTile { get; private set;}
    
    [SerializeField] private GameObject shopTilePrefab;
    [SerializeField] private GameObject fightTilePrefab;
    
    [SerializeField] private SpriteRenderer leftIndication;
    [SerializeField] private SpriteRenderer rightIndication;
    [SerializeField] private SpriteRenderer downIndication;

    [SerializeField] private Sprite fightIndication;
    [SerializeField] private Sprite shopIndication;

    private Dictionary<Tile.TileType, Sprite> tileIndications;

    private int consecutiveSameRoomCount = 0;

    [SerializeField] private int maxConsecutiveFightRoom;
    [SerializeField] private int maxConsecutiveShopRoom;
    
    

    public static LevelManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tileIndications = new Dictionary<Tile.TileType, Sprite>
        {
            { Tile.TileType.Fight, fightIndication },
            { Tile.TileType.Shop, shopIndication }
        };

        // Initialisation de la tuile actuelle avec une tuile par défaut
        currentTileObject = Instantiate(shopTilePrefab, Vector3.zero, quaternion.identity); // Par exemple, une tuile Shop
        currentTile = currentTileObject.GetComponent<Tile>();

        // Génération de la première tuile
        Tile.TileType firstTileType = ChoiceTileType();
        GenerateTile(firstTileType);
    }

    public void GenerateTile(Tile.TileType newTileType)
    {
        if (newTileType == currentTile.type)
        {
            consecutiveSameRoomCount++;
        }
        else
        {
            consecutiveSameRoomCount = 0;
        }
        GameObject newTileObject;
        switch (newTileType)
        {
            case Tile.TileType.Fight : newTileObject = fightTilePrefab; break;
            case Tile.TileType.Shop : newTileObject = shopTilePrefab; break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newTileType), newTileType, null);
        }
        Destroy(currentTileObject);
        currentTileObject = Instantiate(newTileObject, Vector3.zero, quaternion.identity);
        currentTile = currentTileObject.GetComponent<Tile>();

        nextLeftTile = ChoiceTileType();
        nextRightTile = ChoiceTileType();
        nextDownTile = ChoiceTileType();

        switch (nextLeftTile)
        {
            case Tile.TileType.Fight : leftIndication.sprite = fightIndication; break;
            case Tile.TileType.Shop : leftIndication.sprite = shopIndication; break;
        }
        
        leftIndication.sprite = tileIndications.ContainsKey(nextLeftTile) ? tileIndications[nextLeftTile] : null;
        rightIndication.sprite = tileIndications.ContainsKey(nextRightTile) ? tileIndications[nextRightTile] : null;
        downIndication.sprite = tileIndications.ContainsKey(nextDownTile) ? tileIndications[nextDownTile] : null;
    }

    public Tile.TileType ChoiceTileType()
    {
        List<Tile.TileType> possibleTypes = DefinePossibleTypes();

        return SelectByProbability(possibleTypes);
    }

    public List<Tile.TileType> DefinePossibleTypes()
    {
        List<Tile.TileType> possibleTypes = new List<Tile.TileType>();

        if (!(currentTile.type == Tile.TileType.Fight && consecutiveSameRoomCount >= maxConsecutiveFightRoom -1))
        {
            possibleTypes.Add(Tile.TileType.Fight);
        }
        if (!(currentTile.type == Tile.TileType.Shop && consecutiveSameRoomCount >= maxConsecutiveShopRoom -1))
        {
            possibleTypes.Add(Tile.TileType.Shop);
        }

        return possibleTypes;
    }

    public int chanceOfFight;
    public int chanceOfShop;
    
    public Tile.TileType SelectByProbability(List<Tile.TileType> possibleTypes)
    {
        
        int fightChance = chanceOfFight;
        int shopChance = chanceOfShop;

        
        int totalChance = 0;

        if (possibleTypes.Contains(Tile.TileType.Fight))
        {
            totalChance += fightChance;
        }
        else
        {
            fightChance = 0;
        }

        if (possibleTypes.Contains(Tile.TileType.Shop))
        {
            totalChance += shopChance;
        }
        else
        {
            shopChance = 0;
        }

        
        int normalizedFightChance = (fightChance * 100) / totalChance;
        int normalizedShopChance = (shopChance * 100) / totalChance;

        
        int rndNb = Random.Range(0, 100);

        
        if (rndNb < normalizedFightChance)
        {
            return Tile.TileType.Fight;
        }
        else if (rndNb < normalizedFightChance + normalizedShopChance)
        {
            return Tile.TileType.Shop;
        }

        
        return possibleTypes[0]; //default return (normally never append)
    }
    
}
