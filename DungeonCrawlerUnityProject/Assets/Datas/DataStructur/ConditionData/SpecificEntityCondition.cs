using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Conditions/SpecificEntityCondition")]
public class SpecificEntityCondition : Condition
{
    public string entityName;
    public int requiredCount;

    public override bool IsConditionComplete(EntityDataInstance[,] gridToCheck)
    {
        int count = 0;
        foreach (var entity in gridToCheck)
        {
            if (entity != null && entity.name == entityName)
                count++;
        }
        return count >= requiredCount;
    }
}