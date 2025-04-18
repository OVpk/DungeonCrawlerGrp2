using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public enum TurnState
    {
        Player,
        Ai
    }
    public TurnState currentTurn { get; private set; }

    public void SwitchTurn()
    {
        currentTurn = currentTurn == TurnState.Player ? TurnState.Ai : TurnState.Player;
    }

    public CharacterDataInstance[,] playerGrid { get; private set; } = new CharacterDataInstance[2, 3];
    public EnemyDataInstance[,] enemyGrid {get ; private set;} = new EnemyDataInstance[3, 3];

    public CharacterData characterTest;
    public CharacterData characterTest2;

    public static FightManager Instance;

    [field:SerializeField] public FightDisplayer displayer { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        displayer.InitDisplayedGrid(playerGrid);
        displayer.InitDisplayedGrid(enemyGrid);
        
        playerGrid[0, 0] = (CharacterDataInstance)characterTest.Instance();
        playerGrid[0, 1] = (CharacterDataInstance)characterTest.Instance();
        playerGrid[0, 2] = (CharacterDataInstance)characterTest.Instance();
        playerGrid[1, 0] = (CharacterDataInstance)characterTest2.Instance();
        playerGrid[1, 1] = (CharacterDataInstance)characterTest2.Instance();
        playerGrid[1, 2] = (CharacterDataInstance)characterTest2.Instance();
    }

    public AttackStageData FindBestUnlockedStage(AttackData attack)
    {
        AttackStageData bestStage = attack.attackStages[0];
        for (int i = 1; i < attack.attackStages.Length; i++)
        {
            if (!attack.attackStages[i].IsUnlock(playerGrid, enemyGrid)) break;
            bestStage = attack.attackStages[i];
        }
        return bestStage;
    }

    public bool IsPatternOutsideLimit(EntityDataInstance[,] gridToCheck, (int x, int y) originPosition, List<Vector2Int> pattern)
    {
        foreach (var position in pattern)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            if (IsOutsideLimit(gridToCheck, positionInGrid)) return true;
        }
        return false;
    }

    public bool IsOutsideLimit(EntityDataInstance[,] gridToCheck, (int x, int y) position)
    {
        return position.x < 0 ||
               position.y < 0 ||
               position.x >= gridToCheck.GetLength(0) ||
               position.y >= gridToCheck.GetLength(1);
    }

    public void ApplyAttackPattern(EntityDataInstance[,] gridToApply, (int x, int y) originPosition, AttackStageData attackToApply)
    {
        foreach (var position in attackToApply.pattern.positions)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            ApplyDamageAtPosition(gridToApply, positionInGrid, attackToApply.damage);
        }
    }

    private void ApplyDamageAtPosition(EntityDataInstance[,] gridToApply, (int x, int y) position, int damages)
    {
        EntityDataInstance entityAtPosition = gridToApply[position.x, position.y];
        if (entityAtPosition == null) return;
        EntityTakeDamage(entityAtPosition, damages);
    }

    private void EntityTakeDamage(EntityDataInstance entity, int damages)
    {
        entity.durability -= damages;

        if (entity.durability <= 0)
        {
            switch (entity)
            {
                case CharacterDataInstance character : CharacterDeath(ref character); break;
                case EnemyDataInstance enemy : EnemyDeath(enemy); break;
            }
        }
    }

    private void CharacterDeath(ref CharacterDataInstance character)
    {
        if (character.nextLayer == null) character = null;
        else character = (CharacterDataInstance)character.nextLayer.Instance();
    }
    
    private void EnemyDeath(EnemyDataInstance enemy)
    {
        
    }

    private void PlaceCharacterAtPosition(CharacterDataInstance character, (int x, int y) position)
    {
        playerGrid[position.x, position.y] = character;
    }
    
    
    
}
