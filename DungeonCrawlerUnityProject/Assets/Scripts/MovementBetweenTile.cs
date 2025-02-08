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
    

    public enum Directions
    {
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
            case "left" : SwitchTile(Directions.Left); break;
            case "right" : SwitchTile(Directions.Right); break;
        }
    }

    private void SwitchTile(Directions direction)
    {
        Tile.TileType nextTileType;
        switch (direction)
        {
            case Directions.Down: nextTileType = LevelManager.Instance.nextDownTile; break;
            case Directions.Left: nextTileType = LevelManager.Instance.nextLeftTile; break;
            case Directions.Right: nextTileType = LevelManager.Instance.nextRightTile; break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
        
        LevelManager.Instance.GenerateTile(nextTileType);
        
    }
    
}
