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
    public TMP_Text bubbleDurabilityText;
    public int bubbleDurabilityNb;

    #region PossibleHightlightColors

    private Color orange = new Color(1f, 0.5f, 0f);
    private Color pink = new Color(1f, 0.4f, 0.7f);
    private Color hibiscus = new Color(0.772f, 0.192f, 0.412f);
    private Color darkOrange = new Color(0.772f, 0.386f, 0f);
    
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

    public void ResetDisplay()
    {
        isEntityActived = false;
        isBubbleHLeftActive = false;
        isBubbleHMiddleActive = false;
        isBubbleHRightActive = false;
        isBubbleVActive = false;
        
        durabilityText.gameObject.SetActive(false);
        typeText.gameObject.SetActive(false);
        
        entity.ClearHighlight();
        entity.gameObject.SetActive(false);
        
        entityLocation.SetGrayscale(false);
        entityLocation.ClearHighlight();
        
        effectDisplayer.fogAnim.gameObject.SetActive(false);
        effectDisplayer.glueAnim.gameObject.SetActive(false);
        effectDisplayer.explosivePowder.gameObject.SetActive(false);
    }

    private bool isAlreadyDeadOneTime = false;

    private void OnEnable()
    {
        if (!isEntityActived && isAlreadyDeadOneTime)
        {
            entity.gameObject.SetActive(false);
        }
    }


    private bool IsConcerned((int x, int y) pos, FightManager.TurnState evtTeam)
        => team == evtTeam && pos == positionInGrid;

    public void OnEntityDeath((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        isAlreadyDeadOneTime = true;
        
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
        durabilityNb = entityData.durability;
        durabilityText.text = durabilityNb + "<sprite index=4>";

        if (team == FightManager.TurnState.Player)
        {
            typeText.gameObject.SetActive(true);

            typeText.text = entityData.type switch
            {
                EntityData.EntityTypes.Mou => "<sprite index=3>",
                EntityData.EntityTypes.Dur => "<sprite index=0>",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
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

        if (FightManager.Instance.isAttackPatternMovingToDisplay)
        {
            entityLocation.SetHighlight(darkOrange);
            entity.SetHighlight(darkOrange);
        }
        else
        {
            entityLocation.SetHighlight(orange);
            entity.SetHighlight(orange);
        }
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
        {
            entity.PlaySleepAnim();
        }
        entity.SetSleepingState(true);
    }

    public void OnEntityLocationEnabled((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entityLocation.SetGrayscale(false);
        entity.SetSleepingState(false);
    }

    public void OnEntityTakeDamage((int x, int y) position, int nbDamages, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        entity.PlayHitAnim();

        durabilityNb = Math.Clamp(durabilityNb - nbDamages, 0, durabilityNb);
        durabilityText.text = durabilityNb + "<sprite index=4>";
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

    public void OnEntityCreateProtection((int x, int y) position, FightManager.TurnState team, BubbleDirections direction, int bubbleDurability)
    {
        if (!IsConcerned(position, team)) return;
        
        bubbleDurabilityText.gameObject.SetActive(true);
        bubbleDurabilityNb = bubbleDurability;
        bubbleDurabilityText.text = bubbleDurabilityNb.ToString();

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
        
        bubbleDurabilityText.gameObject.SetActive(false);

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
        
        miniFx.sprite = miniFxExplosivePoudre;
        effectDisplayer.anim.Play("MiniFxSpawn");
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

    public void OnGridIsClear(FightManager.TurnState team)
    {
        if (this.team != team) return;
        
        ResetDisplay();
    }

    public void OnBubbleTakeDamage((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;
        
        bubbleDurabilityNb = Math.Clamp(bubbleDurabilityNb--, 0, bubbleDurabilityNb);
        bubbleDurabilityText.text = bubbleDurabilityNb.ToString();
        
        miniFx.sprite = miniFxBoing;
        effectDisplayer.anim.Play("MiniFxSpawn");
    }

    public SpriteRenderer miniFx;

    public Sprite miniFxMissed;
    public Sprite miniFxGlue;
    public Sprite miniFxExplosivePoudre;
    public Sprite miniFxBoing;

    public void OnAttackIsMissed((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        miniFx.sprite = miniFxMissed;
        effectDisplayer.anim.Play("MiniFxSpawn");
    }

    public void OnAttackIsMissedByGlue((int x, int y) position, FightManager.TurnState team)
    {
        if (!IsConcerned(position, team)) return;

        miniFx.sprite = miniFxGlue;
        effectDisplayer.anim.Play("MiniFxSpawn");
    }
}