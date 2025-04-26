using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightAreaController : PlayerController
{
    [SerializeField] private UIFightController uiController;
    
    private (int x, int y) playerGridSelectorPosition = (0,0);
    private (int x, int y) attackOriginPosition = (0,0);

    private int currentPackIndex => FightManager.Instance.packDisplayer.currentIndex;

    private CandyPackDataInstance selectedPack;

    private CharacterDataInstance selectedCharacter;

    enum SelectorState
    {
        OnPlayerGrid,
        SelectAttackPosition,
        SelectPack,
        PlaceCharacter
    }

    private SelectorState currentState = SelectorState.SelectPack;
    

    protected override void Move(Directions direction)
    {
        if (FightManager.Instance.currentTurn == FightManager.TurnState.Enemy) return;
        
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
            case SelectorState.SelectPack : MovePack(directionToGo); break;
            case SelectorState.PlaceCharacter : MoveSelector(directionToGo); break;
        }
        
    }

    private void WantSeePack()
    {
        SwitchState(SelectorState.SelectPack);
        uiController.ToggleC();
    }

    private void DontWantSeePack()
    {
        SwitchState(SelectorState.OnPlayerGrid);
        uiController.ToggleC();
    }

    private void SelectPack()
    {
        selectedPack = FightManager.Instance.candyPack[currentPackIndex];
        FightManager.Instance.sendInformation.EntityHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        SwitchState(SelectorState.PlaceCharacter);
    }

    private void UnselectPack()
    {
        SwitchState(SelectorState.SelectPack);
    }

    private void PlaceCharacter()
    {
        FightManager.Instance.PlaceCharacterFromPack(selectedPack, playerGridSelectorPosition);
    }

    private void MovePack((int x, int y) directionToGo)
    {
        if (directionToGo == (0, 1)) FightManager.Instance.packDisplayer.ScrollRight();
        if (directionToGo == (0, -1)) FightManager.Instance.packDisplayer.ScrollLeft();
    }

    private void MoveSelector((int x, int y) directionToGo)
    {
        if (FightManager.Instance.IsOutsideLimit(FightManager.Instance.playerGrid, (playerGridSelectorPosition.x+directionToGo.x, playerGridSelectorPosition.y+directionToGo.y))) return;
        
        FightManager.Instance.sendInformation.EntityNoLongerHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
            
        playerGridSelectorPosition = (playerGridSelectorPosition.x + directionToGo.x, playerGridSelectorPosition.y + directionToGo.y);
        
        FightManager.Instance.sendInformation.EntityHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
    }

    private void MoveAttackPattern((int x, int y) directionToGo)
    {
        List<Vector2Int> pattern = FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attack).pattern.positions;
        if (FightManager.Instance.IsPatternOutsideLimit(FightManager.Instance.enemyGrid, (attackOriginPosition.x+directionToGo.x, attackOriginPosition.y+directionToGo.y), pattern)) return;

        FightManager.Instance.sendInformation.EntitiesNoLongerTargetedByPatternAt(attackOriginPosition, pattern, selectedCharacter.attack.gridToApply);
        
        attackOriginPosition = (attackOriginPosition.x + directionToGo.x, attackOriginPosition.y + directionToGo.y);
        
        FightManager.Instance.sendInformation.EntitiesTargetedByPatternAt(attackOriginPosition, pattern, selectedCharacter.attack.gridToApply);
    }

    private void SelectCharacter()
    {
        if (FightManager.Instance.IsPositionAlreadyPlayed(playerGridSelectorPosition)) return;
        selectedCharacter = FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y];
        if (selectedCharacter == null) return;
        
        FightManager.Instance.sendInformation.EntityNoLongerHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        FightManager.Instance.sendInformation.EntitySelectedAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        
        SwitchState(SelectorState.SelectAttackPosition);
        
        MoveAttackPattern(attackOriginPosition);
    }

    private void CharacterLooseLayer()
    {
        FightManager.Instance.BreakLayerAt(playerGridSelectorPosition);
    }

    private void CancelAttack()
    {
        List<Vector2Int> pattern = FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attack).pattern.positions;
        
        FightManager.Instance.sendInformation.EntitiesNoLongerTargetedByPatternAt(attackOriginPosition, pattern, selectedCharacter.attack.gridToApply);
        FightManager.Instance.sendInformation.EntityNoLongerSelectedAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        FightManager.Instance.sendInformation.EntityHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        
        SwitchState(SelectorState.OnPlayerGrid);
    }

    private void DoAttack()
    {
        CancelAttack();
        
        StartCoroutine(FightManager.Instance.Attack(playerGridSelectorPosition, attackOriginPosition, FightManager.TurnState.Player));
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
        if (FightManager.Instance.currentTurn == FightManager.TurnState.Enemy) return;
        
        switch (button)
        {
            case Buttons.A : PressA(); break;
            case Buttons.B : PressB(); break;
            case Buttons.X : PressX(); break;
            case Buttons.Y : PressY(); break;
        }
    }

    private void PressA()
    {
        switch (currentState)
        {
            case SelectorState.OnPlayerGrid : SelectCharacter(); break;
            case SelectorState.SelectAttackPosition : DoAttack(); break;
            case SelectorState.SelectPack : SelectPack(); break;
            case SelectorState.PlaceCharacter : PlaceCharacter(); break;
        }
    }

    private void PressB()
    {
        switch (currentState)
        {
            case SelectorState.OnPlayerGrid : ; break;
            case SelectorState.SelectAttackPosition : CancelAttack(); break;
            case SelectorState.SelectPack : DontWantSeePack(); break;
            case SelectorState.PlaceCharacter : UnselectPack(); break;
        }
    }
    
    private void PressX()
    {
        switch (currentState)
        {
            case SelectorState.OnPlayerGrid : CharacterLooseLayer(); break;
            case SelectorState.SelectAttackPosition : ; break;
        }
    }

    private void PressY()
    {
        switch (currentState)
        {
            case SelectorState.OnPlayerGrid : WantSeePack(); break;
            case SelectorState.SelectAttackPosition : ; break;
        }
    }
    
}
