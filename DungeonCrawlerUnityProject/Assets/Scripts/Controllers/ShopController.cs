using System;
using UnityEngine;

public class ShopController : PlayerController
{

    public ShopManager manager;
    private (int x, int y) selectorPosition = (0,0);

    private void Start()
    {
       ChangeActiveState(true);
    }

    protected override void Move(Directions direction)
    {
        (int dx, int dy) = direction switch
        {
            Directions.Left  => (-1,  0),
            Directions.Right => ( 1,  0),
            Directions.Up    => ( 0, -1),
            Directions.Down  => ( 0,  1),
            _ => (0,0)
        };
        manager.MoveSelector(dx, dy);
    }

    protected override void Press(Buttons button)
    {
        if (button == Buttons.A)
        {
            manager.TryPurchase();
        }
    }
}