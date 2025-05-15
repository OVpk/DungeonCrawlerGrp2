using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    
    public bool canUseControlls = true;

    public void SwitchTurn()
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
        UpdateFog(gridToUpdate, endedTurn);
    }

    private void UpdateGlue(EntityDataInstance entity, (int x, int y) entityPosition, TurnState teamToUpdate)
    {
        if(entity == null) return;
        if (!entity.effects.Contains(EntityData.EntityEffects.Glue)) return;
            
        entity.glueDurability--;
        if (entity.glueDurability == 0)
        {
            entity.effects.Remove(EntityData.EntityEffects.Glue);
            sendInformation.EntityLoseGlueEffectAt((entityPosition.x,entityPosition.y), teamToUpdate);
        }
            
    }
    
    
    private void UpdateFog(EntityDataInstance[,] gridToUpdate, TurnState teamToUpdate)
    {
        for (int i = 0; i < gridToUpdate.GetLength(0); i++)
        {
            for (int j = 0; j < gridToUpdate.GetLength(1); j++)
            {
                EntityDataInstance entity = gridToUpdate[i, j];
                if(entity == null) continue;
                if (!entity.effects.Contains(EntityData.EntityEffects.Fog)) continue;
            
                entity.nbOfTurnBeforeFogGone--;
                if (entity.nbOfTurnBeforeFogGone == 0)
                {
                    entity.effects.Remove(EntityData.EntityEffects.Fog);
                    sendInformation.EntityLoseFogEffectAt((i,j), teamToUpdate);
                }
            }
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

    public void AddPositionToAlreadyPlayed((int x, int y) position, TurnState positionTeam)
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
    // Détermination des grilles et de l'attaquant
    EntityDataInstance[,] attackerGrid = attackerTeam == TurnState.Player ? playerGrid : enemyGrid; 
    EntityDataInstance attacker = attackerGrid[attackerPosition.x, attackerPosition.y];
    AttackData attack = attacker.attacks[attackIndex];
    EntityDataInstance[,] gridToApplyAttack = attack.gridToApply == TurnState.Player ? playerGrid : enemyGrid;
    AttackStageData attackToApply = FindBestUnlockedStage(attack);

    // Cas du glue : skip et switch immédiat après auto-damage
    if (attacker.effects.Contains(EntityData.EntityEffects.Glue))
    {
        yield return new WaitForSeconds(2f);
        yield return EntityTakeDamage(attacker, attackerPosition, attackToApply.selfDamage);
        UpdateGlue(attacker, attackerPosition, attackerTeam);
        yield return new WaitForSeconds(2f);

        AddPositionToAlreadyPlayed(attackerPosition, attackerTeam);
        SwitchTurn();
        yield break;
    }

    // Bulles de protection ou spawner
    if (attackToApply.Effect is EntityData.EntityEffects.ProtectedHorizontaly
        || attackToApply.Effect is EntityData.EntityEffects.ProtectedVerticaly)
    {
        ApplyBubbleEffect(attacker, attackerPosition, attackerTeam, attackToApply,
            attackToApply.Effect, attackOriginPosition, gridToApplyAttack,
            attackToApply.pattern.positions);
    }
    else if (attackToApply.Effect is EntityData.EntityEffects.Spawner)
    {
        sendInformation.EntityAttackAt(attackerPosition, attackerTeam);
        yield return WaitAnimationEvent();
        DoSpawn(attackToApply.EntityToSpawn, gridToApplyAttack);
    }
    else
    {
        // 1) Déclenchement de l'animation d'attaque
        sendInformation.EntityAttackAt(attackerPosition, attackerTeam);
        yield return WaitAnimationEvent();

        // 2) Calcul des positions impactées et partition
        var impacted = GetImpactedPositions(gridToApplyAttack, attackOriginPosition, attackToApply.pattern.positions);
        PartitionPositions(gridToApplyAttack, impacted, out var protectedHits, out var regularHits);

        // 3) Collecte des Foggers en amont pour post-traitement
        var preFoggers = new List<(EntityDataInstance fogger, (int x, int y) pos)>();
        
        for (int i = 0; i < enemyGrid.GetLength(0); i++)
        {
            for (int j = 0; j < enemyGrid.GetLength(1); j++)
            {
                EntityDataInstance entity = enemyGrid[i, j];
                if (entity != null && entity.effects.Contains(EntityData.EntityEffects.Fogger))
                    preFoggers.Add((entity, (i,j)));
            }
        }

        // 4) Application des hits protégés et réguliers
        yield return ProcessProtectedHits(gridToApplyAttack, protectedHits);
        var damagedPositions = new List<(int x, int y)>();
        yield return ProcessRegularHits(gridToApplyAttack, regularHits, attackToApply, damagedPositions);

        // 5) Après dégâts et morts, activation des foggers morts


        // 6) Chaîne explosive et glue
        if (attacker.effects.Contains(EntityData.EntityEffects.Explosive))
            TryApplyExplosivePowder(attacker, gridToApplyAttack, attackerPosition, attackerTeam, protectedHits, damagedPositions, attack.gridToApply);
        else
        {
            foreach (var pos in damagedPositions)
            {
                var target = gridToApplyAttack[pos.x, pos.y];
                if (target == null) continue;
                if (target.effects.Contains(EntityData.EntityEffects.Explosive))
                    TryApplyExplosivePowder(target, attackerGrid, pos, attack.gridToApply, new List<(int x, int y)>(), new List<(int x, int y)> { attackerPosition }, attackerTeam);
            }
        }

        if (attackToApply.Effect == EntityData.EntityEffects.Glue)
            TryApplyGlue(gridToApplyAttack, damagedPositions, attackOriginPosition, attackToApply, protectedHits, attack.gridToApply);

        // 7) Auto-damage de l'attaquant
        yield return EntityTakeDamage(attacker, attackerPosition, attackToApply.selfDamage);
        
        foreach (var (fogger, pos) in preFoggers)
        {
            if (fogger.durability <= 0)
            {
                EntityApplyFogEffect(
                    fogger,
                    fogger is CharacterDataInstance ? TurnState.Player : TurnState.Enemy,
                    pos,
                    fogger is CharacterDataInstance ? playerGrid : enemyGrid,
                    fogger.patternWhereFogGone.positions);
            }
        }
    }

    // 8) Fin de tour
    AddPositionToAlreadyPlayed(attackerPosition, attackerTeam);
    SwitchTurn();
}


    private void ApplyBubbleEffect(EntityDataInstance protector,
        (int x, int y) protectorPosition,
        TurnState protectorTeam,
        AttackStageData attackToApply,
        EntityData.EntityEffects effectToApply,
        (int x, int y) originPosition,
        EntityDataInstance[,] gridToApply,
        List<Vector2Int> pattern)
    {
        protector.AddEffect(EntityData.EntityEffects.Protector);
        protector.bubbleDurability = attackToApply.BubbleDurability;
        
        switch (effectToApply)
        {
            case EntityData.EntityEffects.ProtectedHorizontaly : 
                sendInformation.EntityCreateProtectionAt(protectorPosition, protectorTeam, EntityDisplayController.BubbleDirections.Horizontal);
                break;
            case EntityData.EntityEffects.ProtectedVerticaly :
                sendInformation.EntityCreateProtectionAt(protectorPosition, protectorTeam, EntityDisplayController.BubbleDirections.Vertical);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effectToApply), effectToApply, null);
        }
       

        List<(int x, int y)> impactedPositions = GetImpactedPositions(gridToApply, originPosition, pattern);
        foreach (var position in impactedPositions)
        {
            EntityDataInstance entity = gridToApply[position.x, position.y];
            if (entity == null) continue;
            entity.AddEffect(effectToApply);
        }

    }
    
    private void EntityApplyFogEffect(EntityDataInstance fogger,
        TurnState foggerTeam,
        (int x, int y) foggerPosition,
        EntityDataInstance[,] gridToApply,
        List<Vector2Int> pattern)
    {
        List<(int x, int y)> impactedPositions = GetImpactedPositions(gridToApply, foggerPosition, pattern);
        foreach (var position in impactedPositions)
        {
            EntityDataInstance entity = gridToApply[position.x, position.y];
            if (entity == null) continue;
            entity.AddEffect(EntityData.EntityEffects.Fog);
            entity.percentOfChanceOfAvoidingAttackThanksToFog =
                fogger.percentOfChanceOfAvoidingAttackThanksToFog;
            entity.nbOfTurnBeforeFogGone = fogger.nbOfTurnBeforeFogGone;
            
            sendInformation.EntityGetFogEffectAt(position, foggerTeam);
        }
    }


