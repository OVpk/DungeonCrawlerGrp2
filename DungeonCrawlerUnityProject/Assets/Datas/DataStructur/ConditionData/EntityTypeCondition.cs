using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Conditions/EntityTypeCondition")]
public class EntityTypeCondition : Condition
{
    public EntityData.EntityTypes targetType;
    public int requiredCount;

    public override bool IsConditionComplete(EntityDataInstance[,] gridToCheck)
    {
        int count = 0;
        foreach (var entity in gridToCheck)
        {
            if (entity != null && entity.type == targetType)
                count++;
        }
        return count >= requiredCount;
    }
}