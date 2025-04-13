using UnityEngine;

[CreateAssetMenu(menuName = "Conditions/OperatorCondition")]
public class OperatorCondition : Condition
{
    public enum LogicalOperator { AND, OR }
    
    public Condition conditionA;
    public LogicalOperator operatorType;
    public Condition conditionB;

    public override bool IsConditionComplete(CharacterDataInstance[,] playerGrid, EnemyDataInstance[,] enemyGrid)
    {
        if (conditionA == null || conditionB == null)
            throw new System.Exception("2 Conditions must be assigned");

        return operatorType switch
        {
            LogicalOperator.AND => conditionA.IsConditionComplete(playerGrid, enemyGrid) &&
                                   conditionB.IsConditionComplete(playerGrid, enemyGrid),
            LogicalOperator.OR => conditionA.IsConditionComplete(playerGrid, enemyGrid) ||
                                  conditionB.IsConditionComplete(playerGrid, enemyGrid),
            _ => throw new System.ArgumentOutOfRangeException()
        };
    }
}