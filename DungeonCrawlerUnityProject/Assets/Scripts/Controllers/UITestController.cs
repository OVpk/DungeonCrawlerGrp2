using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class UITestController : PlayerController
{
    public TestUIscript UI;
    private void Start()
    {
        UI = gameObject.GetComponent<TestUIscript>();
    }

    protected override void Move(Directions direction)
    {
        if (FightManager.Instance.currentTurn == FightManager.TurnState.Ai) return;

        (int x, int y) directionToGo = direction switch
        {
            Directions.Down => (-1, 0),
            Directions.Up => (1, 0),
            Directions.Right => (0, 1),
            Directions.Left => (0, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };


    }



    protected override void Press(Buttons button)
    {
        
        switch (button)
        {
            case Buttons.A : 
                return;
        }
    }
    
}
