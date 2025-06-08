using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopedieController : PlayerController
{
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
        EncyclopedieManager.Instance.MoveTicketContainer(directionToGo.x);
    }
    
    protected override void Press(Buttons button)
    {
        switch (button)
        {
            case Buttons.B : GameManager.Instance.ChangeGameState(GameManager.GameState.InFightArea); break;
        }
    }
    
}
