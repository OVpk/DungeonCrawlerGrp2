using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FightManager : MonoBehaviour, IFightDisplayerListener
{
    public int nbTurnBeforeEntityGlueGone = 1;

    public int percentOfChanceTakeExplosivePowder;
    
    [field: SerializeField] public FightEventSpeaker sendInformation { get; private set; }
    
    public enum TurnState
    {
        Player,
        Enemy
    }

    public TurnState currentTurn { get; private set; }
    
    [field:SerializeField] public SimpleAi aiController { get; private set; }
    
    public bool canUseControlls = true;

    private void SwitchTurn()
    {
        if (IsCleaningGridNecessary(currentTurn)) CleanAlreadyPlayedPositions(currentTurn);
        EndTurn(currentTurn);
        
        
        currentTurn = currentTurn == TurnState.Player ? TurnState.Enemy : TurnState.Player;
        
        if (IsCleaningGridNecessary(currentTurn)) CleanAlreadyPlayedPositions(currentTurn);

        if (currentTurn == TurnState.Player) canUseControlls = true;
        
        if (currentTurn == TurnState.Enemy) aiController.PlayTurn();
    }

    private void EndTurn(TurnState endedTurn)
    {
        EntityDataInstance[,] gridToUpdate = endedTurn == TurnState.Player ? playerGrid : enemyGrid;
        foreach (var entity in gridToUpdate)
        {
            if(entity == null) continue;
            entity.UpdateEffects();
            if (entity.effects.Contains(EntityData.EntityEffects.Glue) && entity.nbTurnBeforeGlueGone == 0)
                entity.effects.Remove(EntityData.EntityEffects.Glue);
        }
    }



    public CharacterDataInstance[,] playerGrid { get; private set; } = new CharacterDataInstance[2, 3];
    public EnemyDataInstance[,] enemyGrid {get ; private set;} = new EnemyDataInstance[3, 3];

    private HashSet<(int x, int y)> playerAlreadyPlayedPositions = new HashSet<(int x, int y)>();
    public HashSet<(int x, int y)> enemyAlreadyPlayedPositions = new HashSet<(int x, int y)>();
    
    public EnemyGridData enemyGridData;

    public List<CandyPackData> candyPackData;
    public List<CandyPackDataInstance> candyPack = new List<CandyPackDataInstance>();


    [SerializeField] public SimpleCardSlider packDisplayer;

    private void InitEnemyGrid()
    {
        EnemyData[,] enemyGridData2d = enemyGridData.Enemies2D;
        for (int i = 0; i < enemyGrid.GetLength(0); i++)
        {
            for (int j = 0; j < enemyGrid.GetLength(1); j++)
            {
                if (enemyGridData2d[i,j] == null) continue;
                PlaceEntityAtPosition(enemyGridData2d[i,j], (i,j), TurnState.Enemy);
            }
        }
    }
    
    private void InitPack()
    {
        foreach (var pack in candyPackData)
        {
            candyPack.Add(pack.Instance());
        }
        packDisplayer.packs = candyPack;
    }

    public void PlaceCharacterFromPack(CandyPackDataInstance pack, (int x, int y)position)
    {
        if (pack.currentStock <= 0) return;
        if (playerGrid[position.x, position.y] != null) return;
        PlaceEntityAtPosition(pack.candyData, position, TurnState.Player);
        pack.currentStock--;
        packDisplayer.UpdateDisplay();
    }

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
        }
        InitPack();
    }

    private void Start()
    {
        InitDisplayedGrid(playerGrid);
        InitDisplayedGrid(enemyGrid);
        
        InitEnemyGrid();
    }

    #region Display

    public GameObject entityLocationPrefab;
    public GameObject playerGridContainer;
    public GameObject enemyGridContainer;

    public void InitDisplayedGrid(EntityDataInstance[,] grid)
    {
        GameObject container;
        TurnState team;
        float horizontalRapport = 2f;
        float verticalRapport = 2f;

        switch (grid)
        {
            case CharacterDataInstance[,] : 
                container = playerGridContainer;
                team = TurnState.Player;
                break;
            case EnemyDataInstance[,] :
                container = enemyGridContainer;
                team = TurnState.Enemy;
                verticalRapport = 1.5f;
                break;
            default: throw new Exception("Invalid Type");
        }
        
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                GameObject entityLocation = Instantiate(entityLocationPrefab, container.transform);
                entityLocation.transform.localPosition = new Vector3(j * horizontalRapport, i * verticalRapport);
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

    public IEnumerator Attack((int x, int y) attackerPosition, int attackIndex, (int x, int y) attackOriginPosition, TurnState attackerTeam)
    {
        EntityDataInstance[,] attackerGrid;
        EntityDataInstance[,] gridToApplyAttack;
        attackerGrid = attackerTeam == TurnState.Player ? playerGrid : enemyGrid; 
        
        EntityDataInstance attacker = attackerGrid[attackerPosition.x, attackerPosition.y];
        AttackData attack = attacker.attacks[attackIndex];
        gridToApplyAttack = attack.gridToApply == TurnState.Player ? playerGrid : enemyGrid;
        AttackStageData attackToApply = FindBestUnlockedStage(attack);

        if (attackToApply.effect is EntityData.EntityEffects.ProtectedHorizontaly
            or EntityData.EntityEffects.ProtectedVerticaly)
        {
            attacker.AddEffect(EntityData.EntityEffects.Protector);
            switch (attackToApply.effect)
            {
                case EntityData.EntityEffects.ProtectedHorizontaly : 
                    sendInformation.EntityCreateProtectionAt(attackerPosition, attackerTeam, EntityDisplayController.BubbleDirections.Horizontal);
                    break;
                case EntityData.EntityEffects.ProtectedVerticaly :
                    sendInformation.EntityCreateProtectionAt(attackerPosition, attackerTeam, EntityDisplayController.BubbleDirections.Vertical);
                    break;
            }
        }
            
        
        sendInformation.EntityAttackAt(attackerPosition, attackerTeam);
        yield return WaitAnimationEvent();
        
        // 3.3) Snapshot des cases protégées
        var initialProtected = GetProtectedPositions(gridToApplyAttack);
        
        yield return ApplyAttackPattern(gridToApplyAttack, attackOriginPosition, attackToApply);

        if (attacker.effects.Contains(EntityData.EntityEffects.Explosive)) 
            TryApplyExplosivePowder(attacker,
                gridToApplyAttack,
                attackOriginPosition,
                attackToApply,
                attackerPosition,
                attackerTeam,
                initialProtected);
        
        yield return EntityTakeDamage(attacker, attackerPosition, attackToApply.selfDamage);
        
        AddPositionToAlreadyPlayed(attackerPosition, attackerTeam);
        
        SwitchTurn();
    }
    
// 1) Capture des bulles protégées avant application de l'attaque
private HashSet<(int x, int y)> GetProtectedPositions(EntityDataInstance[,] grid)
{
    var set = new HashSet<(int x, int y)>();
    int rows = grid.GetLength(0), cols = grid.GetLength(1);
    for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
        {
            var e = grid[i, j];
            if (e == null) continue;
            if (e.effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly)
             || e.effects.Contains(EntityData.EntityEffects.ProtectedVerticaly))
                set.Add((i, j));
        }
    return set;
}

