using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightAreaController : PlayerController
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
        selectorPosition = (selectorPosition.x + directionToGo.x, selectorPosition.y + directionToGo.y);
    }

    protected override void Press(Buttons button)
    {
        throw new System.NotImplementedException();
    }
}
