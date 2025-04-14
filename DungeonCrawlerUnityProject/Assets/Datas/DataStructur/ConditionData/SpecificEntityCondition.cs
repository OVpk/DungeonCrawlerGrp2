using UnityEngine;

[CreateAssetMenu(menuName = "Conditions/SpecificEntityCondition")]
public class SpecificEntityCondition : Condition
{
    public string entityName;
    public int requiredCount;

    public override bool IsConditionComplete(CharacterDataInstance[,] playerGrid, EnemyDataInstance[,] enemyGrid)
    {
        int count = 0;
        foreach (var entity in enemyGrid)
        {
            if (entity != null && entity.name == entityName)
                count++;
        }
        return count >= requiredCount;
    }
}