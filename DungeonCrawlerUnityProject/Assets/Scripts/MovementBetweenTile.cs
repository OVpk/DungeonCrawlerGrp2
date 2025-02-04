using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class MovementBetweenTile : MonoBehaviour
{

    public bool canMove = true;

    private Tile currentTile;

    private void Start()
    {
        currentTile = GenerateTile(Directions.Up);
    }

    public enum Directions
    {
        Up,
        Down,
        Right,
        Left
    }
    
    public void DPadAction(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        canMove = false;
        switch (context.control.name)
        {
            case "down" : SwitchTile(Directions.Down); break;
            case "up" : SwitchTile(Directions.Up); break;
            case "left" : SwitchTile(Directions.Left); break;
            case "right" : SwitchTile(Directions.Right); break;
        }
    }

    private void SwitchTile(Directions direction)
    {
        Tile nextTile = null;
        
        switch (direction)
        {
            case Directions.Down: nextTile = currentTile.downNeighbor; break;
            case Directions.Up: nextTile = currentTile.upNeighbor; break;
            case Directions.Left: nextTile = currentTile.leftNeighbor; break;
            case Directions.Right: nextTile = currentTile.rightNeighbor; break;
        }
        
        if (nextTile == null)
        {
            nextTile = GenerateTile(direction);
        }
        
        currentTile = nextTile;
    }

    [SerializeField] private GameObject shopTilePrefab;
    [SerializeField] private GameObject fightTilePrefab;
    
    private Tile GenerateTile(Directions direction)
    {
        Tile.TileType newTileType = (Random.value > 0.5f) ? Tile.TileType.Shop : Tile.TileType.Fight;
        GameObject newTilePrefab;
        switch (newTileType)
        {
            case Tile.TileType.Fight : newTilePrefab = fightTilePrefab; break;
            case Tile.TileType.Shop : newTilePrefab = shopTilePrefab; break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GameObject newTileObject = Instantiate(newTilePrefab, Vector3.zero, quaternion.identity);
        Tile newTile = newTileObject.AddComponent<Tile>();
        
        newTile.Initialize(newTileType);

        if (currentTile == null) return newTile;
        
        switch (direction)
        {
            case Directions.Down:
                currentTile.SetNeighbor(newTile, Directions.Down);
                newTile.SetNeighbor(currentTile, Directions.Up);
                break;
            case Directions.Up:
                currentTile.SetNeighbor(newTile, Directions.Up);
                newTile.SetNeighbor(currentTile, Directions.Down);
                break;
            case Directions.Left:
                currentTile.SetNeighbor(newTile, Directions.Left);
                newTile.SetNeighbor(currentTile, Directions.Right);
                break;
            case Directions.Right:
                currentTile.SetNeighbor(newTile, Directions.Right);
                newTile.SetNeighbor(currentTile, Directions.Left);
                break;
        }

        return newTile;
    }
    
}
