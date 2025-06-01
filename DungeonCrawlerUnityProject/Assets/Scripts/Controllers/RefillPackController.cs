using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillPackController : PlayerController
{
    private int currentPackIndex => RefillPackManager.Instance.packDisplayer.currentIndex;

    protected override void Move(Directions direction)
    {
        (int x, int y) directionToGo = direction switch
        {
            Directions.Down => (-1, 0),
            Directions.Up => (1, 0),
            Directions.Right => (0, 1),
            Directions.Left => (0, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
        MovePack(directionToGo);
    }

    private void SelectPack()
    {
        CandyPack selectedPack = GameManager.Instance.candyPacks[currentPackIndex];
        RefillPackManager.Instance.RefillPack(selectedPack);
    }

    private void MovePack((int x, int y) directionToGo)
    {
        if (directionToGo == (0, 1)) RefillPackManager.Instance.packDisplayer.ScrollRight();
        if (directionToGo == (0, -1)) RefillPackManager.Instance.packDisplayer.ScrollLeft();
    }
    
    protected override void Press(Buttons button)
    {
        switch (button)
        {
            case Buttons.A : SelectPack(); break;
            case Buttons.B : RefillPackManager.Instance.ReturnToExploration(); break;
        }
    }
    
}
