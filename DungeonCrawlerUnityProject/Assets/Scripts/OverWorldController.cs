using System;
using UnityEngine;

public class OverWorldController : PlayerController
{
    private (int x, int y) selectorPosition = (0,0);
    protected override void Move(Directions direction)
    {
        (int x, int y) directionToGo = direction switch
        {
            Directions.Down => (1, 0),
            Directions.Up => (-1, 0),
            Directions.Right => (0, 1),
            Directions.Left => (0, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
        if (LevelManager.Instance.levelDisplayer.IsOutsideLimits((selectorPosition.x+directionToGo.x, selectorPosition.y+directionToGo.y))) return;
        LevelManager.Instance.levelDisplayer.MoveSelector(selectorPosition,directionToGo);
        selectorPosition = (selectorPosition.x + directionToGo.x, selectorPosition.y + directionToGo.y);
        
        Debug.Log(selectorPosition.x+" , "+selectorPosition.y);
    }


}
