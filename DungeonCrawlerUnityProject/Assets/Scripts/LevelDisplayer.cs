using Unity.Mathematics;
using UnityEngine;

public class LevelDisplayer : MonoBehaviour
{
    [SerializeField] private int displayedLevelHeight;
    [SerializeField] private int displayedLevelWidth;
    
    [SerializeField] private GameObject areaIconPrefab;
    private SpriteRenderer[,] displayedAreas;
    
    public void InitDisplay()
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
        
        RefreshSprites();
    }

    public void RefreshSprites()
    {
        for (int i = 0; i < displayedLevelHeight; i++)
        {
            displayedAreas[i, displayedLevelWidth/2].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x + i, LevelManager.Instance.currentAreaPosition.y].iconDistance1;
            
            for (int j = 1; j < displayedLevelWidth/2+1; j++)
            {
                displayedAreas[i, displayedLevelWidth/2 +j].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x+i, LevelManager.Instance.currentAreaPosition.y+j].iconDistance1;
                displayedAreas[i, displayedLevelWidth/2 -j].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x+i, LevelManager.Instance.currentAreaPosition.y-j].iconDistance1;
            }
        }
    }
}