private void TryApplyExplosivePowder(
    EntityDataInstance attacker,
    EntityDataInstance[,] grid,
    (int x, int y) attackerPosition,
    TurnState attackerTeam,
    List<(int x, int y)> protectedHits,
    List<(int x, int y)> damagedPositions,
    TurnState targetTeam)
{

    // Parcourt uniquement les positions qui ont pris des dégâts
    foreach (var pos in damagedPositions)
    {
        if (Random.Range(0, 100) >= attacker.percentOfChanceToGiveExplosive) continue;
        
        // Ignore si protégée initialement
        if (protectedHits.Contains(pos)) 
            continue;

        var target = grid[pos.x, pos.y];
        if (target == null) 
            continue;
        
        if (target.effects.Contains(EntityData.EntityEffects.Explosive)) continue;

        // On pose l’effet explosif et on notifie
        target.AddEffect(EntityData.EntityEffects.Explosive);
        target.percentOfChanceToGiveExplosive = attacker.percentOfChanceToGiveExplosive;
        target.explosionDamages = attacker.explosionDamages;
        sendInformation.EntityGetExplosiveEffectAt(pos, targetTeam);

        if (!attacker.isImmuneToExplosions)
        {
            // On retire l’effet de l’attaquant et on notifie
            attacker.effects.Remove(EntityData.EntityEffects.Explosive);
            sendInformation.EntityLoseExplosiveEffectAt(attackerPosition, attackerTeam);
        } // un seul spawn, comme avant
    }
}


