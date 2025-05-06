using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Entity/Enemy/Spawner")]
public class SpawnerData : EnemyData
{
    [field: SerializeField] public EnemyData enemyToSpawn { get; private set; }
    
    public override EntityDataInstance Instance()
    {
        return new SpawnerDataInstance(this);
    }
}

public class SpawnerDataInstance : EnemyDataInstance
{

    public EnemyData enemyToSpawn;

    public SpawnerDataInstance(SpawnerData data) : base(data)
    {
        enemyToSpawn = data.enemyToSpawn;
    }
}