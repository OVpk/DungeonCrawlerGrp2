using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
    public enum Directions
    {
        Down,
        Right,
        Left,
        Up
    }
    
    public void MovementAction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
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

    protected abstract void Move(Directions direction);

}
