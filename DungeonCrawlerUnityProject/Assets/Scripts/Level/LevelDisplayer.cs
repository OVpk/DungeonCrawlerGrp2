using TMPro;
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
    [SerializeField] private GameObject areaDisplayer;

    [SerializeField] private Sprite playerPositionSprite;

    [SerializeField] private TMP_Text areaDescription;

    private (int x, int y) currentAreaPosition => LevelManager.Instance.currentAreaPosition;
    private AreaData[,] level => LevelManager.Instance.level;

    private void Start()
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
                    Instantiate(areaIconPrefab, new Vector3(j, -i, 0) * distanceRatioBetweenAreas,
                        quaternion.identity, levelContainer.transform).GetComponent<SpriteRenderer>();
            }
        }
        displayedAreas[0, 0].sprite = playerPositionSprite;
    }

    private void RefreshSprites()
    {
        for (int i = 0; i < displayedLevelHeight; i++)
        {
            for (int j = 0; j < displayedLevelWidth; j++)
            {
                if (i==0 && j==0) continue;
                
                (int x, int y) areaToDisplay = (currentAreaPosition.x + i, currentAreaPosition.y + j);
                if (LevelManager.Instance.IsOutsideLimits(areaToDisplay))
                {
                    displayedAreas[i, j].sprite = null;
                    continue;
                }
                displayedAreas[i, j].sprite = level[areaToDisplay.x, areaToDisplay.y].iconDistance1;
            }
        }
    }

    public void DisplayLevel(bool state)
    {
        levelContainer.SetActive(state);
        RefreshSprites();
    }

    public void DisplayArea(bool state)
    {
        areaDisplayer.SetActive(state);
    }

    private void DisplayDescription(AreaData area) => areaDescription.text = area.description;

    private void CleanDescription() => areaDescription.text = "";

    public void MoveSelector((int x, int y) selectorPosition, (int x, int y) directionToGo)
    {
        selector.transform.position =
            new Vector3(selectorPosition.y + directionToGo.y, -(selectorPosition.x + directionToGo.x), 0) * distanceRatioBetweenAreas;
        
        (int x, int y) newPosition = (currentAreaPosition.x + selectorPosition.x + directionToGo.x, currentAreaPosition.y + selectorPosition.y + directionToGo.y);
        if (LevelManager.Instance.IsOutsideLimits(newPosition))
        {
            CleanDescription();
        }
        else
        {
            DisplayDescription(level[newPosition.x, newPosition.y]);
        }
    }
    
    public bool IsOutsideLimits((int x, int y) position)
    {
        return position.x < 0 || position.y < 0 || position.x >= displayedLevelHeight || position.y >= displayedLevelWidth;
    }
}
