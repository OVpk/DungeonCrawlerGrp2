using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public abstract bool IsConditionComplete(CharacterDataInstance[,] playerGrid, EnemyDataInstance[,] enemyGrid);
}