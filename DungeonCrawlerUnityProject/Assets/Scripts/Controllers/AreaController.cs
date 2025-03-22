using System;

public class AreaController : PlayerController
{
    protected override void Move(Directions direction)
    {
        throw new NotImplementedException();
    }

    protected override void Press(Buttons button)
    {
        switch (button)
        {
            case Buttons.B :
                LevelManager.Instance.ExitArea();
                break;
        }
    }
}
