using System;
using TMPro;
using UnityEngine;

public class EntityDisplayController : MonoBehaviour, IFightEventListener
{
    public FightManager.TurnState team;
    public (int x, int y) positionInGrid;

    public EntityLocationDisplayer entityLocation;
    public EntityDisplayer entity;
    public EffectDisplayer effectDisplayer;

    public TMP_Text durabilityText;
    public int durabilityNb;
    public TMP_Text typeText;

    #region PossibleHightlightColors

    private Color orange = new Color(1f, 0.5f, 0f);
    private Color pink = new Color(1f, 0.4f, 0.7f);
    private Color hibiscus = new Color(0.772f, 0.192f, 0.412f);
    
    #endregion

    private bool isBubbleHLeftActive = false;
    private bool isBubbleHMiddleActive = false;
    private bool isBubbleHRightActive = false;
    private bool isBubbleVActive = false;
    private bool isEntityActived = false;

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

        isEntityActived = false;
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

        isEntityActived = true;
        entity.gameObject.SetActive(true);
        entity.InitVisual(entityData);
        entity.PlaySpawnAnim();
        durabilityText.gameObject.SetActive(true);
        typeText.gameObject.SetActive(true);

        durabilityNb = entityData.durability;
        durabilityText.text = durabilityNb + "b";
        typeText.text = entityData.type switch
        {
            EntityData.EntityTypes.Mou => "a",
            EntityData.EntityTypes.Dur => "`",
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
        if (isEntityActived) 
            entity.PlaySleepAnim();
    }

    public void OnEntityLocationEnabled((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetGrayscale(false);
        if (isEntityActived)
            entity.PlayIdleAnim();
    }

    public void OnEntityTakeDamage((int x, int y) position, int nbDamages, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entity.PlayHitAnim();

        durabilityNb = Math.Clamp(durabilityNb - nbDamages, 0, durabilityNb);
        durabilityText.text = durabilityNb.ToString();
    }

    public void OnEntityDisplayBubble((int x, int y) position, FightManager.TurnState team, bool state, BubbleDirections direction)
    {
        if (!IsConcerned(position, team)) return;
        
        switch (direction)
        {
            case BubbleDirections.Horizontal :
                switch (position.y)
                {
                    case 0 :
                        if (state)
                        {
                            effectDisplayer.anim.Play("BubbleHorizontalLeftOn");
                            isBubbleHLeftActive = true;
                        }
                        else
                        {
                            if (isBubbleHLeftActive == true)
                            {
                                effectDisplayer.anim.Play("BubbleHorizontalLeftOff");
                                isBubbleHLeftActive = false;
                            }
                        }
                        break;
                    case 1 :
                        if (state)
                        {
                            effectDisplayer.anim.Play("BubbleHorizontalMiddleOn");
                            isBubbleHMiddleActive = true;
                        }
                        else
                        {
                            if (isBubbleHMiddleActive == true)
                            {
                                effectDisplayer.anim.Play("BubbleHorizontalMiddleOff");
                                isBubbleHMiddleActive = false;
                            }
                        }
                        break;
                    case 2 :
                        if (state)
                        {
                            effectDisplayer.anim.Play("BubbleHorizontalRightOn");
                            isBubbleHRightActive = true;
                        }
                        else
                        {
                            if (isBubbleHRightActive == true)
                            {
                                effectDisplayer.anim.Play("BubbleHorizontalRightOff");
                                isBubbleHRightActive = false;
                            }
                        }
                        break;
                };
                break;
            case BubbleDirections.Vertical :
                if (state)
                {
                    effectDisplayer.anim.Play("BubbleVerticalOn");
                    isBubbleVActive = true;
                }
                else
                {
                    if (isBubbleVActive == true)
                    {
                        effectDisplayer.anim.Play("BubbleVerticalOff");
                        isBubbleVActive = false;
                    }
                }
                break;
        }
    }

    public enum BubbleDirections
    {
        Horizontal,
        Vertical
    }

    public void OnEntityCreateProtection((int x, int y) position, FightManager.TurnState team, BubbleDirections direction)
    {
        if (!IsConcerned(position, team)) return;

        switch (direction)
        {
            case BubbleDirections.Horizontal :
                FightManager.Instance.sendInformation.EntityDisplayBubbleAt(position, team, true, direction); break;
                
            case BubbleDirections.Vertical :
                FightManager.Instance.sendInformation.EntityDisplayBubbleAt((0, positionInGrid.y), team, true, direction); break;
        }
    }

    public void OnEntityLoseProtection((int x, int y) position, FightManager.TurnState team, BubbleDirections direction)
    {
        if (!IsConcerned(position, team)) return;

        switch (direction)
        {
            case BubbleDirections.Horizontal : 
                FightManager.Instance.sendInformation.EntityDisplayBubbleAt(position, team, false, direction); break;
            case BubbleDirections.Vertical :
                FightManager.Instance.sendInformation.EntityDisplayBubbleAt((0, positionInGrid.y), team, false, direction); break;
        }
    }

    public void OnEntityExplode((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        effectDisplayer.anim.Play("Explosion");
    }

    public void OnEntityGetExplosiveEffect((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        effectDisplayer.explosivePowder.gameObject.SetActive(true);
    }
    
    public void OnEntityLoseExplosiveEffect((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        effectDisplayer.explosivePowder.gameObject.SetActive(false);
    }

    public void OnEntityGetGlueEffect((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        effectDisplayer.glueAnim.gameObject.SetActive(true);
        effectDisplayer.PlayGlueAnim();
    }

    public void OnEntityLoseGlueEffect((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        effectDisplayer.glueAnim.gameObject.SetActive(false);
    }

    public void OnEntityGetFogEffect((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        effectDisplayer.fogAnim.gameObject.SetActive(true);
        effectDisplayer.PlayFogAnim();
    }

    public void OnEntityLoseFogEffect((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        effectDisplayer.fogAnim.gameObject.SetActive(false);
    }
}