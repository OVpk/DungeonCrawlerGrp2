using Unity.Mathematics;
using UnityEngine;

public class LevelDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject levelContainer;
    [SerializeField] private int displayedLevelHeight;
    [SerializeField] private int displayedLevelWidth;
    [SerializeField] private int distanceRatioBetweenAreas;
    
    [SerializeField] private GameObject areaIconPrefab;
    private SpriteRenderer[,] displayedAreas;

    [SerializeField] private GameObject selector;
    
    public void InitDisplay()
    {
        displayedAreas = new SpriteRenderer[displayedLevelHeight, displayedLevelWidth];
        
        for (int i = 0; i < displayedLevelHeight; i++)
        {
            for (int j = 0; j < displayedLevelWidth; j++)
            {
                displayedAreas[i,j] = 
                    Instantiate(areaIconPrefab, new Vector3(j, -i, 0) * distanceRatioBetweenAreas, quaternion.identity, levelContainer.transform).GetComponent<SpriteRenderer>();
            }
        }
        
        RefreshSprites();
    }

    public void RefreshSprites()
    {
        for (int i = 0; i < displayedLevelHeight; i++)
        {
            for (int j = 0; j < displayedLevelWidth; j++)
            {
                displayedAreas[i, j].sprite = LevelManager.Instance.level[LevelManager.Instance.currentAreaPosition.x+i, LevelManager.Instance.currentAreaPosition.y+j].iconDistance1;
            }
        }
    }

    public void MoveSelector((int x, int y) selectorPosition, (int x, int y) directionToGo)
    {
        selector.transform.position =
            new Vector3(selectorPosition.y + directionToGo.y, -(selectorPosition.x + directionToGo.x), 0) * distanceRatioBetweenAreas;
    }
    
    public bool IsOutsideLimits((int x, int y) position)
    {
        return position.x < 0 || position.y < 0 || position.x >= displayedLevelHeight || position.y >= displayedLevelWidth;
    }
}
