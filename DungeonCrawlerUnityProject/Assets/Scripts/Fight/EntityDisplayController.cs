using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EntityDisplayController : MonoBehaviour, IFightEventListener
{
    public FightManager.TurnState team;
    public (int x, int y) positionInGrid;

    public EntityLocationDisplayer entityLocation;
    public EntityDisplayer entity;

    public TMP_Text durabilityText;
    public int durabilityNb;
    public TMP_Text typeText;

    #region PossibleHightlightColors

    private Color orange = new Color(1f, 0.5f, 0f);
    private Color pink = new Color(1f, 0.4f, 0.7f);
    private Color hibiscus = new Color(0.772f, 0.192f, 0.412f);
    
    #endregion

    public void Init()
    {
        entityLocation.SetTeam(team);
        entityLocation.GetComponent<SpriteRenderer>().sortingOrder -= positionInGrid.x;
        entity.GetComponent<SpriteRenderer>().sortingOrder -= positionInGrid.x;
    }
    

    private bool IsConcerned((int x, int y) pos, FightManager.TurnState evtTeam)
        => team == evtTeam && pos == positionInGrid;

    public void OnEntityDeath((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        entity.PlayDeathAnim();
        durabilityText.gameObject.SetActive(false);
        typeText.gameObject.SetActive(false);
    }

    public void OnEntityAttack((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        entity.PlayAttackAnim();
    }

    public void OnEntitySpawn((int x, int y) position, FightManager.TurnState team, EntityDataInstance entityData)
    {
        if (!IsConcerned(position, team)) return;

        entity.gameObject.SetActive(true);
        entity.InitVisual(entityData);
        entity.PlaySpawnAnim();
        durabilityText.gameObject.SetActive(true);
        typeText.gameObject.SetActive(true);

        durabilityNb = entityData.durability;
        durabilityText.text = durabilityNb.ToString();
        typeText.text = entityData.type switch
        {
            EntityData.EntityTypes.Mou => "Mou",
            EntityData.EntityTypes.Dur => "Dur",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void OnEntityHovered((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        entityLocation.SetHighlight(pink);
        entity.SetHighlight(pink);
    }

    public void OnEntityNoLongerHovered((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.ClearHighlight();
        entity.ClearHighlight();
    }
    
    public void OnEntityTargeted((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetHighlight(orange);
        entity.SetHighlight(orange);
    }
    
    public void OnEntityNoLongerTargeted((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.ClearHighlight();
        entity.ClearHighlight();
    }

    public void OnEntitySelected((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetHighlight(hibiscus);
        entity.SetHighlight(hibiscus);
    }

    public void OnEntityNoLongerSelected((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.ClearHighlight();
        entity.ClearHighlight();
    }

    public void OnEntityLocationDisabled((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetGrayscale(true);
        entity.PlaySleepAnim();
    }

    public void OnEntityLocationEnabled((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetGrayscale(false);
        entity.PlayIdleAnim();
    }

    public void OnEntityTakeDamage((int x, int y) position, int nbDamages, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entity.PlayHitAnim();

        durabilityNb = Math.Clamp(durabilityNb - nbDamages, 0, durabilityNb);
        durabilityText.text = durabilityNb.ToString();
    }
}