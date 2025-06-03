public interface IFightEventListener
{
    void OnEntityDeath((int x, int y) position, FightManager.TurnState team);
    void OnEntityAttack((int x, int y) position, FightManager.TurnState team);
    void OnEntitySpawn((int x, int y) position, FightManager.TurnState team, EntityDataInstance entityData);

    void OnEntityHovered((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityNoLongerHovered((int x, int y) position, FightManager.TurnState team);

    void OnEntityTargeted((int x, int y) position, FightManager.TurnState team);

    void OnEntityNoLongerTargeted((int x, int y) position, FightManager.TurnState team);
    
    void OnEntitySelected((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityNoLongerSelected((int x, int y) position, FightManager.TurnState team);
    void OnEntityLocationDisabled((int x, int y) position, FightManager.TurnState team);
    void OnEntityLocationEnabled((int x, int y) position, FightManager.TurnState team);

    void OnEntityTakeDamage((int x, int y) position, int nbDamages, FightManager.TurnState team);

    void OnEntityDisplayBubble((int x, int y) position, FightManager.TurnState team, bool state, EntityDisplayController.BubbleDirections direction);

    void OnEntityCreateProtection((int x, int y) position, FightManager.TurnState team, EntityDisplayController.BubbleDirections direction, int bubbleDurability);

    void OnEntityLoseProtection((int x, int y) position, FightManager.TurnState team, EntityDisplayController.BubbleDirections direction);

    void OnEntityExplode((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityGetExplosiveEffect((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityLoseExplosiveEffect((int x, int y) position, FightManager.TurnState team);

    void OnEntityGetGlueEffect((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityLoseGlueEffect((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityGetFogEffect((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityLoseFogEffect((int x, int y) position, FightManager.TurnState team);

    void OnGridIsClear(FightManager.TurnState team);
    
    void OnBubbleTakeDamage((int x, int y) position, FightManager.TurnState team);

}