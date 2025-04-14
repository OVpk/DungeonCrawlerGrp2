using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class FightAreaController : PlayerController
{
    
    private (int x, int y) playerGridSelectorPosition = (0,0);
    private (int x, int y) attackOriginPosition = (0,0);

    private CharacterDataInstance selectedCharacter;

    enum SelectorState
    {
        OnPlayerGrid,
        SelectAttackPosition
    }

    private SelectorState currentState = SelectorState.OnPlayerGrid;
    
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
        switch (currentState)
        {
            case SelectorState.OnPlayerGrid : MoveSelector(directionToGo); break;
            case SelectorState.SelectAttackPosition : MoveAttackPattern(directionToGo); break;
        }
        
    }

    private void MoveSelector((int x, int y) directionToGo)
    {
        if (FightManager.Instance.IsOutsideLimit(FightManager.Instance.playerGrid, (playerGridSelectorPosition.x+directionToGo.x, playerGridSelectorPosition.y+directionToGo.y))) return;
        
        playerGridSelectorPosition = (playerGridSelectorPosition.x + directionToGo.x, playerGridSelectorPosition.y + directionToGo.y);
        FightManager.Instance.displayer.MoveSelectorDisplayTo(playerGridSelectorPosition);
    }

    private void MoveAttackPattern((int x, int y) directionToGo)
    {
        List<Vector2Int> pattern = FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attack).pattern.positions;
        if (FightManager.Instance.IsPatternOutsideLimit(FightManager.Instance.enemyGrid, (attackOriginPosition.x+directionToGo.x, attackOriginPosition.y+directionToGo.y), pattern)) return;

        attackOriginPosition = (attackOriginPosition.x + directionToGo.x, attackOriginPosition.y + directionToGo.y);
        
        FightManager.Instance.displayer.DisplayPattern(attackOriginPosition, pattern);
    }

    private void SelectCharacterAt((int x, int y) position)
    {
        selectedCharacter = FightManager.Instance.playerGrid[position.x, position.y];
        FightManager.Instance.displayer.DisplayPattern(attackOriginPosition, FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attack).pattern.positions);
        
        SwitchState(SelectorState.SelectAttackPosition);

    }

    private void CancelAttack()
    {
        FightManager.Instance.displayer.CleanPatternDisplay();
        
        SwitchState(SelectorState.OnPlayerGrid);
    }

    private void DoAttackAt((int x, int y) position)
    {
        FightManager.Instance.ApplyAttackPattern(FightManager.Instance.enemyGrid, position, FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attack));
        FightManager.Instance.SwitchTurn();
    }

    private void SwitchState(SelectorState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case SelectorState.SelectAttackPosition :
                attackOriginPosition = (0, 0);
                break;
        }
    }
    
    

    protected override void Press(Buttons button)
    {
        if (FightManager.Instance.currentTurn == FightManager.TurnState.Ai) return;
        
        switch (button)
        {
            case Buttons.A : PressA(); break;
            case Buttons.B : PressB(); break;
        }
    }

    private void PressA()
    {
        switch (currentState)
        {
            case SelectorState.OnPlayerGrid : SelectCharacterAt(playerGridSelectorPosition); break;
            case SelectorState.SelectAttackPosition : DoAttackAt(attackOriginPosition); break;
        }
    }

    private void PressB()
    {
        switch (currentState)
        {
            case SelectorState.OnPlayerGrid : ; break;
            case SelectorState.SelectAttackPosition : CancelAttack(); break;
        }
    }
}
