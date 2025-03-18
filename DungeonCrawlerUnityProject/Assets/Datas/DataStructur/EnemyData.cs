using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Entity/Enemy")]
public class EnemyData : EntityData
{
    
    //[field: SerializeField] public  { get; private set; }
    
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