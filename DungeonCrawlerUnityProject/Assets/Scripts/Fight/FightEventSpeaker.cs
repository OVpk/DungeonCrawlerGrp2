using System.Collections.Generic;
using UnityEngine;

public class FightEventSpeaker : MonoBehaviour
{
    private List<IFightEventListener> listeners = new List<IFightEventListener>();

    public void Register(IFightEventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void Unregister(IFightEventListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }

    public void EntityDeathAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityDeath(position, team);
        }
    }

    public void EntityAttackAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityAttack(position, team);
        }
    }

    public void EntitySpawnAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntitySpawn(position, team);
        }
    }

    public void EntityHoveredAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityHovered(position, team);
        }
    }
    
    public void EntityNoLongerHoveredAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityNoLongerHovered(position, team);
        }
    }
}