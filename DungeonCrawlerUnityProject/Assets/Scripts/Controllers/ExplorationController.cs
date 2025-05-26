using System;

public class ExplorationController : PlayerController
{
    private (int x, int y) cursorPosition = (0,0);
    
    protected override void Move(Directions direction)
    {
        (int x, int y) directionToGo = direction switch
        {
            Directions.Right => (0, 1),
            Directions.Left => (1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
        cursorPosition = directionToGo;
        ExplorationManager.Instance.MoveAreaSelector(cursorPosition);
    }

    protected override void Press(Buttons button)
    {
        switch (button)
        {
            case Buttons.A :
                EnterArea();
                break;
        }
    }

    private void EnterArea()
    {
        if (cursorPosition == (0,0)) return;
        
        ExplorationManager.Instance.EnterArea((
            ExplorationManager.Instance.currentAreaPosition.x + cursorPosition.x,
            ExplorationManager.Instance.currentAreaPosition.y + cursorPosition.y
            ));
        cursorPosition = (0, 0);
    }
}
