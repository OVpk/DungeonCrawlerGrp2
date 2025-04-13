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

    public GameObject entityLocationPrefab;
    public GameObject playerGridPosition;
    public GameObject enemyGridPosition;

    public GameObject[,] playerGridDisplayed;
    public GameObject[,] enemyGridDisplayed;
    
    public static FightManager Instance;
    
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
        InitDisplay();

        playerGrid[0, 0] = (CharacterDataInstance)characterTest.Instance();
        playerGrid[0, 1] = (CharacterDataInstance)characterTest.Instance();
        playerGrid[0, 2] = (CharacterDataInstance)characterTest.Instance();
        playerGrid[1, 0] = (CharacterDataInstance)characterTest2.Instance();
        playerGrid[1, 1] = (CharacterDataInstance)characterTest2.Instance();
        playerGrid[1, 2] = (CharacterDataInstance)characterTest2.Instance();
    }

    private void InitDisplay()
    {
        playerGridDisplayed = new GameObject[playerGrid.GetLength(0), playerGrid.GetLength(1)];
        for (int i = 0; i < playerGrid.GetLength(0); i++)
        {
            for (int j = 0; j < playerGrid.GetLength(1); j++)
            {
                playerGridDisplayed[i, j] = Instantiate(entityLocationPrefab, playerGridPosition.transform);
                playerGridDisplayed[i, j].transform.localPosition = new Vector3(j, i) * 2f;
            }
        }
        
        enemyGridDisplayed = new GameObject[enemyGrid.GetLength(0), enemyGrid.GetLength(1)];
        for (int i = 0; i < enemyGrid.GetLength(0); i++)
        {
            for (int j = 0; j < enemyGrid.GetLength(1); j++)
            {
                enemyGridDisplayed[i, j] = Instantiate(entityLocationPrefab, enemyGridPosition.transform);
                enemyGridDisplayed[i, j].transform.localPosition = new Vector3(j, i) * 2f;
            }
        }
    }

    public void DisplayPattern((int x, int y) originPosition, List<Vector2Int> pattern)
    {
        CleanPatternDisplay();
        foreach (var position in pattern)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            enemyGridDisplayed[positionInGrid.x, positionInGrid.y].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void CleanPatternDisplay()
    {
        foreach (var cell in enemyGridDisplayed)
        {
            cell.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public GameObject selectorDisplayed;

    public void MoveSelectorDisplayTo((int x, int y) newPosition)
    {
        selectorDisplayed.transform.localPosition = new Vector3(newPosition.y, newPosition.x) * 2f;
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
