using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
    private bool isActive;

    protected enum Directions
    {
        Down,
        Right,
        Left,
        Up
    }

    protected enum Buttons
    {
        A,
        B,
        X,
        Y,
        Menu
    }
    
    public void ChangeActiveState(bool state)
    {
        isActive = state;
    }
    
    public void MovementAction(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed) return;
        
        Directions selectedDirection = context.control.name switch
        {
            "down" => Directions.Down,
            "up" => Directions.Up,
            "left" => Directions.Left,
            "right" => Directions.Right,
            _ => throw new ArgumentOutOfRangeException()
        };
        Move(selectedDirection);
    }

    public void ButtonAction(InputAction.CallbackContext context)
    {
        if (!isActive || !context.performed) return;
        
        Buttons pressedButton = context.control.name switch
        {
            "buttonSouth" => Buttons.A,
            "buttonEast" => Buttons.B,
            "buttonWest" => Buttons.X,
            "buttonNorth" => Buttons.Y,
            "start" => Buttons.Menu,
            _ => throw new ArgumentOutOfRangeException()
        };
        Press(pressedButton);
    }

    protected abstract void Move(Directions direction);
    protected abstract void Press(Buttons button);

}
