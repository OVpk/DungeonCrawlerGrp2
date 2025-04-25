using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour, IFightDisplayerListener
{
    
    [field: SerializeField] public FightEventSpeaker sendInformation { get; private set; }
    
    public enum TurnState
    {
        Player,
        Enemy
    }

    public TurnState currentTurn { get; private set; }
    
    [field:SerializeField] public SimpleAi aiController { get; private set; }

    private void SwitchTurn()
    {
        if (IsCleaningGridNecessary(currentTurn)) CleanAlreadyPlayedPositions(currentTurn);
        
        currentTurn = currentTurn == TurnState.Player ? TurnState.Enemy : TurnState.Player;
        
        if (IsCleaningGridNecessary(currentTurn)) CleanAlreadyPlayedPositions(currentTurn);
        
        if (currentTurn == TurnState.Enemy) aiController.PlayTurn();
    }

    public CharacterDataInstance[,] playerGrid { get; private set; } = new CharacterDataInstance[2, 3];
    public EnemyDataInstance[,] enemyGrid {get ; private set;} = new EnemyDataInstance[3, 3];

    private HashSet<(int x, int y)> playerAlreadyPlayedPositions = new HashSet<(int x, int y)>();
    private HashSet<(int x, int y)> enemyAlreadyPlayedPositions = new HashSet<(int x, int y)>();

    public CharacterData characterTest;
    public CharacterData characterTest2;
    public EnemyData enemyTest;

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
        InitDisplayedGrid(playerGrid);
        InitDisplayedGrid(enemyGrid);

        PlaceEntityAtPosition(characterTest, (0, 0), TurnState.Player);
        PlaceEntityAtPosition(characterTest, (0, 1), TurnState.Player);
        PlaceEntityAtPosition(characterTest, (0, 2), TurnState.Player);
        PlaceEntityAtPosition(characterTest2, (1, 0), TurnState.Player);
        PlaceEntityAtPosition(characterTest2, (1, 1), TurnState.Player);
        PlaceEntityAtPosition(characterTest2, (1, 2), TurnState.Player);
        
        PlaceEntityAtPosition(enemyTest, (0, 0), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (0, 1), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (0, 2), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (1, 0), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (1, 1), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (1, 2), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (2, 0), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (2, 1), TurnState.Enemy);
        PlaceEntityAtPosition(enemyTest, (2, 2), TurnState.Enemy);
    }

    #region Display

    public GameObject entityLocationPrefab;
    public GameObject playerGridContainer;
    public GameObject enemyGridContainer;

    public void InitDisplayedGrid(EntityDataInstance[,] grid)
    {
        GameObject container;
        TurnState team;

        switch (grid)
        {
            case CharacterDataInstance[,] : 
                container = playerGridContainer;
                team = TurnState.Player;
                break;
            case EnemyDataInstance[,] :
                container = enemyGridContainer;
                team = TurnState.Enemy;
                break;
            default: throw new Exception("Invalid Type");
        }
        
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                GameObject entityLocation = Instantiate(entityLocationPrefab, container.transform);
                entityLocation.transform.localPosition = new Vector3(j, i) * 2f;
                EntityDisplayController entityController = entityLocation.GetComponent<EntityDisplayController>();
                entityController.team = team;
                entityController.positionInGrid = (i, j);
                entityController.Init();
                sendInformation.Register(entityController);
            }
        }
    }

    #endregion

    public AttackStageData FindBestUnlockedStage(AttackData attack)
    {
        EntityDataInstance[,] gridToCheck = attack.gridToCheck switch
        {
            TurnState.Player => playerGrid,
            TurnState.Enemy => enemyGrid,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        AttackStageData bestStage = attack.attackStages[0];
        for (int i = attack.attackStages.Length-1; i >= 0; i--)
        {
            if (attack.attackStages[i].IsUnlock(gridToCheck))
            {
                bestStage = attack.attackStages[i];
                break;
            }
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

    private void AddPositionToAlreadyPlayed((int x, int y) position, TurnState positionTeam)
    {
        sendInformation.EntityLocationDisabledAt(position, positionTeam);
        
        switch (positionTeam)
        {
            case TurnState.Player : playerAlreadyPlayedPositions.Add(position); break;
            case TurnState.Enemy : enemyAlreadyPlayedPositions.Add(position); break;
            default:
                throw new ArgumentOutOfRangeException(nameof(positionTeam), positionTeam, null);
        }
    }

    private void CleanAlreadyPlayedPositions(TurnState teamToClean)
    {
        HashSet<(int x, int y)> positionsToClean = teamToClean switch
        {
            TurnState.Player => playerAlreadyPlayedPositions,
            TurnState.Enemy => enemyAlreadyPlayedPositions,
            _ => throw new ArgumentOutOfRangeException(nameof(teamToClean), teamToClean, null)
        };
        sendInformation.EntitiesLocationEnabledAt(positionsToClean, teamToClean);
        positionsToClean.Clear();
    }

    public IEnumerator Attack((int x, int y) attackerPosition, (int x, int y) attackOriginPosition, TurnState attackerTeam)
    {
        EntityDataInstance[,] attackerGrid;
        EntityDataInstance[,] gridToApplyAttack;
        attackerGrid = attackerTeam == TurnState.Player ? playerGrid : enemyGrid; 
        
        EntityDataInstance attacker = attackerGrid[attackerPosition.x, attackerPosition.y];
        gridToApplyAttack = attacker.attack.gridToApply == TurnState.Player ? playerGrid : enemyGrid;
        AttackStageData attackToApply = FindBestUnlockedStage(attacker.attack);
        
        sendInformation.EntityAttackAt(attackerPosition, attackerTeam); // l'information va donc etre traité et l'entité concerné va executer son animation d'attaque
        yield return WaitAnimationEvent();
        // ici le script se bloque et attend que l'event soit envoyer par une frame de l'animation lui indiquant q'il peut continuer
        
        yield return ApplyAttackPattern(gridToApplyAttack, attackOriginPosition, attackToApply);
        yield return EntityTakeDamage(attacker, attackerPosition, attackToApply.selfDamage);
        
        AddPositionToAlreadyPlayed(attackerPosition, attackerTeam);
        
        SwitchTurn();
    }

    private IEnumerator ApplyAttackPattern(EntityDataInstance[,] gridToApply, (int x, int y) originPosition, AttackStageData attackToApply)
    {
        foreach (var position in attackToApply.pattern.positions)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            yield return ApplyDamageAtPosition(gridToApply, positionInGrid, attackToApply.damage);
        }
    }

    private IEnumerator ApplyDamageAtPosition(EntityDataInstance[,] gridToApply, (int x, int y) position, int damages)
    {
        EntityDataInstance entity = gridToApply[position.x, position.y];
        if (entity == null) yield break;
        yield return EntityTakeDamage(entity, position, damages);
    }

    private IEnumerator EntityTakeDamage(EntityDataInstance entity, (int x, int y) entityPosition, int damages)
    {
        TurnState entityTeam = entity is CharacterDataInstance ? TurnState.Player : TurnState.Enemy;
        entity.durability -= damages;
        
        sendInformation.EntityTakeDamageAt(entityPosition, entityTeam);
        yield return WaitAnimationEvent();

        if (entity.durability <= 0)
        {
            switch (entityTeam)
            {
                case TurnState.Player : yield return CharacterDeathAt(entityPosition); break;
                case TurnState.Enemy : yield return EnemyDeathAt(entityPosition); break;
            }
        }
    }

    private IEnumerator CharacterDeathAt((int x, int y) position)
    {
        if (playerGrid[position.x, position.y].nextLayer == null)
        {
            playerGrid[position.x, position.y] = null;
            sendInformation.EntityDeathAt(position, TurnState.Player);
            yield return WaitAnimationEvent();
        }
        else
        {
            sendInformation.EntityDeathAt(position, TurnState.Player);
            yield return WaitAnimationEvent();
            PlaceEntityAtPosition(playerGrid[position.x, position.y].nextLayer, position, TurnState.Player);
        }
    }
    
    private IEnumerator EnemyDeathAt((int x, int y) position)
    {
        enemyGrid[position.x, position.y] = null;
        sendInformation.EntityDeathAt(position, TurnState.Enemy);
        yield return WaitAnimationEvent();
    }

    private void PlaceEntityAtPosition(EntityData entity, (int x, int y) position, TurnState team)
    {
        EntityDataInstance[,] gridToPlace = team == TurnState.Player ? playerGrid : enemyGrid;
        gridToPlace[position.x, position.y] = entity.Instance();
        sendInformation.EntitySpawnAt(position, team, gridToPlace[position.x, position.y]);
    }

    public void BreakLayerAt((int x, int y) position)
    {
        StartCoroutine(CharacterDeathAt(position));
    }

    public bool IsPositionAlreadyPlayed((int x, int y) position)
    {
        return playerAlreadyPlayedPositions.Contains(position);
    }

    private bool IsCleaningGridNecessary(TurnState teamToCheck)
    {
        return AreAllPositionsPlayed(teamToCheck) ||
               IsCharacterStockEmpty(teamToCheck) && ArePossiblePositionsEmpty(teamToCheck);
    }

    private bool AreAllPositionsPlayed(TurnState teamToCheck)
    {
        return teamToCheck switch
        {
            TurnState.Player => playerAlreadyPlayedPositions.Count >= playerGrid.GetLength(0) * playerGrid.GetLength(1),
            TurnState.Enemy => enemyAlreadyPlayedPositions.Count >= enemyGrid.GetLength(0) * enemyGrid.GetLength(1),
            _ => throw new ArgumentOutOfRangeException(nameof(teamToCheck), teamToCheck, null)
        };
    }

    private bool ArePossiblePositionsEmpty(TurnState teamToCheck)
    {
        EntityDataInstance[,] gridToCheck = teamToCheck switch
        {
            TurnState.Player => playerGrid,
            TurnState.Enemy => enemyGrid,
            _ => throw new ArgumentOutOfRangeException(nameof(teamToCheck), teamToCheck, null)
        };
        HashSet<(int x, int y)> alreadyPlayedPositions = teamToCheck switch
        {
            TurnState.Player => playerAlreadyPlayedPositions,
            TurnState.Enemy => enemyAlreadyPlayedPositions,
            _ => throw new ArgumentOutOfRangeException(nameof(teamToCheck), teamToCheck, null)
        };
        for (int i = 0; i < gridToCheck.GetLength(0); i++)
        {
            for (int j = 0; j < gridToCheck.GetLength(1); j++)
            {
                if (alreadyPlayedPositions.Contains((i, j))) continue;
                if (gridToCheck[i, j] != null) return false;
            }
        }
        return true;
    }

    private bool IsCharacterStockEmpty(TurnState teamToCheck)
    {
        // à changer par une verification de l'inventaire
        return teamToCheck switch
        {
            TurnState.Player => false,
            TurnState.Enemy => true,
            _ => throw new ArgumentOutOfRangeException(nameof(teamToCheck), teamToCheck, null)
        };
    }

    private bool canContinue = false;

    public void OnDisplayerSaidCanContinue() => canContinue = true;

    public IEnumerator WaitAnimationEvent()
    {
        canContinue = false;
        Debug.Log("attend une réponse");
        yield return new WaitUntil(() => canContinue);
        Debug.Log("reponse recu");
    }
}