// 2) Tentative d'application de poudre explosive basée sur l'état initial des bulles
private void TryApplyExplosivePowder(
    EntityDataInstance attacker,
    EntityDataInstance[,] grid,
    (int x, int y) origin,
    AttackStageData stage,
    (int x, int y) attackerPosition,
    TurnState attackerTeam,
    HashSet<(int x, int y)> initialProtected)
{

    // 2.2) Test de probabilité
    int roll = Random.Range(0, 100);
    if (roll >= percentOfChanceTakeExplosivePowder)
        return;

    // 2.3) On parcourt le pattern et on cherche la première cible non protégée initialement
    foreach (var off in stage.pattern.positions)
    {
        var pos = (x: origin.x + off.x, y: origin.y + off.y);
        if (IsOutsideLimit(grid, pos)) continue;
        if (initialProtected.Contains(pos)) continue;

        var target = grid[pos.x, pos.y];
        if (target == null) continue;

        // 2.4) On pose l'effet explosif et on notifie
        target.AddEffect(EntityData.EntityEffects.Explosive);
        var targetTeam = attackerTeam == TurnState.Player ? TurnState.Enemy : TurnState.Player;
        sendInformation.EntityGetExplosiveEffectAt(pos, targetTeam);

        // 2.5) On retire l'effet de l'attaquant et on notifie
        attacker.effects.Remove(EntityData.EntityEffects.Explosive);
        sendInformation.EntityLoseExplosiveEffectAt(attackerPosition, attackerTeam);
        break;
    }
}


