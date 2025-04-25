using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Conditions/OperatorCondition")]
public class OperatorCondition : Condition
{
    public enum LogicalOperator { AND, OR }
    
    public Condition conditionA;
    public LogicalOperator operatorType;
    public Condition conditionB;

    public override bool IsConditionComplete(EntityDataInstance[,] gridToCheck)
    {
        if (conditionA == null || conditionB == null)
            throw new System.Exception("2 Conditions must be assigned");

        return operatorType switch
        {
            LogicalOperator.AND => conditionA.IsConditionComplete(gridToCheck) &&
                                   conditionB.IsConditionComplete(gridToCheck),
            LogicalOperator.OR => conditionA.IsConditionComplete(gridToCheck) ||
                                  conditionB.IsConditionComplete(gridToCheck),
            _ => throw new System.ArgumentOutOfRangeException()
        };
    }
}