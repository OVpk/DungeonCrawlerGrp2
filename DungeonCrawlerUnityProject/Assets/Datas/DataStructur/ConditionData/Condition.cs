using UnityEngine;

public abstract class Condition : ScriptableObject
{
    [field: SerializeField] public string description { get; private set; }
    public abstract bool IsConditionComplete(EntityDataInstance[,] gridToCheck);
}