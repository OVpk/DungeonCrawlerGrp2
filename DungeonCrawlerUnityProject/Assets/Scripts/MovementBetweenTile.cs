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
        (int x, int y) nextTilePosition = LevelManager.Instance.currentTile.coordinates;
        switch (direction)
        {
            case Directions.Down: nextTilePosition.x -= 1; break;
            case Directions.Up: nextTilePosition.x += 1; break;
            case Directions.Left: nextTilePosition.y -= 1; break;
            case Directions.Right: nextTilePosition.y += 1; break;
        }
        
        if (LevelManager.Instance.IsOutSideLimits(nextTilePosition)) return;
        
        LevelManager.Instance.GenerateTile(nextTilePosition);
        
        Tile nextTile = LevelManager.Instance.level[nextTilePosition.x, nextTilePosition.y];
        LevelManager.Instance.currentTile = nextTile;
    }
    
}
