using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightAreaController : PlayerController
{
    
    [SerializeField] private UIFightController uiController;

    [SerializeField] private ComplexCarousel attackSelectorController;
    
    private (int x, int y) playerGridSelectorPosition = (0,0);
    private (int x, int y) attackOriginPosition = (0,0);

    private int currentPackIndex => FightManager.Instance.packDisplayer.currentIndex;

    private CandyPack selectedPack;

    private CharacterDataInstance selectedCharacter;

    public HoveredInfoController hoveredInfoController;

    public ButtonHelpController buttonHelpController;

    private int currentAttackIndex => attackSelectorController.currentAttack;
    private int currentStageIndex => attackSelectorController.currentAttackStage;

    public enum SelectorState
    {
        OnPlayerGrid,
        SelectAttack,
        SelectAttackPosition,
        SelectPack,
        PlaceCharacter
    }

    private SelectorState currentState = SelectorState.SelectPack;
    
    
    

    protected override void Move(Directions direction)
    {
        if (!FightManager.Instance.canUseControlls) return;
        
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
            case SelectorState.SelectAttack : MoveAttackSelector(directionToGo); break;
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

    private void DontWantSelectAttack()
    {
        List<Vector2Int> obsoletePattern = selectedCharacter.attacks[currentAttackIndex].attackStages[currentStageIndex].pattern.positions;
        FightManager.Instance.sendInformation.EntitiesNoLongerTargetedByPatternAt(attackOriginPosition, obsoletePattern, selectedCharacter.attacks[currentAttackIndex].gridToApply);
        
        FightManager.Instance.sendInformation.EntityNoLongerSelectedAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        FightManager.Instance.sendInformation.EntityHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        SwitchState(SelectorState.OnPlayerGrid);
        uiController.SwitchAB();
    }

    private void SelectPack()
    {
        selectedPack = GameManager.Instance.candyPacks[currentPackIndex];
        FightManager.Instance.sendInformation.EntityHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        hoveredInfoController.UpdateInformations(FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y]);
        SwitchState(SelectorState.PlaceCharacter);
    }

    private void UnselectPack()
    {
        SwitchState(SelectorState.SelectPack);
    }

    private void PlaceCharacter()
    {
        FightManager.Instance.PlaceCharacterFromPack(selectedPack, playerGridSelectorPosition);
        hoveredInfoController.UpdateInformations(FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y]);
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
        
        hoveredInfoController.UpdateInformations(FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y]);
    }

    private void MoveAttackPattern((int x, int y) directionToGo)
    {
        
        List<Vector2Int> pattern = FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attacks[currentAttackIndex]).pattern.positions;
        if (FightManager.Instance.IsOutsideLimit(
                selectedCharacter.attacks[currentAttackIndex].gridToApply == FightManager.TurnState.Player 
                    ? FightManager.Instance.playerGrid 
                    :FightManager.Instance.enemyGrid,
                (attackOriginPosition.x + directionToGo.x, attackOriginPosition.y+ directionToGo.y))) return;

        FightManager.Instance.sendInformation.EntitiesNoLongerTargetedByPatternAt(attackOriginPosition, pattern, selectedCharacter.attacks[currentAttackIndex].gridToApply);
        
        if (!selectedCharacter.attacks[currentAttackIndex].isPositionLocked)
            attackOriginPosition = (attackOriginPosition.x + directionToGo.x, attackOriginPosition.y + directionToGo.y);
        
        FightManager.Instance.sendInformation.EntitiesTargetedByPatternAt(attackOriginPosition, pattern, selectedCharacter.attacks[currentAttackIndex].gridToApply);
    }

    private void SelectCharacter()
    {
        if (FightManager.Instance.IsPositionAlreadyPlayed(playerGridSelectorPosition)) return;
        selectedCharacter = FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y];
        if (selectedCharacter == null) return;
        if (selectedCharacter.effects.Contains(EntityData.EntityEffects.Glue)) return;
        
        FightManager.Instance.sendInformation.EntityNoLongerHoveredAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        FightManager.Instance.sendInformation.EntitySelectedAt(playerGridSelectorPosition, FightManager.TurnState.Player);
        
        SwitchState(SelectorState.SelectAttack);
        
        uiController.SwitchAB();
        attackSelectorController.LoadData(selectedCharacter.attacks);
        SetAttackOriginPosition();
        MoveAttackPattern((0,0));
    }

    private void SetAttackOriginPosition()
    {
        attackOriginPosition = selectedCharacter.attacks[currentAttackIndex].isPositionLocked 
            ? playerGridSelectorPosition 
            : (0, 0);
    }

    private void MoveAttackSelector((int x, int y) directionToGo)
    {
        List<Vector2Int> pattern = selectedCharacter.attacks[currentAttackIndex].attackStages[currentStageIndex].pattern.positions;
        FightManager.Instance.sendInformation.EntitiesNoLongerTargetedByPatternAt(attackOriginPosition, pattern, selectedCharacter.attacks[currentAttackIndex].gridToApply);
        if (directionToGo == (0, 1)) attackSelectorController.ScrollHorizontal(1);
        if (directionToGo == (0, -1)) attackSelectorController.ScrollHorizontal(-1);
        if (directionToGo == (1, 0)) attackSelectorController.ScrollVertical(1);
        if (directionToGo == (-1, 0)) attackSelectorController.ScrollVertical(-1);
        SetAttackOriginPosition();
        
        List<Vector2Int> newPattern = selectedCharacter.attacks[currentAttackIndex].attackStages[currentStageIndex].pattern.positions;
        FightManager.Instance.sendInformation.EntitiesTargetedByPatternAt(attackOriginPosition, newPattern, selectedCharacter.attacks[currentAttackIndex].gridToApply);
    }

    private void SelectAttack()
    {
        if (FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attacks[currentAttackIndex]) == null) return;
        
        SwitchState(SelectorState.SelectAttackPosition);
        
        List<Vector2Int> obsoletePattern = selectedCharacter.attacks[currentAttackIndex].attackStages[currentStageIndex].pattern.positions;
        FightManager.Instance.sendInformation.EntitiesNoLongerTargetedByPatternAt(attackOriginPosition, obsoletePattern, selectedCharacter.attacks[currentAttackIndex].gridToApply);
        
        attackSelectorController.FixStageToBest();
        SetAttackOriginPosition();
        MoveAttackPattern((0,0));
    }

    private void CharacterLooseLayer()
    {
        if (FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y] == null) return;
        FightManager.Instance.BreakLayerAt(playerGridSelectorPosition);
        if (FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y] ==
            null)
        {
            hoveredInfoController.UpdateInformations(null);
        }
        else
        {
            hoveredInfoController.UpdateInformations((CharacterDataInstance)FightManager.Instance.playerGrid[playerGridSelectorPosition.x, playerGridSelectorPosition.y].nextLayer.Instance());
        }
    }

    private void CancelAttack()
    {
        List<Vector2Int> pattern = FightManager.Instance.FindBestUnlockedStage(selectedCharacter.attacks[currentAttackIndex]).pattern.positions;
        
        FightManager.Instance.sendInformation.EntitiesNoLongerTargetedByPatternAt(attackOriginPosition, pattern, selectedCharacter.attacks[currentAttackIndex].gridToApply);
        
        SwitchState(SelectorState.SelectAttack);
        
        attackSelectorController.FixStageToBest();
        SetAttackOriginPosition();
        MoveAttackPattern((0,0));
    }

    private void DoAttack()
    {
        FightManager.Instance.canUseControlls = false;
        DontWantSelectAttack();
        StartCoroutine(FightManager.Instance.Attack(playerGridSelectorPosition, currentAttackIndex, attackOriginPosition, FightManager.TurnState.Player));
    }

    private void SwitchState(SelectorState newState)
    {
        currentState = newState;
        buttonHelpController.DisplayHelp(currentState);
    }
    
    

    protected override void Press(Buttons button)
    {
        if (!FightManager.Instance.canUseControlls) return;
        
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
            case SelectorState.SelectAttack : SelectAttack(); break;
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
            case SelectorState.SelectAttack : DontWantSelectAttack(); break;
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
