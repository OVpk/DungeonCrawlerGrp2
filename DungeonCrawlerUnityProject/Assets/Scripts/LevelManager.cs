using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Tile[,] level { get; private set; }

    [SerializeField] private int height;
    [SerializeField] private int width;

    public Tile currentTile;
    
    [SerializeField] private GameObject shopTilePrefab;
    [SerializeField] private GameObject fightTilePrefab;

    public static LevelManager Instance;
    
    private void Awake()
    {
        level = new Tile[height, width];
        Instance = this;
    }

    private void Start()
    {
        (int x, int y) firstTilePosition = (height, width / 2);
        currentTile = GenerateTile(firstTilePosition);
    }

    public Tile GenerateTile((int x, int y) position)
    {
        return level[position.x, position.y];
    }

    public void DisplayTile()
    {
        
    }

    public bool IsOutSideLimits((int x, int y) position)
    {
        if (position.x < 0 || position.x > height) return true;
        if (position.y < 0 || position.y > width) return true;
        return false;
    }
    
}