// ApplyAttackPattern orchestrates the whole hit process
public IEnumerator ApplyAttackPattern(EntityDataInstance[,] gridToApply, (int x, int y) origin, AttackStageData attack)
{
    // 1) Récupérer les positions impactées
    var impactedPositions = GetImpactedPositions(gridToApply, origin, attack.pattern.positions);

    // 2) Séparer en entités protégées et non protégées
    PartitionPositions(gridToApply, impactedPositions,
        out List<(int x, int y)> protectedHits,
        out List<(int x, int y)> regularHits);

    // 3) Gérer les protections (suppression de bulles)
    yield return ProcessProtectedHits(gridToApply, protectedHits);

    // 4) Appliquer les dégâts et effets aux cibles non protégées
    yield return ProcessRegularHits(gridToApply, regularHits, attack);
}

// 1) Calcule les positions valides impactées par le pattern
private List<(int x, int y)> GetImpactedPositions(EntityDataInstance[,] grid, (int x, int y) origin, List<Vector2Int> pattern)
{
    var list = new List<(int x, int y)>();
    foreach (var offset in pattern)
    {
        int x = origin.x + offset.x;
        int y = origin.y + offset.y;
        if (!IsOutsideLimit(grid, (x, y)))
            list.Add((x, y));
    }
    return list;
}

// 2) Sépare les positions en hits protégés et non protégés
private void PartitionPositions(
    EntityDataInstance[,] grid,
    List<(int x, int y)> positions,
    out List<(int x, int y)> protectedHits,
    out List<(int x, int y)> regularHits)
{
    protectedHits = new List<(int x, int y)>();
    regularHits = new List<(int x, int y)>();
    foreach (var pos in positions)
    {
        var e = grid[pos.x, pos.y];
        if (e == null) continue;
        bool isProtected = e.effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly)
                         || e.effects.Contains(EntityData.EntityEffects.ProtectedVerticaly);
        if (isProtected) protectedHits.Add(pos);
        else regularHits.Add(pos);
    }
}

// 3) Supprime toutes les bulles pour les protections touchées
private IEnumerator ProcessProtectedHits(
    EntityDataInstance[,] grid,
    List<(int x, int y)> protectedHits)
{
    foreach (var pos in protectedHits)
    {
        // Détermine l'équipe en fonction du type d'entité
        TurnState team = grid[pos.x, pos.y] is CharacterDataInstance
            ? TurnState.Player : TurnState.Enemy;
        RemoveBubbleAt(pos, team);
        // Optionnel: attendre une animation si souhaité
        yield return null;
    }
}

