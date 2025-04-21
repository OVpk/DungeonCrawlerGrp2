using System;
using UnityEngine;

public class EntityDisplayController : MonoBehaviour, IFightEventListener
{
    public FightManager.TurnState team;
    public (int x, int y) positionInGrid;

    public EntityLocationDisplayer entityLocation;

    public void Init()
    {
        entityLocation.SetTeam(team);
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

    public void OnEntitySpawn((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        // TODO: spawn visuel de l’entité
    }

    public void OnEntityHovered((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        entityLocation.SetHighlight(Color.magenta);
    }

    public void OnEntityNoLongerHovered((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.ClearHighlight();
        
        
    }
}