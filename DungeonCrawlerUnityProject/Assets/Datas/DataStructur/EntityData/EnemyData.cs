using UnityEngine;

[CreateAssetMenu(menuName = "Data/Entity/Enemy")]
public class EnemyData : EntityData
{
    
    public override EntityDataInstance Instance()
    {
        return new EnemyDataInstance(this);
    }
 
}

public class EnemyDataInstance : EntityDataInstance
{

    public EnemyDataInstance(EnemyData data) : base(data)
    {
        
    }
}