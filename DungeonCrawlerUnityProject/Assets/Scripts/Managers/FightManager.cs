using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public enum TurnState
    {
        Player,
        Enemy
    }
    public TurnState currentTurn { get; private set; }

    public void SwitchTurn()
    {
        currentTurn = currentTurn == TurnState.Player ? TurnState.Enemy : TurnState.Player;
    }

    public CharacterDataInstance[,] playerGrid { get; private set; } = new CharacterDataInstance[2, 3];
    public EnemyDataInstance[,] enemyGrid {get ; private set;} = new EnemyDataInstance[3, 3];

    private HashSet<(int x, int y)> playerAlreadyPlayedPositions = new HashSet<(int x, int y)>();
    private HashSet<(int x, int y)> enemyAlreadyPlayedPositions = new HashSet<(int x, int y)>();

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

    private void AddPositionToAlreadyPlayed((int x, int y) position,  TurnState positionTeam)
    {
        switch (positionTeam)
        {
            case TurnState.Player : playerAlreadyPlayedPositions.Add(position); break;
            case TurnState.Enemy : enemyAlreadyPlayedPositions.Add(position); break;
            default:
                throw new ArgumentOutOfRangeException(nameof(positionTeam), positionTeam, null);
        }
    }

    public void Attack((int x, int y) attackerPosition, (int x, int y) attackOriginPosition, TurnState attackerTeam)
    {
        EntityDataInstance[,] attackerGrid;
        EntityDataInstance[,] opponentGrid;
        attackerGrid = attackerTeam == TurnState.Player ? playerGrid : enemyGrid; 
        opponentGrid = attackerTeam == TurnState.Player ? enemyGrid : playerGrid;
        
        EntityDataInstance attacker = attackerGrid[attackerPosition.x, attackerPosition.y];
        AttackStageData attackToApply = FindBestUnlockedStage(attacker.attack);
        
        // selon les data une attaque peut se faire sur son propre terrain plutot que opponent grid
        ApplyAttackPattern(opponentGrid, attackOriginPosition, attackToApply);
        EntityTakeDamage(attacker, attackerPosition, attackToApply.selfDamage);
        AddPositionToAlreadyPlayed(attackerPosition, attackerTeam);
        
        SwitchTurn();
    }

    private void ApplyAttackPattern(EntityDataInstance[,] gridToApply, (int x, int y) originPosition, AttackStageData attackToApply)
    {
        foreach (var position in attackToApply.pattern.positions)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            ApplyDamageAtPosition(gridToApply, positionInGrid, attackToApply.damage);
        }
    }

    private void ApplyDamageAtPosition(EntityDataInstance[,] gridToApply, (int x, int y) position, int damages)
    {
        EntityDataInstance entity = gridToApply[position.x, position.y];
        if (entity == null) return;
        EntityTakeDamage(entity, position, damages);
    }

    private void EntityTakeDamage(EntityDataInstance entity, (int x, int y) entityPosition, int damages)
    {
        entity.durability -= damages;

        if (entity.durability <= 0)
        {
            switch (entity)
            {
                case CharacterDataInstance : CharacterDeathAt(entityPosition); break;
                case EnemyDataInstance : EnemyDeathAt(entityPosition); break;
            }
        }
    }

    private void CharacterDeathAt((int x, int y) position)
    {
        playerGrid[position.x, position.y] = playerGrid[position.x, position.y].nextLayer == null
            ? null
            : (CharacterDataInstance)playerGrid[position.x, position.y].nextLayer.Instance();
    }
    
    private void EnemyDeathAt((int x, int y) position)
    {
        enemyGrid[position.x, position.y] = null;
    }

    private void PlaceCharacterAtPosition(CharacterDataInstance character, (int x, int y) position)
    {
        playerGrid[position.x, position.y] = character;
    }

    public void BreakLayerAt((int x, int y) position)
    {
        CharacterDeathAt(position);
    }

    public bool IsPositionAlreadyPlayed((int x, int y) position)
    {
        return playerAlreadyPlayedPositions.Contains(position);
    }
    
    
    
}
