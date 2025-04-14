using UnityEngine;

[CreateAssetMenu(menuName = "Conditions/EntityTypeCondition")]
public class EntityTypeCondition : Condition
{
    public EntityData.EntityTypes targetType;
    public int requiredCount;

    public override bool IsConditionComplete(CharacterDataInstance[,] playerGrid, EnemyDataInstance[,] enemyGrid)
    {
        int count = 0;
        foreach (var entity in playerGrid)
        {
            if (entity != null && entity.type == targetType)
                count++;
        }
        return count >= requiredCount;
    }
}