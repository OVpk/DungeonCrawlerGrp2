using System;
using UnityEngine;

public class ShopController : PlayerController
{
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
        ShopManager.Instance.MoveSelector(dx, dy);
    }

    protected override void Press(Buttons button)
    {
        switch (button)
        {
            case Buttons.A : ShopManager.Instance.TryPurchase(); break;
            case Buttons.B : ShopManager.Instance.ExitShop(); break;
        }
    }
}