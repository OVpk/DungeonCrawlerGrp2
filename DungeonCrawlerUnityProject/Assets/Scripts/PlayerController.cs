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
    
    public void DPadAction(InputAction.CallbackContext context)
    {
        Directions selectedDirection = context.control.name switch
        {
            "down" => Directions.Down,
            "up" => Directions.Up,
            "left" => Directions.Left,
            "right" => Directions.Right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
}