private void TryApplyGlue(
    EntityDataInstance[,] grid,
    List<(int x, int y)> damaged,
    (int x, int y) origin,
    AttackStageData stage,
    List<(int x, int y)> protectedHits,
    TurnState targetTeam)
{
    foreach (var pos in damaged)
    {
        if (protectedHits.Contains(pos)) continue;
        if (Random.Range(0, 100) >= stage.PercentOfChanceOfGlue) continue;
        var e = grid[pos.x, pos.y];
        if (e == null) continue;
        e.AddEffect(EntityData.EntityEffects.Glue);
        e.glueDurability = stage.GlueDurability;
        sendInformation.EntityGetGlueEffectAt(pos, targetTeam);
    }
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
        {
            list.Add((x, y));
        }
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
    var protectorPositions = new HashSet<(int x, int y)>();
    
    foreach (var protectedHit in protectedHits)
    {
        foreach (var protectorPosition in FindProtectorPositions(protectedHit, grid))
        {
            protectorPositions.Add(protectorPosition);
        }
    }
    foreach (var protectorPosition in protectorPositions)
    {
        EntityDataInstance protector = grid[protectorPosition.x, protectorPosition.y];
        protector.bubbleDurability--;
        if (protector.bubbleDurability <= 0)
        {
            RemoveBubbleAt(protectorPosition, grid);
            yield return null;
        }
    }
    yield return null;
}

private HashSet<(int x, int y)> FindProtectorPositions((int x, int y) protectedPosition, EntityDataInstance[,] gridToCheck)
{
    HashSet<(int x, int y)> protectors = new HashSet<(int x, int y)>();
    
    EntityDataInstance protectedEntity = gridToCheck[protectedPosition.x, protectedPosition.y];
    if (protectedEntity.effects.Contains(EntityData.EntityEffects.ProtectedHorizontaly))
    {
        for (int i = 0; i < gridToCheck.GetLength(1); i++)
        {
            if (gridToCheck[protectedPosition.x, i] == null) continue;
            if (gridToCheck[protectedPosition.x, i].effects.Contains(EntityData.EntityEffects.Protector))
                protectors.Add((protectedPosition.x, i));
        }
    }
    if (protectedEntity.effects.Contains(EntityData.EntityEffects.ProtectedVerticaly))
    {
        for (int i = 0; i < gridToCheck.GetLength(0); i++)
        {
            if (gridToCheck[i, protectedPosition.y] == null) continue;
            if (gridToCheck[i, protectedPosition.y].effects.Contains(EntityData.EntityEffects.Protector))
                protectors.Add((i, protectedPosition.y));
        }
    }
    return protectors;
}

