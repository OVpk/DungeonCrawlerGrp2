using System.Collections.Generic;
using Unity.VisualScripting;
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

    public void EntitySpawnAt((int x, int y) position, FightManager.TurnState team, EntityDataInstance entityData)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntitySpawn(position, team, entityData);
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
    
    public void EntityTargetedAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityTargeted(position, team);
        }
    }
    
    public void EntityNoLongerTargetedAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityNoLongerTargeted(position, team);
        }
    }

    public void EntitiesTargetedByPatternAt((int x, int y) originPosition, List<Vector2Int> pattern, FightManager.TurnState team)
    {
        foreach (var position in pattern)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            EntityTargetedAt(positionInGrid, team);
        }
    }
    
    public void EntitiesNoLongerTargetedByPatternAt((int x, int y) originPosition, List<Vector2Int> pattern, FightManager.TurnState team)
    {
        foreach (var position in pattern)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            EntityNoLongerTargetedAt(positionInGrid, team);
        }
    }
    
    public void EntitySelectedAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntitySelected(position, team);
        }
    }
    
    public void EntityNoLongerSelectedAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityNoLongerSelected(position, team);
        }
    }

    public void EntitiesLocationEnabledAt(HashSet<(int x, int y)> positions, FightManager.TurnState team)
    {
        foreach (var position in positions)
        {
            EntityLocationEnabledAt(position, team);
        }
    }
    
    public void EntityLocationDisabledAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityLocationDisabled(position, team);
        }
    }
    
    public void EntityLocationEnabledAt((int x, int y) position, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityLocationEnabled(position, team);
        }
    }

    public void EntityTakeDamageAt((int x, int y) position, int nbDamages, FightManager.TurnState team)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityTakeDamage(position, nbDamages, team);
        }
    }

    public void EntityDisplayBubbleAt((int x, int y) position, FightManager.TurnState team, bool state, EntityDisplayController.BubbleDirections direction)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityDisplayBubble(position, team, state, direction);
        }
    }

    public void EntityCreateProtectionAt((int x, int y) position, FightManager.TurnState team, EntityDisplayController.BubbleDirections direction)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityCreateProtection(position, team, direction);
        }
    }

    public void EntityLoseProtectionAt((int x, int y) position, FightManager.TurnState team, EntityDisplayController.BubbleDirections direction)
    {
        foreach (var listener in listeners)
        {
            listener.OnEntityLoseProtection(position, team, direction);
        }
    }
    
}