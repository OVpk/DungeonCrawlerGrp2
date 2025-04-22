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
}