using System;
using UnityEngine;

public class EntityDisplayController : MonoBehaviour, IFightEventListener
{
    public FightManager.TurnState team;
    public (int x, int y) positionInGrid;

    public EntityLocationDisplayer entityLocation;
    public EntityDisplayer entity;

    #region PossibleHightlightColors

    private Color orange = new Color(1f, 0.5f, 0f);
    private Color pink = new Color(1f, 0.4f, 0.7f);
    private Color hibiscus = new Color(0.772f, 0.192f, 0.412f);
    
    #endregion

    public void Init()
    {
        entityLocation.SetTeam(team);
        entity.SetTeam(team);
    }
    

    private bool IsConcerned((int x, int y) pos, FightManager.TurnState evtTeam)
        => team == evtTeam && pos == positionInGrid;

    public void OnEntityDeath((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        // TODO: déclencher animation de mort, vider l'affichage, etc.
    }

    public void OnEntityAttack((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        // TODO: animation d’attaque, clignotement, etc.
    }

    public void OnEntitySpawn((int x, int y) position, FightManager.TurnState team, EntityDataInstance entityData)
    {
        if (!IsConcerned(position, team)) return;

        entity.InitVisual(entityData);
        entity.PlaySpawnAnim();
    }

    public void OnEntityHovered((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        entityLocation.SetHighlight(pink);
    }

    public void OnEntityNoLongerHovered((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.ClearHighlight();
    }
    
    public void OnEntityTargeted((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetHighlight(orange);
    }
    
    public void OnEntityNoLongerTargeted((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.ClearHighlight();
    }

    public void OnEntitySelected((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetHighlight(hibiscus);
    }

    public void OnEntityNoLongerSelected((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.ClearHighlight();
    }

    public void OnEntityLocationDisabled((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetGrayscale(true);
    }

    public void OnEntityLocationEnabled((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetGrayscale(false);
    }
}