// 4) Applique dégâts et effets aux entités non protégées
private IEnumerator ProcessRegularHits(
    EntityDataInstance[,] grid,
    List<(int x, int y)> regularHits,
    AttackStageData attack)
{
    foreach (var pos in regularHits)
    {
        yield return ApplyDamageAtPosition(grid, pos, attack.damage);
        yield return ApplyEffectAtPosition(grid, pos, attack.effect);
    }
}


    private IEnumerator ApplyDamageAtPosition(EntityDataInstance[,] gridToApply, (int x, int y) position, int damages)
    {
        EntityDataInstance entity = gridToApply[position.x, position.y];
        if (entity == null) yield break;
        if (damages <= 0) yield break;
        yield return EntityTakeDamage(entity, position, damages);
    }

    private IEnumerator ApplyEffectAtPosition(EntityDataInstance[,] gridToApply, (int x, int y) position, EntityData.EntityEffects effect)
    {
        EntityDataInstance entity = gridToApply[position.x, position.y];
        if (entity == null) yield break;
        if (effect == EntityData.EntityEffects.Empty) yield break;
        if (entity.effects.Contains(effect)) yield break;
        entity.AddEffect(effect);
    }

    private IEnumerator EntityTakeDamage(EntityDataInstance entity, (int x, int y) entityPosition, int damages)
    {
        if (damages <= 0) yield break;
        TurnState entityTeam = entity is CharacterDataInstance ? TurnState.Player : TurnState.Enemy;
        EntityDataInstance[,] gridToApply = entityTeam == TurnState.Player ? playerGrid : enemyGrid;
        if (entity.effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly) ||
            entity.effects.Contains(EntityData.EntityEffects.ProtectedVerticaly))
        {
            RemoveBubbleAt(entityPosition, entityTeam);
        }
        else
        {
            entity.durability -= damages;
            sendInformation.EntityTakeDamageAt(entityPosition, damages, entityTeam);
            yield return WaitAnimationEvent();
        }

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
        if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Protector))
            RemoveBubbleAt(position, TurnState.Player);
        
        if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Explosive)&&
            !playerGrid[position.x, position.y].isImmuneToExplosions)
            yield return EntityExplodeAt(position, TurnState.Player);
        else
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
            
                List<EntityData.EntityEffects> effectsToPass = new List<EntityData.EntityEffects>();
                if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly))
                    effectsToPass.Add(EntityData.EntityEffects.ProtectedHorizontaly);
                if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.ProtectedVerticaly))
                    effectsToPass.Add(EntityData.EntityEffects.ProtectedVerticaly);
                EntityDataInstance newLayer = PlaceEntityAtPosition(playerGrid[position.x, position.y].nextLayer, position, TurnState.Player);
                foreach (var effect in effectsToPass)
                {
                    newLayer.AddEffect(effect);
                }
            }
        
        }
    }
    
    private IEnumerator EnemyDeathAt((int x, int y) position)
    {
        if (enemyGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Explosive)&&
            !enemyGrid[position.x, position.y].isImmuneToExplosions)
            yield return EntityExplodeAt(position, TurnState.Enemy);
        else
        {
            enemyGrid[position.x, position.y] = null;
            sendInformation.EntityDeathAt(position, TurnState.Enemy);
            yield return WaitAnimationEvent();
        }
    }
    
