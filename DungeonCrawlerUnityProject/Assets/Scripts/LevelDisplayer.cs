using Unity.Mathematics;
using UnityEngine;

public class LevelDisplayer : MonoBehaviour
{
    public int displayedLevelHeight;
    public int displayedLevelWidth;
    
    [SerializeField] private GameObject areaIconPrefab;
    private SpriteRenderer[,] displayedAreas;
    
    void Start()
    {
        InitDisplay();
    }
    
    
    private void InitDisplay()
    {
        displayedAreas = new SpriteRenderer[displayedLevelHeight, displayedLevelWidth];
        
        for (int i = 0; i < displayedLevelHeight; i++)
        {
            for (int j = 0; j < displayedLevelWidth; j++)
            {
                displayedAreas[i,j] = 
                    Instantiate(areaIconPrefab, new Vector3(j, 2 - i, 0) * 6, quaternion.identity).GetComponent<SpriteRenderer>();
            }
        }
        
        displayedAreas[0, displayedLevelWidth/2].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x, LevelManager.Instance.currentAreaPosition.y].iconDistance1;
        displayedAreas[0, displayedLevelWidth/2 +1].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x, LevelManager.Instance.currentAreaPosition.y+1].iconDistance1;
        displayedAreas[0, displayedLevelWidth/2 -1].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x, LevelManager.Instance.currentAreaPosition.y-1].iconDistance1;
        displayedAreas[1, displayedLevelWidth/2].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x+1, LevelManager.Instance.currentAreaPosition.y].iconDistance1;
    }
    
}
