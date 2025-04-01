using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    private enum TurnState
    {
        Player,
        Ai
    }
    private TurnState currentTurn;

    private void SwitchTurn()
    {
        currentTurn = currentTurn == TurnState.Player ? TurnState.Ai : TurnState.Player;
    }

    private CharacterDataInstance[,] playerGrid = new CharacterDataInstance[2, 3];
    private EnemyDataInstance[,] aiGrid = new EnemyDataInstance[3, 3];

    public PatternData pattern;

    private void Start()
    {
        if (pattern.positions == null)
        {
            Debug.Log("nulll");
        }
        foreach (var position in pattern.positions)
        {
            Debug.Log(position.x+" + "+position.y);
        }
    }
}
