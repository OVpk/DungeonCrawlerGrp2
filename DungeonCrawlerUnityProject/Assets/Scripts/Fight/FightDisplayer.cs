using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightDisplayer : MonoBehaviour
{
    public GameObject[,] playerGridDisplayed;
    public GameObject[,] enemyGridDisplayed;
    
    public GameObject entityLocationPrefab;
    public GameObject playerGridPosition;
    public GameObject enemyGridPosition;

    public void InitDisplayedGrid(EntityDataInstance[,] grid)
    {
        GameObject[,] gridDisplayed = new GameObject[grid.GetLength(0), grid.GetLength(1)];;

        GameObject gridPosition = grid switch
        {
            CharacterDataInstance[,] => playerGridPosition,
            EnemyDataInstance[,] => enemyGridPosition,
            _ => throw new InvalidOperationException("Invalid Type")
        };
        
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                gridDisplayed[i, j] = Instantiate(entityLocationPrefab, gridPosition.transform);
                gridDisplayed[i, j].transform.localPosition = new Vector3(j, i) * 2f;
            }
        }

        switch (grid)
        {
            case CharacterDataInstance[,] : playerGridDisplayed = gridDisplayed; break;
            case EnemyDataInstance[,] : enemyGridDisplayed = gridDisplayed; break;
            default : throw new InvalidOperationException("Invalid Type");
        }
    }
    
    public void DisplayPattern((int x, int y) originPosition, List<Vector2Int> pattern)
    {
        CleanPatternDisplay();
        foreach (var position in pattern)
        {
            (int x, int y) positionInGrid = (originPosition.x + position.x, originPosition.y + position.y);
            enemyGridDisplayed[positionInGrid.x, positionInGrid.y].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void CleanPatternDisplay()
    {
        foreach (var cell in enemyGridDisplayed)
        {
            cell.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    
    public GameObject selectorDisplayed;

    public void MoveSelectorDisplayTo((int x, int y) newPosition)
    {
        selectorDisplayed.transform.localPosition = new Vector3(newPosition.y, newPosition.x) * 2f;
    }
}
