public interface IFightEventListener
{
    void OnEntityDeath((int x, int y) position, FightManager.TurnState team);
    void OnEntityAttack((int x, int y) position, FightManager.TurnState team);
    void OnEntitySpawn((int x, int y) position, FightManager.TurnState team);

    void OnEntityHovered((int x, int y) position, FightManager.TurnState team);
    
    void OnEntityNoLongerHovered((int x, int y) position, FightManager.TurnState team);
}