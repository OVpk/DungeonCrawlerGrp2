using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleAi : MonoBehaviour
{
    private FightManager fightManager => FightManager.Instance;

    private List<(int x, int y)> GetPlayablePositions()
    {
        List<(int x, int y)> positions = new List<(int x, int y)>();

        for (int i = 0; i < fightManager.enemyGrid.GetLength(0); i++)
        {
            for (int j = 0; j < fightManager.enemyGrid.GetLength(1); j++)
            {
                if (fightManager.IsPositionAlreadyPlayed((i, j))) continue;
                if (fightManager.enemyGrid[i, j] == null) continue;
                
                positions.Add((i, j));
            }
        }

        return positions;
    }

    public void PlayTurn()
    {
        List<(int x, int y)> playablePositions = GetPlayablePositions();

        if (playablePositions.Count == 0)
        {
            Debug.Log("No playable positions for the enemy.");
            return;
        }
        
        (int x, int y) chosenPosition = GetRandomPosition(playablePositions);

        EnemyDataInstance chosenEnemy = fightManager.enemyGrid[chosenPosition.x, chosenPosition.y];
        if (chosenEnemy == null || chosenEnemy.attack == null)
        {
            Debug.LogWarning("Enemy or enemy attack data missing at position " + chosenPosition);
            return;
        }

        AttackStageData bestAttackStage = fightManager.FindBestUnlockedStage(chosenEnemy.attack);
        if (bestAttackStage == null)
        {
            Debug.LogWarning("No unlocked attack stage found for enemy at position " + chosenPosition);
            return;
        }

        (int x, int y) bestAttackPosition = FindBestOriginPosition(bestAttackStage);

        fightManager.Attack(chosenPosition, bestAttackPosition, FightManager.TurnState.Enemy);
    }

    private (int x, int y) FindBestOriginPosition(AttackStageData attackStage)
    {
        int bestScore = int.MaxValue;
        (int x, int y) bestOriginPosition = (0,0);

        for (int i = 0; i < fightManager.playerGrid.GetLength(0); i++)
        {
            for (int j = 0; j < fightManager.playerGrid.GetLength(1); j++)
            {
                (int x, int y) position = (i, j);
                if (fightManager.IsPatternOutsideLimit(fightManager.playerGrid, position, attackStage.pattern.positions)) continue;

                int patternScore = EvaluateAttackScoreAt(position, attackStage);

                if (patternScore < bestScore)
                {
                    bestScore = patternScore;
                    bestOriginPosition = position;
                }
            }
        }

        return bestOriginPosition;
    }

    private int EvaluateAttackScoreAt((int x, int y) originPosition, AttackStageData attackStage)
    {
        int potentialDamages = attackStage.damage * attackStage.pattern.positions.Count;
        int potentialLossOfDurability = 0;

        foreach (var position in attackStage.pattern.positions)
        {
            (int x, int y) targetPosition = (originPosition.x + position.x, originPosition.y + position.y);
            if (fightManager.IsOutsideLimit(fightManager.playerGrid, targetPosition)) continue;

            CharacterDataInstance targetCharacter = fightManager.playerGrid[targetPosition.x, targetPosition.y];
            if (targetCharacter == null) continue;
            
            potentialLossOfDurability += targetCharacter.durability;
            
        }

        return potentialDamages - potentialLossOfDurability;
    }

    private (int x, int y) GetRandomPosition(List<(int x, int y)> positions)
    {
        int rnd = Random.Range(0, positions.Count);
        return positions[rnd];
    }
}