private IEnumerator ProcessRegularHits(
    EntityDataInstance[,] grid,
    List<(int x, int y)> hits,
    AttackStageData stage,
    List<(int x, int y)> damagedPositions)
{
    
    foreach (var pos in hits)
    {
        var e = grid[pos.x, pos.y];
        if (e == null) continue;
        if (e.effects.Contains(EntityData.EntityEffects.Fog) &&
            Random.Range(0, 100) <= e.percentOfChanceOfAvoidingAttackThanksToFog)
            continue;

        yield return ApplyDamageAtPosition(grid, pos, stage.damage);
        damagedPositions.Add(pos);
    }
}



    private IEnumerator ApplyDamageAtPosition(EntityDataInstance[,] gridToApply, (int x, int y) position, int damages)
    {
        EntityDataInstance entity = gridToApply[position.x, position.y];
        if (entity == null) yield break;
        if (damages <= 0) yield break;
        yield return EntityTakeDamage(entity, position, damages);
    }



    private IEnumerator EntityTakeDamage(EntityDataInstance entity, (int x, int y) entityPosition, int damages)
    {
        if (damages <= 0) yield break;
        TurnState entityTeam = entity is CharacterDataInstance ? TurnState.Player : TurnState.Enemy;
        EntityDataInstance[,] gridToApply = entityTeam == TurnState.Player ? playerGrid : enemyGrid;
        if (entity.effects.Contains(EntityData.EntityEffects.Fog) && 
            Random.Range(0, 100) <= entity.percentOfChanceOfAvoidingAttackThanksToFog) yield break;
        
        entity.durability -= damages; 
        sendInformation.EntityTakeDamageAt(entityPosition, damages, entityTeam); 
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
        if (playerGrid[position.x, position.y] == null) yield break;
        
        if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Explosive) &&
            !playerGrid[position.x, position.y].isImmuneToExplosions)
            yield return EntityExplodeAt(position, TurnState.Player);
        
        if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Protector))
            RemoveBubbleAt(position, playerGrid);

        if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Fog))
        {
            sendInformation.EntityLoseFogEffectAt(position, TurnState.Player);
        }
        
        if (playerGrid[position.x, position.y].nextLayer == null)
        {
            if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Glue))
            {
                sendInformation.EntityLoseGlueEffectAt(position, TurnState.Player);
            }
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
            if (playerGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Glue))
            {
                effectsToPass.Add(EntityData.EntityEffects.Glue);
            }
            EntityDataInstance newLayer = PlaceEntityAtPosition(playerGrid[position.x, position.y].nextLayer, position, TurnState.Player);
            foreach (var effect in effectsToPass)
            {
                newLayer.AddEffect(effect);
            }
        }
            
    }
    
    private IEnumerator EnemyDeathAt((int x, int y) position)
    {
        if (enemyGrid[position.x, position.y] == null) yield break;
        
        if (enemyGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Explosive)&&
            !enemyGrid[position.x, position.y].isImmuneToExplosions)
            yield return EntityExplodeAt(position, TurnState.Enemy);
        if (enemyGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Glue))
        {
            sendInformation.EntityLoseGlueEffectAt(position, TurnState.Enemy);
        }
        if (enemyGrid[position.x, position.y].effects.Contains(EntityData.EntityEffects.Fog))
        {
            sendInformation.EntityLoseFogEffectAt(position, TurnState.Enemy);
        }
        enemyGrid[position.x, position.y] = null;
        sendInformation.EntityDeathAt(position, TurnState.Enemy);
        yield return WaitAnimationEvent();
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

    if (originInsideBubble)
    {
        RemoveBubbleAt(position, gridToApply);
    }

    // 5) Suppression des bulles si explosion hors bulle (les protégés survivent)
    if (!originInsideBubble)
    {
        foreach (var pos in protectedPositions)
            RemoveBubbleAt(pos, gridToApply);
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
        if ((i,j) == position) continue;
        
        var e = gridToApply[i, j];
        if (e == null) continue;

        // on retire l'effet explosif pour éviter toute relance mécanique
        e.effects.Remove(EntityData.EntityEffects.Explosive);

        yield return EntityTakeDamage(e, (i, j), originEntity.explosionDamages);
    }
}





    public EntityDataInstance PlaceEntityAtPosition(EntityData entity, (int x, int y) position, TurnState team)
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

    public void RemoveBubbleAt((int x, int y) position, EntityDataInstance[,] gridToApply)
    {
        TurnState team = gridToApply is CharacterDataInstance[,] ? TurnState.Player : TurnState.Enemy;
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
    
    
    
    
    private void DoSpawn(EntityData entityToSpawn, EntityDataInstance[,] gridToApply)
    {
        TurnState targetTeam = gridToApply is CharacterDataInstance[,] ? TurnState.Player : TurnState.Enemy;
        List<(int x, int y)> emptyPositions = GetEmptyPositions(gridToApply);
        foreach (var position in emptyPositions)
        {
            PlaceEntityAtPosition(entityToSpawn, position, FightManager.TurnState.Enemy);
        }
    }
    
    private List<(int x, int y)> GetEmptyPositions(EntityDataInstance[,] targetedGrid)
    {
        List<(int x, int y)> positions = new List<(int x, int y)>();

        for (int i = 0; i < targetedGrid.GetLength(0); i++)
        {
            for (int j = 0; j < targetedGrid.GetLength(1); j++)
            {
                if (targetedGrid[i, j] == null) 
                    positions.Add((i, j));
            }
        }
        return positions;
    }
}
