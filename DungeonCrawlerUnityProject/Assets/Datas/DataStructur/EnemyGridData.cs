using UnityEngine;

[CreateAssetMenu(menuName = "Data/Fight/Enemy/EnemyGrid")]
public class EnemyGridData : ScriptableObject {
    [SerializeField] private EnemyData[] enemies;

    public EnemyData[,] Enemies2D => ConvertTo2D();

    public void Init(EnemyData[,] grid) {
        int size = grid.GetLength(0);
        enemies = new EnemyData[size * size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                enemies[i * size + j] = grid[i, j];
            }
        }
    }

    private EnemyData[,] ConvertTo2D() {
        int size = Mathf.RoundToInt(Mathf.Sqrt(enemies.Length));
        EnemyData[,] result = new EnemyData[size, size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                result[i, j] = enemies[i * size + j];
            }
        }
        return result;
    }
}