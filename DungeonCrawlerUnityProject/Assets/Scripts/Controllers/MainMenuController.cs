using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : PlayerController
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
        MainMenuManager.Instance.MoveSelector(directionToGo.x);
    }
    
    protected override void Press(Buttons button)
    {
        switch (button)
        {
            case Buttons.A : MainMenuManager.Instance.ApplyOption(); break;
        }
    }
    
}
