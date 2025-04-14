using System;

public class OverWorldController : PlayerController
{
    private LevelManager levelManager => LevelManager.Instance;
    private LevelDisplayer levelDisplayer => LevelManager.Instance.levelDisplayer;
    
    private (int x, int y) selectorPosition = (0,0);
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
        if (levelDisplayer.IsOutsideLimits((selectorPosition.x+directionToGo.x, selectorPosition.y+directionToGo.y))) return;
        levelDisplayer.MoveSelector(selectorPosition,directionToGo);
        selectorPosition = (selectorPosition.x + directionToGo.x, selectorPosition.y + directionToGo.y);
    }

    protected override void Press(Buttons button)
    {
        switch (button)
        {
            case Buttons.A :
                levelManager.EnterArea((levelManager.currentAreaPosition.x+selectorPosition.x, levelManager.currentAreaPosition.y+selectorPosition.y));
                break;
        }
    }
}