public IEnumerator EntityExplodeAt((int x, int y) position, TurnState team)
{
    // 1) Préparation et animation de l'explosion initiale
    sendInformation.EntityLoseExplosiveEffectAt(position, team);
    sendInformation.EntityExplodeAt(position, team);

    EntityDataInstance[,] gridToApply = team == TurnState.Player ? playerGrid : enemyGrid;
    var originEntity = gridToApply[position.x, position.y];
    originEntity.effects.Remove(EntityData.EntityEffects.Explosive);
    sendInformation.EntityLoseExplosiveEffectAt(position, team);

    // 2) Détection du contexte d'explosion (dans ou hors bulle)
    bool insideBubbleH = originEntity.effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly);
    bool insideBubbleV = originEntity.effects.Contains(EntityData.EntityEffects.ProtectedVerticaly);
    bool originInsideBubble = insideBubbleH || insideBubbleV;

    // 3) Capture de l'état de protection initial des entités
    var protectedPositions = new HashSet<(int x, int y)>();
    for (int i = 0; i < gridToApply.GetLength(0); i++)
    {
        for (int j = 0; j < gridToApply.GetLength(1); j++)
        {
            var e = gridToApply[i, j];
            if (e == null) continue;
            if (e.effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly) ||
                e.effects.Contains(EntityData.EntityEffects.ProtectedVerticaly))
                protectedPositions.Add((i, j));
        }
    }

    // 4) Détermination des cibles basées sur l'état initial
    var targets = new List<(int x, int y)>();
    var visuals = new List<(int x, int y)>();
    for (int i = 0; i < gridToApply.GetLength(0); i++)
    {
        for (int j = 0; j < gridToApply.GetLength(1); j++)
        {
            var e = gridToApply[i, j];
            if (e == null) continue;

            bool wasProtected = protectedPositions.Contains((i, j));
            bool shouldBeTarget = originInsideBubble ? wasProtected : !wasProtected;
            if (!shouldBeTarget) continue;

            targets.Add((i, j));
            if (e.effects.Contains(EntityData.EntityEffects.Explosive))
                visuals.Add((i, j)); // on note pour animation visuelle
        }
    }

    // 5) Suppression des bulles si explosion hors bulle (les protégés survivent)
    if (!originInsideBubble)
    {
        foreach (var pos in protectedPositions)
            RemoveBubbleAt(pos, team);
    }

    // 6) Application des animations visuelles d'explosion pour chaîne, sans dégâts
    foreach (var (i, j) in visuals)
    {
        // perte visuelle de l'effet explosif
        sendInformation.EntityLoseExplosiveEffectAt((i, j), team);
        // animation visuelle d'explosion
        sendInformation.EntityExplodeAt((i, j), team);
    }

    // 7) Application de la destruction sans nouvelle explosion
    foreach (var (i, j) in targets)
    {
        var e = gridToApply[i, j];
        if (e == null) continue;

        // on retire l'effet explosif pour éviter toute relance mécanique
        e.effects.Remove(EntityData.EntityEffects.Explosive);

        // mort de l'entité
        if (team == TurnState.Player)
            yield return CharacterDeathAt((i, j));
        else
            yield return EnemyDeathAt((i, j));
    }
}





    private EntityDataInstance PlaceEntityAtPosition(EntityData entity, (int x, int y) position, TurnState team)
    {
        EntityDataInstance[,] gridToPlace = team == TurnState.Player ? playerGrid : enemyGrid;
        gridToPlace[position.x, position.y] = entity.Instance();
        sendInformation.EntitySpawnAt(position, team, gridToPlace[position.x, position.y]);
        return gridToPlace[position.x, position.y];
    }

    public void BreakLayerAt((int x, int y) position)
    {
        
        StartCoroutine(CharacterDeathAt(position));
    }

    public void RemoveBubbleAt((int x, int y) position, TurnState team)
    {
        EntityDataInstance[,] gridToApply = team == TurnState.Player ? playerGrid : enemyGrid;
        EntityDataInstance entity = gridToApply[position.x, position.y];
        
        if (entity.effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly))
        {
            for (int i = 0; i < gridToApply.GetLength(1); i++)
            {
                if (gridToApply[position.x, i] == null) continue;
                if (!gridToApply[position.x, i].effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly)) continue;
                gridToApply[position.x, i].effects.Remove(EntityData.EntityEffects.ProtectedHorizontaly);
                if (gridToApply[position.x, i].effects.Contains(EntityData.EntityEffects.Protector))
                    gridToApply[position.x, i].effects.Remove(EntityData.EntityEffects.Protector);
                sendInformation.EntityLoseProtectionAt((position.x, i), team, EntityDisplayController.BubbleDirections.Horizontal);
            }
        }
        else if (entity.effects.Contains(EntityData.EntityEffects.ProtectedVerticaly))
        {
            for (int i = 0; i < gridToApply.GetLength(0); i++)
            {
                if (gridToApply[i, position.y] == null) continue;
                if(!gridToApply[i, position.y].effects.Contains(EntityData.EntityEffects.ProtectedVerticaly)) continue;
                gridToApply[i, position.y].effects.Remove(EntityData.EntityEffects.ProtectedVerticaly);
                if (gridToApply[i, position.y].effects.Contains(EntityData.EntityEffects.Protector))
                    gridToApply[i, position.y].effects.Remove(EntityData.EntityEffects.Protector);
                sendInformation.EntityLoseProtectionAt((i, position.y), team, EntityDisplayController.BubbleDirections.Vertical);
            }
        }
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
        if (teamToCheck == TurnState.Enemy) return true;
        
        foreach (var pack in candyPack)
        {
            if (pack.currentStock > 0) return false;
        }

        return true;
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
