using System.Collections.Generic;
using UnityEngine;

public class PatternData : ScriptableObject
{
    [field:SerializeField] public List<Vector2Int> positions { get; private set; }

    public void Init(HashSet<(int x, int y)> selectedCells)
    {
        positions = new List<Vector2Int>();
        foreach (var cell in selectedCells)
        {
            positions.Add(new Vector2Int(cell.x, cell.y));
        }
    }
}
